using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class DeadAnnounceInfo
	{
		[Key(2)] public int aliveCount;


		[Key(1)] public int deadCharacterCode;


		[Key(0)] public int deadPlayerObjectId;
	}
}