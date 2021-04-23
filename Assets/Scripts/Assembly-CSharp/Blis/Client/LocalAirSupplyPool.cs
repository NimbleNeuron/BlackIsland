using Blis.Common;

namespace Blis.Client
{
	public class LocalAirSupplyPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool<LocalDrone>(15, SingletonMonoBehaviour<ResourceManager>.inst.LoadObject("HaulerDrone_01"));
			AllocPool<LocalAirSupplyItemBox>(70,
				SingletonMonoBehaviour<ResourceManager>.inst.GetClientPrefab(ObjectType.AirSupplyItemBox));
		}
	}
}