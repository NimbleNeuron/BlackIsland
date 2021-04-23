using Blis.Common;

namespace Blis.Client
{
	public class LocalSightObjectPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(100, SingletonMonoBehaviour<ResourceManager>.inst.GetClientPrefab(ObjectType.SightObject));
		}
	}
}