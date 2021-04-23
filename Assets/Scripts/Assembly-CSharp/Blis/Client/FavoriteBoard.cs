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
	public class FavoriteBoard : BaseUI, ISlotEventListener
	{
		[SerializeField] private ItemDataSlot idsPrefab = default;


		[SerializeField] private FavoriteBoardArea areaPrefab = default;


		[SerializeField] private GameObject blank = default;


		[SerializeField] private FavoriteBoardCreate create = default;


		[SerializeField] private GameObject board = default;


		[SerializeField] private GameObject extend = default;


		[SerializeField] private Text txtBlank = default;


		[SerializeField] private Image imgMastery = default;


		[SerializeField] private Text txtFavoriteName = default;


		[SerializeField] private Text txtCreatorName = default;


		[SerializeField] private Text txtVersion = default;


		[SerializeField] private GameObject btnCreate = default;


		[SerializeField] private GameObject items = default;


		[SerializeField] private Sprite[] detailSprites = default;


		[SerializeField] private Sprite[] editSprites = default;


		[SerializeField] private InfoMaker infoMakerEdit = default;


		[SerializeField] private Button btnDetail = default;


		[SerializeField] private Button btnEdit = default;


		[SerializeField] private Button btnDelete = default;


		[SerializeField] private Button btnShare = default;


		[SerializeField] private Button btnID = default;


		[SerializeField] private BaseControl slotMove = default;


		[SerializeField] private GameObject slotMoveSelect = default;


		[SerializeField] private Transform rawItems = default;


		[SerializeField] private Transform pathItems = default;


		[SerializeField] private GameObject obj_PathEmpty = default;


		[SerializeField] private Image[] emptys = default;


		[SerializeField] private Image[] filterOns = default;


		private readonly List<FavoriteBoardArea> areaSlots = new List<FavoriteBoardArea>();


		private readonly Dictionary<ItemData, int> favoriteSources = new Dictionary<ItemData, int>();


		private readonly List<ItemDataSlot> itemDataSlots = new List<ItemDataSlot>();


		private readonly List<ItemDataSlot> rawItemslots = new List<ItemDataSlot>();


		private Image bg;


		private Image btnDetailImage;


		private Image btnEditImage;


		private Favorite favorite;


		private bool isCheckMode;


		private int slotId;


		public int FavoriteOrder {
			get
			{
				if (favorite == null)
				{
					return 0;
				}

				return favorite.order;
			}
		}


		public int FavoriteSlotId {
			get
			{
				if (favorite == null)
				{
					return slotId;
				}

				return favorite.slotId;
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
			if (!extend.activeSelf)
			{
				return;
			}

			ItemData itemData = (slot as ItemDataSlot).GetItemData();
			List<ItemData> list = null;
			if (itemData.code > 0)
			{
				ItemData itemData2 = GameDB.item.FindItemByCode(itemData.code);
				list = (from x in GameDB.item.AnalyzeItem(itemData2)
					where x.IsLeafNodeItem()
					select x).ToList<ItemData>();
			}

			foreach (ItemData itemData3 in list)
			{
				foreach (ItemDataSlot itemDataSlot in rawItemslots)
				{
					if (itemDataSlot.GetItemData().code == itemData3.code)
					{
						itemDataSlot.EnableSelection(true);
						break;
					}
				}
			}
		}


		public void OnPointerExit(Slot slot)
		{
			if (!extend.activeSelf)
			{
				return;
			}

			foreach (ItemDataSlot itemDataSlot in rawItemslots)
			{
				itemDataSlot.EnableSelection(false);
			}
		}

		
		
		public event Action<Favorite, int, bool> editAction = delegate { };


		
		
		public event Action<Favorite, int> bringAction = delegate { };


		
		
		public event Action<Favorite, FavoriteBoard> resetAction = delegate { };


		
		
		public event Action shareAction = delegate { };


		
		
		public event Action<int, int, string> loadAction = delegate { };


		
		
		public event Action<FavoriteBoard, PointerEventData> beginDragAction = delegate { };


		
		
		public event Action<FavoriteBoard, PointerEventData> endDragAction = delegate { };


		
		
		public event Action<FavoriteBoard, PointerEventData> dragAction = delegate { };


		
		
		public event Action<PointerEventData> scrollAction = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			bg = GameUtil.Bind<Image>(gameObject, ref bg);
			btnEditImage = btnEdit.GetComponent<Image>();
			btnDetailImage = btnDetail.GetComponent<Image>();
			create.editAction += Create;
			create.bringAction += Bring;
			slotMove.OnPointerDownEvent += delegate { slotMoveSelect.SetActive(true); };
			slotMove.OnPointerUpEvent += delegate { slotMoveSelect.SetActive(false); };
			slotMove.OnBeginDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				Action<FavoriteBoard, PointerEventData> action = beginDragAction;
				if (action == null)
				{
					return;
				}

				action(this, eventData);
			};
			slotMove.OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				Action<FavoriteBoard, PointerEventData> action = endDragAction;
				if (action == null)
				{
					return;
				}

				action(this, eventData);
			};
			slotMove.OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				Action<FavoriteBoard, PointerEventData> action = dragAction;
				if (action == null)
				{
					return;
				}

				action(this, eventData);
			};
			slotMove.OnScrollEvent += delegate(BaseControl control, PointerEventData eventData)
			{
				Action<PointerEventData> action = scrollAction;
				if (action == null)
				{
					return;
				}

				action(eventData);
			};
		}


		public void SetFavorite(Favorite favorite)
		{
			Image[] array = emptys;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}

			extend.SetActive(true);
			this.favorite = favorite;
			SetButtons();
			SetSlotMove();
			SetMasteryIcon();
			SetFavoriteName();
			SetCreatorName();
			SetVersion();
			SetItems();
			SetRawItems();
			SetPathItems();
			SetFilterType();
			SetDefaultBoardUI();
		}


		public void SetDefaultBoardUI()
		{
			EnableBackground(true);
			blank.SetActive(false);
			create.gameObject.SetActive(false);
			board.SetActive(true);
			extend.SetActive(false);
		}


		private void EnableBackground(bool enable)
		{
			if (bg != null)
			{
				bg.enabled = enable;
			}
		}


		public void SetCreate(int characterCode, int slotId)
		{
			favorite = null;
			this.slotId = slotId;
			EnableBackground(true);
			blank.SetActive(false);
			board.SetActive(false);
			extend.SetActive(false);
			create.gameObject.SetActive(true);
			create.SetMasterys(characterCode);
			create.SetSlotId(slotId);
		}


		public void SetBlank(string text)
		{
			favorite = null;
			txtBlank.text = text;
			EnableBackground(false);
			blank.SetActive(true);
			create.gameObject.SetActive(false);
			board.SetActive(false);
			extend.SetActive(false);
		}


		public void SetButtons(bool isActive = true)
		{
			if (isActive)
			{
				if (favorite != null)
				{
					SetCustomButtonStyle();
				}
			}
			else
			{
				btnDetailImage.color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnEdit.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnDelete.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnShare.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnID.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnDetail.enabled = false;
				btnEdit.enabled = false;
				btnDelete.enabled = false;
				btnShare.enabled = false;
				btnID.enabled = false;
			}
		}


		public void SetSlotMove(bool isActive = false)
		{
			slotMove.gameObject.SetActive(isActive);
		}


		private void SetMasteryIcon()
		{
			imgMastery.sprite = favorite.weaponType.GetWeaponMasteryType().GetIcon();
		}


		public void SetFavoriteName()
		{
			string text = string.IsNullOrEmpty(favorite.title)
				? Ln.Format("{0} 추천 루트", Ln.Get(string.Format("WeaponType/{0}", favorite.weaponType)))
				: favorite.title;
			txtFavoriteName.text = text;
		}


		private void SetCreatorName()
		{
			txtCreatorName.text = favorite.init ? "NimbleNeuron" : favorite.recommendUserNickname;
		}


		private void SetVersion()
		{
			if (string.IsNullOrEmpty(favorite.version))
			{
				txtVersion.text = "";
				return;
			}

			txtVersion.text = "ver. " + favorite.version;
		}


		public void SetCustomButtonStyle()
		{
			infoMakerEdit.desc = "길잡이 편집";
			btnEditImage.sprite = editSprites[0];
			btnDetailImage.sprite = detailSprites[0];
			bool flag = favorite.weaponCodes != null && favorite.weaponCodes.Count > 0;
			if (flag)
			{
				btnDetailImage.color = new Color(0.705f, 0.705f, 0.705f, 1f);
				btnEdit.GetComponent<Image>().color = new Color(0.705f, 0.705f, 0.705f, 1f);
				btnDelete.GetComponent<Image>().color = new Color(0.705f, 0.705f, 0.705f, 1f);
			}
			else
			{
				btnDetailImage.color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnEdit.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnDelete.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
			}

			btnDetail.enabled = flag;
			btnEdit.enabled = flag;
			btnDelete.enabled = flag;
			btnID.gameObject.SetActive(true);
			if (favorite.share)
			{
				btnShare.GetComponent<Image>().color = new Color(0.396f, 0.91f, 0.906f, 1f);
				btnShare.enabled = true;
				btnID.GetComponent<Image>().color = new Color(0.705f, 0.705f, 0.705f, 1f);
				btnID.enabled = true;
			}
			else
			{
				if (string.IsNullOrEmpty(favorite.recommendUserNickname) && !favorite.init)
				{
					btnShare.GetComponent<Image>().color = new Color(0.705f, 0.705f, 0.705f, 1f);
					btnShare.enabled = true;
				}
				else
				{
					btnShare.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
					btnShare.enabled = false;
				}

				btnID.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
				btnID.enabled = false;
			}

			isCheckMode = false;
		}


		public void SetRecommendButtonStyle()
		{
			infoMakerEdit.desc = "길잡이 확인하기";
			btnEditImage.sprite = editSprites[1];
			btnDetailImage.sprite = detailSprites[0];
			btnDetailImage.color = new Color(0.705f, 0.705f, 0.705f, 1f);
			btnEdit.GetComponent<Image>().color = new Color(0.705f, 0.705f, 0.705f, 1f);
			btnDelete.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
			btnShare.GetComponent<Image>().color = new Color(0.219f, 0.219f, 0.219f, 1f);
			btnDetail.enabled = true;
			btnEdit.enabled = true;
			btnDelete.enabled = false;
			btnShare.enabled = false;
			btnID.gameObject.SetActive(false);
			isCheckMode = true;
		}


		private void SetItems()
		{
			itemDataSlots.Clear();
			if (favorite.weaponCodes != null && favorite.weaponCodes.Count > 0)
			{
				btnCreate.SetActive(false);
				items.SetActive(true);
				FillItems();
				return;
			}

			btnCreate.SetActive(true);
			items.SetActive(false);
		}


		private void SetRawItems()
		{
			int startWeapon = GameDB.recommend
				.FindStartingData(favorite.characterCode, favorite.weaponType.GetWeaponMasteryType()).startWeapon;
			List<int> list = new List<int>();
			Dictionary<ItemData, int> dictionary = new Dictionary<ItemData, int>();
			Dictionary<ItemData, int> dictionary2 = new Dictionary<ItemData, int>();
			list.Add(startWeapon);
			GameDB.item.SetFavoriteSourceDictionary(list, dictionary2, true);
			GameDB.item.SetFavoriteSourceDictionary(favorite.weaponCodes, favoriteSources, true);
			foreach (KeyValuePair<ItemData, int> keyValuePair in favoriteSources)
			{
				foreach (KeyValuePair<ItemData, int> keyValuePair2 in dictionary2)
				{
					if (keyValuePair2.Key.code == keyValuePair.Key.code)
					{
						if (dictionary.ContainsKey(keyValuePair.Key))
						{
							dictionary[keyValuePair.Key] = keyValuePair.Value - 1;
						}
						else
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value - 1);
						}
					}
				}
			}

			foreach (KeyValuePair<ItemData, int> keyValuePair3 in dictionary)
			{
				if (dictionary.ContainsKey(keyValuePair3.Key))
				{
					if (dictionary[keyValuePair3.Key] > 0)
					{
						favoriteSources[keyValuePair3.Key] = dictionary[keyValuePair3.Key];
					}
					else
					{
						favoriteSources.Remove(keyValuePair3.Key);
					}
				}
			}

			rawItems.GetComponentsInChildren<ItemDataSlot>(true, rawItemslots);
			int num = Mathf.Max(0, favoriteSources.Count - rawItemslots.Count);
			for (int i = 0; i < num; i++)
			{
				rawItemslots.Add(Instantiate<ItemDataSlot>(idsPrefab, rawItems));
			}

			foreach (ItemDataSlot itemDataSlot in rawItemslots)
			{
				itemDataSlot.gameObject.SetActive(false);
			}

			int num2 = 0;
			foreach (KeyValuePair<ItemData, int> keyValuePair4 in favoriteSources)
			{
				ItemData key = keyValuePair4.Key;
				rawItemslots[num2].gameObject.SetActive(keyValuePair4.Value > 0);
				rawItemslots[num2].SetItemData(key);
				rawItemslots[num2].SetSlotType(SlotType.None);
				rawItemslots[num2].SetSprite(key.GetSprite());
				rawItemslots[num2].SetBackground(key.GetGradeSprite());
				rawItemslots[num2].SetStackText(keyValuePair4.Value.ToString());
				num2++;
			}
		}


		private void SetPathItems()
		{
			pathItems.GetComponentsInChildren<FavoriteBoardArea>(true, areaSlots);
			if (favorite.paths.Count == 0)
			{
				obj_PathEmpty.SetActive(true);
				foreach (FavoriteBoardArea favoriteBoardArea in areaSlots)
				{
					favoriteBoardArea.SetHideAreaSlot();
				}

				return;
			}

			obj_PathEmpty.SetActive(false);
			int num = Mathf.Max(0, favorite.paths.Count - areaSlots.Count);
			for (int i = 0; i < num; i++)
			{
				areaSlots.Add(Instantiate<FavoriteBoardArea>(areaPrefab, pathItems));
			}

			for (int j = 0; j < areaSlots.Count; j++)
			{
				if (j < favorite.paths.Count)
				{
					int areaCode = favorite.paths[j];
					areaSlots[j].SetAreaSlot(j + 1, areaCode);
					areaSlots[j].pointerEnter -= OnPointerEnterPath;
					areaSlots[j].pointerEnter += OnPointerEnterPath;
					areaSlots[j].pointerExit -= OnPointerExitPath;
					areaSlots[j].pointerExit += OnPointerExitPath;
				}
				else
				{
					areaSlots[j].SetHideAreaSlot();
				}
			}
		}


		private void OnPointerEnterPath(int areaCode)
		{
			foreach (ItemDataSlot itemDataSlot in rawItemslots)
			{
				itemDataSlot.EnableNeedMark(
					Singleton<ItemService>.inst.IsDropArea(areaCode, itemDataSlot.GetItemData().code));
			}
		}


		private void OnPointerExitPath()
		{
			foreach (ItemDataSlot itemDataSlot in rawItemslots)
			{
				itemDataSlot.EnableNeedMark(false);
			}
		}


		private void FillItems()
		{
			int count = favorite.weaponCodes.Count;
			for (int i = 0; i < items.transform.childCount; i++)
			{
				ItemDataSlot component = items.transform.GetChild(i).GetComponent<ItemDataSlot>();
				component.SetEventListener(this);
				if (i >= count)
				{
					component.ResetSlot();
					emptys[i].enabled = true;
				}
				else
				{
					emptys[i].enabled = false;
					ItemData itemData = GameDB.item.FindItemByCode(favorite.weaponCodes[i]);
					component.SetItemData(itemData);
					component.SetSlotType(SlotType.None);
					component.SetSprite(itemData.GetSprite());
					component.SetBackground(itemData.GetGradeSprite());
					itemDataSlots.Add(component);
				}
			}
		}


		private void SetFilterType()
		{
			Image[] array = filterOns;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}

			string text = favorite.routeFilterType.ToString();
			if (text.Equals("ALL"))
			{
				array = filterOns;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}

				return;
			}

			if (text.Contains("SOLO"))
			{
				filterOns[0].enabled = true;
			}

			if (text.Contains("DUO"))
			{
				filterOns[1].enabled = true;
			}

			if (text.Contains("SQUAD"))
			{
				filterOns[2].enabled = true;
			}
		}


		public void Bring(Favorite favorite)
		{
			bringAction(favorite, transform.GetSiblingIndex());
		}


		public void Create(Favorite favorite)
		{
			editAction(favorite, transform.GetSiblingIndex(), false);
		}


		public void ClickDetail()
		{
			if (extend.activeSelf)
			{
				btnDetailImage.sprite = detailSprites[0];
				extend.SetActive(false);
				return;
			}

			btnDetailImage.sprite = detailSprites[1];
			extend.SetActive(true);
		}


		public void ClickEdit()
		{
			if (editAction != null)
			{
				editAction(favorite, transform.GetSiblingIndex(), isCheckMode);
			}
		}


		public void ClickShare()
		{
			string key = favorite.share ? "루트 공유 해제 확인" : "루트 공유 확인";
			string key2 = favorite.share ? "확인" : "공유하기";
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get(key), new Popup.Button
			{
				text = Ln.Get(key2),
				type = Popup.ButtonType.Confirm,
				callback = delegate
				{
					if (!favorite.share)
					{
						RouteApi.Share(favorite, FinishedShare);
						return;
					}

					RouteApi.ShareCancel(favorite, FinishedShare);
				}
			}, new Popup.Button
			{
				text = Ln.Get("닫기"),
				type = Popup.ButtonType.Cancel
			});
		}


		private void FinishedShare()
		{
			shareAction();
		}


		public void ClickDelete()
		{
			string text = favorite.share
				? Ln.Get("공유 루트 삭제 확인")
				: Ln.Format("{0} 을 초기화 하시겠습니까?\n초기화된 즐겨찾기는 되돌릴 수 없습니다.", favorite.title);
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = text;
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				text = Ln.Get("확인"),
				type = Popup.ButtonType.Confirm,
				callback = delegate
				{
					if (resetAction != null)
					{
						resetAction(favorite, this);
					}
				}
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.text = Ln.Get("취소");
			button.type = Popup.ButtonType.Cancel;
			button.callback = delegate { };
			array[num] = button;
			inst.Message(msg, array);
		}


		public void ClickID()
		{
			string text = Ln.Format("루트 ID 가져오기", favorite.shareWeaponRouteId);
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = text;
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				text = Ln.Get("클립보드 복사"),
				type = Popup.ButtonType.Confirm,
				callback = delegate { GUIUtility.systemCopyBuffer = favorite.shareWeaponRouteId.ToString(); }
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.text = Ln.Get("취소");
			button.type = Popup.ButtonType.Cancel;
			button.callback = delegate { };
			array[num] = button;
			inst.Message(msg, array);
		}


		public void ClickLoadID()
		{
			MonoBehaviourInstance<Popup>.inst.Input(Ln.Get("루트 ID 불러오기"), Ln.Get("루트 ID를 입력"),
				Ln.Get("공유된 루트 ID를 직접입력하여 해당 루트를 가져올 수 있습니다."), delegate(string strInput)
				{
					Action<int, int, string> action = loadAction;
					if (action == null)
					{
						return;
					}

					action(slotId, transform.GetSiblingIndex(), strInput);
				}, new Popup.Button
				{
					text = Ln.Get("확인"),
					type = Popup.ButtonType.Confirm
				}, new Popup.Button
				{
					text = Ln.Get("닫기"),
					type = Popup.ButtonType.Cancel
				});
		}
	}
}