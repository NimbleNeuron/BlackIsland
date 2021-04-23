using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class ItemSnapshot : ISnapshot
	{
		[Key(0)] public Item item;
	}
}