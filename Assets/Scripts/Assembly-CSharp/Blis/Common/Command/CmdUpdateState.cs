using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateState, false)]
	public class CmdUpdateState : LocalCharacterCommandPacket
	{
		[Key(2)] public int casterId;


		[Key(6)] public BlisFixedPoint createdTime;


		[Key(5)] public BlisFixedPoint duration;


		[Key(1)] public int group;


		[Key(4)] public int reserveCount;


		[Key(3)] public int stackCount;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.UpdateEffectState(group, casterId, stackCount, reserveCount, duration.Value, createdTime.Value);
		}
	}
}