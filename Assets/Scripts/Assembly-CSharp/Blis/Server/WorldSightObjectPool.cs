using Blis.Common;

namespace Blis.Server
{
	
	public class WorldSightObjectPool : ObjectPool
	{
		
		public override void InitPool()
		{
			base.AllocPool(100, SingletonMonoBehaviour<ResourceManager>.inst.GetServerPrefab(ObjectType.SightObject));
		}
	}
}
