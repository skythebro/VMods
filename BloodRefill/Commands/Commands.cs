using BepInEx.Unity.IL2CPP;
using VampireCommandFramework;
using VMods.Shared;

namespace VMods.BloodRefill.Commands.VCFCompat;

public static partial class Commands
{
    static Commands()
    {
        Enabled = IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.deca.VampireCommandFramework", out var info);
        if (Enabled) Utils.Logger.LogWarning($"VCF Version: {info.Metadata.Version}");
    }

    public static bool Enabled { get; private set; }

    public static void Register() => CommandRegistry.RegisterAll();

    [CommandGroup("bloodrefill", "blrf")]
    public class BloodRefillCommands
    {
        [Command("saveall", "svall", "bloodrefill saveall/svall", "Saves all data of all VMod plugins", null, true)]
        private static void OnSaveAllCommand(ChatCommandContext command)
        {
            SaveSystem.SaveAll();
            command.Reply($"VMod Plugin '{Utils.PluginName}' saved successfully.");
        }
        /*
        [Command(
            "togglebloodrefillmessages",
            "tbrm", "togglebloodrefillmessages(tbrm) [on/off]","Toggles the Blood Refill chat messages (on/off)", null, true)]

        private static void OnToggleBloodRefillMessages(ChatCommandContext command, string args)
        {
            var platformId = command.User.PlatformId;
            BloodRefillSystem.BloodRefillData bloodRefillData = null;

            bool? showRefillMessages = null;
            if (args.Length >= 1)
            {
                switch (args)
                {
                    case "on":
                    case "true":
                    case "1":
                        showRefillMessages = true;
                        break;

                    case "off":
                    case "false":
                    case "0":
                        showRefillMessages = false;
                        break;

                    default:
                        command.Error("<color=#ff0000>Invalid toggle options. Options are: on, off</color>");
                        break;
                }
            }
            else
            {
                if (BloodRefillSystem.BloodRefills.TryGetValue(platformId, out bloodRefillData))
                {
                    showRefillMessages = !bloodRefillData.ShowRefillMessages;
                }
                else
                {
                    showRefillMessages = false;
                }
            }

            if (showRefillMessages.HasValue)
            {
                if (bloodRefillData == null && !BloodRefillSystem.BloodRefills.TryGetValue(platformId, out bloodRefillData))
                {
                    bloodRefillData = new BloodRefillSystem.BloodRefillData();
                    BloodRefillSystem.BloodRefills[platformId] = bloodRefillData;
                }

                bloodRefillData.ShowRefillMessages = showRefillMessages.Value;

                command.Reply(
                    $"Blood Refill messages have now been turned <color=#{(showRefillMessages.Value ? "00ff00" : "ff0000")}>{(showRefillMessages.Value ? "on" : "off")}</color>");
            }
        }
*/
        
    }
}