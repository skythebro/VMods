using Bloodstone.API;
using Il2CppSystem.Collections.Generic;
using Stunlock.Network;

namespace VMods.ResourceStashWithdrawal;

public class WithdrawItemsCommand : VNetworkMessage
{
    public ulong PlatformId { get; set; }
    public Dictionary<int, int> ItemsToWithdraw { get; set; }

    public void Serialize(ref NetBufferOut writer)
    {
        writer.Write(PlatformId);
        writer.Write(ItemsToWithdraw.Count);
        foreach (var item in ItemsToWithdraw)
        {
            writer.Write(item.Key);
            writer.Write(item.Value);
        }
    }

    public void Deserialize(NetBufferIn reader)
    {
        PlatformId = reader.ReadUInt64();
        int count = reader.ReadInt32();
        ItemsToWithdraw = new Dictionary<int, int>(count);
        for (int i = 0; i < count; i++)
        {
            int key = reader.ReadInt32();
            int value = reader.ReadInt32();
            ItemsToWithdraw[key] = value;
        }
    }
}