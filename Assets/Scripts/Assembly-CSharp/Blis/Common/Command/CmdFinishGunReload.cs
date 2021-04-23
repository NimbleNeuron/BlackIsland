using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdFinishGunReload, false)]
	public class CmdFinishGunReload : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public bool cancelReload;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnFinishGunReload(cancelReload);
		}
	}
}