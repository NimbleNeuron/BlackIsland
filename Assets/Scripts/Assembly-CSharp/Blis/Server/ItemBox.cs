using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class ItemBox : IItemBox
	{
		
		
		public List<Item> Items
		{
			get
			{
				return this.items;
			}
		}

		
		
		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		
		
		public int Capacity
		{
			get
			{
				return this.capacity;
			}
		}

		
		public ItemBox(int objectId, int capacity)
		{
			this.items = new List<Item>();
			this.objectId = objectId;
			this.capacity = capacity;
		}

		
		public bool IsEmpty()
		{
			return this.items.Count <= 0;
		}

		
		public bool IsFullCapacity()
		{
			return this.items.Count >= this.capacity;
		}

		
		public List<Item> Open(PlayerSession playerSession)
		{
			this.observerAgent.Register(playerSession);
			playerSession.SetOpenBoxId(this.objectId);
			return this.items;
		}

		
		public void Close(PlayerSession playerSession)
		{
			this.observerAgent.Deregister(playerSession);
			playerSession.SetOpenBoxId(0);
		}

		
		public void AddItem(Item item)
		{
			if (this.items.Count >= this.capacity)
			{
				return;
			}
			int num = this.items.FindIndex((Item x) => x.id == item.id);
			if (num >= 0)
			{
				this.items[num] = item;
			}
			else
			{
				this.items.Add(item);
			}
			this.observerAgent.Send(new RpcItemBoxAdd
			{
				item = item
			});
		}

		
		public void SetItems(List<Item> items)
		{
			this.items = items;
		}

		
		public Item FindItem(int itemId)
		{
			return this.items.Find((Item x) => x != null && x.id == itemId);
		}

		
		public void RemoveItem(int itemId)
		{
			Item item = this.items.Find((Item x) => x.id == itemId);
			if (item != null)
			{
				this.items.Remove(item);
				this.observerAgent.Send(new RpcItemBoxRemove
				{
					itemId = itemId
				});
			}
		}

		
		private int objectId;

		
		protected List<Item> items;

		
		private readonly ObserverAgent observerAgent = new ObserverAgent();

		
		private int capacity;
	}
}
