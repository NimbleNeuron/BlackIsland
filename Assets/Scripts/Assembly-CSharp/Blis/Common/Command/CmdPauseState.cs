using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdPauseState, false)]
	public class CmdPauseState : LocalCharacterCommandPacket
	{
		[Key(2)] public int casterId;


		[Key(4)] public float duration;


		[Key(3)] public float durationPauseEndTime;


		[Key(1)] public int group;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.DurationPauseState(group, casterId, durationPauseEndTime, duration);
		}
	}
}