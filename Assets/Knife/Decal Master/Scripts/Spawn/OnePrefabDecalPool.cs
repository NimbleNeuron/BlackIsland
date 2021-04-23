using UnityEngine;
using System.Collections.Generic;

namespace Knife.DeferredDecals.Spawn
{
    public class OnePrefabDecalPool : IOnePrefabPool<Decal>
    {
        Queue<Decal> pool;
        readonly int count;
        readonly bool autoPopulateOnZero;

        Decal cachedPrefab;

        public OnePrefabDecalPool(int prewarmCount, bool autoPopulateOnZero)
        {
            count = prewarmCount;
            this.autoPopulateOnZero = autoPopulateOnZero;
            pool = new Queue<Decal>();
        }

        public void Populate(Decal prefab)
        {
            cachedPrefab = prefab;
            Decal instance;
            for (int i = 0; i < count; i++)
            {
                instance = Object.Instantiate(prefab);
                instance.gameObject.SetActive(false);
                pool.Enqueue(instance);
            }
        }

        public Decal Spawn()
        {
            if(pool.Count == 0 && autoPopulateOnZero)
            {
                Populate(cachedPrefab);
            }

            return pool.Dequeue();
        }

        public void Destroy(Decal instance)
        {
            pool.Enqueue(instance);
        }
    }
}