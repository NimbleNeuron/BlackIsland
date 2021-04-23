using System.Collections.Generic;
using Blis.Common;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(OpenBox), typeof(UpdateBox), typeof(CloseBox))]
	public class ItemBoxStore : UIStore<ItemBoxStore>
	{
		private int boxId;


		private BoxWindowType boxWindowType;


		public BoxWindowType BoxWindowType => boxWindowType;


		public int BoxId => boxId;


		public bool IsBoxOpen => boxId > 0;


		public List<Item> BoxItems { get; } = new List<Item>();


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<OpenBox>(delegate(OpenBox data)
			{
				boxId = data.boxId;
				BoxItems.Clear();
				BoxItems.AddRange(data.boxItems);
				boxWindowType = data.boxWindowType;
			});
			action.IfTypeIs<UpdateBox>(delegate(UpdateBox data)
			{
				if (boxId > 0)
				{
					if (data.addedItems != null)
					{
						BoxItems.AddRange(data.addedItems);
					}

					if (data.removedItemIds != null)
					{
						for (int i = 0; i < data.removedItemIds.Length; i++)
						{
							int index = i;
							BoxItems.RemoveAll(x => x.id == data.removedItemIds[index]);
						}
					}
				}
			});
			action.IfTypeIs<CloseBox>(delegate(CloseBox data)
			{
				if (data.targetBoxId == 0 || boxId == data.targetBoxId)
				{
					boxId = 0;
				}
			});
		}


		protected override void PreCommit() { }


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			boxId = 0;
			BoxItems.Clear();
		}
	}
}