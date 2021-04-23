using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SimpleUserInfo
	{
		[Key(3)] public MoveAgentSnapshot moveAgentSnapshot;


		[Key(1)] public byte[] playerSnapshot;


		[Key(2)] public BlisVector position;


		[Key(0)] public long userId;
	}
}