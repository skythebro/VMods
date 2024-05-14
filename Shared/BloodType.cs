using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using Bloodstone.API;
using ProjectM.UI;
using Stunlock.Core;

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
    }

    public static class BloodTypeExtensions
    {
        #region Consts

        public static readonly Dictionary<BloodType, PrefabGUID> BloodTypeToPrefabGUIDMapping = new()
        {
            [BloodType.Creature] = new PrefabGUID(1897056612), //CHAR_Forest_Deer
            [BloodType.Warrior] = new PrefabGUID(-1128238456), //CHAR_Bandit_Bomber
            [BloodType.Rogue] = new PrefabGUID(-1030822544), //CHAR_Bandit_Deadeye
            [BloodType.Brute] = new PrefabGUID(-1464869978), //CHAR_ChurchOfLight_Cleric
            [BloodType.Scholar] = new PrefabGUID(-700632469), //CHAR_Militia_Nun
            [BloodType.Worker] = new PrefabGUID(-1342764880), //CHAR_Farmlands_Farmer
            [BloodType.Mutant] = new PrefabGUID(572729167), //CHAR_Mutant_Wolf
            [BloodType.Draculin] = new PrefabGUID(-669027288), //CHAR_Legion_Guardian_DraculaMinion
        };

        #endregion

        #region Public Methods

        public static bool ParseBloodType(this PrefabGUID prefabGUID, out BloodType bloodType)
        {
            int guidHash = prefabGUID.GuidHash;
            if (!Enum.IsDefined(typeof(BloodType), guidHash))
            {
                
                bloodType = BloodType.Frailed;
                return false;
            }

            bloodType = (BloodType)guidHash;
            return true;
        }

        public static BloodType? ToBloodType(this PrefabGUID prefabGUID)
        {
            int guidHash = prefabGUID.GuidHash;
            if (!Enum.IsDefined(typeof(BloodType), guidHash))
            {
#if DEBUG
                Utils.Logger.LogError($"Failed to convert PrefabGUID '{prefabGUID}' to BloodType.");
#endif
                return null;
            }
#if DEBUG
            Utils.Logger.LogMessage($"Converted PrefabGUID '{prefabGUID}' to BloodType '{(BloodType)guidHash}'.");
#endif

            return (BloodType)guidHash;
        }

        public static PrefabGUID ToPrefabGUID(this BloodType bloodType) => BloodTypeToPrefabGUIDMapping[bloodType];

        public static void ApplyToPlayer(this BloodType bloodType, User user, float quality, int addAmount)
        {
            ConsumeBloodDebugEvent ConsumeBloodEvent = new()
            {
                Source = bloodType.ToPrefabGUID(),
                Quality = quality,
                Amount = addAmount,
            };
            VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>().ConsumeBloodEvent(user.Index, ref ConsumeBloodEvent);
#if DEBUG
            Utils.Logger.LogMessage($"Applied blood type '{bloodType}' to player '{user.Index}' with quality '{quality}' and amount '{addAmount}'.");
#endif
        }

        #endregion
    }
}