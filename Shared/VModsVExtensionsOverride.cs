using Unity.Entities;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using VAMP;

namespace VMods.Shared;

public static class VModsVExtensionsOverride
{
    public static void WithComponentDataVmod<T>(this Entity entity, ActionRefVmod<T> action) where T : struct
    {
        Core.Server.EntityManager.TryGetComponentData<T>(entity, out var componentData);
        action(ref componentData);
        Core.Server.EntityManager.SetComponentData<T>(entity, componentData);
    }

    public static void WithComponentDataVmodAOT<T>(this Entity entity, ActionRefVmod<T> action) where T : unmanaged
    {
        // ReSharper disable once AmbiguousInvocation
        var componentData = Core.Server.EntityManager.GetComponentDataAOTUnsafe<T>(entity);
        action(ref componentData);
        Core.Server.EntityManager.SetComponentData<T>(entity, componentData);
    }

    private static Type GetType<T>() => Il2CppType.Of<T>();

    public static unsafe T GetComponentDataAOTUnsafe<T>(this EntityManager entityManager, Entity entity) where T : unmanaged
    {
        var type = TypeManager.GetTypeIndex(GetType<T>());
        var result = (T*)entityManager.GetComponentDataRawRW(entity, type);
        return *result;
    }

    public delegate void ActionRefVmod<T>(ref T item);
}