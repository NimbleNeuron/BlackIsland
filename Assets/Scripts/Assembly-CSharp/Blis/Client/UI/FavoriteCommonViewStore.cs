using System.Collections.Generic;

namespace Blis.Client.UI
{
	[UIActionMapping(typeof(UpdateFavoriteCommonViewItems))]
	public class FavoriteCommonViewStore : UIStore<FavoriteCommonViewStore>
	{
		private readonly List<UIFavoritesItem> favoritesItems = new List<UIFavoritesItem>();

		public IEnumerable<UIFavoritesItem> GetFavoritesItems()
		{
			return favoritesItems;
		}


		protected override void ActionHandle(UIAction action)
		{
			action.IfTypeIs<UpdateFavoriteCommonViewItems>(delegate(UpdateFavoriteCommonViewItems data)
			{
				favoritesItems.Clear();
				favoritesItems.AddRange(data.items);
			});
		}


		protected override void PreCommit() { }


		public override void OnSceneLoaded()
		{
			base.OnSceneLoaded();
			favoritesItems.Clear();
		}
	}
}