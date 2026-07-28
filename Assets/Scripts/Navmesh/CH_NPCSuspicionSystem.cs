using System;
using System.Collections.Generic;
using UnityEngine;

namespace CH_AICharacter
{
    [Serializable]
    public class CH_NPCSuspicionTraitDefinition
    {
        [SerializeField] private string id = "";
        [SerializeField] private int weight = 1;
        [SerializeField] private bool visible = true;
        [SerializeField] private List<string> counterTraits = new List<string>();

        public string Id { get => id; set => id = value; }
        public int Weight { get => weight; set => weight = value; }
        public bool Visible { get => visible; set => visible = value; }
        public List<string> CounterTraits { get => counterTraits; set => counterTraits = value; }
    }

    [Serializable]
    public class CH_NPCSuspicionProfile
    {
        [SerializeField] private string id = "";
        [SerializeField] private bool isActuallyDangerous;
        [SerializeField] private string dangerType;
        [SerializeField] private string dangerLevel = "none";
        [SerializeField] private List<CH_NPCSuspicionTraitDefinition> visibleTraits = new List<CH_NPCSuspicionTraitDefinition>();
        [SerializeField] private int trueSuspicionScore;
        [SerializeField] private bool isRedHerring;

        public string Id { get => id; set => id = value; }
        public bool IsActuallyDangerous { get => isActuallyDangerous; set => isActuallyDangerous = value; }
        public string DangerType { get => dangerType; set => dangerType = value; }
        public string DangerLevel { get => dangerLevel; set => dangerLevel = value; }
        public List<CH_NPCSuspicionTraitDefinition> VisibleTraits { get => visibleTraits; set => visibleTraits = value; }
        public int TrueSuspicionScore { get => trueSuspicionScore; set => trueSuspicionScore = value; }
        public bool IsRedHerring { get => isRedHerring; set => isRedHerring = value; }
    }

    [Serializable]
    public class CH_NPCSuspicionConfig
    {
        [Range(0, 100)]
        [SerializeField] private int dangerRate = 15;

        [Range(0, 100)]
        [SerializeField] private int redHerringRate = 20;

        [Range(0, 100)]
        [SerializeField] private int dangerousHighSuspicionChance = 70;

        [Range(0, 100)]
        [SerializeField] private int dangerousLowSuspicionChance = 30;

        [Range(0, 100)]
        [SerializeField] private int safeLowSuspicionChance = 80;

        [Range(0, 100)]
        [SerializeField] private int safeHighSuspicionChance = 20;

        [SerializeField] private List<CH_NPCSuspicionTraitDefinition> traitPool = new List<CH_NPCSuspicionTraitDefinition>();

        [SerializeField] private string[] dangerTypes = new[] { "butcher", "watcher", "drifter" };


        [SerializeField] private bool SetConfigDefaultForAllCharacters = false;

        public int DangerRate => dangerRate;
        public int RedHerringRate => redHerringRate;
        public int DangerousHighSuspicionChance => dangerousHighSuspicionChance;
        public int DangerousLowSuspicionChance => dangerousLowSuspicionChance;
        public int SafeLowSuspicionChance => safeLowSuspicionChance;
        public int SafeHighSuspicionChance => safeHighSuspicionChance;
        public List<CH_NPCSuspicionTraitDefinition> TraitPool => traitPool;
        public string[] DangerTypes => dangerTypes;

        public bool setConfigDefaultForAllCharacters => SetConfigDefaultForAllCharacters;
    }

    public class CH_NPCSuspicionState : MonoBehaviour
    {
        [SerializeField] private CH_NPCSuspicionProfile suspicionProfile;

        public CH_NPCSuspicionProfile SuspicionProfile => suspicionProfile;

        public void Initialize(CH_NPCSuspicionProfile profile)
        {
            suspicionProfile = profile;
        }

        public string GetSummary()
        {
            if (suspicionProfile == null)
            {
                return "No suspicion profile assigned.";
            }

            string traitsText = "none";
            if (suspicionProfile.VisibleTraits != null && suspicionProfile.VisibleTraits.Count > 0)
            {
                List<string> traitIds = new List<string>();
                foreach (CH_NPCSuspicionTraitDefinition trait in suspicionProfile.VisibleTraits)
                {
                    if (trait != null && !string.IsNullOrEmpty(trait.Id))
                    {
                        traitIds.Add(trait.Id);
                    }
                }

                if (traitIds.Count > 0)
                {
                    traitsText = string.Join(", ", traitIds.ToArray());
                }
            }

            return $"danger={suspicionProfile.IsActuallyDangerous} | level={suspicionProfile.DangerLevel} | type={suspicionProfile.DangerType ?? "none"} | score={suspicionProfile.TrueSuspicionScore} | redHerring={suspicionProfile.IsRedHerring} | traits={traitsText}";
        }
    }

