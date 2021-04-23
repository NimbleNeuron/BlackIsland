using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class ItemGuide
	{
		private readonly List<ItemData> targetItems = new List<ItemData>();

		
		
		public event Action<Dictionary<RecommendItemType, List<ItemData>>, List<ItemData>>
			OnNavigationRecommendsUpdate = delegate { };


		public void InitTutorialGuide(TutorialType tutorialType)
		{
			Log.H(string.Format("[InitTutorialGuide] Start Load Tutorial. TutorialType({0})", tutorialType));
			List<ItemData> recommendItems = GameDB.tutorial.GetRecommendItems(tutorialType, RecommendItemType.Key);
			RecommendArea recommendArea = GameDB.tutorial.GetRecommendArea(tutorialType);
			if (recommendArea == null)
			{
				Log.H(string.Format("[InitTutorialGuide] Error : recommendAreaData is null. TutorialType({0})",
					tutorialType));
			}

			foreach (ItemData itemData in recommendItems)
			{
				Log.H(string.Format("[InitTutorialGuide] CoreItem({0})", itemData.code));
			}

			Dictionary<RecommendItemType, List<ItemData>> dictionary =
				new Dictionary<RecommendItemType, List<ItemData>>();
			dictionary.Add(RecommendItemType.Key, recommendItems);
			dictionary.Add(RecommendItemType.Weapon,
				GameDB.tutorial.GetRecommendItems(tutorialType, RecommendItemType.Weapon));
			dictionary.Add(RecommendItemType.Armor,
				GameDB.tutorial.GetRecommendItems(tutorialType, RecommendItemType.Armor));
			dictionary.Add(RecommendItemType.Consume,
				GameDB.tutorial.GetRecommendItems(tutorialType, RecommendItemType.Consume));
			UpdateRecommendTargetItems(dictionary);
			Favorite favorite = new Favorite();
			favorite.weaponCodes = (from x in dictionary[RecommendItemType.Key]
				select x.code).ToList<int>();
			favorite.paths = recommendArea != null ? recommendArea.GetAreaList() : null;
			Favorite favorite2 = favorite;
			MonoBehaviourInstance<GameUI>.inst.ProductionGoalWindow.LoadData(favorite2);
			UpdateFavorite(favorite2);
			Log.H("[InitGuide] Complete Load Tutorial");
		}


		public void InitGuide(int characterCode, WeaponType weaponType)
		{
			Log.H(string.Format("[InitGuide] Start Load Guide. CharacterCode({0}), WeaponType({1})", characterCode,
				weaponType));
			if (MonoBehaviourInstance<ClientService>.inst.SelectFavorite != null)
			{
				LoadFavoriteData(MonoBehaviourInstance<ClientService>.inst.SelectFavorite);
				return;
			}

			if (weaponType != WeaponType.None)
			{
				LoadRecommendData(characterCode, weaponType.GetWeaponMasteryType());
			}
		}


		private void LoadFavoriteData(Favorite favorite)
		{
			List<ItemData> list = (from x in favorite.weaponCodes
				select GameDB.item.FindItemByCode(x)).ToList<ItemData>();
			string str = (from x in list
				select x.code.ToString()).Aggregate((x, y) => x + ", " + y);
			Log.H("[LoadRecommendData] CoreItem (" + str + ")");
			Dictionary<RecommendItemType, List<ItemData>> recommendTargetItems =
				BuildRecommendTargetItems(favorite.characterCode, favorite.weaponType.GetWeaponMasteryType(), list);
			UpdateRecommendTargetItems(recommendTargetItems);
			UpdateFavorite(favorite);
			Log.H(string.Format("[InitGuide] Complete Load Favorite. MasteryType({0})",
				favorite.weaponType.GetWeaponMasteryType()));
		}


		private void LoadRecommendData(int characterCode, MasteryType masteryType)
		{
			List<ItemData> list =
				(from x in GameDB.recommend.FindRecommendItems(characterCode, masteryType, RecommendItemType.Key)
					select GameDB.item.FindItemByCode(x.itemCode)).ToList<ItemData>();
			string str = (from x in list
				select x.code.ToString()).Aggregate((x, y) => x + ", " + y);
			Log.H("[LoadRecommendData] CoreItem (" + str + ")");
			Dictionary<RecommendItemType, List<ItemData>> dictionary =
				BuildRecommendTargetItems(characterCode, masteryType, list);
			UpdateRecommendTargetItems(dictionary);
			Favorite favorite = new Favorite();
			favorite.weaponCodes = (from x in dictionary[RecommendItemType.Key]
				select x.code).ToList<int>();
			RecommendArea recommendArea = GameDB.recommend.FindRecommendAreaData(characterCode, masteryType);
			favorite.paths = recommendArea != null ? recommendArea.GetAreaList() : null;
			Favorite favorite2 = favorite;
			UpdateFavorite(favorite2);
			Log.H(string.Format("[InitGuide] Complete Load Recommend. MasteryType({0})", masteryType));
		}


		private Dictionary<RecommendItemType, List<ItemData>> BuildRecommendTargetItems(int characterCode,
			MasteryType masteryType, List<ItemData> coreItemDatas)
		{
			Dictionary<RecommendItemType, List<ItemData>> dictionary =
				new Dictionary<RecommendItemType, List<ItemData>>();
			dictionary.Add(RecommendItemType.Key, coreItemDatas);
			dictionary.Add(RecommendItemType.Weapon,
				(from x in GameDB.recommend.FindRecommendItems(characterCode, masteryType, RecommendItemType.Weapon)
					select GameDB.item.FindItemByCode(x.itemCode)).ToList<ItemData>());
			dictionary.Add(RecommendItemType.Armor,
				(from x in GameDB.recommend.FindRecommendItems(characterCode, masteryType, RecommendItemType.Armor)
					select GameDB.item.FindItemByCode(x.itemCode)).ToList<ItemData>());
			dictionary.Add(RecommendItemType.Consume,
				(from x in GameDB.recommend.FindRecommendItems(characterCode, masteryType, RecommendItemType.Consume)
					select GameDB.item.FindItemByCode(x.itemCode)).ToList<ItemData>());
			return dictionary;
		}


		private void UpdateRecommendTargetItems(Dictionary<RecommendItemType, List<ItemData>> recommendTargetItems)
		{
			ClearTargetItemList();
			AddTargetItemList(recommendTargetItems[RecommendItemType.Key]);
			OnNavigationRecommendsUpdate(recommendTargetItems,
				GameDB.item.AnalyzePreferItemData(recommendTargetItems[RecommendItemType.Key]));
		}


		private void UpdateFavorite(Favorite favorite)
		{
			if (favorite.paths == null)
			{
				Log.H("[UpdateFavorite] path is null.");
			}
			else if (favorite.paths.Count == 0)
			{
				Log.H("[UpdateFavorite] path is Empty.");
			}

			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateFavoritesData(favorite);
			UISystem.Action(new UpdateNavFocus
			{
				focusItem = favorite.weaponCodes.Count > 0 ? GameDB.item.FindItemByCode(favorite.weaponCodes[0]) : null
			});
		}


		private void ClearTargetItemList()
		{
			targetItems.Clear();
		}


		private void AddTargetItemList(List<ItemData> itemList)
		{
			bool flag = false;
			int num = 0;
			while (num < itemList.Count && targetItems.Count < 6)
			{
				targetItems.Add(itemList[num]);
				flag = true;
				num++;
			}

			if (flag)
			{
				OnUpdateTargetItems();
			}
		}


		public void AddTargetItem(ItemData itemData)
		{
			if (targetItems.Count < 6)
			{
				targetItems.Add(itemData);
				OnUpdateTargetItems();
				UISystem.Action(new UpdateNavFocus
				{
					focusItem = itemData
				});
			}
		}


		public void RemoveTargetItem(ItemData itemData)
		{
			if (targetItems.Contains(itemData))
			{
				targetItems.Remove(itemData);
			}

			OnUpdateTargetItems();
		}


		public void ChangeFavorite(Favorite favorite)
		{
			List<ItemData> itemList = (from x in favorite.weaponCodes
				select GameDB.item.FindItemByCode(x)).ToList<ItemData>();
			MonoBehaviourInstance<GameUI>.inst.MapWindow.SetFavorite(favorite);
			MonoBehaviourInstance<GameUI>.inst.HyperloopWindow.SetFavorite(favorite);
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.SetFavoriteName(favorite.title);
			ClearTargetItemList();
			AddTargetItemList(itemList);
			UISystem.Action(new UpdateNavTarget
			{
				targetItems = targetItems
			});
			UISystem.Action(new UpdateNavFocus
			{
				focusItem = targetItems[0]
			});
		}


		private void OnUpdateTargetItems()
		{
			UISystem.Action(new UpdateNavTarget
			{
				targetItems = targetItems
			});
		}
	}
}