using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUserConnected, false)]
	public class CmdUserConnected : CommandPacket
	{
		[Key(0)] public int connectedObjectId;


		public override void Action(ClientService service)
		{
			LocalPlayerCharacter localPlayerCharacter = service.World.Find<LocalPlayerCharacter>(connectedObjectId);
			if (localPlayerCharacter != null)
			{
				localPlayerCharacter.Connected();
			}
		}
	}
}