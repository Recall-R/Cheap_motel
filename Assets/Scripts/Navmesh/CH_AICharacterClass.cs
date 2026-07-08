using UnityEngine;
using UnityEngine.AI;

namespace CH_AICharacter
{
    [System.Serializable]
    public class CH_AICharacterClass
    {
        public enum CH_AICharacterType
        {
            CH_KillerButhcher,
            CH_Guest
        }

        [SerializeField]
        private CH_AICharacterType characterType = CH_AICharacterType.CH_KillerButhcher;

        [SerializeField]
        private GameObject killerPrefab;

        [SerializeField]
        private GameObject guestPrefab;

        [SerializeField]
        private string characterName = "";

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
            GameObject prefab = characterType == CH_AICharacterType.CH_KillerButhcher
                ? killerPrefab
                : guestPrefab;

            if (prefab == null)
            {
                Debug.LogWarning($"Character prefab for {characterType} is not assigned.");
                return;
            }

            GameObject instance = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            instance.name = string.IsNullOrEmpty(characterName) ? characterType.ToString() : characterName;
            AttachMovementDriver(instance);
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
    }
}
