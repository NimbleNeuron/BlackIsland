using Blis.Common;

namespace Blis.Server
{
	
	public class WorldMonsterPool : ObjectPool
	{
		
		public override void InitPool()
		{
			base.AllocPool(171, SingletonMonoBehaviour<ResourceManager>.inst.GetServerPrefab(ObjectType.Monster));
		}
	}
}
