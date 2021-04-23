using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdDyingCondition, false)]
	public class CmdDyingCondition : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int hp;


		[Key(2)] public int sp;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.SetIsRest(false);
			self.OnDyingCondition(hp, sp);
		}
	}
}