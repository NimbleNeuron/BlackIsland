using Blis.Common;

namespace Blis.Client
{
	public class LocalAirSupplyLegendBoxPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(6, SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplyItemBox(ItemGrade.Legend));
		}
	}
}