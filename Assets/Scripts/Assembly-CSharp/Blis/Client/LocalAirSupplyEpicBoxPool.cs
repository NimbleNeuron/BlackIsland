using Blis.Common;

namespace Blis.Client
{
	public class LocalAirSupplyEpicBoxPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(30, SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplyItemBox(ItemGrade.Epic));
		}
	}
}