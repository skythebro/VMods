using System;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using VMods.Shared;
using Bloodstone.API;

namespace VMods.ResourceStashWithdrawal
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.Bloodstone")]
    [Reloadable]
    public class Plugin : BasePlugin, IRunOnInitialized
    {
        #region Variables

        private Harmony _hooks;

        #endregion

        #region Public Methods

        public sealed override void Load()
        {
            Utils.Initialize(Log, MyPluginInfo.PLUGIN_NAME);
            ResourceStashWithdrawalConfig.Initialize(Config);
        }

        public void OnGameInitialized()
        {
            if (VWorld.IsClient)
            {
                UIClickHook.Reset();
            }

            ResourceStashWithdrawalSystem.Initialize();

            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} (v{MyPluginInfo.PLUGIN_VERSION}) is loaded!");
        }

        public sealed override bool Unload()
        {
            _hooks?.UnpatchSelf();
            ResourceStashWithdrawalSystem.Deinitialize();
            Config.Clear();
            Utils.Deinitialize();
            return true;
        }

        #endregion
    }
}