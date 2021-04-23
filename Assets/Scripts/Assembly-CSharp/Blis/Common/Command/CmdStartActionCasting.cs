using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdStartActionCasting, false)]
	public class CmdStartActionCasting : LocalPlayerCharacterCommandPacket
	{
		
		[Key(2)] public BlisFixedPoint castTime;

		
		[Key(3)] public int extraParam;

		
		[Key(1)] public CastingActionType type;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnStartActionCasting(type, castTime.Value, extraParam);
		}
	}
}