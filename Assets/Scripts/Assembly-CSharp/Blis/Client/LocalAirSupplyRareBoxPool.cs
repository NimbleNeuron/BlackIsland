using Blis.Common;

namespace Blis.Client
{
	public class LocalAirSupplyRareBoxPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(50, SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplyItemBox(ItemGrade.Rare));
		}
	}
}