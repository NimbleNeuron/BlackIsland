using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public static class ShopTwitchDropsService
	{
		public delegate void OnReceivedDlcReward(int characterCode);


		private static List<PurchasedDLC> twitchDropsList = new List<PurchasedDLC>();


		private static float lastRequestTime = float.MinValue;


		private static readonly List<string> requestedDropsRewardIds;


		private static bool includeOwnedCharacter;


		// Note: this type is marked as 'beforefieldinit'.
		static ShopTwitchDropsService()
		{
			onReceivedDlcCharacter = delegate { };
			requestedDropsRewardIds = new List<string>();
			includeOwnedCharacter = false;
		}

		
		
		public static event OnReceivedDlcReward onReceivedDlcCharacter;


		public static int GetTwitchDropsCount()
		{
			return twitchDropsList.Count;
		}


		public static PurchasedDLC GetTwitchDrops(int index)
		{
			if (index >= twitchDropsList.Count)
			{
				return null;
			}

			return twitchDropsList[index];
		}


		public static void RequestTwitchDropsList(Action callback)
		{
			if (Time.time - lastRequestTime >= 5f)
			{
				lastRequestTime = Time.time;
				RequestDelegate.requestCoroutine<ProductApi.TwitchDropsListResult>(LobbyApi.GetTwitchDrops(),
					delegate(RequestDelegateError err, ProductApi.TwitchDropsListResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
							return;
						}

						twitchDropsList = res.purchasedDLCList;
						Action callback3 = callback;
						if (callback3 == null)
						{
							return;
						}

						callback3();
					});
				return;
			}

			Action callback2 = callback;
			if (callback2 == null)
			{
				return;
			}

			callback2();
		}


		public static void RequestDropsReward(string productId, string rewardId)
		{
			PurchasedDLC userProduct = twitchDropsList.Find(x => x.productId == productId);
			if (userProduct == null)
			{
				MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
					.UpdateTwitchDrops(false);
				return;
			}

			if (requestedDropsRewardIds.Contains(rewardId))
			{
				return;
			}

			requestedDropsRewardIds.Add(rewardId);
			RequestDelegate.requestCoroutine<ProductApi.DlcRewardResult>(LobbyApi.GetTwitchDropsReward(
				new TwitchDropsRewardParam
				{
					productId = productId,
					rewardId = rewardId
				}), delegate(RequestDelegateError err, ProductApi.DlcRewardResult res)
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

				twitchDropsList.Remove(userProduct);
				MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
					.UpdateTwitchDrops(false);
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