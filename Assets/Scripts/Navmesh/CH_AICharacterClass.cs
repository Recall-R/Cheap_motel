using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

namespace CH_AICharacter
{
    [System.Serializable]
    public class CH_AICharacterClass
    {
        [SerializeField]
        private GameObject killerPrefab;

        [SerializeField]
        private GameObject guestPrefab;

        [SerializeField]
        private string characterName = "";

        [SerializeField]
        private CH_NPCSuspicionConfig suspicionConfig;

        // movement variables for StartMovement()
        [Header("Movement")]
        [SerializeField]
        private Transform customDestination;

        [SerializeField]
        private Transform[] secondaryTargets = new Transform[6];

        [SerializeField]
        [Range(0, 5)]
        private int selectedSecondaryIndex;

        [SerializeField]
        private float arrivalDistance = 0.5f;

        public void InvokeCharacter(Vector3 spawnPosition)
        {
            GameObject prefab = killerPrefab != null ? killerPrefab : guestPrefab;

            if (prefab == null)
            {
                Debug.LogWarning("No character prefab assigned for spawn.");
                return;
            }

            string generatedName = string.IsNullOrEmpty(characterName)
                ? GenerateRandomCharacterName()
                : characterName;

            GameObject instance = UnityEngine.Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            instance.name = generatedName;
            AttachMovementDriver(instance);
            AttachSuspicionState(instance, generatedName);
            if(instance.gameObject.activeSelf == false)
            {
                instance.gameObject.SetActive(true);
            }
        }

        // This method attaches the movement driver to the instantiated character and initializes it with the movement parameters from CH_AICharacterMovemen
        private void AttachMovementDriver(GameObject characterInstance)
        {
            if (characterInstance == null)
                return;
            NavMeshAgent agent = characterInstance.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = characterInstance.AddComponent<NavMeshAgent>();
            }
            CH_AICharacterMovementDriver driver = characterInstance.GetComponent<CH_AICharacterMovementDriver>();
            if (driver == null)
            {
                driver = characterInstance.AddComponent<CH_AICharacterMovementDriver>();
            }

            driver.Initialize(agent, customDestination, secondaryTargets, selectedSecondaryIndex, arrivalDistance);
        }

        private void AttachSuspicionState(GameObject characterInstance, string npcName)
        {
            if (characterInstance == null)
                return;

            CH_NPCSuspicionState suspicionState = characterInstance.GetComponent<CH_NPCSuspicionState>();
            if (suspicionState == null)
            {
                suspicionState = characterInstance.AddComponent<CH_NPCSuspicionState>();
            }

            CH_NPCSuspicionConfig configToUse = suspicionConfig != null ? suspicionConfig : new CH_NPCSuspicionConfig();
            CH_NPCSuspicionProfile profile = CH_NPCSuspicionSystem.GenerateProfile(configToUse, npcName);
            suspicionState.Initialize(profile);

            Debug.Log($"[{characterInstance.name}] Suspicion profile generated: {suspicionState.GetSummary()}");
        }

        private string GenerateRandomCharacterName()
        {
            string prefix = killerPrefab != null ? "Butcher" : "Guest";
            string randomName = LoadRandomNameFromJson();
            return string.IsNullOrEmpty(randomName) ? $"{prefix}_{UnityEngine.Random.Range(100, 999)}" : $"{prefix}_{randomName}";
        }

        private string LoadRandomNameFromJson()
        {
            try
            {
                string path = Path.Combine(Application.dataPath, "Data", "npc_names.json");
                if (!File.Exists(path))
                {
                    return string.Empty;
                }

                string json = File.ReadAllText(path);
                List<NpcNameEntry> names = JsonUtility.FromJson<NpcNameListWrapper>($"{{\"items\":{json}}}").items;
                if (names == null || names.Count == 0)
                {
                    return string.Empty;
                }

                NpcNameEntry selectedEntry = names[UnityEngine.Random.Range(0, names.Count)];
                return string.IsNullOrEmpty(selectedEntry.lastName)
                    ? selectedEntry.firstName
                    : $"{selectedEntry.firstName} {selectedEntry.lastName}";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load NPC names from JSON: {ex.Message}");
                return string.Empty;
            }
        }

        [System.Serializable]
        private class NpcNameEntry
        {
            public string firstName;
            public string lastName;
        }

        [System.Serializable]
        private class NpcNameListWrapper
        {
            public List<NpcNameEntry> items;
        }
    }
}
