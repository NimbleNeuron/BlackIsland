using System;
using System.Collections.Generic;
using Blis.Common;
using Neptune.Http;

namespace Blis.Client
{
	public static class ProductApi
	{
		public static Func<HttpRequest> purchase(string productId)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/buy/" + productId, Array.Empty<object>()), null);
		}


		public static Func<HttpRequest> PurchaseChangeNickname(string newNickname)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/purchase/nickname", Array.Empty<object>()),
				newNickname);
		}


		public static Func<HttpRequest> GetShopProducts(ShopType shopType)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(string.Format("/shop/?shopType={0}", shopType),
				Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetShopProducts(ShopType shopType, int code)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/shop/listByCode/?shopType={0}&code={1}", shopType, code), Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetShopDlcReward(string productId)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/receiveDLC/", Array.Empty<object>()), productId);
		}


		public static Func<HttpRequest> GetShopWeaponRouteSlot()
		{
			return HttpRequestFactory.Get(ApiConstants.Url("/shop/weaponRouteSlot/", Array.Empty<object>()));
		}


		public static Func<HttpRequest> BuyWeaponRouteSlot(string productId, int characterCode)
		{
			WeaponRouteSlotParam data = new WeaponRouteSlotParam
			{
				productId = productId,
				characterCode = characterCode
			};
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/buyWeaponRouteSlot/", Array.Empty<object>()), data);
		}


		public static Func<HttpRequest> MakePayload(string productId)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/iap/token", Array.Empty<object>()),
				new KeyValueList("productId", productId).ToHashtable());
		}


		public static Func<HttpRequest> InAppReceiptSteam(string orderId)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/iap/receipt", Array.Empty<object>()),
				new KeyValueList("orderId", orderId).ToHashtable());
		}


		public static Func<HttpRequest> InAppPurchaseRestore(string failedReceipt)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/shop/iap/restore", Array.Empty<object>()), failedReceipt);
		}


		public class PurchaseResult
		{
			public LobbyApi.ItemContainer itemContainer;
		}


		public class ShopProductListResult
		{
			public string failedReceipt;

			public List<PurchasedDLC> purchasedDLCs;


			public List<ShopProduct> shopProducts;
		}


		public class TwitchDropsListResult
		{
			public List<PurchasedDLC> purchasedDLCList;
		}


		public class DlcRewardResult
		{
			public LobbyApi.ItemContainer itemContainer;
		}


		public class WeaponRouteSlotResult
		{
			public List<WeaponRouteSlotShop> weaponRouteSlotShops;
		}


		public class PayloadResult
		{
			public string iapToken;
		}
	}
}