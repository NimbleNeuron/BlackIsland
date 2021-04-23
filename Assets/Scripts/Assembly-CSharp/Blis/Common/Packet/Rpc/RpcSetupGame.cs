using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcSetupGame, false)]
	public class RpcSetupGame : RpcPacket
	{
		[Key(0)] public int standbySecond;


		public override void Action(ClientService clientService)
		{
			Log.V("[GameClient] Setup Game Receive");
			clientService.SetupGame(standbySecond);
		}
	}
}