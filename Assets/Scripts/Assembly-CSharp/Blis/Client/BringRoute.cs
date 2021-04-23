using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class BringRoute : BaseUI
	{
		private const int slotCount = 20;


		private readonly List<RecommendWeaponRoute> mergeRecommendWewaponRoutes = new List<RecommendWeaponRoute>();


		private readonly List<PageButton> pageButtons = new List<PageButton>();


		private readonly List<RecommendRouteSlot> recommendSlots = new List<RecommendRouteSlot>();


		private Button btnLeftPage;


		private Button btnRightPage;


		private Transform content;


		private Favorite favorite;


		private Dropdown filterDropdown;


		private List<string> filterList;


		private Toggle latestVersion;


		private int maxPage;


		private List<RecommendWeaponRoute> myRecommendWeaponRoutes = new List<RecommendWeaponRoute>();


		private bool myRoutesFlag;


		private int pageIndex = 1;


		private Transform pages;


		private List<RecommendWeaponRoute> recommendWeaponRoutes = new List<RecommendWeaponRoute>();


		private RouteFilterType routeFilterType;


		private RouteFilterUI routeFilterUI;


		private ScrollRect scrollRect;


		private RecommendRouteSortType sortType = RecommendRouteSortType.RECENT_DATETIME;


		private Toggle viewMyRoutes;


		public RouteFilterType RouteFilterType => routeFilterType;


		public int PageIndex => pageIndex;


		public RecommendRouteSortType SortType => sortType;


		public bool RecentlyVersion => latestVersion.isOn;


		
		
		public event Action<Favorite> OnClickRouteSlot;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			routeFilterUI = GameUtil.Bind<RouteFilterUI>(this.gameObject, "RouteFilterUI");
			routeFilterUI.changeFilterType += ChangeTeamModeFilter;
			filterDropdown = GameUtil.Bind<Dropdown>(this.gameObject, "FilterDropdown");
			viewMyRoutes = GameUtil.Bind<Toggle>(this.gameObject, "ViewMyRoutes/Toggle");
			viewMyRoutes.onValueChanged.AddListener(delegate(bool isOn) { UpdateViewMyRoutes(isOn); });
			latestVersion = GameUtil.Bind<Toggle>(this.gameObject, "LatestVersion/Toggle");
			latestVersion.onValueChanged.AddListener(delegate(bool isOn) { UpdateLastVersion(isOn); });
			scrollRect = GameUtil.Bind<ScrollRect>(this.gameObject, "Routes");
			content = transform.FindRecursively("Slots");
			content.GetComponentsInChildren<RecommendRouteSlot>(true, recommendSlots);
			for (int i = 0; i < recommendSlots.Count; i++)
			{
				recommendSlots[i].OnClickRouteSlot += OnClickRouteSlot;
			}

			pages = GameUtil.Bind<Transform>(this.gameObject, "ChangePage/Pages");
			Button[] componentsInChildren = pages.GetComponentsInChildren<Button>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				int index = j;
				componentsInChildren[j].onClick.AddListener(delegate
				{
					int num = pageButtons[index].PageIndex;
					RequestChangePage(num);
				});
				LnText txtBasicNumber = GameUtil.Bind<LnText>(componentsInChildren[j].gameObject, "TXT_BasicNumber");
				GameObject gameObject = componentsInChildren[j].transform.FindRecursively("Focus").gameObject;
				LnText txtFocusNumber = GameUtil.Bind<LnText>(gameObject, "TXT_FocusNumber");
				PageButton item = new PageButton(componentsInChildren[j].gameObject, txtBasicNumber, gameObject,
					txtFocusNumber);
				pageButtons.Add(item);
			}

			btnLeftPage = GameUtil.Bind<Button>(this.gameObject, "ChangePage/BTN_LeftPage");
			btnLeftPage.onClick.AddListener(delegate { ChangePageByArrow(true); });
			btnRightPage = GameUtil.Bind<Button>(this.gameObject, "ChangePage/BTN_RightPage");
			btnRightPage.onClick.AddListener(delegate { ChangePageByArrow(false); });
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			latestVersion.isOn = true;
			filterList = new List<string>();
			filterList.Add(Ln.Get("최신등록순"));
			filterList.Add(Ln.Get("공유 수"));
			filterList.Add(Ln.Get("사용 수"));
			filterDropdown.AddOptions(filterList);
			filterDropdown.onValueChanged.AddListener(delegate(int filterIndex)
			{
				ChangeDropdownFilter(filterIndex + 1);
			});
		}


		private void UpdateViewMyRoutes(bool isOn)
		{
			myRoutesFlag = isOn;
			SetMergeRecommendRoutes();
			RenderRouteSlots(favorite.slotId, favorite.order);
		}


		private void UpdateLastVersion(bool isOn)
		{
			RequestDelegate.request<RouteApi.BringRouteInfo>(
				RouteApi.GetBringRoute(new RecommendSearchParam(sortType, favorite.characterCode, favorite.weaponType,
					1, routeFilterType, latestVersion.isOn)), false,
				delegate(RequestDelegateError err, RouteApi.BringRouteInfo res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					favorite.routeFilterType = routeFilterType;
					SetBringRoutes(favorite, res.recommendWeaponRoutesCount, res.recommendWeaponRoutes,
						res.myRecommendWeaponRoutes, res.initRecommendWeaponRoute);
					SetMergeRecommendRoutes();
					RenderRouteSlots(favorite.slotId, favorite.order);
				});
		}


		private void SetMergeRecommendRoutes()
		{
			mergeRecommendWewaponRoutes.Clear();
			if (myRecommendWeaponRoutes != null && myRoutesFlag)
			{
				foreach (RecommendWeaponRoute item in myRecommendWeaponRoutes)
				{
					mergeRecommendWewaponRoutes.Add(item);
				}
			}

			if (recommendWeaponRoutes != null)
			{
				foreach (RecommendWeaponRoute recommendWeaponRoute in recommendWeaponRoutes)
				{
					if (!myRoutesFlag || recommendWeaponRoute.userNum != Lobby.inst.User.UserNum)
					{
						mergeRecommendWewaponRoutes.Add(recommendWeaponRoute);
					}
				}
			}
		}


		private void ChangeTeamModeFilter(RouteFilterType routeFilterType)
		{
			RequestDelegate.request<RouteApi.BringRouteInfo>(
				RouteApi.GetBringRoute(new RecommendSearchParam(sortType, favorite.characterCode, favorite.weaponType,
					1, routeFilterType, latestVersion.isOn)), false,
				delegate(RequestDelegateError err, RouteApi.BringRouteInfo res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					Debug.Log(string.Format("ChangeTeamModeFilter = {0}", routeFilterType));
					favorite.routeFilterType = routeFilterType;
					SetBringRoutes(favorite, res.recommendWeaponRoutesCount, res.recommendWeaponRoutes,
						res.myRecommendWeaponRoutes, res.initRecommendWeaponRoute);
					SetMergeRecommendRoutes();
					RenderRouteSlots(favorite.slotId, favorite.order);
				});
		}


		private void ChangeDropdownFilter(int filterIndex)
		{
			sortType = (RecommendRouteSortType) filterIndex;
			RequestDelegate.request<RouteApi.BringRouteInfo>(
				RouteApi.GetBringRoute(new RecommendSearchParam(sortType, favorite.characterCode, favorite.weaponType,
					1, favorite.routeFilterType, latestVersion.isOn)), false,
				delegate(RequestDelegateError err, RouteApi.BringRouteInfo res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					SetBringRoutes(favorite, res.recommendWeaponRoutesCount, res.recommendWeaponRoutes,
						res.myRecommendWeaponRoutes, res.initRecommendWeaponRoute);
					SetMergeRecommendRoutes();
					RenderRouteSlots(favorite.slotId, favorite.order);
				});
		}


		public void ResetValue()
		{
			pageIndex = 1;
			routeFilterType = RouteFilterType.ALL;
		}


		public void SetBringRoutes(Favorite favorite, int recommendWeaponRoutesCount,
			List<RecommendWeaponRoute> recommendWeaponRoutes, List<RecommendWeaponRoute> myRecommendWeaponRoutes,
			InitRecommendWeaponRoute initRecommendWeaponRoute)
		{
			this.favorite = favorite;
			if (recommendWeaponRoutesCount <= 20)
			{
				maxPage = 1;
			}
			else
			{
				int num = recommendWeaponRoutesCount / 20;
				if (recommendWeaponRoutesCount % 20 == 0)
				{
					maxPage = num;
				}
				else
				{
					maxPage = num + 1;
				}
			}

			this.recommendWeaponRoutes = recommendWeaponRoutes;
			this.myRecommendWeaponRoutes = myRecommendWeaponRoutes;
			SetMergeRecommendRoutes();
			RenderRouteSlots(favorite.slotId, favorite.order);
			UpdatePageNumbers();
			routeFilterUI.ChangeFilterType(favorite.routeFilterType);
		}


		private void InitSelect() { }


		private void RenderRouteSlots(int slotId, int order)
		{
			content.GetComponentsInChildren<RecommendRouteSlot>(true, recommendSlots);
			for (int i = 0; i < recommendSlots.Count; i++)
			{
				if (i < mergeRecommendWewaponRoutes.Count)
				{
					recommendSlots[i].gameObject.SetActive(true);
					recommendSlots[i].SetRecommendUI(slotId, order, mergeRecommendWewaponRoutes[i], sortType);
				}
				else
				{
					recommendSlots[i].gameObject.SetActive(false);
				}
			}

			InitSelect();
			scrollRect.ScrollToTop();
		}


		public void ResetSelected()
		{
			foreach (RecommendRouteSlot recommendRouteSlot in recommendSlots)
			{
				recommendRouteSlot.HideSelected();
			}
		}


		private void ChangePageByArrow(bool isLeft)
		{
			int num = pageIndex;
			pageIndex = isLeft ? pageIndex - 1 : pageIndex + 1;
			if (pageIndex < 1 || pageIndex > maxPage)
			{
				pageIndex = num;
				return;
			}

			RequestChangePage(pageIndex);
		}


		private void RequestChangePage(int pageIndex)
		{
			if (pageIndex > maxPage)
			{
				return;
			}

			Debug.Log(string.Format("PageIndex = {0}", pageIndex));
			RequestDelegate.request<RouteApi.BringRouteInfo>(
				RouteApi.GetBringRoute(new RecommendSearchParam(sortType, favorite.characterCode, favorite.weaponType,
					pageIndex, favorite.routeFilterType, latestVersion.isOn)), false,
				delegate(RequestDelegateError err, RouteApi.BringRouteInfo res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					this.pageIndex = pageIndex;
					SetBringRoutes(favorite, res.recommendWeaponRoutesCount, res.recommendWeaponRoutes,
						res.myRecommendWeaponRoutes, res.initRecommendWeaponRoute);
					SetMergeRecommendRoutes();
					RenderRouteSlots(favorite.slotId, favorite.order);
				});
		}


		public void UpdatePageNumbers()
		{
			int num;
			if (pageIndex - 2 <= 1)
			{
				num = 1;
			}
			else if (pageIndex >= maxPage - 2)
			{
				num = maxPage - 4;
			}
			else
			{
				num = pageIndex - 2;
			}

			num = Math.Max(1, num);
			for (int i = 0; i < 5; i++)
			{
				if (i < maxPage)
				{
					pageButtons[i].Show(true);
				}
				else
				{
					pageButtons[i].Show(false);
				}
			}

			for (int j = 0; j < 5; j++)
			{
				int num2 = num + j;
				pageButtons[j].SetPageNumber(num2);
				if (pageIndex == num2)
				{
					pageButtons[j].ShowFocus(true);
				}
				else
				{
					pageButtons[j].ShowFocus(false);
				}
			}
		}


		public class PageButton
		{
			public GameObject focuss;


			public GameObject obj;


			private int pageIndex;


			public LnText txtBasicNumber;


			public LnText txtFocusNumber;


			public PageButton(GameObject obj, LnText txtBasicNumber, GameObject focuss, LnText txtFocusNumber)
			{
				this.obj = obj;
				this.txtBasicNumber = txtBasicNumber;
				this.focuss = focuss;
				this.txtFocusNumber = txtFocusNumber;
			}


			public int PageIndex => pageIndex;


			public void Show(bool show)
			{
				obj.SetActive(show);
			}


			public void SetPageNumber(int pageIndex)
			{
				this.pageIndex = pageIndex;
				txtBasicNumber.text = pageIndex.ToString();
				txtFocusNumber.text = pageIndex.ToString();
			}


			public void ShowFocus(bool show)
			{
				focuss.SetActive(show);
			}
		}
	}
}