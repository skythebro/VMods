using BepInEx.Configuration;

namespace VMods.BloodRefill
{
    public static class BloodRefillConfig
    {
        #region Properties

        public static ConfigEntry<bool> BloodRefillEnabled { get; private set; }
        
        public static ConfigEntry<bool> BloodRefillKeepSecondaryBloodBuffs { get; private set; }
        public static ConfigEntry<bool> BloodRefillCreatureEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillWarriorEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillRogueEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillBruteEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillScholarEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillWorkerEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillMutantEnabled { get; private set; }

        public static ConfigEntry<bool> BloodRefillVBloodEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillDraculinEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillGateBossEnabled { get; private set; }
        public static ConfigEntry<bool> BloodRefillDraculaTheImmortalEnabled { get; private set; }
        
        public static ConfigEntry<bool> BloodRefillCorruptionEnabled { get; private set; }
        
        public static ConfigEntry<bool> BloodRefillRequiresFeeding { get; private set; }

        public static ConfigEntry<bool> BloodRefillRequiresSameBloodType { get; private set; }

        public static ConfigEntry<bool> BloodRefillExcludeVBloodFromSameBloodTypeCheck { get; private set; }

        public static ConfigEntry<float> BloodRefillDifferentBloodTypeMultiplier { get; private set; }

        public static ConfigEntry<int> BloodRefillVBloodRefillType { get; private set; }

        public static ConfigEntry<int> BloodRefillBloodRefillType { get; private set; }

        public static ConfigEntry<float> BloodRefillVBloodRefillMultiplier { get; private set; }

        public static ConfigEntry<bool> BloodRefillRandomRefill { get; private set; }

        public static ConfigEntry<float> BloodRefillAmount { get; private set; }

        public static ConfigEntry<float> BloodRefillMultiplier { get; private set; }

        public static ConfigEntry<bool> BloodRefillSendRefillMessage { get; private set; }

        public static ConfigEntry<float> BloodRefillBloodCutoffThreshold { get; private set; }
        public static ConfigEntry<float> BloodRefillVBloodCutoffThreshold { get; private set; }

        #endregion

        #region Config initialization

        public static void Initialize(ConfigFile config)
        {
            BloodRefillEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillEnabled), true,
                "Enabled/disable the blood refilling system.");
            
            BloodRefillKeepSecondaryBloodBuffs = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillKeepSecondaryBloodBuffs),
                true, "When enabled, secondary blood buffs are kept when refilling blood.");

            BloodRefillRequiresFeeding = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRequiresFeeding),
                true, "When enabled, blood can only be refilled when feeding (i.e. when aborting the feed).");

            BloodRefillRequiresSameBloodType = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillRequiresSameBloodType), true,
                "When enabled, blood can only be refilled when the target has the same blood type.");

            BloodRefillCreatureEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillCreatureEnabled),
                true, "When enabled, creature blood can be refilled.");
            BloodRefillWarriorEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillWarriorEnabled),
                true, "When enabled, warrior blood can be refilled.");
            BloodRefillRogueEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRogueEnabled), true,
                "When enabled, rogue blood can be refilled.");
            BloodRefillBruteEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillBruteEnabled), true,
                "When enabled, brute blood can be refilled.");
            BloodRefillScholarEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillScholarEnabled),
                true, "When enabled, scholar blood can be refilled.");
            BloodRefillScholarEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillScholarEnabled),
                true, "When enabled, scholar blood can be refilled.");
            BloodRefillWorkerEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillWorkerEnabled),
                true, "When enabled, worker blood can be refilled.");
            BloodRefillMutantEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillMutantEnabled),
                true, "When enabled, mutant blood can be refilled.");
            BloodRefillVBloodEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillVBloodEnabled), true,
                "When enabled, V-blood can be refilled.");
            BloodRefillDraculinEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillDraculinEnabled), true,
                "When enabled, Draculin blood can be refilled.");
            BloodRefillGateBossEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillGateBossEnabled), true,
                "When enabled, GateBoss blood can be refilled.");
            BloodRefillDraculaTheImmortalEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillDraculaTheImmortalEnabled), true,
                "When enabled, DraculaTheImmortal blood can be refilled.");
            BloodRefillCorruptionEnabled = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillCorruptionEnabled), true,
                "When enabled, Corruption blood can be refilled.");
            BloodRefillExcludeVBloodFromSameBloodTypeCheck = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillExcludeVBloodFromSameBloodTypeCheck), true,
                "When enabled, V-blood is excluded from the 'same blood type' check (i.e. it's always considered to be 'the same blood type' as the player's blood type).");

            BloodRefillVBloodRefillType = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillVBloodRefillType), 3,
                $"0 = disabled (i.e. normal refill); 1 = fully refill; 2 = refill based on V-blood monster level; 3 = refill based on V-blood monster level but not if the player is above {nameof(BloodRefillVBloodCutoffThreshold)} % of blood quality. only works when {nameof(BloodRefillVBloodEnabled)} is enabled.");

            BloodRefillBloodRefillType = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillBloodRefillType), 1,
                $"0 = disabled (i.e. normal refill); 1 = normal refill but not if the player is above {nameof(BloodRefillBloodCutoffThreshold)} % of blood quality.");

            BloodRefillBloodCutoffThreshold = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillBloodCutoffThreshold), 95f,
                $"[Only applies when {nameof(BloodRefillBloodRefillType)} is set to 1] The blood quality percentage cutoff threshold (i.e. if the player's blood quality is above this threshold, npc blood will not refill the player's blood).");

            BloodRefillVBloodCutoffThreshold = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillVBloodCutoffThreshold), 95f,
                $"[Only applies when {nameof(BloodRefillVBloodRefillType)} is set to 3] The blood quality percentage cutoff threshold (i.e. if the player's blood quality is above this threshold, V-blood will not refill the player's blood).");

            BloodRefillVBloodRefillMultiplier = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillVBloodRefillMultiplier), 0.1f,
                $"[Only applies when {nameof(BloodRefillVBloodRefillType)} is set to 2 or 3] The multiplier used in the V-blood refill calculation ('EnemyLevel' * '{nameof(BloodRefillVBloodRefillMultiplier)}' * '{nameof(BloodRefillMultiplier)}').");

            BloodRefillRandomRefill = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillRandomRefill), true,
                "When enabled, the amount of refilled blood is randomized (between 1 and the calculated refillable amount).");

            BloodRefillAmount = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillAmount), 1.0f,
                "The maximum amount of blood to refill with no level difference, a matching blood type and quality (Expressed in Litres of blood).");

            BloodRefillMultiplier = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillMultiplier), 0.5f,
                $"The multiplier used in the blood refill calculation. [Formula: (('Enemy Level' / 'Player Level') * ((100 - ('Player Blood Quality %' - 'Enemy Blood Quality %')) / 100)) * '{nameof(BloodRefillAmount)}' * '(If applicable) {nameof(BloodRefillDifferentBloodTypeMultiplier)}' * '{nameof(BloodRefillMultiplier)}']");

            BloodRefillDifferentBloodTypeMultiplier = config.Bind(nameof(BloodRefillConfig),
                nameof(BloodRefillDifferentBloodTypeMultiplier), 0.1f,
                $"The multiplier used in the blood refill calculation as a penalty for feeding on a different blood type (only works when {nameof(BloodRefillRequiresSameBloodType)} is disabled).");

            BloodRefillSendRefillMessage = config.Bind(nameof(BloodRefillConfig), nameof(BloodRefillSendRefillMessage),
                false,
                "When enabled, a refill chat message is sent to the player.");
        }

        #endregion
    }
}