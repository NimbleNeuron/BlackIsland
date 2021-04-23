using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShopAsset : BasePage
	{
		[SerializeField] private ProductMoneySlot productMoneySlotPrefab = default;


		[SerializeField] private Transform content = default;


		private string failedReceipt = default;


		private Button inquiryBtn = default;


		private float lastRequestTime = float.MinValue;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			inquiryBtn = GameUtil.Bind<Button>(gameObject, "Btn_Inquiry");
			inquiryBtn.onClick.AddListener(OnClickInquiry);
			ShopProductWindow shopProductWindow = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
			shopProductWindow.assetShopCloseAction = (Action) Delegate.Remove(shopProductWindow.assetShopCloseAction,
				new Action(RequestShopAssetList));
			ShopProductWindow shopProductWindow2 = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
			shopProductWindow2.assetShopCloseAction = (Action) Delegate.Combine(shopProductWindow2.assetShopCloseAction,
				new Action(RequestShopAssetList));
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			if (Time.time - lastRequestTime < ShopProductService.RefreshTime)
			{
				SetView();
				return;
			}

			lastRequestTime = Time.time;
			RequestShopAssetList();
		}


		private void RequestShopAssetList()
		{
			ShopProductService.RequestShopAssetList(SetView, SetFailReceipt);
		}


		private void SetView()
		{
			if (ShopProductService.GetShopAssetsCount == 0)
			{
				return;
			}

			int childCount = content.childCount;
			int num = Mathf.Max(0, ShopProductService.GetShopAssetsCount - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<ProductMoneySlot>(productMoneySlotPrefab, content);
			}

			ProductMoneySlot[] componentsInChildren = content.GetComponentsInChildren<ProductMoneySlot>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (j < ShopProductService.GetShopAssetsCount)
				{
					ShopProduct shopAsset = ShopProductService.GetShopAsset(j);
					if (shopAsset != null)
					{
						componentsInChildren[j].gameObject.SetActive(true);
						componentsInChildren[j].SetSlot(shopAsset, 0);
					}
					else
					{
						componentsInChildren[j].gameObject.SetActive(false);
					}
				}
				else
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}
		}


		private void SetFailReceipt(string failedReceipt)
		{
			this.failedReceipt = failedReceipt;
			inquiryBtn.gameObject.SetActive(!string.IsNullOrEmpty(failedReceipt));
		}


		private void OnClickInquiry()
		{
			if (string.IsNullOrEmpty(failedReceipt))
			{
				return;
			}

			MonoBehaviourInstance<Lobby_InAppPurchaseResult>.inst.RequestIAPRestore(failedReceipt);
			SetFailReceipt("");
		}
	}
}