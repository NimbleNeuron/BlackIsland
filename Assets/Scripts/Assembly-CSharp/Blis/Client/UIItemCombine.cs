using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemCombine : BaseUI
	{
		public delegate void OnClickItemEvent(ItemData itemData);


		[SerializeField] private UIItemHeader uiItemHeader = default;


		[SerializeField] private UIItemDrop uiItemDrop = default;


		[SerializeField] private UITextGrid uiItemOptionGrid = default;


		[SerializeField] private Text uiItemStatus = default;


		[SerializeField] private Button combineButton = default;


		[SerializeField] private Button navButton = default;


		[SerializeField] private Button adminButton = default;


		[SerializeField] private ItemData currentItemData = default;


		private List<Item> equipmentItems = new List<Item>();


		private List<Item> inventoryItems = new List<Item>();

		
		
		public event OnClickItemEvent OnClickCombineItem = delegate { };


		
		
		public event OnClickItemEvent OnClickNavigation = delegate { };


		
		
		public event OnClickItemEvent OnClickAdmin = delegate { };


		protected override void OnStartUI()
		{
			base.OnStartUI();
			if (combineButton != null)
			{
				combineButton.onClick.AddListener(OnClickCombineButton);
			}

			if (navButton != null)
			{
				navButton.onClick.AddListener(OnClickNavButton);
			}

			if (adminButton != null)
			{
				adminButton.onClick.AddListener(OnClickAdminButton);
			}

			UpdateButtonState();
		}


		public void EmptyUI()
		{
			uiItemHeader.EmptyUI();
			uiItemDrop.EmptyUI();
			uiItemOptionGrid.Clear();
			uiItemStatus.text = "";
		}


		private void OnClickCombineButton()
		{
			if (currentItemData != null)
			{
				OnClickCombineItem(currentItemData);
			}
		}


		private void OnClickNavButton()
		{
			if (currentItemData != null)
			{
				OnClickNavigation(currentItemData);
			}
		}


		private void OnClickAdminButton()
		{
			if (currentItemData != null)
			{
				OnClickAdmin(currentItemData);
			}
		}


		public void SetTargetItem(ItemData itemData)
		{
			currentItemData = itemData;
			UpdateButtonState();
			uiItemHeader.UpdateUI(itemData);
			uiItemDrop.UpdateUI(itemData);
			uiItemOptionGrid.Clear();
			List<ItemOptionUtil.ItemOptionText> itemOptionTexts = ItemOptionUtil.GetItemOptionTexts(itemData);
			for (int i = 0; i < itemOptionTexts.Count; i++)
			{
				uiItemOptionGrid.PushText(itemOptionTexts[i].text);
				uiItemOptionGrid.GetText(i).color = itemOptionTexts[i].color;
			}

			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(Ln.Get("최대 수량"));
			stringBuilder.Append(" : ");
			stringBuilder.AppendLine(itemData.stackable.ToString());
			ItemType itemType = itemData.itemType;
			if (itemType - ItemType.Weapon > 1)
			{
				if (itemType == ItemType.Consume)
				{
					stringBuilder.AppendLine(Ln.Get("좌클릭으로 사용 가능"));
				}
			}
			else
			{
				stringBuilder.AppendLine(Ln.Get("좌클릭으로 착용 가능"));
			}

			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				List<int> dropArea = Singleton<ItemService>.inst.GetDropArea(itemData.code);
				int count = dropArea.Count;
				if (count > 0)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(Ln.Get("지역별 드랍 개수"));
					stringBuilder.Append(" : ");
					for (int j = 0; j < count; j++)
					{
						int dropCountInBox = Singleton<ItemService>.inst.GetDropCountInBox(dropArea[j], itemData.code);
						stringBuilder.Append(Ln.Get(LnType.Area_Name, dropArea[j].ToString()));
						stringBuilder.Append("(");
						stringBuilder.Append(dropCountInBox.ToString());
						if (j + 1 == count)
						{
							stringBuilder.Append(")");
						}
						else
						{
							stringBuilder.Append("), ");
						}
					}
				}

				stringBuilder.AppendLine();
			}

			uiItemStatus.text = stringBuilder.ToString();
		}


		private void UpdateButtonState()
		{
			bool interactable = false;
			bool interactable2 = false;
			if (currentItemData != null && SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene &&
			    MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			    MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsAlive)
			{
				interactable = IsHave(currentItemData.makeMaterial1) && IsHave(currentItemData.makeMaterial2);
				interactable2 = !IsHave(currentItemData.code) &&
				                MonoBehaviourInstance<GameUI>.inst.NavigationHud.TargetItemList.Count < 6;
			}

			if (combineButton != null)
			{
				combineButton.interactable = interactable;
			}

			if (navButton != null)
			{
				navButton.interactable = interactable2;
			}
		}


		private bool IsHave(int itemCode)
		{
			return inventoryItems.Exists(x => x.ItemData.code == itemCode) ||
			       equipmentItems.Exists(x => x.ItemData.code == itemCode);
		}


		public void OnUpdateInventory(List<Item> items)
		{
			inventoryItems = items;
			UpdateButtonState();
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			equipmentItems = items;
			UpdateButtonState();
		}


		public void OnUpdateButtonState()
		{
			UpdateButtonState();
		}
	}
}