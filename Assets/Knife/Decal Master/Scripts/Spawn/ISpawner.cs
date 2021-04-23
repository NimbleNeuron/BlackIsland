using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    public interface ISpawner<T> where T : MonoBehaviour
    {
        T Spawn();
    }
}