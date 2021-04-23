using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShopEtc : BasePage
	{
		public delegate void LackOfAssetCallback();


		[SerializeField] private ShopMasteryButton masteryPrefab = default;


		[SerializeField] private ProductInstantSlot productInstantSlotPrefab = default;


		[SerializeField] private ProductRouteSlot productRouteSlotPrefab = default;


		private readonly List<WeaponType> selectedWeaponTypes = new List<WeaponType>();


		private Transform accountContent;


		private ScrollRect accountScrollRect;


		private Toggle accountToggle;


		private bool checkBoxHaveRouteSlot = true;


		private string editString = string.Empty;


		private LnText emptyProduct;


		private Toggle holdCheckToggle;


		private InputFieldExtension inputField;


		public LackOfAssetCallback lackOfAssetCallback;


		private float lastRequestAccountTime = float.MinValue;


		private float lastRequestRouteTime = float.MinValue;


		private List<ShopMasteryButton> masteryButtons;


		private RectTransform masterys;


		private Transform routeContent;


		private ScrollRect routeScrollRect;


		private Toggle routeToggle;


		private Button searchDeleteBtn;


		private RectTransform searchProduct;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			accountScrollRect = GameUtil.Bind<ScrollRect>(gameObject, "AccountScrollView");
			routeScrollRect = GameUtil.Bind<ScrollRect>(gameObject, "RouteSlotScrollView");
			routeToggle = GameUtil.Bind<Toggle>(gameObject, "ShopEtcTab/Tab01");
			accountToggle = GameUtil.Bind<Toggle>(gameObject, "ShopEtcTab/Tab02");
			accountContent = GameUtil.Bind<Transform>(gameObject, "AccountScrollView/Viewport/AccountContent");
			routeContent = GameUtil.Bind<Transform>(gameObject, "RouteSlotScrollView/Viewport/RouteContent");
			emptyProduct = GameUtil.Bind<LnText>(gameObject, "Blank");
			searchProduct = GameUtil.Bind<RectTransform>(gameObject, "SearchProduct");
			masterys = GameUtil.Bind<RectTransform>(searchProduct.gameObject, "Masterys");
			inputField = GameUtil.Bind<InputFieldExtension>(searchProduct.gameObject, "Search/EditedTitle");
			holdCheckToggle = GameUtil.Bind<Toggle>(searchProduct.gameObject, "HaveProductCheck/CheckBox");
			searchDeleteBtn = GameUtil.Bind<Button>(gameObject, "SearchProduct/Search/BtnDelete");
			routeToggle.onValueChanged.AddListener(OnChangeRoute);
			accountToggle.onValueChanged.AddListener(OnChangeAccount);
			holdCheckToggle.onValueChanged.AddListener(ClickedHaveRouteSlot);
			inputField.onValueChanged.AddListener(OnInputValueChange);
			searchDeleteBtn.onClick.AddListener(ClearInputField);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			SetMasteryButtons();
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.buySuccessCallback = OnPurchaseSuccess;
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.buySuccessRouteSlotCallback = OnPurchaseSuccess;
			MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.noMoneyOpenShopCallback = OnLackOfAssetCallback;
			if (routeToggle.isOn)
			{
				OnChangeRoute(true);
				return;
			}

			OnChangeAccount(true);
		}


		private void OnInputValueChange(string input)
		{
			if (editString != input)
			{
				editString = input;
				Filtering();
			}
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
			List<WeaponRouteSlotShop> list = ShopProductService.GetWeaponRouteSlots();
			List<MasteryType> masteryTypes = new List<MasteryType>();
			foreach (WeaponType weaponType in selectedWeaponTypes)
			{
				masteryTypes.Add(weaponType.GetWeaponMasteryType());
			}

			if (searchString != string.Empty)
			{
				list = list.FindAll(p =>
					LnUtil.GetSearchName(LnUtil.GetCharacterName(p.characterCode)).Contains(searchString));
			}

			if (!masteryTypes.Contains(MasteryType.None))
			{
				list = list.FindAll(delegate(WeaponRouteSlotShop p)
				{
					foreach (MasteryType item in GameDB.mastery.GetCharacterMasteryData(p.characterCode).GetMasteries())
					{
						if (masteryTypes.Contains(item))
						{
							return true;
						}
					}

					return false;
				});
			}

			SetView((from p in list
				orderby p.characterCode descending
				select p).ToList<WeaponRouteSlotShop>());
		}


		private void ClickedHaveRouteSlot(bool holdFlag)
		{
			checkBoxHaveRouteSlot = holdFlag;
			Filtering();
		}


		private void ClearInputField()
		{
			inputField.text = string.Empty;
			inputField.textComponent.text = string.Empty;
			Filtering();
		}


		private void SetView(List<WeaponRouteSlotShop> characterShopDataList)
		{
			if (characterShopDataList == null || !characterShopDataList.Any<WeaponRouteSlotShop>())
			{
				routeContent.gameObject.SetActive(false);
				emptyProduct.gameObject.SetActive(true);
			}
			else
			{
				routeContent.gameObject.SetActive(true);
			}

			ProductRouteSlot[] componentsInChildren = routeContent.GetComponentsInChildren<ProductRouteSlot>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}

			if (characterShopDataList != null)
			{
				for (int j = 0; j < characterShopDataList.Count; j++)
				{
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						if (characterShopDataList[j].characterCode == componentsInChildren[k].CharacterCode)
						{
							if (checkBoxHaveRouteSlot)
							{
								componentsInChildren[k].gameObject
									.SetActive(componentsInChildren[k].IsAvailablePurchase);
							}
							else
							{
								componentsInChildren[k].gameObject.SetActive(true);
							}
						}
					}
				}
			}

			emptyProduct.gameObject.SetActive(false);
		}


		private void OnChangeRoute(bool isOn)
		{
			if (!isOn)
			{
				routeScrollRect.gameObject.SetActive(false);
				searchProduct.gameObject.SetActive(false);
				Filtering();
				return;
			}

			if (Time.time - lastRequestRouteTime < ShopProductService.RefreshTime)
			{
				routeScrollRect.gameObject.SetActive(true);
				searchProduct.gameObject.SetActive(true);
				Filtering();
				return;
			}

			lastRequestRouteTime = Time.time;
			ShopProductService.RequestShopWeaponRouteSlot(delegate
			{
				SetProductRouteSlot();
				routeScrollRect.gameObject.SetActive(true);
				searchProduct.gameObject.SetActive(true);
				Filtering();
			});
		}


		private void OnChangeAccount(bool isOn)
		{
			if (!isOn)
			{
				accountScrollRect.gameObject.SetActive(false);
				return;
			}

			if (Time.time - lastRequestAccountTime < ShopProductService.RefreshTime)
			{
				accountScrollRect.gameObject.SetActive(true);
				return;
			}

			lastRequestAccountTime = Time.time;
			ShopProductService.RequestShopNickNameChange(delegate(List<ShopProduct> shopProducts)
			{
				SetProductAccount(shopProducts);
				accountScrollRect.gameObject.SetActive(true);
			});
		}


		private void SetProductAccount(List<ShopProduct> productList)
		{
			if (productList == null || productList.Count == 0)
			{
				Log.E("Instant Product Data is null or empty.");
				emptyProduct.gameObject.SetActive(true);
				return;
			}

			emptyProduct.gameObject.SetActive(false);
			int childCount = accountContent.childCount;
			int num = Mathf.Max(0, productList.Count - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<ProductInstantSlot>(productInstantSlotPrefab, accountContent);
			}

			ProductInstantSlot[] componentsInChildren =
				accountContent.GetComponentsInChildren<ProductInstantSlot>(true);
			for (int j = 0; j < productList.Count; j++)
			{
				if (j < productList.Count)
				{
					ShopProduct slot = productList.ElementAt(j);
					componentsInChildren[j].gameObject.SetActive(true);
					componentsInChildren[j].Init();
					componentsInChildren[j].SetSlot(slot);
					componentsInChildren[j].OnBeginDragEvent +=
						delegate(BaseControl control, PointerEventData eventData)
						{
							accountScrollRect.OnBeginDrag(eventData);
						};
					componentsInChildren[j].OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
					{
						accountScrollRect.OnDrag(eventData);
					};
					componentsInChildren[j].OnEndDragEvent += delegate(BaseControl control, PointerEventData eventData)
					{
						accountScrollRect.OnEndDrag(eventData);
					};
				}
				else
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}
		}


		private void SetProductRouteSlot()
		{
			List<WeaponRouteSlotShop> weaponRouteSlots = ShopProductService.GetWeaponRouteSlots();
			if (weaponRouteSlots == null || weaponRouteSlots.Count == 0)
			{
				Log.E("Instant Product Data is null or empty.");
				emptyProduct.gameObject.SetActive(true);
				return;
			}

			emptyProduct.gameObject.SetActive(false);
			int childCount = routeContent.childCount;
			int num = Mathf.Max(0, weaponRouteSlots.Count - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<ProductRouteSlot>(productRouteSlotPrefab, routeContent);
			}

			ProductRouteSlot[] componentsInChildren = routeContent.GetComponentsInChildren<ProductRouteSlot>(true);
			for (int j = 0; j < weaponRouteSlots.Count; j++)
			{
				if (j < weaponRouteSlots.Count)
				{
					WeaponRouteSlotShop weaponRouteSlotShop = weaponRouteSlots.ElementAt(j);
					if (weaponRouteSlotShop == null)
					{
						Log.E("[ShopEtc] SetProductRouteSlot() productData is null");
						componentsInChildren[j].gameObject.SetActive(false);
					}
					else
					{
						componentsInChildren[j].gameObject.SetActive(true);
						componentsInChildren[j].Init();
						componentsInChildren[j].SetSlot(weaponRouteSlotShop);
						componentsInChildren[j].OnBeginDragEvent +=
							delegate(BaseControl control, PointerEventData eventData)
							{
								routeScrollRect.OnBeginDrag(eventData);
							};
						componentsInChildren[j].OnDragEvent += delegate(BaseControl control, PointerEventData eventData)
						{
							routeScrollRect.OnDrag(eventData);
						};
						componentsInChildren[j].OnEndDragEvent +=
							delegate(BaseControl control, PointerEventData eventData)
							{
								routeScrollRect.OnEndDrag(eventData);
							};
						componentsInChildren[j].OnScrollEvent +=
							delegate(BaseControl control, PointerEventData eventData)
							{
								routeScrollRect.OnScroll(eventData);
							};
					}
				}
				else
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}
		}


		private void OnPurchaseSuccess(object data)
		{
			if (data == null)
			{
				ProductInstantSlot[] componentsInChildren =
					accountContent.GetComponentsInChildren<ProductInstantSlot>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].RefreshInfo();
				}

				return;
			}

			WeaponRouteSlotShop weaponRouteSlotShop;
			if ((weaponRouteSlotShop = data as WeaponRouteSlotShop) != null)
			{
				foreach (ProductRouteSlot productRouteSlot in
					routeContent.GetComponentsInChildren<ProductRouteSlot>(true))
				{
					if (weaponRouteSlotShop.characterCode == productRouteSlot.CharacterCode)
					{
						weaponRouteSlotShop.availablePurchaseCount--;
						productRouteSlot.SetSlot(weaponRouteSlotShop);
						return;
					}
				}
			}
		}


		private void OnLackOfAssetCallback()
		{
			LackOfAssetCallback lackOfAssetCallback = this.lackOfAssetCallback;
			if (lackOfAssetCallback == null)
			{
				return;
			}

			lackOfAssetCallback();
		}
	}
}