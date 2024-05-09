using BepInEx.Logging;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using ProjectM.UI;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using Stunlock.Core;
using Stunlock.Localization;

namespace VMods.Shared
{
    public static class Utils
    {
        #region Variables

        private static ComponentType[] _containerComponents = null;

        #endregion

        #region Properties

        public static World CurrentWorld => VWorld.Game;

        public static ManualLogSource Logger { get; private set; }
        public static string PluginName { get; private set; }

        private static ComponentType[] ContainerComponents
        {
            get
            {
                if (_containerComponents == null)
                {
                    _containerComponents = new[]
                    {
                        ComponentType.ReadOnly(Il2CppType.Of<Team>()),
                        ComponentType.ReadOnly(Il2CppType.Of<CastleHeartConnection>()),
                        ComponentType.ReadOnly(Il2CppType.Of<InventoryBuffer>()),
                        ComponentType.ReadOnly(Il2CppType.Of<NameableInteractable>()),
                    };
                }

                return _containerComponents;
            }
        }

        #endregion

        #region Public Methods

        public static void Initialize(ManualLogSource logger, string pluginName)
        {
            Logger = logger;
            PluginName = pluginName;
        }

        public static void Deinitialize()
        {
            Logger = null;
        }

        public static NativeArray<Entity> GetStashEntities(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ContainerComponents);
            return query.ToEntityArray(Allocator.Temp);
        }

        public static IEnumerable<Entity> GetAlliedStashes(EntityManager entityManager, Entity character)
        {
            // GetComponentDataFromEntity no longer exists
            //ComponentDataFromEntity<Team> getTeam = entityManager.GetComponentDataFromEntity<Team>();
            
            foreach (var stash in GetStashEntities(entityManager))
            {
                if (entityManager.HasComponent<Team>(stash) && entityManager.HasComponent<Team>(character))
                {
                    // var stashTeam = getTeam[stash];
                    // var characterTeam = getTeam[character];
                    entityManager.TryGetComponentData<Team>(stash, out var stashTeam);
                    entityManager.TryGetComponentData<Team>(character, out var characterTeam);
                    if (Team.IsAllies(stashTeam, characterTeam))
                    {
                        yield return stash;
                    }
                }
            }
        }

        public static int GetStashItemCount(EntityManager entityManager, Entity character, PrefabGUID itemGUID,
            StoredBlood? storedBlood = null)
        {
            int stashCount = 0;
            int stashesCounted = 0;
            var stashes = GetAlliedStashes(entityManager, character);
            foreach (var stash in stashes)
            {
                entityManager.TryGetBuffer<InventoryBuffer>(stash, out var stashInventory);

                for (int i = 0; i < stashInventory.Length; i++)
                {
                    var stashItem = stashInventory[i];

                    if (stashItem.ItemType == itemGUID)
                    {
                        if (storedBlood != null)
                        {
                            entityManager.TryGetComponentData<StoredBlood>(stashItem.ItemEntity._Entity, out var itemStoredBlood);

                            if (storedBlood.Value.BloodType != itemStoredBlood.BloodType ||
                                storedBlood.Value.BloodQuality != itemStoredBlood.BloodQuality)
                            {
                                continue;
                            }
                        }

                        stashCount += stashItem.Amount;
                    }
                }

                stashesCounted++;
            }

            return stashesCounted == 0 ? -1 : stashCount;
        }

        public static string GetItemName(PrefabGUID itemGUID, GameDataSystem gameDataSystem = null,
            EntityManager? entityManager = null, PrefabLookupMap? prefabLookupMap = null)
        {
            if (itemGUID == PrefabGUID.Empty)
            {
                return string.Empty;
            }

            entityManager ??= CurrentWorld.EntityManager;
            gameDataSystem ??= CurrentWorld.GetExistingSystemManaged<GameDataSystem>();
            prefabLookupMap ??= CurrentWorld.GetExistingSystemManaged<PrefabCollectionSystem>().PrefabLookupMap;
            try
            {
                var itemName = GameplayHelper.TryGetItemName(gameDataSystem, entityManager.Value, prefabLookupMap.Value,
                    itemGUID);
                if (Localization.HasKey(itemName))
                {
                    return Localization.Get(itemName);
                }
            }
            catch (Exception)
            {
            }

            return $"[{itemGUID}]";
        }

        public static void SendMessage(Entity userEntity, string message, ServerChatMessageType messageType)
        {
            if (!VWorld.IsServer)
            {
                return;
            }

            EntityManager em = VWorld.Server.EntityManager;
            em.TryGetComponentData<User>(userEntity, out var user);
            // int index = user.Index;
            // em.TryGetComponentData<NetworkId>(userEntity, out var id);

            // Entity entity = em.CreateEntity(
            //     ComponentType.ReadOnly<NetworkEventType>(),
            //     ComponentType.ReadOnly<SendEventToUser>(),
            //     ComponentType.ReadOnly<ChatMessageServerEvent>()
            // );
            //
            // ChatMessageServerEvent ev = new()
            // {
            //     MessageText = message,
            //     MessageType = messageType,
            //     FromUser = id,
            //     TimeUTC = DateTime.Now.ToFileTimeUtc()
            // };
            //
            // entity.WithComponentDataVmodAOT((ref SendEventToUser us) => us.UserIndex = index);
            //
            // entity.WithComponentDataVmodAOT((ref NetworkEventType ev) =>
            // {
            //     ev.EventId = NetworkEvents.EventId_ChatMessageServerEvent;
            //     ev.IsAdminEvent = false;
            //     ev.IsDebugEvent = false;
            // });
            //
            // em.SetComponentData(entity, ev);
            
            ServerChatUtils.SendSystemMessageToClient(em, user, message);
        }