    public static class CH_NPCSuspicionSystem
    {
        public static CH_NPCSuspicionProfile GenerateProfile(CH_NPCSuspicionConfig config, string npcId)
        {
            if (config == null)
            {
                config = CreateDefaultConfig();
            }

            if (config.setConfigDefaultForAllCharacters)
            {
                config = CreateDefaultConfig();
            }
            bool isDangerous = RollChance(config.DangerRate);
            bool isHighSuspicion = isDangerous
                ? RollChance(config.DangerousHighSuspicionChance)
                : RollChance(config.SafeHighSuspicionChance);

            bool isRedHerring = !isDangerous && isHighSuspicion && RollChance(config.RedHerringRate);
            string dangerLevel = isDangerous ? "real_threat" : (RollChance(20) ? "minor_incident" : "none");
            string dangerType = null;

            if (isDangerous)
            {
                dangerType = PickDangerType(config);
            }

            List<CH_NPCSuspicionTraitDefinition> visibleTraits = SelectTraits(config, isHighSuspicion || isRedHerring);
            int suspicionScore = 0;
            foreach (CH_NPCSuspicionTraitDefinition trait in visibleTraits)
            {
                if (trait != null)
                {
                    suspicionScore += Mathf.Max(0, trait.Weight);
                }
            }

            return new CH_NPCSuspicionProfile
            {
                Id = string.IsNullOrEmpty(npcId) ? "npc_generated" : npcId,
                IsActuallyDangerous = isDangerous,
                DangerType = dangerType,
                DangerLevel = dangerLevel,
                VisibleTraits = visibleTraits,
                TrueSuspicionScore = suspicionScore,
                IsRedHerring = isRedHerring
            };
        }

        private static bool RollChance(int percentage)
        {
            return UnityEngine.Random.Range(0, 100) < Mathf.Clamp(percentage, 0, 100);
        }

        private static string PickDangerType(CH_NPCSuspicionConfig config)
        {
            if (config == null || config.DangerTypes == null || config.DangerTypes.Length == 0)
            {
                return "butcher";
            }

            return config.DangerTypes[UnityEngine.Random.Range(0, config.DangerTypes.Length)];
        }

        private static List<CH_NPCSuspicionTraitDefinition> SelectTraits(CH_NPCSuspicionConfig config, bool prioritizeSuspicion)
        {
            List<CH_NPCSuspicionTraitDefinition> selectedTraits = new List<CH_NPCSuspicionTraitDefinition>();
            if (config == null || config.TraitPool == null || config.TraitPool.Count == 0)
            {
                return selectedTraits;
            }

            int targetCount = prioritizeSuspicion ? UnityEngine.Random.Range(2, 5) : UnityEngine.Random.Range(1, 3);
            HashSet<string> usedIds = new HashSet<string>();

            for (int i = 0; i < targetCount; i++)
            {
                CH_NPCSuspicionTraitDefinition trait = PickWeightedTrait(config.TraitPool, usedIds);
                if (trait == null)
                {
                    break;
                }

                usedIds.Add(trait.Id);
                selectedTraits.Add(trait);

                if (prioritizeSuspicion && trait.CounterTraits != null && trait.CounterTraits.Count > 0 && UnityEngine.Random.value < 0.4f)
                {
                    CH_NPCSuspicionTraitDefinition counterTrait = FindCounterTrait(config.TraitPool, trait.CounterTraits, usedIds);
                    if (counterTrait != null)
                    {
                        usedIds.Add(counterTrait.Id);
                        selectedTraits.Add(counterTrait);
                    }
                }
            }

            return selectedTraits;
        }

        private static CH_NPCSuspicionTraitDefinition PickWeightedTrait(List<CH_NPCSuspicionTraitDefinition> pool, HashSet<string> usedIds)
        {
            int totalWeight = 0;
            foreach (CH_NPCSuspicionTraitDefinition trait in pool)
            {
                if (trait == null || !trait.Visible || string.IsNullOrEmpty(trait.Id) || usedIds.Contains(trait.Id))
                {
                    continue;
                }

                totalWeight += Mathf.Max(1, trait.Weight);
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = UnityEngine.Random.Range(1, totalWeight + 1);
            int runningWeight = 0;
            foreach (CH_NPCSuspicionTraitDefinition trait in pool)
            {
                if (trait == null || !trait.Visible || string.IsNullOrEmpty(trait.Id) || usedIds.Contains(trait.Id))
                {
                    continue;
                }

                runningWeight += Mathf.Max(1, trait.Weight);
                if (roll <= runningWeight)
                {
                    return trait;
                }
            }

            return null;
        }

        private static CH_NPCSuspicionTraitDefinition FindCounterTrait(List<CH_NPCSuspicionTraitDefinition> pool, List<string> counterTraitIds, HashSet<string> usedIds)
        {
            if (pool == null || counterTraitIds == null || counterTraitIds.Count == 0)
            {
                return null;
            }

            foreach (string counterTraitId in counterTraitIds)
            {
                foreach (CH_NPCSuspicionTraitDefinition trait in pool)
                {
                    if (trait == null || string.IsNullOrEmpty(trait.Id) || usedIds.Contains(trait.Id))
                    {
                        continue;
                    }

                    if (trait.Id == counterTraitId)
                    {
                        return trait;
                    }
                }
            }

            return null;
        }

        private static CH_NPCSuspicionConfig CreateDefaultConfig()
        {
            CH_NPCSuspicionConfig config = new CH_NPCSuspicionConfig();
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "no_eye_contact", Weight = 2, Visible = true, CounterTraits = new List<string> { "shy_apology" } });
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "shy_apology", Weight = -1, Visible = true, CounterTraits = new List<string>() });
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "cash_only", Weight = 1, Visible = true, CounterTraits = new List<string> { "mentions_lost_card" } });
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "mentions_lost_card", Weight = 0, Visible = true, CounterTraits = new List<string>() });
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "muddy_clothes", Weight = 3, Visible = true, CounterTraits = new List<string>() });
            config.TraitPool.Add(new CH_NPCSuspicionTraitDefinition { Id = "no_luggage", Weight = 2, Visible = true, CounterTraits = new List<string>() });
            return config;
        }
    }
}
