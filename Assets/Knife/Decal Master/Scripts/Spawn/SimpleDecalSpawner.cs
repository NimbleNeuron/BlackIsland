namespace Knife.DeferredDecals.Spawn
{
    public class SimpleDecalSpawner : DecalSpawner
    {
        public Decal Prefab;
        public bool PopulateOnAwake = true;
        public int PrewarmCount = 500;
        public bool AutoPopulateOnZero = true;

        private OnePrefabDecalPool decalsPool;

        private void Awake()
        {
            decalsPool = new OnePrefabDecalPool(PrewarmCount, AutoPopulateOnZero);

            if(PopulateOnAwake)
                decalsPool.Populate(Prefab);
        }

        public override Decal Spawn()
        {
            return decalsPool.Spawn();
        }

        public override void DestroyDecal(Decal decal)
        {
            decalsPool.Destroy(decal);
        }
    }
}