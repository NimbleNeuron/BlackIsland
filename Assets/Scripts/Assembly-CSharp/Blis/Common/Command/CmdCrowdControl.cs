using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdCrowdControl, false)]
	public class CmdCrowdControl : LocalMovableCharacterCommandPacket
	{
		[Key(1)] public StateType stateType;


		public override void Action(ClientService service, LocalMovableCharacter self)
		{
			self.OnCrowdControl(stateType);
		}
	}
}