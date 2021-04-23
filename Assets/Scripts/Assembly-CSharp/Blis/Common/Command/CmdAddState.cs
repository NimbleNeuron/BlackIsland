using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdAddState, false)]
	public class CmdAddState : LocalCharacterCommandPacket
	{
		[Key(5)] public int casterId;


		[Key(1)] public int code;


		[Key(2)] public BlisFixedPoint duration;


		[Key(6)] public BlisFixedPoint originalDuration;


		[Key(4)] public int reserveCount;


		[Key(3)] public int stackCount;


		public override void Action(ClientService service, LocalCharacter self)
		{
			CharacterStateData data = GameDB.characterState.GetData(code);
			self.AddEffectState(new CharacterStateValue(data.code, service.CurrentServerFrameTime, duration.Value,
				stackCount, reserveCount, casterId, originalDuration.Value));
		}
	}
}