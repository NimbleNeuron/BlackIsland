using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class InvenItem
	{
		[Key(1)] public readonly Item item;


		[Key(0)] public readonly int slot;


		[SerializationConstructor]
		public InvenItem(int slot, Item item)
		{
			this.slot = slot;
			this.item = item;
		}
	}
}