using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdTeamRevival, false)]
	public class CmdTeamRevival : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int hp;


		[Key(2)] public int sp;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnTeamRevival(hp, sp);
		}
	}
}