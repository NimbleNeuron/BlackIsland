namespace Knife.DeferredDecals.Spawn
{
    public class MultiDecalSpawner : DecalSpawner
    {
        public Decal[] Prefabs;
        public bool PopulateOnAwake = true;
        public int PrewarmCount = 500;
        public bool AutoPopulateOnZero = true;

        private MultiPrefabDecalPool decalsPool;

        private void Awake()
        {
            decalsPool = new MultiPrefabDecalPool(PrewarmCount, AutoPopulateOnZero);

            if(PopulateOnAwake)
                decalsPool.Populate(Prefabs);
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