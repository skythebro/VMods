using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using Stunlock.Core;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class BuffSystemHook
	{
		#region Events

		public delegate void ProcessBuffEventHandler(Entity entity, PrefabGUID buffGUID);
		public static event ProcessBuffEventHandler ProcessBuffEvent;
		private static void FireProcessBuffEvent(Entity entity, PrefabGUID buffGUID) => ProcessBuffEvent?.Invoke(entity, buffGUID);

		#endregion

		#region Private Methods

		[HarmonyPatch(typeof(BuffSystem_Spawn_Server), nameof(BuffSystem_Spawn_Server.OnUpdate))]
		[HarmonyPrefix]
		private static void OnUpdate(BuffSystem_Spawn_Server __instance)
		{
			if(!Utils.IsServer || __instance.__query_401358634_0.IsEmpty)
			{
				return;
			}

			var entityManager = __instance.EntityManager;

			var entities = __instance.__query_401358634_0.ToEntityArray(Allocator.Temp);
			foreach(var entity in entities)
			{
				entityManager.TryGetComponentData<PrefabGUID>(entity, out var buffGUID);
				FireProcessBuffEvent(entity, buffGUID);
			}
		}

		#endregion
	}
}
