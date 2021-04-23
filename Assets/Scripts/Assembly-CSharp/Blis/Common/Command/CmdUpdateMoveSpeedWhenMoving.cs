using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateMoveSpeedWhenMoving, false)]
	public class CmdUpdateMoveSpeedWhenMoving : LocalCharacterCommandPacket
	{
		
		[Key(1)] public BlisFixedPoint moveSpeed;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.E("[MoveCommandPacket] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			localMoveAgentOwner.UpdateMoveSpeed(moveSpeed.Value);
			if (service.MyObjectId == self.ObjectId)
			{
				service.MyPlayer.OnUpdateStat();
			}
		}
	}
}