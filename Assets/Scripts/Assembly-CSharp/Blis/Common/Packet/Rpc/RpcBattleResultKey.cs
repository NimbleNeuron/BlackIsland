using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcBattleResultKey, false)]
	public class RpcBattleResultKey : RpcPacket
	{
		[Key(0)] public string battleResultKey;


		public override void Action(ClientService service)
		{
			GlobalUserData.battleResultKey = battleResultKey;
		}
	}
}