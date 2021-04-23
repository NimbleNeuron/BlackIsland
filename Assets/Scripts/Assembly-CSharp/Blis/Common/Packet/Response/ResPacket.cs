using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(ResSuccess))]
	[Union(1, typeof(ResMissingCommand))]
	[Union(2, typeof(ResHandshake))]
	[Union(3, typeof(ResError))]
	[Union(4, typeof(ResJoin))]
	[Union(5, typeof(ResUseSkill))]
	[Union(6, typeof(ResStop))]
	[Union(7, typeof(ResPickUpItem))]
	[Union(8, typeof(ResOpenCorpse))]
	[Union(9, typeof(ResUseItem))]
	[Union(10, typeof(ResWalkableArea))]
	[Union(11, typeof(ResAskGameSetup))]
	[Union(12, typeof(ResAskGameStart))]
	[Union(13, typeof(ResWorldSnapshot))]
	[Union(14, typeof(ResExitTeamGame))]
	[Union(15, typeof(ResGameSnapshot))]
	[Union(16, typeof(ResNicknamePair))]
	[Union(17, typeof(ResEquipItem))]
	[MessagePackObject]
	public abstract class ResPacket : IPacket
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = 0;


		[Key(0)] public uint reqId;


		public virtual NetChannel ResponseChannel()
		{
			return NetChannel.ReliableOrdered;
		}
	}
}