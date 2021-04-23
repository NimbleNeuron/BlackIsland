using System.Collections.Generic;
using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcSpawnAirSupply, false)]
	public class RpcSpawnAirSupply : RpcPacket
	{
		[Key(0)] public List<SnapshotWrapper> spawnSnapshots;


		public override void Action(ClientService clientService)
		{
			foreach (SnapshotWrapper snapshotWrapper in spawnSnapshots)
			{
				LocalAirSupplyItemBox localAirSupplyItemBox =
					MonoBehaviourInstance<ClientService>.inst.World.SpawnAirSupply(snapshotWrapper);
				if (!(localAirSupplyItemBox == null))
				{
					ItemGrade maxItemGrade = localAirSupplyItemBox.MaxItemGrade;
					MonoBehaviourInstance<GameUI>.inst.Events.OnSpawnAirSupply(localAirSupplyItemBox.ObjectId,
						SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplySprite(maxItemGrade));
				}
			}

			MonoBehaviourInstance<ClientService>.inst.ClearAirSupplyPositionEffect();
		}
	}
}