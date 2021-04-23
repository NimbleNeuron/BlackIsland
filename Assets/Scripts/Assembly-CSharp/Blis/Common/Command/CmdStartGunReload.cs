using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdStartGunReload, false)]
	public class CmdStartGunReload : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public bool playReloadAnimation;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnStartGunReload(playReloadAnimation);
		}
	}
}