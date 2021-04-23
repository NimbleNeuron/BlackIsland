using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoritesEditorPage : BasePage, ILnEventHander
	{
		private const float bringModeMove = 80f;


		private const string FAVORITE_GUIDE_SHOW = "FAVORITE_GUIDE_SHOW";


		[SerializeField] private Text stepNumber = default;


		[SerializeField] private Text stepDesc = default;


		[SerializeField] private BringRoute bringRoute = default;


		private CanvasGroup canvasGroupItemBookView;


		private Button closeBtn;


		private FavoritesEditorStep currentFavoritesEditorStep;


		private FavoriteEditorViewStyle editorViewStyle;


		private Favorite favorite;


		private GameObject favoriteItemGuide;


		private GameObject favoriteRouteGuide;


		private FavoritesCommonView favoritesCommonView;


		private FavoritesRouteView favoritesRouteView;


		private ItemData focusItemData;


		private Button guideBtn;


		private RectTransform header;


		private LobbyItemBookView lobbyItemBookView;


		private Vector2 originPosition_book;


		private Vector2 originPosition_common;


		private Vector2 originPosition_header;


		private Vector2 originPosition_route;


		private GameObject routeGuide;


		public BringRoute BringRoute => bringRoute;


		public Favorite Favorite => favorite;


		public FavoritesEditorStep CurrentFavoritesEditorStep => currentFavoritesEditorStep;


		public void OnLnDataChange()
		{
			favoritesRouteView.LocalizeUIMapAreaName();
		}

		
		
		public event Action<bool, bool> OnChangeState = delegate { };


		
		
		public event Action<Favorite> OnRequestSave = delegate { };


		
		
		public event Action OnRequestDiscard = delegate { };


		
		
		public event Action<Favorite> OnRequestCopy = delegate { };


		
		
		public event Action OnRequestBringClose = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			bringRoute.OnClickRouteSlot += UpdateRecommendFavoriteUI;
			header = GameUtil.Bind<RectTransform>(gameObject, "Header");
			favoritesCommonView = GameUtil.Bind<FavoritesCommonView>(gameObject, "FavoritesCommonView");
			favoritesCommonView.OnEdited += UpdateFavoriteUI;
			favoritesCommonView.OnRequestNextStep += NextStep;
			favoritesCommonView.OnRequestPreStep += PreStep;
			favoritesCommonView.OnRequestSave += delegate { OnRequestSave(favorite); };
			favoritesCommonView.OnRequestDiscard += delegate { OnRequestDiscard(); };
			favoritesCommonView.OnRequestCopy += OnRequestCopy;
			favoritesCommonView.OnRequestBringClose += delegate { OnRequestBringClose(); };
			favoritesCommonView.OnChangeState += OnChangeState;
			favoritesCommonView.OnClickFavItem += OnClickFavStepItem;
			lobbyItemBookView = GameUtil.Bind<LobbyItemBookView>(gameObject, "ItemBookView");
			lobbyItemBookView.OnRightClickItem += favoritesCommonView.AddFavItem;
			lobbyItemBookView.SetDraggable(true);
			GameUtil.Bind<CanvasGroup>(lobbyItemBookView.gameObject, ref canvasGroupItemBookView);
			favoritesRouteView = GameUtil.Bind<FavoritesRouteView>(gameObject, "FavoritesRouteView");
			favoritesRouteView.OnEdited += UpdateFavoriteUI;
			favoritesRouteView.OnChangeState += OnChangeState;
			routeGuide = GameUtil.Bind<Transform>(gameObject, "RouteGuide").gameObject;
			favoriteItemGuide = GameUtil.Bind<Transform>(gameObject, "RouteGuide/RouteEdit1").gameObject;
			favoriteRouteGuide = GameUtil.Bind<Transform>(gameObject, "RouteGuide/RouteEdit2").gameObject;
			guideBtn = GameUtil.Bind<Button>(gameObject, "BTN_ShowGuide");
			guideBtn.onClick.AddListener(OnClickGuideShow);
			closeBtn = GameUtil.Bind<Button>(gameObject, "BTN_Close");
			closeBtn.onClick.AddListener(delegate { OnRequestBringClose(); });
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent -= OnCompleteMatching;
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent += OnCompleteMatching;
		}


		private void OnCompleteMatching()
		{
			OnClickGuideClose();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			originPosition_header = header.anchoredPosition;
			originPosition_common = favoritesCommonView.GetComponent<RectTransform>().anchoredPosition;
			originPosition_book = lobbyItemBookView.GetComponent<RectTransform>().anchoredPosition;
			originPosition_route = favoritesRouteView.GetComponent<RectTransform>().anchoredPosition;
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			RenderPage();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			favorite = null;
			focusItemData = null;
			MonoBehaviourInstance<LobbyUI>.inst.ShowBringGuard(false);
			stepNumber.gameObject.SetActive(true);
			guideBtn.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(false);
			lobbyItemBookView.EmptyUI();
			favoritesCommonView.DisableFrameEffects();
			favoritesCommonView.ResetUI();
			favoritesRouteView.ResetUI();
			favoritesRouteView.UnEnableSelectUI();
			if (editorViewStyle == FavoriteEditorViewStyle.ROUTEBRING)
			{
				bringRoute.gameObject.SetActive(false);
				bringRoute.ResetValue();
				bringRoute.ResetSelected();
			}

			header.anchoredPosition = originPosition_header;
			favoritesCommonView.SetAnchoredPosition(originPosition_common);
			lobbyItemBookView.SetAnchoredPosition(originPosition_book);
			favoritesRouteView.SetAnchoredPosition(originPosition_route);
			editorViewStyle = FavoriteEditorViewStyle.NORMAL;
		}


		public void CheckModeUI(FavoriteEditorViewStyle editorViewStyle)
		{
			this.editorViewStyle = editorViewStyle;
			favoritesCommonView.SetCheckModeButtons(editorViewStyle);
			favoritesRouteView.DeleteHide(editorViewStyle);
		}


		public void BringModeUI()
		{
			editorViewStyle = FavoriteEditorViewStyle.ROUTEBRING;
			stepNumber.gameObject.SetActive(false);
			closeBtn.gameObject.SetActive(true);
			bringRoute.gameObject.SetActive(true);
			favoritesCommonView.SetBringModeButtons(editorViewStyle);
			favoritesRouteView.DeleteHide(editorViewStyle);
			Vector2 b = new Vector2(80f, 0f);
			header.anchoredPosition = originPosition_header + b;
			favoritesCommonView.SetAnchoredPosition(originPosition_common + b);
			lobbyItemBookView.SetAnchoredPosition(originPosition_book + b);
			favoritesRouteView.SetAnchoredPosition(originPosition_route + b);
		}


		public RouteFilterType GetBringFilterType()
		{
			return bringRoute.RouteFilterType;
		}


		public RecommendRouteSortType GetBringSortType()
		{
			return bringRoute.SortType;
		}


		public int GetBringPageIndex()
		{
			return bringRoute.PageIndex;
		}


		public bool GetRecentlyVersion()
		{
			return bringRoute.RecentlyVersion;
		}


		public void Load(Favorite favorite)
		{
			this.favorite = (Favorite) favorite.Clone();
		}


		private void RenderPage()
		{
			if (favorite != null)
			{
				lobbyItemBookView.SetWeaponTypes(new[]
				{
					favorite.weaponType
				});
			}

			if (editorViewStyle == FavoriteEditorViewStyle.ROUTEBRING)
			{
				SetStep(FavoritesEditorStep.Route);
				return;
			}

			SetStep(FavoritesEditorStep.Item);
		}


		private void UpdateRecommendFavoriteUI(Favorite favorite)
		{
			Load(favorite);
			favoritesCommonView.Load(favorite);
			favoritesRouteView.Load(favorite);
			favoritesRouteView.UnEnableSelectUI();
			bringRoute.ResetSelected();
		}


		private void UpdateFavoriteUI(Favorite favorite)
		{
			favoritesCommonView.Load(favorite);
			if (favoritesRouteView.IsActive())
			{
				favoritesRouteView.Load(favorite);
			}

			if (currentFavoritesEditorStep == FavoritesEditorStep.Route)
			{
				if (focusItemData != null && !favorite.weaponCodes.Contains(focusItemData.code))
				{
					focusItemData = null;
				}

				favoritesRouteView.HighLightTargetItem(focusItemData);
			}
		}


		public void NextStep()
		{
			if (currentFavoritesEditorStep < FavoritesEditorStep.Route)
			{
				SetStep(currentFavoritesEditorStep + 1);
			}
		}


		public void PreStep()
		{
			if (currentFavoritesEditorStep > FavoritesEditorStep.Item)
			{
				SetStep(currentFavoritesEditorStep - 1);
			}
		}


		private void OnClickFavStepItem(ItemData data)
		{
			focusItemData = data;
			favoritesCommonView.DisableFrameEffects();
			lobbyItemBookView.OnClickCommonViewItem(data);
			favoritesRouteView.HighLightTargetItem(data);
			favoritesRouteView.DrawRoutePathNumber(favorite);
		}


		private void SetStep(FavoritesEditorStep favoritesEditorStep)
		{
			if (favoritesEditorStep != FavoritesEditorStep.Item)
			{
				if (favoritesEditorStep != FavoritesEditorStep.Route)
				{
					throw new ArgumentOutOfRangeException("favoritesEditorStep", favoritesEditorStep, null);
				}

				favoritesCommonView.gameObject.SetActive(true);
				canvasGroupItemBookView.alpha = 0f;
				favoritesRouteView.gameObject.SetActive(true);
				stepDesc.text = Ln.Get("루트 복사하기");
				if (editorViewStyle != FavoriteEditorViewStyle.ROUTEBRING)
				{
					stepDesc.text = Ln.Get("지역 선택");
					favoritesCommonView.EnableNextButton(false);
					favoritesCommonView.EnableSaveButton(true);
					favoritesCommonView.EnablePreButton(true);
					ShowRouteGuide();
				}
			}
			else
			{
				guideBtn.gameObject.SetActive(true);
				favoritesCommonView.gameObject.SetActive(true);
				canvasGroupItemBookView.alpha = 1f;
				favoritesRouteView.gameObject.SetActive(false);
				favoritesCommonView.EnableNextButton(true);
				favoritesCommonView.EnableSaveButton(true);
				favoritesCommonView.EnablePreButton(false);
				stepDesc.text = Ln.Get("아이템 선택");
				ShowItemGuide();
			}

			favoritesCommonView.SetStep(favoritesEditorStep);
			currentFavoritesEditorStep = favoritesEditorStep;
			Text text = stepNumber;
			int num = (int) currentFavoritesEditorStep;
			text.text = num.ToString();
			UpdateFavoriteUI(favorite);
		}


		public void ShowItemGuide()
		{
			if (editorViewStyle != FavoriteEditorViewStyle.ROUTEBRING)
			{
				int @int = PlayerPrefs.GetInt("FAVORITE_GUIDE_SHOW", 0);
				if ((@int & 1) == 0)
				{
					routeGuide.SetActive(true);
					favoriteItemGuide.SetActive(true);
					PlayerPrefs.SetInt("FAVORITE_GUIDE_SHOW", @int + 1);
				}
			}
		}


		public void ShowRouteGuide()
		{
			if (editorViewStyle != FavoriteEditorViewStyle.ROUTEBRING)
			{
				int @int = PlayerPrefs.GetInt("FAVORITE_GUIDE_SHOW", 0);
				if ((@int & 2) == 0)
				{
					routeGuide.SetActive(true);
					favoriteRouteGuide.SetActive(true);
					PlayerPrefs.SetInt("FAVORITE_GUIDE_SHOW", @int + 2);
				}
			}
		}


		public bool IsOpenGuide()
		{
			return favoriteItemGuide.activeSelf || favoriteRouteGuide.activeSelf;
		}


		public void OnClickGuideClose()
		{
			if (favoriteItemGuide.activeSelf)
			{
				routeGuide.SetActive(false);
				favoriteItemGuide.SetActive(false);
				return;
			}

			if (favoriteRouteGuide.activeSelf)
			{
				routeGuide.SetActive(false);
				favoriteRouteGuide.SetActive(false);
			}
		}


		private void OnClickGuideShow()
		{
			FavoritesEditorStep favoritesEditorStep = currentFavoritesEditorStep;
			if (favoritesEditorStep == FavoritesEditorStep.Item)
			{
				routeGuide.SetActive(true);
				favoriteItemGuide.SetActive(true);
				return;
			}

			if (favoritesEditorStep != FavoritesEditorStep.Route)
			{
				return;
			}

			routeGuide.SetActive(true);
			favoriteRouteGuide.SetActive(true);
		}
	}
}