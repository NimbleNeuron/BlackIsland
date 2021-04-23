using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdLockRotation, false)]
	public class CmdLockRotation : LocalCharacterCommandPacket
	{
		[Key(1)] public bool isLock;


		[Key(2)] public BlisFixedPoint rotationY;


		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.V("[CmdMoveStraightWithoutNav] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			localMoveAgentOwner.LockRotation(isLock, Quaternion.Euler(new Vector3(0f, rotationY.Value, 0f)));
		}
	}
}