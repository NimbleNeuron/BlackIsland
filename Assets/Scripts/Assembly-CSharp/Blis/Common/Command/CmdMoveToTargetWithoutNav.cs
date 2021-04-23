using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdMoveToTargetWithoutNav, false)]
	public class CmdMoveToTargetWithoutNav : LocalCharacterCommandPacket
	{
		
		[Key(4)] public BlisFixedPoint arriveRadius;

		
		[Key(3)] public BlisFixedPoint moveSpeed;

		
		[Key(1)] public BlisVector startPos;

		
		[Key(2)] public int targetId;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.V("[CmdMoveToTargetWithoutNav] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			localMoveAgentOwner.MoveToTargetWithoutNav(startPos.ToVector3(),
				service.World.Find<LocalCharacter>(targetId), moveSpeed.Value, arriveRadius.Value);
		}
	}
}