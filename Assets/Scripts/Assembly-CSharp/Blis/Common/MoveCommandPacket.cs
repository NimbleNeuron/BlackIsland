using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(CmdMoveInCurve))]
	[Union(1, typeof(CmdMoveStraight))]
	[Union(2, typeof(CmdMoveToDestination))]
	[Union(3, typeof(CmdMoveByDirection))]
	[Union(4, typeof(CmdWarpTo))]
	[Union(5, typeof(CmdStopMove))]
	[Union(6, typeof(CmdUpdateMoveSpeedWhenMoving))]
	[Union(7, typeof(CmdLockRotation))]
	[MessagePackObject]
	public abstract class MoveCommandPacket : LocalCharacterCommandPacket
	{
		[IgnoreMember] protected new const int LAST_KEY_IDX = 1;


		[Key(1)] public BlisVector position;


		[IgnoreMember] private float timeDelta;


		public void SetExpolatedTime(float time)
		{
			timeDelta = time;
		}


		protected abstract void MoveAction(ClientService service, ILocalMoveAgentOwner iMoveAgent, LocalCharacter self);


		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.E("[MoveCommandPacket] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			MoveAction(service, localMoveAgentOwner, self);
			if (localMoveAgentOwner.MoveAgent.PositioningCorrection(position.ToVector3(), timeDelta))
			{
				Singleton<GameEventLogger>.inst.warpCount++;
			}
		}
	}
}