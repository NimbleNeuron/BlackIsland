using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public static class ShopProductService
	{
		public delegate void OnReceivedDlcReward(int characterCode);


		private static List<ShopProduct> shopAssets = new List<ShopProduct>();


		private static List<ShopProduct> shopCharacters = new List<ShopProduct>();


		private static List<ShopProduct> shopSkins = new List<ShopProduct>();


		private static List<PurchasedDLC> purchasedDlcs = new List<PurchasedDLC>();


		private static List<WeaponRouteSlotShop> weaponRouteSlotShops = new List<WeaponRouteSlotShop>();


		public static readonly float RefreshTime;


		private static readonly List<string> requestedDlcRewardIds;


		private static bool includeOwnedCharacter;


		// Note: this type is marked as 'beforefieldinit'.
		static ShopProductService()
		{
			onReceivedDlcCharacter = delegate { };
			RefreshTime = 5f;
			requestedDlcRewardIds = new List<string>();
			includeOwnedCharacter = false;
		}


		public static int GetShopAssetsCount {
			get
			{
				if (shopAssets != null && shopAssets.Count > 0)
				{
					return shopAssets.Count;
				}

				return 0;
			}
		}

		
		
		public static event OnReceivedDlcReward onReceivedDlcCharacter;


		public static void RequestShopAssetList(Action callback, Action<string> callback2)
		{
			RequestDelegate.requestCoroutine<ProductApi.ShopProductListResult>(ProductApi.GetShopProducts(ShopType.NP),
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					shopAssets = res.shopProducts;
					Action callback3 = callback;
					if (callback3 != null)
					{
						callback3();
					}

					if (!string.IsNullOrEmpty(res.failedReceipt))
					{
						Action<string> callback4 = callback2;
						if (callback4 == null)
						{
							return;
						}

						callback4(res.failedReceipt);
					}
				});
		}


		public static void RequestShopCharacterList(Action callback)
		{
			RequestDelegate.requestCoroutine<ProductApi.ShopProductListResult>(
				ProductApi.GetShopProducts(ShopType.CHARACTER),
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					shopCharacters = new List<ShopProduct>();
					using (List<CharacterData>.Enumerator enumerator =
						GameDB.character.GetAllCharacterData().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CharacterData characterData = enumerator.Current;
							List<ShopProduct> list = res.shopProducts.FindAll(p => p.code == characterData.code);
							if (list.Count >= 2)
							{
								ShopProduct item = new ShopProduct(list[0], list[1]);
								shopCharacters.Add(item);
							}
						}
					}

					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void RequestShopCharacter(Action<ShopProduct> callback, int code)
		{
			RequestDelegate.requestCoroutine<ProductApi.ShopProductListResult>(
				ProductApi.GetShopProducts(ShopType.CHARACTER, code),
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					ShopProduct obj = new ShopProduct();
					using (List<ShopProduct>.Enumerator enumerator = res.shopProducts.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShopProduct shopProduct = enumerator.Current;
							List<ShopProduct> list = res.shopProducts.FindAll(p => p.code == shopProduct.code);
							if (list.Count >= 2)
							{
								obj = new ShopProduct(list[0], list[1]);
							}
						}
					}

					Action<ShopProduct> callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2(obj);
				});
		}


		public static void RequestShopSkinList(Action callback)
		{
			RequestDelegate.requestCoroutine<ProductApi.ShopProductListResult>(
				ProductApi.GetShopProducts(ShopType.SKIN),
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					shopSkins = new List<ShopProduct>();
					using (List<ShopProduct>.Enumerator enumerator = res.shopProducts.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShopProduct shopProduct = enumerator.Current;
							List<ShopProduct> list = res.shopProducts.FindAll(p => p.code == shopProduct.code);
							if (list.Count < 2)
							{
								ShopProduct shopData = new ShopProduct(list[0]);
								if (shopSkins.FirstOrDefault(p => p.code == shopData.code) == null)
								{
									shopSkins.Add(shopData);
								}
							}
							else
							{
								ShopProduct shopData = new ShopProduct(list[0], list[1]);
								if (shopSkins.FirstOrDefault(p => p.code == shopData.code) == null)
								{
									shopSkins.Add(shopData);
								}
							}
						}
					}

					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void RequestFavoriteWeaponRouteSlot(Action<ShopProduct> callback, int characterCode)
		{
			RequestDelegate.request<ProductApi.ShopProductListResult>(ProductApi.GetShopProducts(ShopType.WEAPON_ROUTE),
				false, delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					ShopProduct obj = new ShopProduct();
					using (List<ShopProduct>.Enumerator enumerator = res.shopProducts.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShopProduct shopProduct = enumerator.Current;
							List<ShopProduct> list = res.shopProducts.FindAll(p => p.code == shopProduct.code);
							if (list.Count >= 2)
							{
								obj = new ShopProduct(list[0], list[1])
								{
									code = characterCode
								};
							}
						}
					}

					Action<ShopProduct> callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2(obj);
				});
		}


		public static void RequestShopWeaponRouteSlot(Action callback)
		{
			RequestDelegate.request<ProductApi.WeaponRouteSlotResult>(ProductApi.GetShopWeaponRouteSlot(), false,
				delegate(RequestDelegateError err, ProductApi.WeaponRouteSlotResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					weaponRouteSlotShops = res.weaponRouteSlotShops;
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static void RequestShopNickNameChange(Action<List<ShopProduct>> callback)
		{
			RequestDelegate.request<ProductApi.ShopProductListResult>(
				ProductApi.GetShopProducts(ShopType.NICKNAME_CHANGE), false,
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					Action<List<ShopProduct>> callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2(res.shopProducts);
				});
		}


		public static void RequestPurchasedDLCList(Action callback)
		{
			RequestDelegate.requestCoroutine<ProductApi.ShopProductListResult>(ProductApi.GetShopProducts(ShopType.DLC),
				delegate(RequestDelegateError err, ProductApi.ShopProductListResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					purchasedDlcs = res.purchasedDLCs;
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public static ShopProduct GetShopAsset(int index)
		{
			if (index >= shopAssets.Count)
			{
				return null;
			}

			return shopAssets[index];
		}


		public static List<ShopProduct> GetShopCharacters()
		{
			return shopCharacters;
		}


		public static ShopProduct GetShopCharacter(int characterCode)
		{
			return shopCharacters.FirstOrDefault(p => p.code == characterCode);
		}


		public static List<ShopProduct> GetShopSkins()
		{
			return shopSkins;
		}


		public static ShopProduct GetShopSkin(int code)
		{
			return shopSkins.FirstOrDefault(p => p.code == code);
		}


		public static List<WeaponRouteSlotShop> GetWeaponRouteSlots()
		{
			return weaponRouteSlotShops;
		}


		public static int GetPurchasedDLCCount()
		{
			return purchasedDlcs.Count;
		}


		public static PurchasedDLC GetPurchasedDLC(int index)
		{
			if (index >= purchasedDlcs.Count)
			{
				return null;
			}

			return purchasedDlcs[index];
		}


		public static void RequestDlcReward(string productId)
		{
			PurchasedDLC userProduct = purchasedDlcs.Find(x => x.productId == productId);
			if (userProduct == null)
			{
				MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
					.UpdateDlcProducts(false);
				return;
			}

			if (requestedDlcRewardIds.Contains(productId))
			{
				return;
			}

			requestedDlcRewardIds.Add(productId);
			RequestDelegate.requestCoroutine<ProductApi.DlcRewardResult>(ProductApi.GetShopDlcReward(productId),
				delegate(RequestDelegateError err, ProductApi.DlcRewardResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					includeOwnedCharacter = false;
					List<RewardItemInfo> list = new List<RewardItemInfo>();
					foreach (LobbyApi.Item item in res.itemContainer.items)
					{
						if (item.type == LobbyApi.ItemType.CHARACTER)
						{
							if (Lobby.inst.IsHaveCharacter(item.itemCode))
							{
								includeOwnedCharacter = true;
							}
							else
							{
								Lobby.inst.AddCharacter(item.itemCode);
								OnReceivedDlcReward onReceivedDlcReward = onReceivedDlcCharacter;
								if (onReceivedDlcReward != null)
								{
									onReceivedDlcReward(item.itemCode);
								}
							}

							list.Add(new RewardItemInfo(RewardItemType.Character, item.itemCode));
						}
						else if (item.type == LobbyApi.ItemType.SKIN)
						{
							if (!Lobby.inst.IsHaveSkin(item.itemCode))
							{
								Lobby.inst.AddSkin(item.itemCode);
								OnReceivedDlcReward onReceivedDlcReward2 = onReceivedDlcCharacter;
								if (onReceivedDlcReward2 != null)
								{
									onReceivedDlcReward2(item.itemCode);
								}
							}

							list.Add(new RewardItemInfo(RewardItemType.Skin, item.itemCode));
						}
						else if (item.GetAssetType() == AssetType.ACOIN)
						{
							list.Add(new RewardItemInfo(RewardItemType.ACoin, item.GetAssetItemAmount()));
						}
						else if (item.GetAssetType() == AssetType.NP)
						{
							list.Add(new RewardItemInfo(RewardItemType.NP, item.GetAssetItemAmount()));
						}
					}

					purchasedDlcs.Remove(userProduct);
					MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
						.UpdateDlcProducts(false);
					if (list.Count > 0)
					{
						if (includeOwnedCharacter)
						{
							MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.OnCloseEvent += ShowPopup;
							includeOwnedCharacter = false;
						}

						MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.Open();
						if (list.Count <= 5)
						{
							RewardInfo rewardInfo = new RewardInfo(RewardType.MailBox, list);
							MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.AddReward(rewardInfo);
						}
						else
						{
							foreach (RewardInfo rewardInfo2 in GetRewardInfos(list))
							{
								MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.AddReward(rewardInfo2);
							}
						}

						MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.ShowReward();
					}
				});
		}


		private static void ShowPopup()
		{
			MonoBehaviourInstance<LobbyUI>.inst.RewardWindow.OnCloseEvent -= ShowPopup;
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("OwnedCharacterRefund"), new Popup.Button
			{
				text = Ln.Get("확인")
			});
		}


		private static List<RewardInfo> GetRewardInfos(List<RewardItemInfo> itemInfos)
		{
			List<RewardInfo> list = new List<RewardInfo>();
			int num = 5;
			int num2 = itemInfos.Count / num;
			int num3 = itemInfos.Count % num == 0 ? num2 : num2 + 1;
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				List<RewardItemInfo> list2 = new List<RewardItemInfo>();
				int num5 = num4;
				while (i < itemInfos.Count)
				{
					list2.Add(itemInfos[num5]);
					num4++;
					if (list2.Count == num || num4 == itemInfos.Count)
					{
						RewardInfo item = new RewardInfo(RewardType.MailBox, list2);
						list.Add(item);
						break;
					}

					num5++;
				}
			}

			return list;
		}
	}
}