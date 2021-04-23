using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdMoveByDirection, false)]
	public class CmdMoveByDirection : MoveCommandPacket
	{
		[Key(2)] public BlisVector direction;


		protected override void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self)
		{
			iMoveAgent.MoveInDirection(direction.ToVector3());
		}
	}
}