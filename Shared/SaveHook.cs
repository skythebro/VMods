using HarmonyLib;
using ProjectM;
//using VMods.BloodRefill.Commands.VCFCompat;

namespace VMods.Shared
{
	[HarmonyPatch]
	public static class SaveHook
	{
		#region Private Methods

		[HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.TriggerSave))]
		[HarmonyPrefix]
		private static void TriggerSave()
		{
			//Commands.BloodRefillCommands.SaveAll();
		}

		[HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnDestroy))]
		[HarmonyPrefix]
		private static void OnDestroy()
		{
			//Commands.BloodRefillCommands.SaveAll();
		}

		#endregion
	}
}
