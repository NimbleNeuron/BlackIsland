using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public interface IOnePrefabPool<T> : ISpawner<T> where T : MonoBehaviour
    {
        void Populate(T prefab);
        void Destroy(T instance);
    }
}