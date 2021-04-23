using System.Collections.Generic;
using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcNoticeAirSupply, false)]
	public class RpcNoticeAirSupply : RpcPacket
	{
		[Key(0)] public List<AirSupplyInfo> info;


		public override void Action(ClientService clientService)
		{
			foreach (AirSupplyInfo airSupplyInfo in info)
			{
				MonoBehaviourInstance<GameUI>.inst.Events.OnAirSupplyAnnounce(airSupplyInfo.objectId,
					airSupplyInfo.dropPosition,
					SingletonMonoBehaviour<ResourceManager>.inst.GetAirSupplyAnnounceSprite(airSupplyInfo.itemGrade));
				MonoBehaviourInstance<ClientService>.inst.ShowAirSupplyPositionEffect(airSupplyInfo.dropPosition);
			}
		}
	}
}