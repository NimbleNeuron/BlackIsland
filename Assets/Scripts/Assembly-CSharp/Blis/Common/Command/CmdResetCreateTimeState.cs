using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResetCreateTimeState, false)]
	public class CmdResetCreateTimeState : LocalCharacterCommandPacket
	{
		
		[Key(2)] public int casterId;

		
		[Key(1)] public int group;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.ResetCreateTimeEffectState(group, casterId);
		}
	}
}