using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcExitGame, false)]
	public class RpcExitGame : RpcPacket
	{
		public override void Action(ClientService service)
		{
			service.GoToLobby();
		}
	}
}