using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdRest, false)]
	public class CmdRest : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public bool rest;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.SetIsRest(rest);
		}
	}
}