using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using ProjectM.Shared;
using Stunlock.Core;
using Unity.Entities;
using UnityEngine;
using VAMP;

namespace VMods.Shared
{
    public enum BloodType
    {
        // old bloodtypes?
        // Frailed = -899826404, //(UnitBloodType) BloodType_None
        // Creature = -77658840, //(UnitBloodType) BloodType_Creature
        // Warrior = -1094467405, //(UnitBloodType) BloodType_Warrior
        // Rogue = 793735874, //(UnitBloodType) BloodType_Rogue
        // Brute = 581377887, //(UnitBloodType) BloodType_Brute
        // Scholar = -586506765, //(UnitBloodType) BloodType_Scholar
        // Worker = -540707191, //(UnitBloodType) BloodType_Worker
        // VBlood = 1557174542, //(UnitBloodType) BloodType_VBlood
        // Mutant = -2017994753, //(UnitBloodType) BloodType_Mutant
        Frailed = 447918373, //(UnitBloodType) BloodType_None
        Creature = 524822543, //(UnitBloodType) BloodType_Creature
        Warrior = -516976528, //(UnitBloodType) BloodType_Warrior
        Rogue = -1620185637, //(UnitBloodType) BloodType_Rogue
        Brute = 804798592, //(UnitBloodType) BloodType_Brute
        Scholar = 1476452791, //(UnitBloodType) BloodType_Scholar
        Worker = -1776904174, //(UnitBloodType) BloodType_Worker
        VBlood = -338774148, //(UnitBloodType) BloodType_VBlood
        Mutant = 1821108694, //(UnitBloodType) BloodType_Mutant
        DraculaTheImmortal = 2010023718, //(UnitBloodType) BloodType_DraculaTheImmortal
        Draculin = 1328126535, //(UnitBloodType) BloodType_Draculin
        GateBoss = 910644396, //(UnitBloodType) BloodType_GateBoss
        Corruption = -1382693416, //(UnitBloodType) BloodType_Corruption
    }

    public static class BloodTypeExtensions
    {
        #region Consts

        public static readonly Dictionary<BloodType, PrefabGUID> BloodTypeToPrefabGuidMapping = new()
        {
            [BloodType.Creature] = new PrefabGUID(1897056612), //CHAR_Forest_Deer
            [BloodType.Warrior] = new PrefabGUID(-1128238456), //CHAR_Bandit_Bomber
            [BloodType.Rogue] = new PrefabGUID(-1030822544), //CHAR_Bandit_Deadeye
            [BloodType.Brute] = new PrefabGUID(-1464869978), //CHAR_ChurchOfLight_Cleric
            [BloodType.Scholar] = new PrefabGUID(-700632469), //CHAR_Militia_Nun
            [BloodType.Worker] = new PrefabGUID(-1342764880), //CHAR_Farmlands_Farmer
            [BloodType.Mutant] = new PrefabGUID(572729167), //CHAR_Mutant_Wolf
            [BloodType.Draculin] = new PrefabGUID(-669027288), //CHAR_Legion_Guardian_DraculaMinion
            [BloodType.Corruption] = new PrefabGUID(616274140), // CHAR_Corrupted_Wolf
            [BloodType.DraculaTheImmortal] = new PrefabGUID(-327335305), //CHAR_Vampire_Dracula_VBlood
        };

        #endregion

        #region Public Methods

        public static bool ParseBloodType(this PrefabGUID prefabGuid, out BloodType bloodType)
        {
            int guidHash = prefabGuid.GuidHash;
            if (!Enum.IsDefined(typeof(BloodType), guidHash))
            {
                bloodType = BloodType.Frailed;
                return false;
            }

            bloodType = (BloodType)guidHash;
            return true;
        }

        public static BloodType? ToBloodType(this PrefabGUID prefabGuid)
        {
            int guidHash = prefabGuid.GuidHash;
            if (!Enum.IsDefined(typeof(BloodType), guidHash))
            {
#if DEBUG
                Utils.Logger?.LogError($"Failed to convert PrefabGUID '{prefabGuid}' to BloodType.");
#endif
                return null;
            }
#if DEBUG
            Utils.Logger?.LogInfo($"Converted PrefabGUID '{prefabGuid}' to BloodType '{(BloodType)guidHash}'.");
#endif

            return (BloodType)guidHash;
        }

        public static PrefabGUID ToPrefabGuid(this BloodType bloodType) => BloodTypeToPrefabGuidMapping[bloodType];

        public static void ApplyToPlayer(this BloodType bloodType, Entity userEntity, float quality,
            SecondaryBloodData secondaryBloodData, int addAmount, bool keepSecondaryBuffs)
        {
            var server = Utils.CurrentWorld;
            var em = server.EntityManager;

            if (!em.TryGetComponentData<User>(userEntity, out var user))
            {
                Utils.Logger?.LogError($"ApplyToPlayer: userEntity {userEntity} has no User component.");
                return;
            }

            var charEntity = user.LocalCharacter.GetEntityOnServer();
            if (!em.Exists(charEntity))
            {
                Utils.Logger?.LogError(
                    $"ApplyToPlayer: Character entity {charEntity} does not exist for user {user.Index}.");
                return;
            }

            if (!(quality > 0f && quality <= 100f))
            {
                Utils.Logger?.LogError($"ApplyToPlayer: Primary quality {quality} out of range (0,100].");
                return;
            }

#if DEBUG
            Utils.Logger?.LogInfo(
                $"ApplyToPlayer: user.Index={user.Index}, charEntity={charEntity}, quality={quality}, amount={addAmount}, keepSecondary={keepSecondaryBuffs}");
#endif

            // ConsumeBloodAdminEvent consumeEvent = keepSecondaryBuffs
            //     ? new ConsumeBloodAdminEvent
            //     {
            //         PrimaryType = bloodType.ToPrefabGuid(),
            //         PrimaryQuality = quality,
            //         SecondaryType = secondaryBloodData.Type,
            //         SecondaryQuality = secondaryBloodData.Quality,
            //         SecondaryBuffIndex = secondaryBloodData.BuffIndex,
            //         ApplyTier4SecondaryBuff = Mathf.Approximately(quality, 100f),
            //         Amount = addAmount
            //     }
            //     : new ConsumeBloodAdminEvent
            //     {
            //         PrimaryType = bloodType.ToPrefabGuid(),
            //         PrimaryQuality = quality,
            //         SecondaryType = default,
            //         SecondaryQuality = 0,
            //         SecondaryBuffIndex = 0,
            //         ApplyTier4SecondaryBuff = false,
            //         Amount = addAmount
            //     };

            // var fromCharacter = new FromCharacter
            // {
            //     User = userEntity,
            //     Character = charEntity
            // };
            
            ChangeBloodDebugEvent changeBloodDebugEvent = new ChangeBloodDebugEvent
            {
                Amount = addAmount
            };
            server.GetExistingSystemManaged<DebugEventsSystem>().ChangeBloodEvent(user.Index, ref changeBloodDebugEvent);
            // ConsumeBloodEvent method has been removed in v1.1, no clue how to change types or apply secondary blood changes.
            // server.GetExistingSystemManaged<DebugEventsSystem>().ConsumeBloodEvent(user.Index, ref ConsumeBloodEvent);
#if DEBUG
            Utils.Logger?.LogMessage($"Applied blood type '{bloodType}' to player '{user.Index}' with quality '{quality}' and amount '{addAmount}'.");
#endif
        }

        #endregion
    }
}

