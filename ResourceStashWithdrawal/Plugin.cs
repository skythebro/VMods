using HarmonyLib;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using VMods.Shared;

namespace VMods.ResourceStashWithdrawal
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin
    {
        #region Variables

        private Harmony _hooks;

        #endregion

        #region Public Methods

        public sealed override void Load()
        {
            Utils.Initialize(Log, MyPluginInfo.PLUGIN_NAME);
            ResourceStashWithdrawalConfig.Initialize(Config);
            _hooks = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnGameInitialized()
        {
            if (Utils.IsClient)
            {
                UIClickHook.Reset();
            }

            //ResourceStashWithdrawalSystem.Initialize();
            HandleWithdrawItemsCommandSystem.Initialize();
            
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} (v{MyPluginInfo.PLUGIN_VERSION}) is loaded!");
            if (!Utils.IsClient)
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