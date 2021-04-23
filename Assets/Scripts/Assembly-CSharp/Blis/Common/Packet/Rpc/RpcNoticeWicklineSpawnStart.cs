using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcNoticeWicklineSpawnStart, false)]
	public class RpcNoticeWicklineSpawnStart : RpcPacket
	{
		
		[Key(1)] public BlisFixedPoint wicklineRespawnTime;

		
		public override void Action(ClientService clientService)
		{
			MonoBehaviourInstance<ClientService>.inst.SettingWicklineRespawnTime(wicklineRespawnTime.Value);
		}
	}
}