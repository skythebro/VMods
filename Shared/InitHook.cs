#if BLOODREFILL
using VMods.BloodRefill;
#elif RESOURCESTASHWITHDRAWAL
using VMods.ResourceStashWithdrawal;
#endif
using HarmonyLib;
using ProjectM;

namespace VMods.Shared;

[HarmonyPatch(typeof(GameBootstrap), nameof(GameBootstrap.Start))]
public static class InitHook
{
    public static void Postfix()
    {
#if BLOODREFILL || RESOURCESTASHWITHDRAWAL
        var plugin = new Plugin();
        plugin.OnGameInitialized();
#else
            // No plugin symbol defined for this build - nothing to initialize.
#endif
    }
}