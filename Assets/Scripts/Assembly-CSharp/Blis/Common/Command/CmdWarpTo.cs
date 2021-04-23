using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdWarpTo, false)]
	public class CmdWarpTo : LocalCharacterCommandPacket
	{
		[Key(1)] public BlisVector destination;


		public override void Action(ClientService service, LocalCharacter self)
		{
			ILocalMoveAgentOwner localMoveAgentOwner = self as ILocalMoveAgentOwner;
			if (localMoveAgentOwner == null)
			{
				Log.E("[MoveCommandPacket] Can't cast ILocalMoveAgentOwner : " + self.GetType());
				return;
			}

			localMoveAgentOwner.WarpTo(destination);
		}
	}
}