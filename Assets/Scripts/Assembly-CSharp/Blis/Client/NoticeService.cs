using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public static class NoticeService
	{
		public delegate void OnReceivedCharacter(int characterCode);


		private const string LAST_NOTICE_CHECK_TIME = "LNCT";


		private const string LAST_GIFT_CHECK_TIME = "LGCT";


		private static readonly List<LobbyApi.LobbyNotice> noticeList = new List<LobbyApi.LobbyNotice>();


		private static readonly List<LobbyApi.UserGiftMail> giftMailList = new List<LobbyApi.UserGiftMail>();


		private static float lastNoticeRequestTime = float.MinValue;


		private static float lastGiftRequestTime = float.MinValue;


		private static int lastCheckedNoticeId = int.MinValue;


		private static long lastCheckedGiftId = long.MinValue;


		private static readonly List<long> requestedGiftIds;


		// Note: this type is marked as 'beforefieldinit'.
		static NoticeService()
		{
			onReceivedCharacter = delegate { };
			requestedGiftIds = new List<long>();
		}

		
		
		public static event OnReceivedCharacter onReceivedCharacter;


		public static void Init()
		{
			if (PlayerPrefs.HasKey("LNCT"))
			{
				lastCheckedNoticeId = PlayerPrefs.GetInt("LNCT");
			}

			if (PlayerPrefs.HasKey("LGCT"))
			{
				long.TryParse(PlayerPrefs.GetString("LGCT"), out lastCheckedGiftId);
			}
		}


		private static void UpdateNotice(List<LobbyApi.LobbyNotice> lobbyNoticeList)
		{
			noticeList.Clear();
			if (lobbyNoticeList != null)
			{
				foreach (LobbyApi.LobbyNotice lobbyNotice in lobbyNoticeList)
				{
					if (lobbyNotice != null)
					{
						noticeList.Add(lobbyNotice);
					}
				}
			}
		}


		public static int GetNoticeCount()
		{
			return noticeList.Count;
		}


		public static LobbyApi.LobbyNotice GetNotice(int index)
		{
			List<LobbyApi.LobbyNotice> list = noticeList;
			int? num = list != null ? new int?(list.Count) : null;
			if (!((index < num.GetValueOrDefault()) & (num != null)))
			{
				return null;
			}

			return noticeList[index];
		}


		private static void UpdateGiftMail(List<LobbyApi.UserGiftMail> userGiftMailList)
		{
			giftMailList.Clear();
			if (userGiftMailList != null)
			{
				foreach (LobbyApi.UserGiftMail userGiftMail in userGiftMailList)
				{
					if (userGiftMail != null)
					{
						giftMailList.Add(userGiftMail);
					}
				}
			}
		}


		public static int GetGiftMailCount()
		{
			return giftMailList.Count;
		}


		public static LobbyApi.UserGiftMail GetGiftMail(int index)
		{
			if (index >= giftMailList.Count)
			{
				return null;
			}

			return giftMailList[index];
		}


		public static void RequestAll(Action callback)
		{
			RequestLobbyNotice(delegate { RequestGiftMail(callback); });
		}


		private static void RequestLobbyNotice(Action callback)
		{
			if (Time.time - lastNoticeRequestTime >= 5f)
			{
				lastNoticeRequestTime = Time.time;
				RequestDelegate.requestCoroutine<LobbyApi.LobbyNoticeResult>(
					LobbyApi.GetLobbyNotice(Ln.GetCurrentLanguage()),
					delegate(RequestDelegateError err, LobbyApi.LobbyNoticeResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
							return;
						}

						UpdateNotice(res.lobbyNoticeResults);
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


		private static void RequestGiftMail(Action callback)
		{
			if (Time.time - lastGiftRequestTime >= 5f)
			{
				lastGiftRequestTime = Time.time;
				RequestDelegate.requestCoroutine<LobbyApi.GiftMailResult>(LobbyApi.GetGiftMail(Ln.GetCurrentLanguage()),
					delegate(RequestDelegateError err, LobbyApi.GiftMailResult res)
					{
						if (err != null)
						{
							MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
							return;
						}

						UpdateGiftMail(res.userGiftMails);
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


		public static void RequestGift(long id)
		{
			LobbyApi.UserGiftMail giftMail = giftMailList.Find(x => x.id == id);
			if (giftMail == null)
			{
				MonoBehaviourInstance<LobbyUI>.inst.MainMenu.NoticeHud.UpdateScrollView(false);
				return;
			}

			if (giftMail.expireDtm <= DateTime.Now.ToUniversalTime())
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("획득 기간이 지난 보상으로 획득할 수 없습니다."));
				giftMailList.Remove(giftMail);
				MonoBehaviourInstance<LobbyUI>.inst.MainMenu.NoticeHud.UpdateScrollView(false);
				return;
			}

			if (requestedGiftIds.Contains(id))
			{
				return;
			}

			requestedGiftIds.Add(id);
			RequestDelegate.requestCoroutine<LobbyApi.GiftMailItemResult>(LobbyApi.GetGiftMail(id),
				delegate(RequestDelegateError err, LobbyApi.GiftMailItemResult res)
				{
					requestedGiftIds.Remove(id);
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(err.errorType.ToString());
						return;
					}

					List<RewardItemInfo> list = new List<RewardItemInfo>();
					foreach (LobbyApi.Item item in res.itemContainer.items)
					{
						if (item.type == LobbyApi.ItemType.CHARACTER)
						{
							if (!Lobby.inst.IsHaveCharacter(item.itemCode))
							{
								Lobby.inst.AddCharacter(item.itemCode);
								OnReceivedCharacter onReceivedCharacter = NoticeService.onReceivedCharacter;
								if (onReceivedCharacter != null)
								{
									onReceivedCharacter(item.itemCode);
								}
							}

							list.Add(new RewardItemInfo(RewardItemType.Character, item.itemCode));
						}
						else if (item.type == LobbyApi.ItemType.SKIN)
						{
							if (!Lobby.inst.IsHaveSkin(item.itemCode))
							{
								Lobby.inst.AddSkin(item.itemCode);
								OnReceivedCharacter onReceivedCharacter2 = onReceivedCharacter;
								if (onReceivedCharacter2 != null)
								{
									onReceivedCharacter2(item.itemCode);
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

					giftMailList.Remove(giftMail);
					MonoBehaviourInstance<LobbyUI>.inst.MainMenu.NoticeHud.UpdateScrollView(false);
					if (list.Count > 0)
					{
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


		public static bool AnyNewNotice()
		{
			for (int i = 0; i < noticeList.Count; i++)
			{
				LobbyApi.LobbyNotice lobbyNotice = noticeList[i];
				if (lobbyNotice == null)
				{
					noticeList.RemoveAt(i--);
				}
				else if (lastCheckedNoticeId < lobbyNotice.id)
				{
					return true;
				}
			}

			return false;
		}


		public static bool AnyNewGiftMail()
		{
			for (int i = 0; i < giftMailList.Count; i++)
			{
				LobbyApi.UserGiftMail userGiftMail = giftMailList[i];
				if (userGiftMail == null)
				{
					giftMailList.RemoveAt(i--);
				}
				else if (lastCheckedGiftId < userGiftMail.id)
				{
					return true;
				}
			}

			return false;
		}


		public static void CheckingNotice()
		{
			int num = 0;
			for (int i = 0; i < noticeList.Count; i++)
			{
				LobbyApi.LobbyNotice lobbyNotice = noticeList[i];
				if (lobbyNotice == null)
				{
					noticeList.RemoveAt(i--);
				}
				else if (num < lobbyNotice.id)
				{
					num = lobbyNotice.id;
				}
			}

			if (lastCheckedNoticeId < num)
			{
				lastCheckedNoticeId = num;
				PlayerPrefs.SetInt("LNCT", lastCheckedNoticeId);
			}
		}


		public static void CheckingGiftMail()
		{
			long num = 0L;
			for (int i = 0; i < giftMailList.Count; i++)
			{
				LobbyApi.UserGiftMail userGiftMail = giftMailList[i];
				if (userGiftMail == null)
				{
					giftMailList.RemoveAt(i--);
				}
				else if (num < userGiftMail.id)
				{
					num = userGiftMail.id;
				}
			}

			if (lastCheckedGiftId < num)
			{
				lastCheckedGiftId = num;
				PlayerPrefs.SetString("LGCT", lastCheckedGiftId.ToString());
			}
		}
	}
}