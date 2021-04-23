using Blis.Common;

namespace Blis.Client
{
	public class LocalMonsterPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(171, SingletonMonoBehaviour<ResourceManager>.inst.GetClientPrefab(ObjectType.Monster));
		}
	}
}