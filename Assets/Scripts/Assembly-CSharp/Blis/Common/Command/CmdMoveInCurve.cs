using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdMoveInCurve, false)]
	public class CmdMoveInCurve : MoveCommandPacket
	{
		[Key(3)] public BlisFixedPoint angularSpeed;


		[Key(2)] public BlisVector startRotation;


		protected override void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self)
		{
			iMoveAgent.MoveInCurve(startRotation.ToVector3(), angularSpeed.Value);
		}
	}
}