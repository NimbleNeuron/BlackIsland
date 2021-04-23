using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcError, false)]
	public class RpcError : RpcPacket
	{
		[Key(0)] public ErrorType errorType;


		[Key(1)] public string msg;


		public override void Action(ClientService clientService)
		{
			clientService.HandleError(errorType, msg);
		}
	}
}