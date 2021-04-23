using UnityEngine;
using System.Collections.Generic;

namespace Knife.DeferredDecals.Spawn
{
    public interface IMultiPrefabPool<T> : ISpawner<T> where T : MonoBehaviour
    {
        void Populate(IEnumerable<T> prefabs);
        void Destroy(T instance);
    }
}