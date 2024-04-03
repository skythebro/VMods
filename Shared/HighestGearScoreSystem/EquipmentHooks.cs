using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System.Collections.Generic;
using Unity.Collections;
using Bloodstone.API;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class EquipmentHooks
	{
		#region Events

		public delegate void EquipmentChangedEventHandler(FromCharacter fromCharacter);
		public static event EquipmentChangedEventHandler EquipmentChangedEvent;
		private static void FireEquipmentChangedEvent(FromCharacter fromCharacter) => EquipmentChangedEvent?.Invoke(fromCharacter);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(EquipItemSystem), nameof(EquipItemSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void EquipItem(EquipItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				
				entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(EquipItemFromInventorySystem), nameof(EquipItemFromInventorySystem.OnUpdate))]
		[HarmonyPostfix]
		private static void EquipItemFromInventory(EquipItemFromInventorySystem __instance)
		{
			if(!VWorld.IsServer || __instance.__EquipItemFromInventoryJob_entityQuery.IsEmpty)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__EquipItemFromInventoryJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(UnEquipItemSystem), nameof(UnEquipItemSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void UnequipItem(UnEquipItemSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
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
				if(!VWorld.IsServer || __instance._MoveItemBetweenInventoriesEventQuery.IsEmpty)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance._MoveItemBetweenInventoriesEventQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
					if(!__state.Contains(fromCharacter))
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

		[HarmonyPatch(typeof(MoveAllItemsBetweenInventoriesSystem), nameof(MoveAllItemsBetweenInventoriesSystem.OnUpdate))]
		private static class MoveAllItemsBetweenInventories
		{
			private static void Prefix(MoveAllItemsBetweenInventoriesSystem __instance, out List<FromCharacter> __state)
			{
				__state = new List<FromCharacter>();
				if(!VWorld.IsServer || __instance.__MoveAllItemsJob_entityQuery.IsEmpty)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance.__MoveAllItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
					if(!__state.Contains(fromCharacter))
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
			if(!VWorld.IsServer || __instance.__DropInventoryItemJob_entityQuery.IsEmpty)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__DropInventoryItemJob_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
				FireEquipmentChangedEvent(fromCharacter);
			}
		}

		[HarmonyPatch(typeof(DropItemSystem), nameof(DropItemSystem.OnUpdate))]
		private static class DropItem
		{
			private static void Prefix(DropItemSystem __instance, out List<FromCharacter> __state)
			{
				__state = new List<FromCharacter>();
				if(!VWorld.IsServer || __instance.__DropEquippedItemJob_entityQuery.IsEmpty || __instance.__DropEquippedItemJob_entityQuery.IsEmpty)
				{
					return;
				}

				var entityManager = VWorld.Server.EntityManager;
				var entities = __instance.__DropEquippedItemJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
					if(!__state.Contains(fromCharacter))
					{
						__state.Add(fromCharacter);
					}
				}

				entities = __instance.__DropItemsJob_entityQuery.ToEntityArray(Allocator.Temp);
				foreach(var entity in entities)
				{
					entityManager.TryGetComponentData<FromCharacter>(entity, out var fromCharacter);
					if(!__state.Contains(fromCharacter))
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
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
			{
				return;
			}

			var entityManager = VWorld.Server.EntityManager;
			var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
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
