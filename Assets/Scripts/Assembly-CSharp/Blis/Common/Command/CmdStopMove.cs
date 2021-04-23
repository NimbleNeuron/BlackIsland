using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdStopMove, false)]
	public class CmdStopMove : MoveCommandPacket
	{
		protected override void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self)
		{
			iMoveAgent.StopMove();
		}
	}
}