using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateSurvivableTime, false)]
	public class CmdUpdateSurvivableTime : LocalPlayerCharacterCommandPacket
	{
		
		[Key(1)] public BlisFixedPoint survivalTime;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnUpdateSurvivableTime(survivalTime.Value);
		}
	}
}