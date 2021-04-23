using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.AirSupplyItemBox)]
	public class WorldAirSupplyItemBox : WorldItemBox
	{
		
		
		public ItemGrade ItemGrade
		{
			get
			{
				return this.itemGrade;
			}
		}

		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.AirSupplyItemBox;
		}

		
		public void Init(int itemSpawnPointCode, int capacity, ItemGrade itemGrade)
		{
			this.itemGrade = itemGrade;
			base.Init(itemSpawnPointCode, capacity);
		}

		
		public override void RemoveItem(int itemId)
		{
			base.RemoveItem(itemId);
			if (this.itemBox.IsEmpty())
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.DestroyWorldObject(this);
			}
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<AirSupplyItemBoxSnapshot>(new AirSupplyItemBoxSnapshot
			{
				maxItemGrade = this.itemGrade,
				itemSpawnPointCode = this.itemSpawnPointCode
			});
		}

		
		private ItemGrade itemGrade;
	}
}
