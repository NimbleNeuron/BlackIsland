using UnityEngine;

namespace Knife.DeferredDecals.Spawn
{
    [System.Serializable]
    public class DecalSpawnController
    {
        public SpawnType SpawnMode;
        public Decal DecalPrefab;
        public DecalSpawner Spawner;
        public float DestroyDelay = 15f;

        public Decal SpawnDecal()
        {
            Decal decalInstance;
            if (SpawnMode == SpawnType.Instantiate)
            {
                decalInstance = Object.Instantiate(DecalPrefab);
                if(DestroyDelay > 0)
                    Object.Destroy(decalInstance.gameObject, DestroyDelay);
            }
            else if (SpawnMode == SpawnType.Pool)
            {
                decalInstance = Spawner.Spawn();
                if(DestroyDelay > 0)
                    Spawner.DestroyDecal(decalInstance, DestroyDelay);
            }
            else
                throw new System.Exception("Invalid SpawnType");

            decalInstance.gameObject.SetActive(true);
            return decalInstance;
        }
    }

    public enum SpawnType
    {
        Instantiate,
        Pool
    }
}

public enum SpawnControl
{
    ByReceiver,
    BySpawnController
}