        public static bool TryGetPrefabGUIDForItemName(GameDataSystem gameDataSystem, LocalizationKey itemName,
            out PrefabGUID prefabGUID)
            => TryGetPrefabGUIDForItemName(gameDataSystem, Localization.Get(itemName), out prefabGUID);

        public static bool TryGetPrefabGUIDForItemName(GameDataSystem gameDataSystem, string itemName,
            out PrefabGUID prefabGUID)
        {
            foreach (var entry in gameDataSystem.ItemHashLookupMap)
            {
                var item = gameDataSystem.ManagedDataRegistry.GetOrDefault<ManagedItemData>(entry.Key);
                if (Localization.Get(item.Name, false) == itemName)
                {
                    prefabGUID = entry.Key;
                    return true;
                }
            }

            prefabGUID = PrefabGUID.Empty;
            return false;
        }

        public static bool TryGiveItem(EntityManager entityManager, NativeParallelHashMap<PrefabGUID, ItemData>? itemDataMap,
            Entity recipient, PrefabGUID itemType, int amount, out int remainingitems, bool dropRemainder = false)
        {
            itemDataMap ??= CurrentWorld.GetExistingSystemManaged<GameDataSystem>().ItemHashLookupMap;
            var itemSettings = AddItemSettings.Create(entityManager, itemDataMap.Value, false, default, default, false, false, dropRemainder);
            AddItemResponse response = InventoryUtilitiesServer.TryAddItem(itemSettings, recipient, itemType, amount);
            remainingitems = response.RemainingAmount;
            return response.Success;
        }

        public static void ApplyBuff(Entity user, Entity character, PrefabGUID buffGUID)
        {
            ApplyBuff(new FromCharacter()
            {
                User = user,
                Character = character,
            }, buffGUID);
        }

        public static void ApplyBuff(FromCharacter fromCharacter, PrefabGUID buffGUID)
        {
            var des = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
            var buffEvent = new ApplyBuffDebugEvent()
            {
                BuffPrefabGUID = buffGUID
            };
            des.ApplyBuff(fromCharacter, buffEvent);
        }

        public static void RemoveBuff(FromCharacter fromCharacter, PrefabGUID buffGUID)
        {
            RemoveBuff(fromCharacter.Character, buffGUID);
        }

        public static void RemoveBuff(Entity charEntity, PrefabGUID buffGUID, EntityManager? entityManager = null)
        {
            entityManager ??= CurrentWorld.EntityManager;
            if (BuffUtility.HasBuff(entityManager.Value, charEntity, buffGUID))
            {
                BuffUtility.TryGetBuff(entityManager.Value, charEntity, buffGUID, out var buffEntity);
                entityManager.Value.AddComponent<DestroyTag>(buffEntity);
            }
        }

        public static string GetCharacterName(ulong platformId, EntityManager? entityManager = null)
        {
            entityManager ??= CurrentWorld.EntityManager;
            var users = entityManager.Value.CreateEntityQuery(ComponentType.ReadOnly<User>())
                .ToEntityArray(Allocator.Temp);
            foreach (var userEntity in users)
            {
                entityManager.Value.TryGetComponentData<User>(userEntity, out var userData);
                if (userData.PlatformId == platformId)
                {
                    return userData.CharacterName.ToString();
                }
            }

            return null;
        }

        public static AdminLevel GetAdminLevel(Entity userEntity, EntityManager? entityManager = null)
        {
            entityManager ??= CurrentWorld.EntityManager;
            var hasAdminUser = entityManager.Value.TryGetComponentData<AdminUser>(userEntity, out var adminUser);
            if (hasAdminUser)
            {
                
                return adminUser.Level;
            }

            return AdminLevel.None;
        }

        public static void LogAllComponentTypes(Entity entity, EntityManager? entityManager = null)
        {
            if (entity == Entity.Null)
            {
                return;
            }

            entityManager ??= CurrentWorld.EntityManager;

            Logger.LogMessage($"---");
            var types = entityManager.Value.GetComponentTypes(entity);
            foreach (var t in types)
            {
                Logger.LogMessage(
                    $"Component Type: {t} (Shared? {t.IsSharedComponent}) | {t.GetManagedType().FullName}");
            }

            Logger.LogMessage($"---");
        }

        public static void LogAllComponentTypes(EntityQuery entityQuery)
        {
            var types = entityQuery.GetQueryTypes();
            Logger.LogMessage($"---");
            foreach (var t in types)
            {
                Logger.LogMessage($"Query Component Type: {t}");
            }

            Logger.LogMessage($"---");
        }

        #endregion

        #region Nested

        private struct FakeNull
        {
            public int value;
            public bool has_value;
        }

        #endregion
    }
}