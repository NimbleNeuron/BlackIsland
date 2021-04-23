using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SimpleSummonInfo
	{
		[Key(2)] public MoveAgentSnapshot moveAgentSnapshot;


		[Key(0)] public int objectId;


		[Key(1)] public BlisVector position;
	}
}