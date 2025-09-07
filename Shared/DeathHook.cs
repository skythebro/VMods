using System;
using HarmonyLib;
using ProjectM;
using Unity.Collections;

namespace VMods.Shared
{
    [HarmonyPatch]
    public static class DeathHook
    {
        #region Events

        public delegate void DeathEventHandler(DeathEvent deathEvent);

        public static event DeathEventHandler DeathEvent;
        private static void FireDeathEvent(DeathEvent deathEvent) => DeathEvent?.Invoke(deathEvent);

        #endregion

        #region Public Methods

        [HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
        [HarmonyPostfix]
        private static void OnUpdate(DeathEventListenerSystem __instance)
        {
            try
            {
                if (!Utils.IsServer || __instance._DeathEventQuery.IsEmpty)
                {
                    return;
                }
                
                NativeArray<DeathEvent> deathEvents =
                    __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
                foreach (DeathEvent deathEvent in deathEvents)
                {
                    FireDeathEvent(deathEvent);
                }
            }
            catch (Exception e)
            {
                Utils.Logger.LogError(e.Message);
            }
        }

        #endregion
    }
}