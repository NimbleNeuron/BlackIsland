using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcStartGame, false)]
	public class RpcStartGame : RpcPacket
	{
		[Key(0)] public float frameUpdateRate;


		public override void Action(ClientService clientService)
		{
			clientService.StartGame(frameUpdateRate);
		}
	}
}