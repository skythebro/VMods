using System;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using VMods.Shared;
using Bloodstone.API;
using VMods.BloodRefill.Commands;

namespace VMods.BloodRefill
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
            if (!VWorld.IsServer)
            {
                Log.LogMessage($"{MyPluginInfo.PLUGIN_NAME} only needs to be installed server side.");
                return;
            }

            Utils.Initialize(Log, MyPluginInfo.PLUGIN_NAME);

            BloodRefillConfig.Initialize(Config);
        }

        public void OnGameInitialized()
        {
            try
            {
                if (VWorld.IsClient)
                {
                    return;
                }

                BloodRefillSystem.Initialize();
                _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} (v{MyPluginInfo.PLUGIN_VERSION}) is loaded!");


                Log.LogInfo("Looking if VCF is installed:");
                if (Commands.VCFCompat.Commands.Enabled)
                {
                    Commands.VCFCompat.Commands.Register();
                }
                else
                {
                    Log.LogWarning("This mod has optional admin commands, you need to install VampireCommandFramework to use them.");
                }
            }
            catch (Exception e)
            {
                Log.LogWarning("An error occured:" + e.Message);
            }
        }

        public sealed override bool Unload()
        {
            if (VWorld.IsClient)
            {
                return true;
            }

            SaveSystem.SaveAll();

            _hooks?.UnpatchSelf();
            BloodRefillSystem.Deinitialize();
            Config.Clear();
            Utils.Deinitialize();
            return true;
        }

        #endregion
    }
}