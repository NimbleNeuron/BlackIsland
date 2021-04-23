using MessagePack;

namespace Blis.Common
{
	[MessagePackObject()]
	public class BulletItem
	{
		[Key(0)] public readonly Item item;


		[Key(1)] public readonly float remainCooldown;

		[SerializationConstructor]
		public BulletItem(Item item, float remainCooldown)
		{
			this.item = item;
			this.remainCooldown = remainCooldown;
		}
	}
}