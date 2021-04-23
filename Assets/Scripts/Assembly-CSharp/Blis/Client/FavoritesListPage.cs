using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoritesListPage : BasePage
	{
		[SerializeField] private SlimCharacterSelectionView characterSelectionView = default;


		[SerializeField] private VerticalLayoutGroup favoriteListContent = default;


		[SerializeField] private GameObject favoriteBoard = default;


		private readonly List<Favorite> favoritesList = new List<Favorite>();


		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


		private Button btnBuy = default;


		private GameObject btnMove = default;


		private GameObject btnSave = default;


		private int currentCharacter = 1;


		private EventTrigger eventTrigger;


		private FavoriteBoard[] favoriteBoards = default;


		private GameObject focusCustom = default;


		private GameObject focusRecommend = default;


		private int freeMaxSlotCount = default;


		private List<InitRecommendWeaponRoute> initRecommendWeaponRoutes;


		private bool isDragging;


		private int moveCount;


		private int paidMaxSlotCount = default;


		private int paidSlotCount = default;


		private ScrollRect scrollRect = default;


		private GameObject slotPurchase = default;


		private GameObject tabButtons = default;


		private Text txtSlotNumber = default;


		private Coroutine updateDrag;


		private List<UserWeaponRoute> userWeaponRoutes;

		
		
		public event Action<Favorite, bool> OnRequestOpenEditor = delegate { };


		
		
		public event Action<Favorite> OnRequestOpenBringMode = delegate { };


		
		
		public event Action<Favorite, FavoriteBoard> OnRequestReset = delegate { };


		
		
		public event Action OnRequestShare = delegate { };


		
		
		public event Action<int> OnChangeCharacter = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			characterSelectionView.OnCharacterSelected += delegate(int characterCode)
			{
				if (currentCharacter != characterCode)
				{
					ResetButtons();
					OnChangeCharacter(characterCode);
				}
			};
			isDragging = false;
			btnMove = transform.FindRecursively("BTN_Move").gameObject;
			btnSave = transform.FindRecursively("BTN_Save").gameObject;
			focusCustom = transform.FindRecursively("FocusCustom").gameObject;
			focusRecommend = transform.FindRecursively("FocusRecommend").gameObject;
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "Scroll View");
			tabButtons = transform.FindRecursively("TabButtons").gameObject;
			slotPurchase = tabButtons.transform.FindRecursively("SlotPurchase").gameObject;
			btnBuy = GameUtil.Bind<Button>(slotPurchase, "BTN_Buy");
			txtSlotNumber = GameUtil.Bind<Text>(slotPurchase, "SlotNumber");
			GameUtil.BindOrAdd<EventTrigger>(slotPurchase.gameObject, ref eventTrigger);
			eventTrigger.triggers.Clear();
			onEnterEvent.AddListener(OnPointerEnter);
			onExitEvent.AddListener(OnPointerExit);
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerEnter,
				callback = onEnterEvent
			});
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerExit,
				callback = onExitEvent
			});
			favoriteBoards = favoriteListContent.GetComponentsInChildren<FavoriteBoard>(true);
			for (int i = 0; i < favoriteBoards.Length; i++)
			{
				favoriteBoards[i].bringAction += RequestOpenBringMode;
				favoriteBoards[i].editAction += RequestOpenEditor;
				favoriteBoards[i].resetAction += OnClickResetFav;
				favoriteBoards[i].shareAction += OnClickShareFav;
				favoriteBoards[i].loadAction += RequestLoadRouteId;
				favoriteBoards[i].beginDragAction += OnBeginDrag;
				favoriteBoards[i].endDragAction += OnEndDrag;
				favoriteBoards[i].dragAction += OnDrag;
				favoriteBoards[i].scrollAction += scrollRect.OnScroll;
			}
		}


		private void OnPointerEnter(BaseEventData eventData)
		{
			Vector2 vector = slotPurchase.transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(0f, 70f);
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("루트 슬롯 구매 설명"));
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		private void OnPointerExit(BaseEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		private void OnBeginDrag(FavoriteBoard favoriteBoard, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				isDragging = true;
				DraggingUI = favoriteBoard;
				StopCoroutineUpdateDrag();
				updateDrag = this.StartThrowingCoroutine(UpdateDrag(),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][OnBeginDrag] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		private void OnEndDrag(FavoriteBoard favoriteBoard, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				DraggingUI.transform.position = new Vector3(DraggingUI.transform.position.x, eventData.position.y, 0f);
				int siblingIndex = favoriteBoard.transform.GetSiblingIndex();
				RectTransform component =
					favoriteListContent.transform.GetChild(siblingIndex).GetComponent<RectTransform>();
				int siblingIndex2 = siblingIndex;
				for (int i = 0; i < favoriteBoards.Length; i++)
				{
					RectTransform component2 = favoriteListContent.transform.GetChild(i).GetComponent<RectTransform>();
					if (component2.gameObject.activeSelf)
					{
						if (i > siblingIndex)
						{
							if (DraggingUI.transform.position.y < component2.position.y)
							{
								siblingIndex2 = i;
							}
						}
						else if (i < siblingIndex && DraggingUI.transform.position.y > component2.position.y)
						{
							siblingIndex2 = i;
							break;
						}
					}
				}

				component.SetSiblingIndex(siblingIndex2);
				favoriteListContent.SetLayoutVertical();
				isDragging = false;
				DraggingUI = null;
				StopCoroutineUpdateDrag();
			}
		}


		private void OnDrag(FavoriteBoard favoriteBoard, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (DraggingUI == null)
				{
					return;
				}

				DraggingUI.transform.position = new Vector3(DraggingUI.transform.position.x, eventData.position.y, 0f);
				Vector3[] array = new Vector3[4];
				scrollRect.viewport.GetWorldCorners(array);
				if (DraggingUI.transform.position.y < array[0].y)
				{
					scrollRect.verticalNormalizedPosition -= 0.05f;
					return;
				}

				if (DraggingUI.transform.position.y > array[1].y)
				{
					scrollRect.verticalNormalizedPosition += 0.05f;
				}
			}
		}


		private IEnumerator UpdateDrag()
		{
			while (isDragging && DraggingUI != null)
			{
				if (!Input.GetMouseButton(0))
				{
					FavoriteBoard component = DraggingUI.GetComponent<FavoriteBoard>();
					OnEndDrag(component, new PointerEventData(EventSystem.current));
					break;
				}

				yield return new WaitForEndOfFrame();
			}
		}


		private void StopCoroutineUpdateDrag()
		{
			if (updateDrag != null)
			{
				StopCoroutine(updateDrag);
				updateDrag = null;
			}
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			initRecommendWeaponRoutes = new List<InitRecommendWeaponRoute>();
			userWeaponRoutes = new List<UserWeaponRoute>();
			RequestDelegate.request<RouteApi.UserRoute>(RouteApi.GetUserRoute(currentCharacter), false,
				delegate(RequestDelegateError err, RouteApi.UserRoute res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					initRecommendWeaponRoutes = res.initRecommendWeaponRoutes;
					userWeaponRoutes = res.userWeaponRouteResults.Select(e => e.userWeaponRoute).ToList();
					freeMaxSlotCount = res.freeMaxSlotCount;
					paidMaxSlotCount = res.paidMaxSlotCount;
					paidSlotCount = res.paidSlotCount;
					userWeaponRoutes.Sort((x, y) => x.order.CompareTo(y.order));
					if (userWeaponRoutes == null || userWeaponRoutes.Count == 0)
					{
						OnRecommendTab();
						return;
					}

					OnCustomTab();
				});
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			ResetButtons();
		}


		public void SetCurrentCharacter(int currentCharacter)
		{
			this.currentCharacter = currentCharacter;
		}


		private void RenderCustomPage()
		{
			btnMove.SetActive(favoritesList.Count > 0);
			btnSave.SetActive(false);
			slotPurchase.SetActive(true);
			characterSelectionView.FocusCharacter(currentCharacter);
			characterSelectionView.FocusCharacterPosition(currentCharacter);
			int num = freeMaxSlotCount + paidSlotCount;
			int num2 = freeMaxSlotCount + paidMaxSlotCount - favoriteListContent.transform.childCount;
			for (int i = 0; i < num2; i++)
			{
				FavoriteBoard component = Instantiate<GameObject>(favoriteBoard, favoriteListContent.transform)
					.GetComponent<FavoriteBoard>();
				component.bringAction += RequestOpenBringMode;
				component.editAction += RequestOpenEditor;
				component.shareAction += OnClickShareFav;
				component.resetAction += OnClickResetFav;
				component.loadAction += RequestLoadRouteId;
				component.beginDragAction += OnBeginDrag;
				component.endDragAction += OnEndDrag;
				component.dragAction += OnDrag;
				component.scrollAction += scrollRect.OnScroll;
			}

			txtSlotNumber.text = string.Format("{0} / {1}", favoritesList.Count, num);
			btnBuy.interactable = paidSlotCount < paidMaxSlotCount;
			favoriteBoards = favoriteListContent.GetComponentsInChildren<FavoriteBoard>(true);
			List<int> list = new List<int>();
			for (int j = 0; j < favoriteBoards.Length; j++)
			{
				list.Add(j + 1);
			}

			for (int k = 0; k < favoriteBoards.Length; k++)
			{
				favoriteBoards[k].gameObject.SetActive(true);
				if (k < favoritesList.Count)
				{
					favoriteBoards[k].SetFavorite(favoritesList[k]);
					favoriteBoards[k].SetCustomButtonStyle();
					list.Remove(favoritesList[k].slotId);
				}
				else if (k == favoritesList.Count)
				{
					if (k < num)
					{
						favoriteBoards[k].SetCreate(currentCharacter, list[0]);
					}
					else
					{
						favoriteBoards[k].SetBlank(Ln.Get("빈 슬롯(루트 페이지 구매 필요)"));
					}
				}
				else if (k < num)
				{
					favoriteBoards[k].SetBlank(Ln.Get("빈 슬롯"));
				}
				else
				{
					favoriteBoards[k].SetBlank(Ln.Get("빈 슬롯(루트 페이지 구매 필요)"));
				}
			}
		}


		private void RenderRecommendPage()
		{
			btnMove.SetActive(false);
			btnSave.SetActive(false);
			slotPurchase.SetActive(false);
			characterSelectionView.FocusCharacter(currentCharacter);
			characterSelectionView.FocusCharacterPosition(currentCharacter);
			favoriteBoards = favoriteListContent.GetComponentsInChildren<FavoriteBoard>(true);
			for (int i = 0; i < favoriteBoards.Length; i++)
			{
				if (i < favoritesList.Count)
				{
					favoriteBoards[i].gameObject.SetActive(true);
					favoriteBoards[i].SetFavorite(favoritesList[i]);
					favoriteBoards[i].SetRecommendButtonStyle();
				}
				else
				{
					favoriteBoards[i].gameObject.SetActive(false);
				}
			}
		}


		private void RequestOpenEditor(Favorite favorite, int index, bool isCheckMode)
		{
			favorite.order = GetFavoriteOrder(index);
			OnRequestOpenEditor(favorite, isCheckMode);
		}


		private void RequestOpenBringMode(Favorite favorite, int index)
		{
			favorite.order = GetFavoriteOrder(index);
			OnRequestOpenBringMode(favorite);
		}


		private void OnClickResetFav(Favorite favorite, FavoriteBoard favoriteBoard)
		{
			OnRequestReset(favorite, favoriteBoard);
		}


		private void OnClickShareFav()
		{
			OnRequestShare();
		}


		private void RequestLoadRouteId(int slotId, int index, string strInput)
		{
			RequestDelegate.request<RouteApi.UserWeaponRouteResult>(
				RouteApi.GetRecommendRouteById(new UserWeaponRouteParam(currentCharacter, slotId,
					GetFavoriteOrder(index), long.Parse(strInput))), false,
				delegate(RequestDelegateError err, RouteApi.UserWeaponRouteResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
						return;
					}

					userWeaponRoutes.Add(res.userWeaponRoute);
					userWeaponRoutes.Sort((x, y) => x.order.CompareTo(y.order));
					OnCustomTab();
				});
		}


		private int GetFavoriteOrder(int index)
		{
			if (index <= 0)
			{
				return 1;
			}

			return favoriteBoards[index - 1].FavoriteOrder + 1;
		}


		private void OnCustomTab()
		{
			focusCustom.SetActive(true);
			focusRecommend.SetActive(false);
			favoritesList.Clear();
			foreach (UserWeaponRoute userWeaponRoute in userWeaponRoutes)
			{
				List<int> numbersForStringData = GetNumbersForStringData(userWeaponRoute.weaponCodes);
				List<int> numbersForStringData2 = GetNumbersForStringData(userWeaponRoute.paths);
				string title = string.IsNullOrEmpty(userWeaponRoute.title)
					? GetDefaultRouteTitle(userWeaponRoute.weaponType)
					: userWeaponRoute.title;
				Favorite item = new Favorite(Lobby.inst.User.UserNum, userWeaponRoute.characterCode,
					userWeaponRoute.slotId, title, userWeaponRoute.weaponType, numbersForStringData,
					numbersForStringData2, userWeaponRoute.recommendWeaponRouteId,
					userWeaponRoute.recommendUserNickname, userWeaponRoute.share, userWeaponRoute.initRecommend,
					userWeaponRoute.version, userWeaponRoute.teamMode, userWeaponRoute.order,
					userWeaponRoute.shareWeaponRouteId);
				favoritesList.Add(item);
			}

			RenderCustomPage();
		}


		private void OnRecommendTab()
		{
			focusCustom.SetActive(false);
			focusRecommend.SetActive(true);
			favoritesList.Clear();
			int num = 1;
			foreach (InitRecommendWeaponRoute initRecommendWeaponRoute in initRecommendWeaponRoutes)
			{
				if (initRecommendWeaponRoute.weaponType != WeaponType.DualSword)
				{
					List<int> numbersForStringData = GetNumbersForStringData(initRecommendWeaponRoute.weaponCodes);
					List<int> numbersForStringData2 = GetNumbersForStringData(initRecommendWeaponRoute.paths);
					string defaultRouteTitle = GetDefaultRouteTitle(initRecommendWeaponRoute.weaponType);
					Favorite item = new Favorite(-1L, initRecommendWeaponRoute.characterCode, num, defaultRouteTitle,
						initRecommendWeaponRoute.weaponType, numbersForStringData, numbersForStringData2, -1L,
						"NimbleNeuron", true, true, "", RouteFilterType.ALL, 0, -1L);
					favoritesList.Add(item);
					num++;
				}
			}

			RenderRecommendPage();
		}


		private List<int> GetNumbersForStringData(string str)
		{
			List<int> list = new List<int>();
			if (!string.IsNullOrEmpty(str))
			{
				string[] array = str.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					int item = int.Parse(array[i]);
					list.Add(item);
				}
			}

			return list;
		}


		private string GetDefaultRouteTitle(WeaponType weaponType)
		{
			return Ln.Format("{0} 추천 루트", Ln.Get(string.Format("WeaponType/{0}", weaponType)));
		}


		private void ResetButtons()
		{
			tabButtons.SetActive(true);
			btnMove.SetActive(false);
			btnSave.SetActive(false);
			for (int i = 0; i < favoriteBoards.Length; i++)
			{
				favoriteBoards[i].gameObject.SetActive(false);
			}
		}


		public void ClickedCustom()
		{
			OnCustomTab();
		}


		public void ClickedRecommend()
		{
			OnRecommendTab();
		}


		public void ClickedBuy()
		{
			ShopProductService.RequestFavoriteWeaponRouteSlot(delegate(ShopProduct shopProduct)
			{
				if (shopProduct != null)
				{
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(shopProduct);
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.buySuccessRouteSlotCallback = delegate
					{
						paidSlotCount++;
						RenderCustomPage();
					};
					MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.noMoneyOpenShopCallback = delegate
					{
						MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
							.NoMoneyOpenShopCallback();
					};
				}
			}, currentCharacter);
		}


		public void ClickedMove()
		{
			moveCount = 0;
			tabButtons.SetActive(false);
			btnMove.SetActive(false);
			btnSave.SetActive(true);
			for (int i = 0; i < favoriteBoards.Length; i++)
			{
				favoriteBoards[i].gameObject.SetActive(favoriteBoards[i].FavoriteOrder > 0);
				moveCount += favoriteBoards[i].FavoriteOrder > 0 ? 1 : 0;
				favoriteBoards[i].SetButtons(false);
				favoriteBoards[i].SetSlotMove(true);
			}
		}


		public void ClickedSave()
		{
			tabButtons.SetActive(true);
			btnMove.SetActive(true);
			btnSave.SetActive(false);
			for (int i = 0; i < favoriteBoards.Length; i++)
			{
				favoriteBoards[i].gameObject.SetActive(true);
				favoriteBoards[i].SetButtons();
				favoriteBoards[i].SetSlotMove();
			}

			List<UserWeaponRouteParam> list = new List<UserWeaponRouteParam>();
			for (int j = 0; j < favoriteBoards.Length; j++)
			{
				if (favoriteBoards[j].FavoriteOrder > 0)
				{
					list.Add(new UserWeaponRouteParam(currentCharacter, favoriteBoards[j].FavoriteSlotId,
						favoriteBoards[j].transform.GetSiblingIndex() + 1, 0L));
				}
			}

			RequestDelegate.request<NullResponse>(RouteApi.ChangeOrdersRoute(list), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}
				});
		}
	}
}