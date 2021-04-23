using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class ItemBoxSnapshot
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = 0;
	}
}