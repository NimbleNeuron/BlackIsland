using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.ItemBox)]
	public abstract class WorldItemBox : WorldObject, IItemBox
	{
		
		
		public int Capacity
		{
			get
			{
				return this.capacity;
			}
		}

		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.ItemBox;
		}

		
		protected override int GetTeamNumber()
		{
			return 0;
		}

		
		protected override ColliderAgent GetColliderAgent()
		{
			return this.colliderAgent;
		}

		
		
		public List<Item> Items
		{
			get
			{
				return this.itemBox.Items;
			}
		}

		
		protected void Init(int itemSpawnPointCode, int capacity)
		{
			this.capacity = capacity;
			this.itemSpawnPointCode = itemSpawnPointCode;
			this.itemBox = new ItemBox(this.objectId, capacity);
			ItemSpawnPoint itemSpawnPointByCode = MonoBehaviourInstance<GameService>.inst.CurrentLevel.GetItemSpawnPointByCode(itemSpawnPointCode);
			GameUtil.BindOrAdd<ItemBoxColliderAgent>(base.gameObject, ref this.colliderAgent);
			this.colliderAgent.Init(base.ObjectType, itemSpawnPointByCode.shape, itemSpawnPointByCode.GetCollider(), !itemSpawnPointByCode.airSupply);
		}

		
		public bool isCapacityAvailable()
		{
			return this.itemBox.Count < this.capacity;
		}

		
		protected override SkillAgent GetSkillAgent()
		{
			return null;
		}

		
		protected override IItemBox GetItemBox()
		{
			return this;
		}

		
		public override byte[] CreateSnapshot()
		{
			return null;
		}

		
		public List<Item> Open(PlayerSession playerSession)
		{
			playerSession.SetOpenBoxId(base.ObjectId);
			return this.itemBox.Open(playerSession);
		}

		
		public void Close(PlayerSession playerSession)
		{
			this.itemBox.Close(playerSession);
			playerSession.SetOpenBoxId(0);
		}

		
		public void AddItem(Item item)
		{
			if (!this.isCapacityAvailable())
			{
				throw new GameException(ErrorType.NotEnoughItemBox);
			}
			this.itemBox.AddItem(item);
		}

		
		public Item FindItem(int itemId)
		{
			return this.itemBox.FindItem(itemId);
		}

		
		public virtual void RemoveItem(int itemId)
		{
			this.itemBox.RemoveItem(itemId);
		}

		
		public bool IsFullCapacity()
		{
			return this.itemBox.IsFullCapacity();
		}

		
		public bool IsEmpty()
		{
			return this.itemBox.IsEmpty();
		}

		
		protected int itemSpawnPointCode;

		
		private ItemBoxColliderAgent colliderAgent;

		
		protected ItemBox itemBox;

		
		private int capacity;
	}
}
