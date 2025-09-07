using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using Unity.Entities;
using VAMP;
using VMods.BloodRefill.Commands;
using VMods.Shared;
using Random = UnityEngine.Random;

namespace VMods.BloodRefill
{
    public static class BloodRefillSystem
    {
        #region Consts

        private const string BloodRefillFileName = "BloodRefill.json";

        #endregion

        #region Variables

        private static Dictionary<ulong, BloodRefillData> _bloodRefills;

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            _bloodRefills = SaveSystem.Load(BloodRefillFileName, () => new Dictionary<ulong, BloodRefillData>());
            SaveSystem.SaveEvent += Save;
            DeathHook.DeathEvent += OnDeath;
        }

        public static void Deinitialize()
        {
            DeathHook.DeathEvent -= OnDeath;
            SaveSystem.SaveEvent -= Save;
        }

        public static void Save()
        {
            SaveSystem.Save(BloodRefillFileName, _bloodRefills);
        }

        #endregion

        #region Private Methods

        private static void OnDeath(DeathEvent deathEvent)
        {
            try
            {
                EntityManager entityManager = Core.Server.EntityManager;
                // Make sure a player killed an appropriate monster
                var hasPlayerCharacter = entityManager.TryGetComponentData<PlayerCharacter>(deathEvent.Killer, out _);
                var hasEquipment = entityManager.TryGetComponentData<Equipment>(deathEvent.Killer, out _);
                var hasBlood = entityManager.TryGetComponentData<Blood>(deathEvent.Killer, out _);
                var hasMovement = entityManager.TryGetComponentData<Movement>(deathEvent.Died, out _);
                var hasUnitLevel = entityManager.TryGetComponentData<UnitLevel>(deathEvent.Died, out _);
                var hasBloodConsumeSource =
                    entityManager.TryGetComponentData<BloodConsumeSource>(deathEvent.Died, out _);
                if (!BloodRefillConfig.BloodRefillEnabled.Value ||
                    !hasPlayerCharacter ||
                    !hasEquipment ||
                    !hasBlood ||
                    !hasMovement ||
                    !hasUnitLevel ||
                    !hasBloodConsumeSource)
                {
                    return;
                }

                entityManager.TryGetComponentData<PlayerCharacter>(deathEvent.Killer, out var playerCharacter);
                entityManager.TryGetComponentData<Equipment>(deathEvent.Killer, out var playerEquipment);
                entityManager.TryGetComponentData<Blood>(deathEvent.Killer, out var playerBlood);
                entityManager.TryGetComponentData<UnitLevel>(deathEvent.Died, out var unitLevel);
                entityManager.TryGetComponentData<BloodConsumeSource>(deathEvent.Died, out var bloodConsumeSource);

#if DEBUG

                Utils.Logger.LogMessage($"DE.Killer = {deathEvent.Killer.Index}");
                Utils.Logger.LogMessage($"DE.Killer = {deathEvent.Killer.ToString()}");
                Utils.Logger.LogMessage($"DE.Died = {deathEvent.Died.Index}");
                Utils.Logger.LogMessage($"DE.Died = {deathEvent.Died.ToString()}");
                Utils.Logger.LogMessage($"DE.Source = {deathEvent.Source.Index}");
                Utils.Logger.LogMessage($"DE.Source = {deathEvent.Source.ToString()}");
#endif

                Entity userEntity = playerCharacter.UserEntity;
                entityManager.TryGetComponentData<User>(userEntity, out var user);

                //bool killedByFeeding = deathEvent.Killer.Index == deathEvent.Source.Index && deathEvent.Source.Index != 0;
                // hopefully this is a good fix for the above line not working
                bool killedByFeeding = deathEvent.Source.Index == 0 || deathEvent.Killer.Index == deathEvent.Source.Index;

                if (!playerBlood.BloodType.ParseBloodType(out BloodType playerBloodType))
                {
#if DEBUG
                    Utils.Logger.LogMessage($"Player Blood Type: {playerBlood.BloodType._Value}");
                    Utils.Logger.LogMessage($"Invalid or unknown blood type");
#endif
                    // Invalid/unknown blood type
                    return;
                }

                if (!bloodConsumeSource.UnitBloodType._Value.ParseBloodType(out BloodType bloodType))
                {
#if DEBUG
                    Utils.Logger.LogMessage($"Enemy Blood Type: {bloodConsumeSource.UnitBloodType._Value}");
#endif

                    // Invalid/unknown blood type
                    return;
                }

                if (!BloodRefillConfig.BloodRefillCreatureEnabled.Value && bloodType == BloodType.Creature)
                {
                    // Creatures don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillWarriorEnabled.Value && bloodType == BloodType.Warrior)
                {
                    // Warriors don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillRogueEnabled.Value && bloodType == BloodType.Rogue)
                {
                    // Rogues don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillBruteEnabled.Value && bloodType == BloodType.Brute)
                {
                    // Brutes don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillScholarEnabled.Value && bloodType == BloodType.Scholar)
                {
                    // Scholars don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillWorkerEnabled.Value && bloodType == BloodType.Worker)
                {
                    // Workers don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillMutantEnabled.Value && bloodType == BloodType.Mutant)
                {
                    // Mutants don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillVBloodEnabled.Value && bloodType == BloodType.VBlood)
                {
                    // V-Bloods don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillDraculinEnabled.Value && bloodType == BloodType.Draculin)
                {
                    // Draculins don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillGateBossEnabled.Value && bloodType == BloodType.GateBoss)
                {
                    // GateBoss's don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillDraculaTheImmortalEnabled.Value &&
                    bloodType == BloodType.DraculaTheImmortal)
                {
                    // DraculaTheImmortal's don't refill blood
                    return;
                }

                if (!BloodRefillConfig.BloodRefillCorruptionEnabled.Value &&
                    bloodType == BloodType.Corruption)
                {
                    // Corruption's don't refill blood
                    return;
                }

                bool isVBlood = bloodType == BloodType.VBlood;

                // Allow V-Bloods to skip the 'killed by feeding' check, otherwise additional feeders won't get a refill.
                if (!isVBlood && BloodRefillConfig.BloodRefillRequiresFeeding.Value && !killedByFeeding)
                {
                    // Can only gain blood when killing the enemy while feeding (i.e. abort the feed)
                    return;
                }

                bool isSameBloodType = playerBloodType == bloodType ||
                                       (BloodRefillConfig.BloodRefillExcludeVBloodFromSameBloodTypeCheck.Value &&
                                        isVBlood);

                if (BloodRefillConfig.BloodRefillRequiresSameBloodType.Value && !isSameBloodType)
                {
#if DEBUG
                    Utils.Logger.LogMessage($"Same Blood Type: {isSameBloodType}");
#endif
                    // Can only gain blood when killing an enemy of the same blood type
                    return;
                }
                
#if DEBUG
                Utils.Logger.LogMessage($"Player Blood Type: {playerBloodType}");
                Utils.Logger.LogMessage($"Enemy Blood Type: {bloodType}");
#endif

                float bloodTypeMultiplier =
                    isSameBloodType ? 1f : BloodRefillConfig.BloodRefillDifferentBloodTypeMultiplier.Value;

                float playerLevel = playerEquipment.WeaponLevel + playerEquipment.ArmorLevel +
                                    playerEquipment.SpellLevel;
                float enemyLevel = unitLevel.Level;


#if DEBUG
                Utils.Logger.LogMessage($"Player Blood Quality: {playerBlood.Quality}");
                Utils.Logger.LogMessage($"Player Blood Value: {playerBlood.Value}");
                Utils.Logger.LogMessage($"Player Level: {playerLevel}");

                Utils.Logger.LogMessage($"Enemy Blood Quality: {bloodConsumeSource.BloodQuality}");
                Utils.Logger.LogMessage($"Enemy Level {enemyLevel}");
#endif

                float levelRatio = enemyLevel / playerLevel;

                float qualityRatio = (100f - (playerBlood.Quality - bloodConsumeSource.BloodQuality)) / 100f;

                float refillRatio = levelRatio * qualityRatio;

                // Config amount is expressed in 'Litres of blood' -> the game's formule is 'blood value / 10', hence the * 10 multiplier here.
                float refillAmount = BloodRefillConfig.BloodRefillAmount.Value * 10f * refillRatio;

                refillAmount *= bloodTypeMultiplier;


#if DEBUG
                Utils.Logger.LogMessage($"Lvl Ratio: {levelRatio}");
                Utils.Logger.LogMessage($"Quality Ratio: {qualityRatio}");
                Utils.Logger.LogMessage($"Refill Ratio: {refillRatio}");
                Utils.Logger.LogMessage($"Blood Type Multiplier: {bloodTypeMultiplier}");
                Utils.Logger.LogMessage($"Refill Amount: {refillAmount}");
#endif

                if (BloodRefillConfig.BloodRefillRandomRefill.Value)
                {
                    refillAmount = Random.RandomRange(1f, refillAmount);

#if DEBUG
                    Utils.Logger.LogMessage($"Refill Roll: {refillAmount}");
#endif
                }

                switch (isVBlood)
                {
                    case false:
                        switch (BloodRefillConfig.BloodRefillBloodRefillType.Value)
                        {
                            case 0: // Disables this setting
                                break;
                            case 1
                                : // Enemy refills blood but not if the player is above BloodRefillBloodCutoffThreshold % of blood quality
                                if (playerBlood.Quality > BloodRefillConfig.BloodRefillBloodCutoffThreshold.Value)
                                {
#if DEBUG
                                    Utils.Logger.LogMessage($"Blood Quality: {playerBlood.Quality}");
                                    Utils.Logger.LogMessage($"Blood Cutoff Threshold: {BloodRefillConfig.BloodRefillBloodCutoffThreshold.Value}");
#endif
                                    // no blood if you have more than the threshold.
                                    return;
                                }

                                break;
                        }

                        break;
                    case true:
                        switch (BloodRefillConfig.BloodRefillVBloodRefillType.Value)
                        {
                            case 1: // V-blood fully refills the blood pool
                                refillAmount = playerBlood.MaxBlood - playerBlood.Value;
                                break;

                            case 2: // V-blood refills based on the unit's level
                                refillAmount = enemyLevel * BloodRefillConfig.BloodRefillVBloodRefillMultiplier.Value;
                                break;
                            case 3
                                : // V-blood refills based on the unit's level but not if the player is above BloodRefillVBloodCutoffThreshold% of blood quality
                                if (playerBlood.Quality > BloodRefillConfig.BloodRefillVBloodCutoffThreshold.Value)
                                {
                                    // no blood if you have more than the threshold.
                                    return;
                                }

                                refillAmount = enemyLevel * BloodRefillConfig.BloodRefillVBloodRefillMultiplier.Value;

                                break;
                        }

                        break;
                }

                refillAmount *= BloodRefillConfig.BloodRefillMultiplier.Value;

                bool sendMessage = BloodRefillConfig.BloodRefillSendRefillMessage.Value &&
                                   (!_bloodRefills.TryGetValue(user.PlatformId, out var bloodRefillData) ||
                                    bloodRefillData.ShowRefillMessages);

                if (refillAmount > 0f)
                {
                    int roundedRefillAmount = (int)Math.Ceiling(refillAmount);

                    if (roundedRefillAmount > 0)
                    {
#if DEBUG
                        Utils.Logger.LogMessage($"New Blood Amount: {playerBlood.Value + roundedRefillAmount}");
#endif

                        if (sendMessage)
                        {
                            float newTotalBlood = Math.Min(playerBlood.MaxBlood,
                                playerBlood.Value + roundedRefillAmount);
                            float actualBloodGained = newTotalBlood - playerBlood.Value;
                            float refillAmountInLitres = (int)(actualBloodGained * 10f) / 100f;
                            float newTotalBloodInLitres = (int)Math.Round(newTotalBlood) / 10f;
                            Utils.SendMessage(userEntity, $"+{refillAmountInLitres}L Blood ({newTotalBloodInLitres}L)",
                                ServerChatMessageType.Lore);
                        }

                        playerBloodType.ApplyToPlayer(userEntity, playerBlood.Quality, playerBlood.SecondaryBlood, roundedRefillAmount, BloodRefillConfig.BloodRefillKeepSecondaryBloodBuffs.Value);
                        return;
                    }
                }

                if (sendMessage)
                {
                    Utils.SendMessage(userEntity, $"No blood gained from the enemy.", ServerChatMessageType.Lore);
                }
            }
            catch (Exception e)
            {
                Utils.Logger.LogError(e.Message);
            }
        }

        #endregion

        #region Nested

        public class BloodRefillData
        {
            public bool ShowRefillMessages { get; set; }
        }

        #endregion
    }
}