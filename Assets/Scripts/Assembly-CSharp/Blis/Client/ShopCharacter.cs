using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShopCharacter : BasePage, ILnEventHander
	{
		public delegate void NoMoneyOpenShopCallback();


		[SerializeField] private ShopMasteryButton masteryPrefab = default;


		[SerializeField] private ProductCharacterSlot productCharacterSlotPrefab = default;


		private readonly List<WeaponType> selectedWeaponTypes = new List<WeaponType>();


		private RectTransform blank = default;


		private bool checkBoxHaveCharacter;


		private RectTransform content = default;


		private string editString = string.Empty;


		private Toggle holdCheckToggle = default;


		private InputFieldExtension inputField = default;


		private float lastRequestTime = float.MinValue;


		private List<ShopMasteryButton> masteryButtons;


		private RectTransform masterys = default;


		public NoMoneyOpenShopCallback noMoneyOpenShopCallback;


		private Button searchDeleteBtn = default;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			NoticeService.onReceivedCharacter -= OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter -= OnReceivedCharacter;
		}


		public void OnLnDataChange()
		{
			ProductCharacterSlot[] componentsInChildren = content.GetComponentsInChildren<ProductCharacterSlot>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetCharacterName();
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			inputField = GameUtil.Bind<InputFieldExtension>(gameObject, "Search/EditedTitle");
			holdCheckToggle = GameUtil.Bind<Toggle>(gameObject, "HaveProductCheck/CheckBox");
			masterys = GameUtil.Bind<RectTransform>(gameObject, "Masterys");
			content = GameUtil.Bind<RectTransform>(gameObject, "ProductScrollView/Viewport/Content");
			blank = GameUtil.Bind<RectTransform>(gameObject, "ProductScrollView/Viewport/Blank");
			searchDeleteBtn = GameUtil.Bind<Button>(gameObject, "Search/BtnDelete");
			NoticeService.onReceivedCharacter += OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter += OnReceivedCharacter;
			inputField.onValueChanged.AddListener(OnInputValueChange);
			holdCheckToggle.onValueChanged.AddListener(ClickedHaveCharacterCheckBox);
			searchDeleteBtn.onClick.AddListener(ClearInputField);
		}


		private void OnReceivedCharacter(int characterCode)
		{
			Filtering();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			SetMasteryButtons();
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			if (Time.time - lastRequestTime < ShopProductService.RefreshTime)
			{
				Filtering();
				return;
			}

			lastRequestTime = Time.time;
			ShopProductService.RequestShopCharacterList(Filtering);
		}


		private void OnInputValueChange(string input)
		{
			if (editString != input)
			{
				editString = input;
				Filtering();
			}
		}


		private void SetView(IOrderedEnumerable<ShopProduct> characterShopDataList)
		{
			if (characterShopDataList == null || !characterShopDataList.Any<ShopProduct>())
			{
				content.gameObject.SetActive(false);
				SetBlank(true);
				return;
			}

			content.gameObject.SetActive(true);
			SetBlank(false);
			int childCount = content.childCount;
			int num = Mathf.Max(0, characterShopDataList.Count<ShopProduct>() - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<ProductCharacterSlot>(productCharacterSlotPrefab, content);
			}

			ProductCharacterSlot[] componentsInChildren = content.GetComponentsInChildren<ProductCharacterSlot>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (j < characterShopDataList.Count<ShopProduct>())
				{
					ShopProduct data = characterShopDataList.ElementAt(j);
					componentsInChildren[j].gameObject.SetActive(true);
					componentsInChildren[j].SetSlot(data, 0);
					componentsInChildren[j].buySuccessCallback = delegate { Filtering(); };
					componentsInChildren[j].noMoneyOpenShopCallback = delegate { noMoneyOpenShopCallback(); };
				}
				else
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}
		}


		private void SetBlank(bool enable)
		{
			blank.gameObject.SetActive(enable);
		}


		private void SetMasteryButtons()
		{
			List<WeaponTypeInfoData> list = GameDB.mastery.GetAllWeaponTypeInfoDatas().FindAll(x => x.shopFilter == 1);
			masteryButtons = new List<ShopMasteryButton>();
			for (int i = 0; i < list.Count + 1; i++)
			{
				ShopMasteryButton shopMasteryButton = Instantiate<ShopMasteryButton>(masteryPrefab, masterys);
				WeaponType weaponType = i == 0 ? WeaponType.None : list[i - 1].type;
				shopMasteryButton.SetMastery(weaponType);
				shopMasteryButton.OnPointerClickEvent += delegate(BaseControl control, PointerEventData ed)
				{
					ChangeMasteryFilter(weaponType, (ShopMasteryButton) control);
				};
				masteryButtons.Add(shopMasteryButton);
			}

			ChangeMasteryFilter(WeaponType.None, null);
		}


		private void ChangeMasteryFilter(WeaponType weaponType, ShopMasteryButton selectedButton)
		{
			if (weaponType != WeaponType.None)
			{
				if (selectedWeaponTypes.Contains(WeaponType.None))
				{
					selectedWeaponTypes.Clear();
					masteryButtons.ForEach(delegate(ShopMasteryButton x) { x.SetSelected(false); });
				}

				if (!selectedButton.IsActiveSelected() && !selectedWeaponTypes.Contains(weaponType))
				{
					selectedWeaponTypes.Add(weaponType);
					selectedButton.SetSelected(true);
				}
				else if (selectedButton.IsActiveSelected() && selectedWeaponTypes.Contains(weaponType))
				{
					selectedWeaponTypes.Remove(weaponType);
					selectedButton.SetSelected(false);
				}
			}

			if (weaponType == WeaponType.None || selectedWeaponTypes.Count == 0)
			{
				selectedWeaponTypes.Clear();
				selectedWeaponTypes.Add(WeaponType.None);
				masteryButtons.ForEach(delegate(ShopMasteryButton x)
				{
					x.SetSelected(x.WeaponType == WeaponType.None);
				});
			}

			Filtering();
		}


		private void Filtering()
		{
			string searchString = LnUtil.GetSearchName(editString);
			List<ShopProduct> list = ShopProductService.GetShopCharacters();
			List<MasteryType> masteryTypes = new List<MasteryType>();
			foreach (WeaponType weaponType in selectedWeaponTypes)
			{
				masteryTypes.Add(weaponType.GetWeaponMasteryType());
			}

			if (!checkBoxHaveCharacter)
			{
				list = list.FindAll(c => !Lobby.inst.IsHaveCharacter(c.code));
			}

			if (searchString != string.Empty)
			{
				list = list.FindAll(p => LnUtil.GetSearchName(LnUtil.GetCharacterName(p.code)).Contains(searchString));
			}

			if (!masteryTypes.Contains(MasteryType.None))
			{
				list = list.FindAll(delegate(ShopProduct p)
				{
					foreach (MasteryType item in GameDB.mastery.GetCharacterMasteryData(p.code).GetMasteries())
					{
						if (masteryTypes.Contains(item))
						{
							return true;
						}
					}

					return false;
				});
			}

			SetView(from p in list
				orderby Lobby.inst.IsHaveCharacter(p.code) descending, p.price, p.code
				select p);
		}


		private void ClearInputField()
		{
			inputField.text = string.Empty;
			inputField.textComponent.text = string.Empty;
			Filtering();
		}


		private void ClickedHaveCharacterCheckBox(bool holdFlag)
		{
			checkBoxHaveCharacter = holdFlag;
			Filtering();
		}
	}
}