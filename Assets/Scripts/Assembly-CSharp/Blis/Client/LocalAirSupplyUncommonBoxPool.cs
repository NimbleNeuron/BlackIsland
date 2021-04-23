using Blis.Common;

namespace Blis.Client
{
	public class LocalAirSupplyUncommonBoxPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(35, SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplyItemBox(ItemGrade.Uncommon));
		}
	}
}