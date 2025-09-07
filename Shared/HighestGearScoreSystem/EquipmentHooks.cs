using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using Unity.Collections;
using VAMP;

namespace VMods.Shared
{
    [HarmonyPatch]
    public static class EquipmentHooks
    {
        #region Events

        public delegate void EquipmentChangedEventHandler(FromCharacter fromCharacter);

        public static event EquipmentChangedEventHandler EquipmentChangedEvent;

        private static void FireEquipmentChangedEvent(FromCharacter fromCharacter) =>
            EquipmentChangedEvent?.Invoke(fromCharacter);

        #endregion

        #region Private Methods

        [HarmonyPatch(typeof(EquipItemSystem), nameof(EquipItemSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void EquipItem(EquipItemSystem __instance)
        {
            //if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
            if (!Utils.IsServer || __instance.__query_1850505309_0.IsEmpty)
            {
                return;
            }

            var entityManager = Core.Server.EntityManager;
            // var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            var entities = __instance.__query_1850505309_0.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                FireEquipmentChangedEvent(fromCharacter);
            }
        }

        [HarmonyPatch(typeof(EquipItemFromInventorySystem), nameof(EquipItemFromInventorySystem.OnUpdate))]
        [HarmonyPostfix]
        private static void EquipItemFromInventory(EquipItemFromInventorySystem __instance)
        {
            //if(!VWorld.IsServer || __instance.__EquipItemFromInventoryJob_entityQuery.IsEmpty)
            if (!Utils.IsServer || __instance._Query.IsEmpty)
            {
                return;
            }

            var entityManager = Core.Server.EntityManager;
            //var entities = __instance.__EquipItemFromInventoryJob_entityQuery.ToEntityArray(Allocator.Temp);
            var entities = __instance._Query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                FireEquipmentChangedEvent(fromCharacter);
            }
        }

        [HarmonyPatch(typeof(UnEquipItemSystem), nameof(UnEquipItemSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void UnequipItem(UnEquipItemSystem __instance)
        {
            //if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
            if (!Utils.IsServer || __instance.__query_1850505763_0.IsEmpty)
            {
                return;
            }

            var entityManager = Core.Server.EntityManager;
            //var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            var entities = __instance.__query_1850505763_0.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                FireEquipmentChangedEvent(fromCharacter);
            }
        }

        [HarmonyPatch(typeof(MoveItemBetweenInventoriesSystem), nameof(MoveItemBetweenInventoriesSystem.OnUpdate))]
        private static class MoveItemBetweenInventories
        {
            private static void Prefix(MoveItemBetweenInventoriesSystem __instance, out List<FromCharacter> __state)
            {
                __state = new List<FromCharacter>();
                if (!Utils.IsServer || __instance._MoveItemBetweenInventoriesEventQuery.IsEmpty)
                {
                    return;
                }

                var entityManager = Core.Server.EntityManager;
                var entities = __instance._MoveItemBetweenInventoriesEventQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                    if (!__state.Contains(fromCharacter))
                    {
                        __state.Add(fromCharacter);
                    }
                }
            }

            private static void Postfix(List<FromCharacter> __state)
            {
                __state.ForEach(FireEquipmentChangedEvent);
            }
        }

        [HarmonyPatch(typeof(MoveAllItemsBetweenInventoriesSystem),
            nameof(MoveAllItemsBetweenInventoriesSystem.OnUpdate))]
        private static class MoveAllItemsBetweenInventories
        {
            private static void Prefix(MoveAllItemsBetweenInventoriesSystem __instance, out List<FromCharacter> __state)
            {
                __state = new List<FromCharacter>();
                //if(!VWorld.IsServer || __instance.__MoveAllItemsJob_entityQuery.IsEmpty)
                if (!Utils.IsServer || __instance.__query_133601579_0.IsEmpty)
                {
                    return;
                }

                var entityManager = Core.Server.EntityManager;
                //var entities = __instance.__MoveAllItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
                var entities = __instance.__query_133601579_0.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                    if (!__state.Contains(fromCharacter))
                    {
                        __state.Add(fromCharacter);
                    }
                }
            }

            private static void Postfix(List<FromCharacter> __state)
            {
                __state.ForEach(FireEquipmentChangedEvent);
            }
        }

        [HarmonyPatch(typeof(DropInventoryItemSystem), nameof(DropInventoryItemSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void DropInventoryItem(DropInventoryItemSystem __instance)
        {
            //if(!VWorld.IsServer || __instance.__DropInventoryItemJob_entityQuery.IsEmpty)
            if (!Utils.IsServer || __instance._Query.IsEmpty)
            {
                return;
            }

            var entityManager = Core.Server.EntityManager;
            
            //var entities = __instance.__DropInventoryItemJob_entityQuery.ToEntityArray(Allocator.Temp);
            var entities = __instance._Query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                entityManager.TryGetComponentData<DropInventoryItemEvent>(entity, out var dropEvent);
                NetworkIdLookupMap map = Core.SystemService.NetworkIdSystem.GetNetworkIdLookupRW();
                var inventory = dropEvent.Inventory.GetNetworkedEntity(ref map).GetEntityOnServer();
                entityManager.TryGetComponentData<InventoryConnection>(inventory, out var inventoryConnection);
                entityManager.TryGetComponentData<PlayerCharacter>(inventoryConnection.InventoryOwner, out var playerCharacter);
                var fromCharacter = new FromCharacter
                {
                    Character = inventoryConnection.InventoryOwner,
                    User = playerCharacter.UserEntity,
                };
                FireEquipmentChangedEvent(fromCharacter);
            }
        }

        [HarmonyPatch(typeof(DropItemSystem), nameof(DropItemSystem.OnUpdate))]
        private static class DropItem
        {
            private static void Prefix(DropItemSystem __instance, out List<FromCharacter> __state)
            {
                __state = new List<FromCharacter>();
                // if (!VWorld.IsServer || __instance.__DropEquippedItemJob_entityQuery.IsEmpty ||
                //     __instance.__DropEquippedItemJob_entityQuery.IsEmpty)
                if (!Utils.IsServer || __instance.__query_1470978519_0.IsEmpty ||
                    __instance.__query_1470978519_0.IsEmpty)
                {
                    return;
                }

                var entityManager = Core.Server.EntityManager;
                //var entities = __instance.__DropEquippedItemJob_entityQuery.ToEntityArray(Allocator.Temp);
                var entities = __instance.__query_1470978519_0.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                    if (!__state.Contains(fromCharacter))
                    {
                        __state.Add(fromCharacter);
                    }
                }

               // entities = __instance.__DropItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
               entities = __instance._EventQuery2.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
                    if (!__state.Contains(fromCharacter))
                    {
                        __state.Add(fromCharacter);
                    }
                }
            }

            private static void Postfix(List<FromCharacter> __state)
            {
                __state.ForEach(FireEquipmentChangedEvent);
            }
        }

        [HarmonyPatch(typeof(ItemPickupSystem), nameof(ItemPickupSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void ItemPickup(ItemPickupSystem __instance)
        {
            //if (!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
            if (!Utils.IsServer || __instance.__query_1414696066_0.IsEmpty)
            {
                return;
            }

            var entityManager = Core.Server.EntityManager;
            //var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            var entities = __instance.__query_1414696066_0.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var ownerData);
                var characterEntity = ownerData.Owner;
                entityManager.TryGetComponentData<PlayerCharacter>(characterEntity, out var playerCharacter);
                FireEquipmentChangedEvent(new FromCharacter()
                {
                    Character = characterEntity,
                    User = playerCharacter.UserEntity,
                });
            }
        }

        #endregion
    }
}