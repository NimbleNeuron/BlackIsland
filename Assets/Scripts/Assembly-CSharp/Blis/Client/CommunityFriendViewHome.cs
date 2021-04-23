using Blis.Common;
using Steamworks;
using SuperScrollView;
using UnityEngine;

namespace Blis.Client
{
	public class CommunityFriendViewHome : BaseUI
	{
		private CanvasGroup canvasGroup;


		private bool initialized;


		private LoopListView2 listView;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			listView = GameUtil.Bind<LoopListView2>(gameObject, "Anchor/Scroll View");
			initialized = false;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitListView();
		}


		private void InitListView()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			listView.InitListView(CommunityService.GetOnlineFriendCount(), OnGetItemByIndex);
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > CommunityService.GetOnlineFriendCount() - 1)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("CommunityFriendSlotHome");
			CommunityFriendSlotHome component = loopListViewItem.GetComponent<CommunityFriendSlotHome>();
			CSteamID steamID = CSteamID.Nil;
			string steamName = string.Empty;
			int personaState = 0;
			string gameStatus = string.Empty;
			SteamFriendInfo onlineFriendInfo = CommunityService.GetOnlineFriendInfo(index);
			if (onlineFriendInfo != null)
			{
				steamID = onlineFriendInfo.steamID;
				steamName = onlineFriendInfo.name;
				personaState = onlineFriendInfo.personaState;
				gameStatus = onlineFriendInfo.GameStatus;
			}

			component.SetSlot(steamID, steamName, personaState, gameStatus);
			return loopListViewItem;
		}


		public void UpdateFriends(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(CommunityService.GetOnlineFriendCount(), resetPos);
				listView.RefreshAllShownItem();
			}
		}


		public void UpdateFriendInfo(CSteamID steamID)
		{
			if (!CommunityService.IsMe(steamID))
			{
				UpdateFriends(false);
			}
		}


		public void Show()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			UpdateFriends(true);
		}


		public void Hide()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;
		}
	}
}