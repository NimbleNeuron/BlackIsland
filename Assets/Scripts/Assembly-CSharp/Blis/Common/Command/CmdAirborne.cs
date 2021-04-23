using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdAirborne, false)]
	public class CmdAirborne : LocalCharacterCommandPacket
	{
		[Key(1)] public BlisFixedPoint duration;


		[Key(2)] public BlisFixedPoint power;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.Airborne(duration.Value, power.Value);
		}
	}
}