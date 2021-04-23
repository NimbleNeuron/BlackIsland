using Blis.Common;

namespace Blis.Client
{
	public class FloatingUIPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool<UIName>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIName"));
			AllocPool<UIDamage>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIDamage"));
			AllocPool<UIStatusBar>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIStatusBar"));
			AllocPool<UIResourceText>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIResourceText"));
			AllocPool<UIResourceTimer>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UIResourceTimer"));
			AllocPool<UITrackingImage>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UITrackingImage"));
			AllocPool<UISurvivableTimer>(10,
				SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("UISurvivableTimer"));
		}
	}
}