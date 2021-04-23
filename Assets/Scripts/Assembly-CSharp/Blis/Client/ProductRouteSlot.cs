using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ProductRouteSlot : BaseControl
	{
		private const float focusScale = 1f;


		private const float defaultScale = 0.96f;


		private LnText aCoin;


		private LnText availablePurchaseCount;


		private GameObject hoverFocus;


		private bool isInitialized;


		private LnText lackOfAsset;


		private LnText np;


		private Image productImg;


		private LnText productName;


		private WeaponRouteSlotShop weaponRouteSlotShop;


		public int CharacterCode => weaponRouteSlotShop.characterCode;


		public bool IsAvailablePurchase => weaponRouteSlotShop.availablePurchaseCount > 0;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Init();
		}


		public void Init()
		{
			if (isInitialized)
			{
				return;
			}

			isInitialized = true;
			hoverFocus = transform.FindRecursively("Hover_Focus").gameObject;
			productImg = GameUtil.Bind<Image>(gameObject, "IMG/Character");
			productName = GameUtil.Bind<LnText>(gameObject, "TXT_Name");
			np = GameUtil.Bind<LnText>(gameObject, "TXT_NP");
			aCoin = GameUtil.Bind<LnText>(gameObject, "TXT_ACOIN");
			lackOfAsset = GameUtil.Bind<LnText>(gameObject, "TXT_State");
			availablePurchaseCount = GameUtil.Bind<LnText>(gameObject, "TXT_Count");
		}


		public void SetSlot(WeaponRouteSlotShop data)
		{
			weaponRouteSlotShop = data;
			productName.text = LnUtil.GetCharacterName(weaponRouteSlotShop.characterCode);
			productImg.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(
					weaponRouteSlotShop.characterCode);
			if (weaponRouteSlotShop.availablePurchaseCount > 0)
			{
				np.gameObject.SetActive(true);
				aCoin.gameObject.SetActive(true);
				lackOfAsset.gameObject.SetActive(false);
				np.text = StringUtil.AssetToString(weaponRouteSlotShop.np);
				aCoin.text = StringUtil.AssetToString(weaponRouteSlotShop.aCoin);
			}
			else
			{
				np.gameObject.SetActive(false);
				aCoin.gameObject.SetActive(false);
				lackOfAsset.gameObject.SetActive(true);
			}

			availablePurchaseCount.text = string.Format("({0}/{1})", weaponRouteSlotShop.availablePurchaseCount,
				weaponRouteSlotShop.maxPaidCount);
			DisableHoverState();
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (IsAvailablePurchase)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
				MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(weaponRouteSlotShop);
			}
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			transform.localScale = new Vector3(1f, 1f, 1f);
			EnableHoverState();
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			transform.localScale = new Vector3(0.96f, 0.96f, 0.96f);
			DisableHoverState();
		}


		private void EnableHoverState()
		{
			hoverFocus.transform.localScale = Vector3.one;
		}


		private void DisableHoverState()
		{
			hoverFocus.transform.localScale = Vector3.zero;
		}
	}
}