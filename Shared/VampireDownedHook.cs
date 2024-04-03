using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class VampireDownedHook
	{
		#region Events

		public delegate void VampireDownedByVampireEventHandler(Entity killer, Entity victim);
		public static event VampireDownedByVampireEventHandler VampireDownedByVampireEvent;
		private static void FireVampireDownedByVampireEvent(Entity killer, Entity victim) => VampireDownedByVampireEvent?.Invoke(killer, victim);

		public delegate void VampireDownedByMonsterEventHandler(Entity killer, Entity victim);
		public static event VampireDownedByMonsterEventHandler VampireDownedByMonsterEvent;
		private static void FireVampireDownedByMonsterEvent(Entity killer, Entity victim) => VampireDownedByMonsterEvent?.Invoke(killer, victim);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
		[HarmonyPostfix]
		private static void OnUpdate(VampireDownedServerEventSystem __instance)
		{
			if(!VWorld.IsServer || __instance.__OnUpdate_LambdaJob0_entityQuery.IsEmpty)
			{
				return;
			}

			EntityManager entityManager = __instance.EntityManager;
			var eventsQuery = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);

			foreach(var entity in eventsQuery)
			{
				VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, entityManager, out var victim);
				entityManager.TryGetComponentData<VampireDownedBuff>(entity, out var sourceEntity);
				Entity source = sourceEntity.Source;
				VampireDownedServerEventSystem.TryFindRootOwner(source, 1, entityManager, out var killer);

				if(killer.Equals(victim))
				{
					continue;
				}

				var hasVictimPlayerCharacter = entityManager.TryGetComponentData<PlayerCharacter>(victim, out _);
				if(hasVictimPlayerCharacter)
				{
					var hasKillerPlayerCharacter = entityManager.TryGetComponentData<PlayerCharacter>(killer, out _);
					var hasKillerUnitLevel = entityManager.TryGetComponentData<UnitLevel>(killer, out _);
					if(hasKillerPlayerCharacter)
					{
						FireVampireDownedByVampireEvent(killer, victim);
					}
					else if(hasKillerUnitLevel)
					{
						FireVampireDownedByMonsterEvent(killer, victim);
					}
				}
			}
		}

		#endregion
	}
}
