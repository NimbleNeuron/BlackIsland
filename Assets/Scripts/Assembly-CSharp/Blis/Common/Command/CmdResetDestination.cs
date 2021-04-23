using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResetDestination, false)]
	public class CmdResetDestination : LocalCharacterCommandPacket
	{
		
		[Key(1)] public BlisVector destination;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			LocalMovableCharacter localMovableCharacter = self as LocalMovableCharacter;
			if (localMovableCharacter == null)
			{
				Log.E("[CmdResetDestination] Can't cast LocalMovableCharacter : " + self.GetType());
				return;
			}

			localMovableCharacter.MoveAgent.ResetDestination(destination.ToVector3());
		}
	}
}