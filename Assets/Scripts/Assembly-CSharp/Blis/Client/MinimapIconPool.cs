using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MinimapIconPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool<Image>(10, SingletonMonoBehaviour<ResourceManager>.inst.LoadPointPinPrefab());
			AllocPool<NoiseEffect>(5, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("Noise"));
			AllocPool<UIIcon>(5, SingletonMonoBehaviour<ResourceManager>.inst.LoadPrefab("MapIcon"));
		}
	}
}