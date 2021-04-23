using Blis.Common;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.Item)]
	public class WorldItem : WorldObject
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.Item;
		}

		
		protected override int GetTeamNumber()
		{
			return 0;
		}

		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		public Item GetItem()
		{
			return this.item;
		}

		
		public void Init(Item item)
		{
			this.item = item;
			GameUtil.BindOrAdd<ItemColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init();
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		protected override IItemBox GetItemBox()
		{
			throw new GameException(ErrorType.InvalidAction);
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<ItemSnapshot>(new ItemSnapshot
			{
				item = this.item
			});
		}

		
		private ItemColliderAgent colliderAgent;

		
		private Item item;
	}
}
