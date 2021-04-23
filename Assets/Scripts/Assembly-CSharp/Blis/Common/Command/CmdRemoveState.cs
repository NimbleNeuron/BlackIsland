using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdRemoveState, false)]
	public class CmdRemoveState : LocalCharacterCommandPacket
	{
		[Key(2)] public int casterId;


		[Key(1)] public int group;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.RemoveEffectState(group, casterId);
		}
	}
}