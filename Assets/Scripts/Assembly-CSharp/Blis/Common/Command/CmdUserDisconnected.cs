using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUserDisconnected, false)]
	public class CmdUserDisconnected : CommandPacket
	{
		
		[Key(0)] public int disconnectedObjectId;

		
		public override void Action(ClientService service)
		{
			LocalPlayerCharacter localPlayerCharacter = null;
			if (service.World.TryFind<LocalPlayerCharacter>(disconnectedObjectId, ref localPlayerCharacter))
			{
				localPlayerCharacter.Disconnected();
			}
		}
	}
}