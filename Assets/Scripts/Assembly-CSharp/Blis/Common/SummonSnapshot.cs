using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SummonSnapshot : CharacterSnapshot
	{
		[Key(7)] public int ownerId;


		[Key(8)] public int summonId;
	}
}