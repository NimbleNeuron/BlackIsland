using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateCharacterInvisible, false)]
	public class CmdUpdateCharacterInvisible : LocalCharacterCommandPacket
	{
		
		[Key(1)] public bool isInvisible;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.UpdateInvisible(isInvisible);
		}
	}
}