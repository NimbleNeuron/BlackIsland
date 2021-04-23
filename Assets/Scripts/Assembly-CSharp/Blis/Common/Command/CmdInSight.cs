using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdInSight, false)]
	public class CmdInSight : LocalCharacterCommandPacket
	{
		[Key(2)] public MoveAgentSnapshot moveAgentSnapshot;


		[Key(1)] public BlisVector position;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.InSight(position.ToVector3(), moveAgentSnapshot);
		}
	}
}