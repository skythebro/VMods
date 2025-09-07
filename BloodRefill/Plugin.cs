using System;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using VMods.Shared;
using VMods.BloodRefill.Commands;

namespace VMods.BloodRefill
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        #region Variables

        private Harmony _hooks;
        
        public static bool _hasInitialized = false;

        #endregion

        #region Public Methods

        public sealed override void Load()
        {
            if (!Utils.IsServer)
            {
                Log.LogMessage($"{MyPluginInfo.PLUGIN_NAME} only needs to be installed server side.");
                return;
            }

            Utils.Initialize(Log, MyPluginInfo.PLUGIN_NAME);
            BloodRefillConfig.Initialize(Config);
            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnGameInitialized()
        {
            if (_hasInitialized) return;
            try
            {
                if (Utils.IsClient)
                {
                    return;
                }

                BloodRefillSystem.Initialize();
                _hasInitialized = true;
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} (v{MyPluginInfo.PLUGIN_VERSION}) is loaded!");
            }
            catch (Exception e)
            {
                Log.LogWarning("An error occured:" + e.Message);
            }
        }

        public sealed override bool Unload()
        {
            if (Utils.IsClient)
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
