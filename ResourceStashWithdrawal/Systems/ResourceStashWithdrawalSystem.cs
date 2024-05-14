using ProjectM;
using ProjectM.Network;
using System;
using Bloodstone.API;
using Stunlock.Core;
using Unity.Entities;
using VMods.Shared;

namespace VMods.ResourceStashWithdrawal
{
    public static class ResourceStashWithdrawalSystem
    {
        #region Public Methods

        public static void Initialize()
        {
            VNetworkRegistry
                .RegisterServerboundStruct<ResourceStashWithdrawalRequest>(OnResourceStashWithdrawalRequest);
        }

        public static void Deinitialize()
        {
            VNetworkRegistry.UnregisterStruct<ResourceStashWithdrawalRequest>();
        }

        private static void OnResourceStashWithdrawalRequest(FromCharacter fromCharacter,
            ResourceStashWithdrawalRequest request)
        {
            if (!VWorld.IsServer || fromCharacter.Character == Entity.Null)
            {
                // This isn't running on a server, or a non-existing character made the request -> stop trying to move items
                return;
            }

            var server = VWorld.Server;
            var gameDataSystem = server.GetExistingSystemManaged<GameDataSystem>();
            var itemHashLookupMap = gameDataSystem.ItemHashLookupMap;
            var prefabCollectionSystem = server.GetExistingSystemManaged<PrefabCollectionSystem>();
            var prefabLookupMap = prefabCollectionSystem.PrefabLookupMap;
            var entityManager = server.EntityManager;

            if (!InventoryUtilities.TryGetInventoryEntity(entityManager, fromCharacter.Character,
                    out Entity playerInventory) || playerInventory == Entity.Null)
            {
                // Player inventory couldn't be found -> stop trying to move items
                return;
            }


            var remainingAmount = request.Amount;
            var stashes = Utils.GetAlliedStashes(entityManager, fromCharacter.Character);
            foreach (var stash in stashes)
            {
                entityManager.TryGetBuffer<InventoryBuffer>(stash, out var stashInventory);

                for (int i = 0; i < stashInventory.Length; i++)
                {
                    var stashItem = stashInventory[i];

                    // Only withdraw the requested item
                    if (stashItem.ItemType.GuidHash != request.ItemGUIDHash)
                    {
                        continue;
                    }

                    var transferAmount = Math.Min(remainingAmount, stashItem.Amount);
                    if (!Utils.TryGiveItem(entityManager, itemHashLookupMap, playerInventory, stashItem.ItemType,
                            transferAmount, out var remainingStacks))
                    {
                        // Failed to add the item(s) to the player's inventory -> stop trying to move any items at all
                        return;
                    }

                    transferAmount -= remainingStacks;
                    if (!InventoryUtilitiesServer.TryRemoveItem(entityManager, stash, stashItem.ItemType,
                            transferAmount))
                    {
                        // Failed to remove the item from the stash -> Remove the items from the player's inventory & stop trying to move any items at all
                        InventoryUtilitiesServer.TryRemoveItem(entityManager, playerInventory, stashItem.ItemType,
                            transferAmount);
                        return;
                    }

                    InventoryUtilitiesServer.CreateInventoryChangedEvent(entityManager, fromCharacter.Character,
                        stashItem.ItemType, stashItem.Amount, stashItem.ItemEntity._Entity ,InventoryChangedEventType.Moved);
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
                var name = Utils.GetItemName(new PrefabGUID(request.ItemGUIDHash), gameDataSystem, entityManager,
                    prefabLookupMap);
                if (remainingAmount == request.Amount)
                {
                    Utils.SendMessage(fromCharacter.User, $"Couldn't find any {name} in the stash(es).",
                        ServerChatMessageType.System);
                }
                else
                {
                    Utils.SendMessage(fromCharacter.User,
                        $"Couldn't find all {name} in the stash(es). {remainingAmount} {(remainingAmount == 1 ? "is" : "are")} missing.",
                        ServerChatMessageType.System);
                }
            }
        }

        #endregion
    }
}