using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdMoveStraightWithoutNav, false)]
	public class CmdMoveStraightWithoutNav : LocalCharacterCommandPacket
	{
		
		[Key(3)] public BlisFixedPoint duration;

		
		[Key(2)] public BlisVector endPos;

		
		[Key(1)] public BlisVector startPos;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.V("[CmdMoveStraightWithoutNav] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			localMoveAgentOwner.MoveStraightWithoutNav(startPos.ToVector3(), endPos.ToVector3(), duration.Value);
		}
	}
}