using Blis.Common;

namespace Blis.Client
{
	public class MasteryExpUIPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool<UIExp>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIExp"));
		}
	}
}