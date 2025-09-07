using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using VampireCommandFramework;
using VMods.Shared;
using System;
using VAMP;

namespace VMods.ResourceStashWithdrawal;

public class CommandResourceStashWIthDrawalSystem
{
    public static void GetItemsForRecipe(ChatCommandContext ctx)
    {
        if (!Utils.IsServer || ctx.User.LocalCharacter._Entity == Entity.Null)
        {
            // This isn't running on a server, or a non-existing character made the request -> stop trying to move items
            return;
        }

        
        
        var server = Core.Server;
        var gameDataSystem = server.GetExistingSystemManaged<GameDataSystem>();
        var itemHashLookupMap = gameDataSystem.ItemHashLookupMap;
        var prefabCollectionSystem = server.GetExistingSystemManaged<PrefabCollectionSystem>();
        var prefabLookupMap = prefabCollectionSystem.PrefabLookupMap;
        var entityManager = server.EntityManager;

        if (!InventoryUtilities.TryGetInventoryEntity(entityManager, ctx.User.LocalCharacter._Entity,
                out Entity playerInventory) || playerInventory == Entity.Null)
        {
            // Player inventory couldn't be found -> stop trying to move items
            ctx.Reply("Player inventory couldn't be found");
            return;
        }

        var stashes = Utils.GetAlliedStashes(entityManager, ctx.User.LocalCharacter._Entity);
        if (HandleWithdrawItemsCommandSystem.itemsToWithdraw.ContainsKey(ctx.User.PlatformId))
        {
            foreach (var request in HandleWithdrawItemsCommandSystem.itemsToWithdraw[ctx.User.PlatformId])
            {
                var remainingAmount = request.Value;

                foreach (var stash in stashes)
                {
                    entityManager.TryGetBuffer<InventoryBuffer>(stash, out var stashInventory);

                    for (int i = 0; i < stashInventory.Length; i++)
                    {
                        var stashItem = stashInventory[i];

                        // Only withdraw the requested item
                        if (stashItem.ItemType.GuidHash != request.Key)
                        {
                            continue;
                        }

                        var transferAmount = Math.Min(remainingAmount, stashItem.Amount);
                        if (!Utils.TryGiveItem(entityManager, itemHashLookupMap, playerInventory, stashItem.ItemType,
                                transferAmount, out var remainingStacks))
                        {
                            // Failed to add the item(s) to the player's inventory -> stop trying to move any items at all
                            //return;
                            // instead of returning break so the other items can be withdrawn
                            break;
                        }

                        transferAmount -= remainingStacks;
                        if (!InventoryUtilitiesServer.TryRemoveItem(entityManager, stash, stashItem.ItemType,
                                transferAmount))
                        {
                            // Failed to remove the item from the stash -> Remove the items from the player's inventory & stop trying to move any items at all
                            InventoryUtilitiesServer.TryRemoveItem(entityManager, playerInventory, stashItem.ItemType,
                                transferAmount);
                            //return;
                            // instead of returning break so the other items can be withdrawn
                            break;
                        }

                        InventoryUtilitiesServer.CreateInventoryChangedEvent(entityManager,
                            ctx.User.LocalCharacter._Entity,
                            stashItem.ItemType, stashItem.Amount, stashItem.ItemEntity._Entity,
                            InventoryChangedEventType.Moved);
                        remainingAmount -= transferAmount;
                        if (remainingAmount <= 0)
                        {
                            break;
                        }
                    }

                    if (remainingAmount <= 0)
                    {
                        break;
                    }
                }

                if (remainingAmount > 0)
                {
                    var name = Utils.GetItemName(new PrefabGUID(request.Key), gameDataSystem, entityManager,
                        prefabLookupMap);
                    if (remainingAmount == request.Value)
                    {
                        ctx.Reply($"Couldn't find any {name} in the stash(es).");
                    }
                    else
                    {
                        ctx.Reply(
                            $"Couldn't find all {name} in the stash(es). {remainingAmount} {(remainingAmount == 1 ? "is" : "are")} missing.");
                    }
                }
            }
        }
        else
        {
            ctx.Reply("No items to withdraw, did you middle click on a recipe?");
        }
    }
}