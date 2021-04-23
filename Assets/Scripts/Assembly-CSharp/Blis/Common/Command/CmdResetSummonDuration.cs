using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResetSummonDuration, false)]
	public class CmdResetSummonDuration : LocalSummonBaseCommandPacket
	{
		
		[Key(1)] public BlisFixedPoint duration;

		
		public override void Action(ClientService service, LocalSummonBase self)
		{
			self.ResetDuration(duration.GetValue());
		}
	}
}