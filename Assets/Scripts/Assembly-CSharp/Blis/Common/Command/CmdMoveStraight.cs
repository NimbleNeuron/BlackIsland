using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdMoveStraight, false)]
	public class CmdMoveStraight : MoveCommandPacket
	{
		[Key(5)] public bool CanRotate;


		[Key(2)] public BlisVector destination;


		[Key(3)] public BlisFixedPoint duration;


		[Key(4)] public EasingFunction.Ease ease;


		protected override void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self)
		{
			iMoveAgent.MoveStraight(position, destination, duration.Value, ease, CanRotate);
		}
	}
}