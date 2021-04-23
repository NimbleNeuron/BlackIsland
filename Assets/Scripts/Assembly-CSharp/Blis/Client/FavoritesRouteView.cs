using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoritesRouteView : BaseUI, IMapSlicePointerEventHandler, ISlotEventListener
	{
		[SerializeField] private List<UIFavoritesArea> areas = default;


		[SerializeField] private ItemDataSlotTable searchTarget = default;


		[SerializeField] private ItemDataSlotTable gatherTarget = default;


		[SerializeField] private UIMap uiMap = default;


		[SerializeField] private ScrollRect scrollRect = default;


		[SerializeField] private GameObject guideText = default;


		[SerializeField] private GameObject delete = default;


		private readonly Dictionary<int, List<int>> fixAreaItems = new Dictionary<int, List<int>>();


		private readonly Dictionary<ItemData, int> sourceItems = new Dictionary<ItemData, int>();


		private Sprite activeSprite = default;


		private GameObject collection = default;


		private GameObject detailMenu = default;


		private FavoriteEditorViewStyle editorViewStyle = default;


		private Favorite favorite;


		private FilterObject filterObjectCollection = default;


		private FilterObject filterObjectLeafNodeWeapon = default;


		private FilterObject filterObjectOverlap = default;


		private Image imgDetail = default;


		private Sprite inActiveSprite = default;


		private GameObject leafNodeWeapon = default;


		private GameObject overlap = default;


		private ItemData selectItemData = default;


		private Dictionary<ItemData, int> startSources = new Dictionary<ItemData, int>();


		private int startWeaponItemCode = -1;


		public void OnPointerMapClick(int areaCode, PointerEventData.InputButton mouseButton)
		{
			if (mouseButton == PointerEventData.InputButton.Left)
			{
				if (uiMap.GetLaboratoryAreaCode() == areaCode)
				{
					return;
				}

				if (editorViewStyle >= FavoriteEditorViewStyle.ROUTEBRING)
				{
					return;
				}

				if (favorite.paths.Contains(areaCode))
				{
					RequestDeleteArea(areaCode);
					OnPointerMapEnter(areaCode);
					return;
				}

				RequestAddArea(areaCode);
			}
		}


		public void OnPointerMapEnter(int areaCode)
		{
			uiMap.SetRollOver(areaCode, true);
			foreach (ItemData itemData in searchTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot = searchTarget.CreateSlot(itemData);
				if (itemDataSlot != null)
				{
					itemDataSlot.EnableNeedMark(Singleton<ItemService>.inst.IsDropArea(areaCode, itemData.code));
				}
			}

			foreach (ItemData itemData2 in gatherTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot2 = gatherTarget.CreateSlot(itemData2);
				if (itemDataSlot2 != null)
				{
					itemDataSlot2.EnableNeedMark(Singleton<ItemService>.inst.IsDropArea(areaCode, itemData2.code));
				}
			}

			for (int i = 0; i < areas.Count; i++)
			{
				ItemDataSlotTable itemDataSlotTable = areas[i].GetItemDataSlotTable();
				foreach (ItemData itemData3 in itemDataSlotTable.GetKeys())
				{
					ItemDataSlot itemDataSlot3 = itemDataSlotTable.CreateSlot(itemData3);
					if (itemDataSlot3 != null)
					{
						itemDataSlot3.EnableNeedMark(Singleton<ItemService>.inst.IsDropArea(areaCode, itemData3.code));
					}
				}

				areas[i].EnableHoverEffect(areas[i].GetAreaCode() == areaCode);
			}
		}


		public void OnPointerMapExit(int areaCode)
		{
			uiMap.SetRollOver(areaCode, false);
			foreach (ItemData t in searchTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot = searchTarget.CreateSlot(t);
				if (itemDataSlot != null)
				{
					itemDataSlot.EnableNeedMark(false);
				}
			}

			foreach (ItemData t2 in gatherTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot2 = gatherTarget.CreateSlot(t2);
				if (itemDataSlot2 != null)
				{
					itemDataSlot2.EnableNeedMark(false);
				}
			}

			for (int i = 0; i < areas.Count; i++)
			{
				ItemDataSlotTable itemDataSlotTable = areas[i].GetItemDataSlotTable();
				foreach (ItemData t3 in itemDataSlotTable.GetKeys())
				{
					ItemDataSlot itemDataSlot3 = itemDataSlotTable.CreateSlot(t3);
					if (itemDataSlot3 != null)
					{
						itemDataSlot3.EnableNeedMark(false);
					}
				}
			}
		}


		public void OnSlotLeftClick(Slot slot) { }


		public void OnSlotDoubleClick(Slot slot) { }


		public void OnSlotRightClick(Slot slot) { }


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot)
		{
			if (slot is ItemDataSlot)
			{
				ShowMaterialMap(((ItemDataSlot) slot).GetItemData());
			}

			uiMap.SetRouteText(favorite.paths);
		}


		public void OnPointerExit(Slot slot)
		{
			ShowMaterialMap(null);
			uiMap.SetRouteText(favorite.paths);
		}

		
		
		public event Action<Favorite> OnEdited = delegate { };


		
		
		public event Action<bool, bool> OnChangeState = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			detailMenu = transform.FindRecursively("DetailMenu").gameObject;
			imgDetail = GameUtil.Bind<Image>(gameObject, "Setting/Title/Detail");
			inActiveSprite = imgDetail.sprite;
			activeSprite = imgDetail.GetComponent<Button>().spriteState.highlightedSprite;
			overlap = detailMenu.transform.FindRecursively("Overlap").gameObject;
			collection = detailMenu.transform.FindRecursively("Collection").gameObject;
			leafNodeWeapon = detailMenu.transform.FindRecursively("LeafNodeWeapon").gameObject;
			filterObjectOverlap = new FilterObject(overlap);
			filterObjectOverlap.OnToggleChange += delegate { FilterLoad(); };
			filterObjectCollection = new FilterObject(collection);
			filterObjectCollection.OnToggleChange += delegate { FilterLoad(); };
			filterObjectLeafNodeWeapon = new FilterObject(leafNodeWeapon);
			filterObjectLeafNodeWeapon.OnToggleChange += delegate { FilterLoad(); };
			areas.ForEach(delegate(UIFavoritesArea x) { x.OnRequestRemove += RequestDeleteArea; });
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			uiMap.Init(GameDB.level.DefaultLevel);
			uiMap.SetEventHandler(this);
			uiMap.SetMapMode(UIMap.MapModeFlag.SearchItem);
		}


		public void DeleteHide(FavoriteEditorViewStyle editorViewStyle)
		{
			this.editorViewStyle = editorViewStyle;
			delete.SetActive(false);
			foreach (UIFavoritesArea uifavoritesArea in areas)
			{
				uifavoritesArea.DeleteEnable(false);
			}
		}


		public void ResetUI()
		{
			selectItemData = null;
			editorViewStyle = FavoriteEditorViewStyle.NORMAL;
			delete.SetActive(true);
			foreach (UIFavoritesArea uifavoritesArea in areas)
			{
				uifavoritesArea.DeleteEnable(true);
			}
		}


		public void UnEnableSelectUI()
		{
			foreach (UIFavoritesArea uifavoritesArea in areas)
			{
				uifavoritesArea.UnEnableSelection();
			}
		}


		public void Load(Favorite favorite)
		{
			this.favorite = favorite;
			RecommendStarting recommendStarting =
				GameDB.recommend.FindStartingData(favorite.characterCode, favorite.weaponType.GetWeaponMasteryType());
			startWeaponItemCode = recommendStarting.startWeapon;
			if (selectItemData != null && !favorite.weaponCodes.Contains(selectItemData.code))
			{
				selectItemData = null;
			}

			GameDB.item.SetFavoriteSourceDictionary(favorite.weaponCodes, sourceItems, true);
			if (gameObject.activeSelf)
			{
				RenderView();
				RenderCompleteItem();
				HighLightTargetItem(null);
				scrollRect.ScrollToBottom();
			}

			detailMenu.SetActive(false);
			imgDetail.sprite = inActiveSprite;
		}


		public void FilterLoad()
		{
			GameDB.item.SetFavoriteSourceDictionary(favorite.weaponCodes, sourceItems, true);
			if (gameObject.activeSelf)
			{
				RenderView();
				RenderCompleteItem();
				HighLightTargetItem(null);
			}
		}


		private void RenderView()
		{
			areas.ForEach(delegate(UIFavoritesArea x)
			{
				x.Clear();
				x.gameObject.SetActive(false);
			});
			uiMap.SetRouteText(favorite.paths);
			List<ItemData> list = new List<ItemData>();
			List<int> collectAndHuntItems = GameDB.item.GetCollectAndHuntItems();
			foreach (KeyValuePair<ItemData, int> keyValuePair in sourceItems)
			{
				ItemData key = keyValuePair.Key;
				foreach (int num in collectAndHuntItems)
				{
					if (key.code == num && !list.Contains(key))
					{
						list.Add(key);
					}
				}
			}

			int num2 = 0;
			List<int> list2 = new List<int>();
			startSources = new Dictionary<ItemData, int>();
			Dictionary<ItemData, int> dictionary = new Dictionary<ItemData, int>();
			list2.Add(startWeaponItemCode);
			GameDB.item.SetFavoriteSourceDictionary(list2, startSources, true);
			if (filterObjectLeafNodeWeapon.Toggle.isOn)
			{
				foreach (KeyValuePair<ItemData, int> keyValuePair2 in sourceItems)
				{
					foreach (KeyValuePair<ItemData, int> keyValuePair3 in startSources)
					{
						if (keyValuePair3.Key.code == keyValuePair2.Key.code)
						{
							if (dictionary.ContainsKey(keyValuePair2.Key))
							{
								dictionary[keyValuePair2.Key] = keyValuePair2.Value - 1;
							}
							else
							{
								dictionary.Add(keyValuePair2.Key, keyValuePair2.Value - 1);
							}
						}
					}
				}

				foreach (KeyValuePair<ItemData, int> keyValuePair4 in dictionary)
				{
					if (dictionary.ContainsKey(keyValuePair4.Key))
					{
						if (dictionary[keyValuePair4.Key] > 0)
						{
							sourceItems[keyValuePair4.Key] = dictionary[keyValuePair4.Key];
						}
						else
						{
							sourceItems.Remove(keyValuePair4.Key);
						}
					}
				}
			}

			List<int> list3 = new List<int>();
			List<ItemData> list4 = new List<ItemData>();
			fixAreaItems.Clear();
			int num3 = 0;
			while (num3 < favorite.paths.Count && num3 < areas.Count)
			{
				int areaCode = favorite.paths[num3];
				areas[num3].gameObject.SetActive(true);
				areas[num3].SetEventHandler(this);
				areas[num3].SetAreaCode(areaCode);
				areas[num3].SetIndex(num3 + 1);
				IEnumerable<ItemData> enumerable = from x in Singleton<ItemService>.inst.GetDropItems(areaCode, true)
					select GameDB.item.FindItemByCode(x)
					into x
					where sourceItems.ContainsKey(x)
					select x;
				fixAreaItems[num3 + 1] = (from x in enumerable
					select x.code).ToList<int>();
				using (IEnumerator<ItemData> enumerator4 = enumerable.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						ItemData itemData = enumerator4.Current;
						if ((!filterObjectOverlap.Toggle.isOn || !list3.Contains(itemData.code)) &&
						    (!filterObjectCollection.Toggle.isOn || !list.Exists(x => x.code == itemData.code)))
						{
							if (filterObjectLeafNodeWeapon.Toggle.isOn)
							{
								bool flag = false;
								if (itemData.code == startWeaponItemCode)
								{
									foreach (int num4 in dictionary.Values)
									{
										if (num2 == num4)
										{
											flag = true;
											break;
										}
									}

									if (flag)
									{
										num2 = 0;
										continue;
									}

									num2++;
								}
							}

							list3.Add(itemData.code);
							areas[num3].AddItemData(itemData, sourceItems[itemData]);
							list4.Add(itemData);
						}
					}
				}

				num3++;
			}

			if (num3 < areas.Count - 1)
			{
				areas[num3].gameObject.SetActive(true);
				areas[num3].Clear();
				areas[num3].SetIndex(num3 + 1);
			}

			for (int i = 0; i < list4.Count; i++)
			{
				if (sourceItems.ContainsKey(list4[i]))
				{
					sourceItems[list4[i]] = 0;
				}
			}

			list4.Clear();
			searchTarget.Clear();
			gatherTarget.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair5 in sourceItems)
			{
				ItemData key2 = keyValuePair5.Key;
				int value = keyValuePair5.Value;
				if (!list.Contains(key2) && value > 0)
				{
					ItemDataSlot itemDataSlot = searchTarget.CreateSlot(key2);
					itemDataSlot.SetItemData(key2);
					itemDataSlot.SetSlotType(SlotType.None);
					itemDataSlot.SetSprite(key2.GetSprite());
					itemDataSlot.SetBackground(key2.GetGradeSprite());
					itemDataSlot.SetStackText(value.ToString());
					if (!Singleton<ItemService>.inst.GetDropArea(key2.code).Any<int>())
					{
						itemDataSlot.EnableRandomMark(true);
					}

					itemDataSlot.SetEventListener(this);
				}
			}

			foreach (ItemData itemData2 in list)
			{
				ItemDataSlot itemDataSlot2 = gatherTarget.CreateSlot(itemData2);
				itemDataSlot2.SetItemData(itemData2);
				itemDataSlot2.SetSlotType(SlotType.None);
				itemDataSlot2.SetSprite(itemData2.GetSprite());
				itemDataSlot2.SetBackground(itemData2.GetGradeSprite());
				itemDataSlot2.SetStackText(null);
				if (!Singleton<ItemService>.inst.GetDropArea(itemData2.code).Any<int>())
				{
					itemDataSlot2.EnableRandomMark(true);
				}

				itemDataSlot2.SetEventListener(this);
			}
		}


		private void RenderCompleteItem()
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			foreach (int num in favorite.weaponCodes)
			{
				List<ItemData> calItem = GetCalItem(num);
				if (calItem != null)
				{
					foreach (KeyValuePair<ItemData, int> keyValuePair in startSources)
					{
						if (calItem.Contains(keyValuePair.Key))
						{
							calItem.Remove(keyValuePair.Key);
						}
					}

					foreach (KeyValuePair<int, List<int>> keyValuePair2 in fixAreaItems)
					{
						if (calItem.Count == 0)
						{
							break;
						}

						using (List<int>.Enumerator enumerator4 = keyValuePair2.Value.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								int areaItemCode = enumerator4.Current;
								List<ItemData> list = calItem.FindAll(x => x.code == areaItemCode);
								if (list == null)
								{
									break;
								}

								foreach (ItemData item in list)
								{
									calItem.Remove(item);
									if (calItem.Count == 0)
									{
										if (dictionary.ContainsKey(keyValuePair2.Key))
										{
											dictionary[keyValuePair2.Key].Add(num);
											break;
										}

										dictionary[keyValuePair2.Key] = new List<int>
										{
											num
										};
										break;
									}
								}
							}
						}
					}
				}
			}

			using (Dictionary<int, List<int>>.Enumerator enumerator3 = dictionary.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					KeyValuePair<int, List<int>> kvp = enumerator3.Current;
					areas.Find(x => x.GetSlotIndex() == kvp.Key).ShowCompleteItems(kvp.Value);
				}
			}
		}


		private List<ItemData> GetCalItem(int itemCode)
		{
			List<int> collectAndHuntItems = GameDB.item.GetCollectAndHuntItems();
			List<int> list = new List<int>
			{
				401209,
				401304,
				401401,
				401403
			};
			List<ItemData> list2 = GameDB.item.AnalyzeItem(GameDB.item.FindItemByCode(itemCode));
			for (int i = 0; i < list2.Count; i++)
			{
				if (list.Contains(list2[i].code))
				{
					return null;
				}

				if (!list2[i].IsLeafNodeItem())
				{
					list2.Remove(list2[i]);
					i--;
				}
				else if (collectAndHuntItems.Contains(list2[i].code))
				{
					list2.Remove(list2[i]);
					i--;
				}
			}

			return list2;
		}


		private void SetActiveGuideText()
		{
			guideText.SetActive(favorite.paths.Count == 0);
		}


		private void RequestAddArea(int areaCode)
		{
			if (!favorite.paths.Contains(areaCode))
			{
				favorite.paths.Add(areaCode);
				SetActiveGuideText();
				OnEdited(favorite);
				OnChangeState(true, favorite.share);
			}
		}


		private void RequestDeleteArea(int areaCode)
		{
			if (favorite.paths.Contains(areaCode))
			{
				favorite.paths.Remove(areaCode);
				SetActiveGuideText();
				OnEdited(favorite);
				OnChangeState(true, favorite.share);
			}
		}


		private void RequestClearArea()
		{
			favorite.paths.Clear();
			SetActiveGuideText();
			OnEdited(favorite);
		}


		public void DrawRoutePathNumber(Favorite favorite)
		{
			uiMap.SetRouteText(favorite.paths);
		}


		public void HighLightTargetItem(ItemData itemData)
		{
			if (itemData == null)
			{
				if (selectItemData == null)
				{
					int num = 0;
					while (num < areas.Count && areas[num].gameObject.activeSelf)
					{
						areas[num].UnEnableSelection();
						num++;
					}

					foreach (ItemData t in searchTarget.GetKeys())
					{
						ItemDataSlot itemDataSlot = searchTarget.FindSlot(t);
						if (itemDataSlot != null)
						{
							itemDataSlot.EnableSelection(false);
						}
					}

					foreach (ItemData t2 in gatherTarget.GetKeys())
					{
						ItemDataSlot itemDataSlot2 = gatherTarget.FindSlot(t2);
						if (itemDataSlot2 != null)
						{
							itemDataSlot2.EnableSelection(false);
						}
					}

					return;
				}

				itemData = selectItemData;
			}

			selectItemData = itemData;
			int code = itemData.code;
			List<ItemData> list = null;
			if (code > 0)
			{
				ItemData itemData2 = GameDB.item.FindItemByCode(code);
				list = (from x in GameDB.item.AnalyzeItem(itemData2)
					where x.IsLeafNodeItem()
					select x).ToList<ItemData>();
			}

			if (filterObjectLeafNodeWeapon.Toggle.isOn)
			{
				foreach (KeyValuePair<ItemData, int> keyValuePair in startSources)
				{
					if (list.Contains(keyValuePair.Key))
					{
						list.Remove(keyValuePair.Key);
					}
				}
			}

			int num2 = 0;
			while (num2 < areas.Count && areas[num2].gameObject.activeSelf)
			{
				areas[num2].HighLight(list);
				num2++;
			}

			foreach (ItemData itemData3 in searchTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot3 = searchTarget.FindSlot(itemData3);
				if (itemDataSlot3 != null)
				{
					itemDataSlot3.EnableSelection(list != null && list.Contains(itemData3));
				}
			}

			foreach (ItemData itemData4 in gatherTarget.GetKeys())
			{
				ItemDataSlot itemDataSlot4 = gatherTarget.FindSlot(itemData4);
				if (itemDataSlot4 != null)
				{
					itemDataSlot4.EnableSelection(list != null && list.Contains(itemData4));
				}
			}
		}


		private void ShowMaterialMap(ItemData itemData)
		{
			uiMap.SetSearchItem(itemData);
		}


		public void ShowClearAreaPopup()
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("설정된 이동 경로를 모두 삭제하시겠습니까?"), new Popup.Button
			{
				text = Ln.Get("삭제"),
				type = Popup.ButtonType.Confirm,
				callback = RequestClearArea
			}, new Popup.Button
			{
				text = Ln.Get("취소"),
				type = Popup.ButtonType.Cancel
			});
		}


		public void LocalizeUIMapAreaName()
		{
			uiMap.UpdateAreaName();
		}


		public void ClickedDetail()
		{
			detailMenu.SetActive(!detailMenu.activeSelf);
			if (!detailMenu.activeSelf)
			{
				imgDetail.sprite = inActiveSprite;
				return;
			}

			imgDetail.sprite = activeSprite;
		}


		public void SetAnchoredPosition(Vector2 position)
		{
			transform.GetComponent<RectTransform>().anchoredPosition = position;
		}


		private class FilterObject
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			public FilterObject(GameObject gameObject)
			{
				this.gameObject = gameObject;
				Toggle = GameUtil.Bind<Toggle>(this.gameObject, "Toggle");
				Toggle.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn); });
				GameUtil.BindOrAdd<EventTrigger>(gameObject, ref eventTrigger);
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
			}


			public Toggle Toggle { get; }


			
			
			public event Action<bool> OnToggleChange = delegate { };


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(GetLnKey());
				Vector2 vector = gameObject.transform.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(0f, 45f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}


			private string GetLnKey()
			{
				string name = gameObject.name;
				if (name == "Overlap")
				{
					return Ln.Get("앞선 경로에 포함된 재료를 다음 경로에는 표시하지 않습니다.");
				}

				if (name == "Collection")
				{
					return Ln.Get("채집/사냥 재료를 지역별 획득 재료에 표시하지 않음");
				}

				if (!(name == "LeafNodeWeapon"))
				{
					return "";
				}

				return Ln.Get("기본 무기로 제공되는 아이템 1개는 재료에서 제외");
			}
		}
	}
}