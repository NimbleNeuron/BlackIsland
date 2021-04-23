using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(UpdateEquipment), typeof(UpdateInventory))]
	public class MyPlayerCharacterStore : UIStore<MyPlayerCharacterStore>
	{
		private readonly List<Item> belongItems = new List<Item>();


		private readonly List<Item> equipments = new List<Item>();


		private readonly List<Item> inventories = new List<Item>();

		public IEnumerable<Item> GetInventories()
		{
			return inventories;
		}


		public IEnumerable<Item> GetEquipments()
		{
			return equipments;
		}


		public IEnumerable<Item> GetBelongItems()
		{
			return belongItems;
		}


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<UpdateInventory>(delegate(UpdateInventory data)
			{
				inventories.Clear();
				inventories.AddRange(data.inventory);
			});
			action.IfTypeIs<UpdateEquipment>(delegate(UpdateEquipment data)
			{
				equipments.Clear();
				equipments.AddRange(data.equipment);
			});
		}


		protected override void PreCommit()
		{
			belongItems.Clear();
			belongItems.AddRange(inventories);
			belongItems.AddRange(equipments);
		}


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			inventories.Clear();
			equipments.Clear();
			belongItems.Clear();
		}
	}
}