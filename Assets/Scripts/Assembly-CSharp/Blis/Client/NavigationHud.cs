using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class NavigationHud : BaseUI
	{
		[SerializeField] private UINavAreaItem uiNavAreaItem = default;


		[SerializeField] private UINavItem navItem = default;


		[SerializeField] private List<UINavFavoritesItem> favoritesItems = default;


		[SerializeField] private StartingViewRouteButton selectRouteSlotPrefab = default;


		private readonly Dictionary<ItemData, int> ownItems = new Dictionary<ItemData, int>();


		private readonly HashSet<ItemData> ownSourceItemSet = new HashSet<ItemData>();


		private readonly Dictionary<ItemData, int> sourceItems = new Dictionary<ItemData, int>();


		private InfoMaker btnRoutes;


		private ItemData focusedItem;


		private LayoutElement guideUI;


		private GameObject imgCloseRoutes;


		private GameObject imgOpenRoutes;


		private RouteFilterType routeFilterType;


		private RouteFilterUI routeFilterUI;


		private GameObject routeList;


		private List<Favorite> routeListFavorites = new List<Favorite>();


		private bool routeListRunning;


		private Transform routeSlotParent;


		private List<StartingViewRouteButton> routeSlots;


		private LnText txtFavoriteName;


		public List<ItemData> TargetItemList { get; } = new List<ItemData>();


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
			UISystem.RemoveListener<MyPlayerCharacterStore>(OnMyPlayerCharacterStoreUpdate);
		}

		
		
		public event Action<ItemData> OnRequestTargetItemRemove = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			guideUI = GameUtil.Bind<LayoutElement>(gameObject, "ProductionGoal/AddTarget");
			txtFavoriteName = GameUtil.Bind<LnText>(gameObject, "ProductionGoal/Items/TXT_FavoriteName");
			btnRoutes = GameUtil.Bind<InfoMaker>(gameObject, "ProductionGoal/BtnRoutes");
			imgOpenRoutes = transform.FindRecursively("IMG_OpenRoutes").gameObject;
			imgCloseRoutes = transform.FindRecursively("IMG_CloseRoutes").gameObject;
			routeList = transform.FindRecursively("RouteList").gameObject;
			routeSlotParent = transform.FindRecursively("RouteSlotParent");
			routeSlots = routeSlotParent.GetComponentsInChildren<StartingViewRouteButton>(true)
				.ToList<StartingViewRouteButton>();
			routeSlots.ForEach(delegate(StartingViewRouteButton x) { x.onClickCallback += ClickRouteSlot; });
			routeFilterUI = GameUtil.Bind<RouteFilterUI>(routeList.gameObject, "RouteFilterUI");
			routeFilterUI.changeFilterType += ChangeFilterType;
			UISystem.AddListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
			UISystem.AddListener<MyPlayerCharacterStore>(OnMyPlayerCharacterStoreUpdate);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			navItem.Hide();
			navItem.OnNavWayPointItemClick += OnWayPointItemClick;
			navItem.OnNavMaterialItemClick += OnMaterialItemClick;
			navItem.OnNavigationClose += NavigationClosed;
			uiNavAreaItem.OnNavMaterialItemClick += OnMaterialItemClick;
			favoritesItems.ForEach(delegate(UINavFavoritesItem x)
			{
				x.OnSelection += NavFavItemSelected;
				x.OnRequestDelete += DeleteItem;
			});
			routeSlots.ForEach(delegate(StartingViewRouteButton x) { x.InitUI(); });
			btnRoutes.keyCode = MonoBehaviourInstance<GameInput>.inst.GetKeyCode(GameInputEvent.ShowRouteList)
				.ToString();
		}


		private void OnNavigationStoreUpdate(NavigationAndCombineStore navigationAndCombineStore)
		{
			TargetItemList.Clear();
			TargetItemList.AddRange(navigationAndCombineStore.GetTargetItems());
			sourceItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair in navigationAndCombineStore.GetNeedFocusSourceItems())
			{
				sourceItems.Add(keyValuePair.Key, keyValuePair.Value);
			}

			ownItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair2 in navigationAndCombineStore.GetOwnFocusSourceItems())
			{
				ownItems.Add(keyValuePair2.Key, keyValuePair2.Value);
			}

			focusedItem = navigationAndCombineStore.GetFocusItem();
			if (0 < TargetItemList.Count && focusedItem != null)
			{
				navItem.OnUpdateTargetItem(focusedItem, sourceItems, ownItems);
				navItem.Show();
				guideUI.ignoreLayout = true;
				guideUI.transform.localScale = Vector3.zero;
			}
			else
			{
				navItem.Hide();
				guideUI.ignoreLayout = false;
				guideUI.transform.localScale = Vector3.one;
			}

			ownSourceItemSet.Clear();
			ownSourceItemSet.UnionWith(from x in navigationAndCombineStore.GetBelongItems()
				select x.ItemData);
			RenderFavoritesItems();
		}


		private void OnMyPlayerCharacterStoreUpdate(MyPlayerCharacterStore myPlayerCharacterStore)
		{
			ownSourceItemSet.Clear();
			ownSourceItemSet.UnionWith(from x in myPlayerCharacterStore.GetBelongItems()
				select x.ItemData);
			RenderFavoritesItems();
		}


		private void RenderFavoritesItems()
		{
			int i;
			int j;
			for (i = 0; i < favoritesItems.Count; i = j + 1)
			{
				if (i < TargetItemList.Count)
				{
					if (ownSourceItemSet.Any(x => x.code == TargetItemList[i].code))
					{
						DeleteItem(TargetItemList[i]);
						favoritesItems[i].SetItemData(null);
						favoritesItems[i].EnableSelection(false);
					}
					else
					{
						favoritesItems[i].SetItemData(TargetItemList[i]);
						if (TargetItemList[i] == focusedItem)
						{
							favoritesItems[i].EnableSelection(true);
						}
						else
						{
							favoritesItems[i].EnableSelection(false);
						}
					}
				}
				else
				{
					favoritesItems[i].SetItemData(null);
					favoritesItems[i].EnableSelection(false);
				}

				j = i;
			}
		}


		public void SetFavoriteName(string favName)
		{
			txtFavoriteName.text = favName;
		}


		public void SetRouteFilterType(RouteFilterType routeFilterType)
		{
			this.routeFilterType = routeFilterType;
		}


		public void TutorialItemSelected(int itemCode)
		{
			UINavFavoritesItem favoritesItem = favoritesItems.Find(x => x.GetItemData().code == itemCode);
			NavFavItemSelected(favoritesItem);
		}


		private void NavFavItemSelected(UINavFavoritesItem favoritesItem)
		{
			SingletonMonoBehaviour<UIDispatcher>.inst.Action(new UpdateNavFocus
			{
				focusItem = favoritesItem != null ? favoritesItem.GetItemData() : null
			});
		}


		private void DeleteItem(ItemData itemData)
		{
			if (itemData != null)
			{
				OnRequestTargetItemRemove(itemData);
				ChangeNavigationFocus(itemData);
			}
		}


		private void NavigationClosed()
		{
			navItem.Hide();
			guideUI.ignoreLayout = false;
			guideUI.transform.localScale = Vector3.one;
		}


		private void ChangeNavigationFocus(ItemData deletedItemData)
		{
			UINavFavoritesItem favoritesItem = null;
			if (favoritesItems != null)
			{
				for (int i = 0; i < favoritesItems.Count; i++)
				{
					ItemData itemData = favoritesItems[i].GetItemData();
					if (itemData != null && itemData.code == deletedItemData.code)
					{
						if (i < TargetItemList.Count - 1)
						{
							if (favoritesItems[i + 1].GetItemData() != null)
							{
								favoritesItem = favoritesItems[i + 1];
								break;
							}
						}
						else if (favoritesItems[0].GetItemData() != null)
						{
							favoritesItem = favoritesItems[0];
							break;
						}
					}
				}
			}

			NavFavItemSelected(favoritesItem);
		}


		private void OnWayPointItemClick(ItemData root, ItemData itemData)
		{
			MonoBehaviourInstance<GameUI>.inst.CombineWindow.SelectItem(root, itemData);
			MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
		}


		private void OnMaterialItemClick(ItemData itemData)
		{
			if (MonoBehaviourInstance<GameUI>.inst.MapWindow.ContainItemMark(itemData))
			{
				MonoBehaviourInstance<GameUI>.inst.MapWindow.Close();
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.MapWindow);
			MonoBehaviourInstance<GameUI>.inst.MapWindow.SetItemMark(itemData);
		}


		public void OnTargetItemUpdate(List<ItemData> itemDataList)
		{
			TargetItemList.Clear();
			TargetItemList.AddRange(itemDataList);
			for (int i = 0; i < favoritesItems.Count; i++)
			{
				if (i < itemDataList.Count)
				{
					favoritesItems[i].SetItemData(itemDataList[i]);
				}
				else
				{
					favoritesItems[i].SetItemData(null);
				}
			}
		}


		public void OnUpdateCurrentArea(int areaDataCode)
		{
			navItem.SetCurrentArea(areaDataCode);
			uiNavAreaItem.SetCurrentArea(areaDataCode);
		}


		public void OnUpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			navItem.SetAreaStateMap(areaStateMap);
			uiNavAreaItem.SetAreaStateMap(areaStateMap);
		}


		public void ShowTutorialSquareNavi(bool show)
		{
			navItem.ShowTutorialSquareNavi(show);
		}


		public void ShowTutorialBoxNavi(bool show, int itemCode)
		{
			navItem.ShowTutorialBoxNavi(show, itemCode);
		}


		public void ShowTutorialSquareNaviArea(bool show)
		{
			uiNavAreaItem.ShowTutorialSquareNaviArea(show);
		}


		public void ShowTutorialSquareBoxLeather(bool show)
		{
			uiNavAreaItem.ShowTutorialSquareBoxLeather(show);
		}


		public void ShowNaviAreaItem(bool show)
		{
			uiNavAreaItem.gameObject.SetActive(show);
		}


		public void ChangeFilterType(RouteFilterType routeFilterType)
		{
			routeListFavorites = RouteApi.GetFavoritesByFilterType(routeFilterType);
			routeFilterUI.ChangeFilterType(routeFilterType);
			RenderRouteListSlots();
		}


		public void SetTutorialRouteList()
		{
			routeList.SetActive(false);
			imgOpenRoutes.SetActive(false);
			imgCloseRoutes.SetActive(false);
		}


		public void ShowRouteList()
		{
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				return;
			}

			if (routeListRunning)
			{
				return;
			}

			if (routeList.activeSelf)
			{
				routeList.SetActive(false);
				imgOpenRoutes.SetActive(true);
				imgCloseRoutes.SetActive(false);
				return;
			}

			routeList.SetActive(true);
			imgOpenRoutes.SetActive(false);
			imgCloseRoutes.SetActive(true);
		}


		public void SetRouteListSlots()
		{
			routeListFavorites = RouteApi.GetFavoritesByFilterType(routeFilterType);
			routeFilterUI.ChangeFilterType(routeFilterType);
			RenderRouteListSlots();
		}


		public void RenderRouteListSlots()
		{
			int childCount = routeSlotParent.childCount;
			int num = Mathf.Max(0, routeListFavorites.Count - childCount);
			for (int i = 0; i < num; i++)
			{
				StartingViewRouteButton startingViewRouteButton =
					Instantiate<StartingViewRouteButton>(selectRouteSlotPrefab, routeSlotParent);
				startingViewRouteButton.InitUI();
				startingViewRouteButton.onClickCallback += ClickRouteSlot;
			}

			for (int j = 0; j < routeSlots.Count; j++)
			{
				routeSlots[j].SetSelected(false);
				if (j < routeListFavorites.Count)
				{
					routeSlots[j].SetRouteButton(j, routeListFavorites[j]);
				}
				else
				{
					routeSlots[j].gameObject.SetActive(false);
				}
			}

			int num2 = routeListFavorites.FindIndex(x => x == MonoBehaviourInstance<ClientService>.inst.SelectFavorite);
			if (num2 != -1)
			{
				routeSlots[num2].SetSelected(true);
			}
		}


		private void ClickRouteSlot(int index)
		{
			if (routeListRunning)
			{
				return;
			}

			routeListRunning = true;
			Debug.Log(string.Format(" !!! NavigationHud ClickRouteSlot Index = {0}", index));
			for (int i = 0; i < routeSlots.Count; i++)
			{
				routeSlots[i].SetSelected(i == index);
			}

			this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(0.1f, delegate
				{
					MonoBehaviourInstance<ClientService>.inst.SetSelectFavorite(routeListFavorites[index]);
					SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.ChangeFavorite(routeListFavorites[index]);
					routeListRunning = false;
					ShowRouteList();
				}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][ClickRouteSlot] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}
	}
}