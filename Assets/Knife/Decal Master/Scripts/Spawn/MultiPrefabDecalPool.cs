using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Knife.DeferredDecals.Spawn
{
    public class MultiPrefabDecalPool : IMultiPrefabPool<Decal>
    {
        List<Queue<Decal>> pools;
        Dictionary<Decal, int> instancies;
        readonly int count;
        readonly bool autoPopulateOnZero;

        Decal[] cachedPrefabs;

        public MultiPrefabDecalPool(int prewarmCount, bool autoPopulateOnZero)
        {
            count = prewarmCount;
            this.autoPopulateOnZero = autoPopulateOnZero;
            pools = new List<Queue<Decal>>();
            instancies = new Dictionary<Decal, int>();
        }

        public void Populate(IEnumerable<Decal> prefabs)
        {
            cachedPrefabs = prefabs.ToArray();
            var enumerator = prefabs.GetEnumerator();
            int poolIndex = 0;
            while (enumerator.MoveNext())
            {
                pools.Add(new Queue<Decal>());
                var currentPrefab = enumerator.Current;
                Decal instance;
                for (int i = 0; i < count; i++)
                {
                    instance = Object.Instantiate(currentPrefab);
                    instance.gameObject.SetActive(false);
                    instancies.Add(instance, poolIndex);
                    pools[poolIndex].Enqueue(instance);
                }
                poolIndex++;
            }
        }

        void Populate(int poolIndex)
        {
            Decal instance;
            for (int i = 0; i < count; i++)
            {
                instance = Object.Instantiate(cachedPrefabs[i]);
                instance.gameObject.SetActive(false);
                instancies.Add(instance, poolIndex);
                pools[poolIndex].Enqueue(instance);
            }
        }

        public Decal Spawn()
        {
            int randomPoolIndex = Random.Range(0, pools.Count);
            var pool = pools[randomPoolIndex];
            if (pool.Count == 0 && autoPopulateOnZero)
            {
                Populate(randomPoolIndex);
            }

            return pool.Dequeue();
        }

        public void Destroy(Decal instance)
        {
            instance.gameObject.SetActive(false);
            pools[instancies[instance]].Enqueue(instance);
        }
    }
}