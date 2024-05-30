using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using VampireCommandFramework;
using VMods.Shared;

namespace VMods.ResourceStashWithdrawal.Commands;

public static partial class Commands
{
    private static ManualLogSource _log => Utils.Logger;

    static Commands()
    {
        Enabled = IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.deca.VampireCommandFramework", out var info);
        if (Enabled) _log.LogWarning($"VCF Version: {info.Metadata.Version}");
    }

    public static bool Enabled { get; private set; }

    public static void Register() => CommandRegistry.RegisterAll();
    public static void Unregister() => CommandRegistry.UnregisterAssembly();

    private static System.Random _random = new();

    [CommandGroup("ResourceWidthdrawal", "rw")]
    public class ResourceStashWidthdrawalCommands
    {
        [Command("widthraw", "w", adminOnly: false)]
        public void Widthraw(ChatCommandContext ctx)
        {
            CommandResourceStashWIthDrawalSystem.GetItemsForRecipe(ctx);
        }
    }
}

