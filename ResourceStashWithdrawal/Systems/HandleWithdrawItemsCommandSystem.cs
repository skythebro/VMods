using System;
using Il2CppSystem.Collections.Generic;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using VAMP;
using VMods.Shared;

namespace VMods.ResourceStashWithdrawal;

public class HandleWithdrawItemsCommandSystem : SystemBase
{
    // Server-side dictionary to store items to withdraw
    public static Dictionary<ulong, Dictionary<int, int>> itemsToWithdraw = 
        new Dictionary<ulong, Dictionary<int, int>>();

    public static void Initialize()
    {
        try
        {
            // VNetworkRegistry
            //     .RegisterServerbound<WithdrawItemsCommand>(OnWithdrawItemsCommand);
        }
        catch (Exception e)
        {
            Utils.Logger.LogError($"Error while trying initialize ResourceStashWithdrawalRequest as a RegisterServerboundStruct: {e.Message} Stacktrace: {e.StackTrace}");
        }
    }

    public static void Deinitialize()
    {
        // VNetworkRegistry.UnregisterStruct<ResourceStashWithdrawalRequest>();
    }


    private static void OnWithdrawItemsCommand(FromCharacter fromCharacter, WithdrawItemsCommand withdrawItemsCommand)
    {
        // Listen for incoming WithdrawItemsCommand messages
        var query = Core.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<WithdrawItemsCommand>());

        var entities = query.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var command = Core.Server.EntityManager.GetComponentData<WithdrawItemsCommand>(entity);
            if (!itemsToWithdraw.ContainsKey(command.PlatformId))
            {
                itemsToWithdraw[command.PlatformId] = new Dictionary<int, int>();
            }
            itemsToWithdraw[command.PlatformId] = command.ItemsToWithdraw;
            
            Core.Server.EntityManager.DestroyEntity(entity);
        }
        entities.Dispose();
    }
}