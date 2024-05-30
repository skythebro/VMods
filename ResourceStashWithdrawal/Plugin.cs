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
    [BepInDependency("gg.deca.VampireCommandFramework", BepInDependency.DependencyFlags.SoftDependency)]
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

            //ResourceStashWithdrawalSystem.Initialize();
            HandleWithdrawItemsCommandSystem.Initialize();

            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} (v{MyPluginInfo.PLUGIN_VERSION}) is loaded!");
            if (!VWorld.IsClient)
            {
            Log.LogInfo("Trying to find VCF:");
            
                if (Commands.Commands.Enabled)
                {
                    Commands.Commands.Register();
                }
                else
                {
                    Log.LogError("This mod has commands, you need to install VampireCommandFramework to use them.");
                }
            }
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