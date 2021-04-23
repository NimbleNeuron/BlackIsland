using Blis.Common;
using Steamworks;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityFriendView : BaseUI
	{
		private CanvasGroup canvasGroup;


		private Toggle friendToggle;


		private CanvasAlphaTweener hideTweener;


		private bool initialized;


		private Toggle latelyToggle;


		private LoopListView2 listView;


		private new RectTransform rectTransform;


		private PositionTweener showTweener;


		private Toggle teamToggle;


		private ToggleGroup toggleGroup;


		private CommunityUserSlot userSlot;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			canvasGroup.alpha = 1f;
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref hideTweener);
			showTweener = GameUtil.Bind<PositionTweener>(gameObject, "Anchor");
			rectTransform = showTweener.GetComponent<RectTransform>();
			toggleGroup = GameUtil.Bind<ToggleGroup>(showTweener.gameObject, "Tab");
			friendToggle = GameUtil.Bind<Toggle>(toggleGroup.gameObject, "Friends");
			latelyToggle = GameUtil.Bind<Toggle>(toggleGroup.gameObject, "Lately");
			teamToggle = GameUtil.Bind<Toggle>(toggleGroup.gameObject, "Team");
			friendToggle.isOn = true;
			userSlot = GameUtil.Bind<CommunityUserSlot>(showTweener.gameObject, "MyInfo");
			listView = GameUtil.Bind<LoopListView2>(showTweener.gameObject, "Scroll View");
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
			listView.InitListView(CommunityService.GetFriendCount(), OnGetItemByIndex);
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > CommunityService.GetFriendCount() - 1)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("CommunityFriendSlot");
			CommunityFriendSlot component = loopListViewItem.GetComponent<CommunityFriendSlot>();
			CSteamID steamID = CSteamID.Nil;
			string steamName = string.Empty;
			int personaState = 0;
			string gameStatus = string.Empty;
			SteamFriendInfo friendInfo = CommunityService.GetFriendInfo(index);
			if (friendInfo != null)
			{
				steamID = friendInfo.steamID;
				steamName = friendInfo.name;
				personaState = friendInfo.personaState;
				gameStatus = friendInfo.GameStatus;
			}

			component.SetSlot(steamID, steamName, personaState, gameStatus);
			return loopListViewItem;
		}


		public void UpdateMyInfo()
		{
			CommunityUserSlot communityUserSlot = userSlot;
			if (communityUserSlot == null)
			{
				return;
			}

			communityUserSlot.SetSlot(CommunityService.MySteamID);
		}


		public void UpdateFriends(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(CommunityService.GetFriendCount(), resetPos);
				listView.RefreshAllShownItem();
			}
		}


		public void UpdateFriendInfo(CSteamID steamID)
		{
			if (CommunityService.IsMe(steamID))
			{
				UpdateMyInfo();
				return;
			}

			UpdateFriends(false);
		}


		public void Show()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			rectTransform.anchoredPosition3D = new Vector3(-510f, 0f, 0f);
			showTweener.from = rectTransform.anchoredPosition3D;
			showTweener.to = Vector3.zero;
			showTweener.StopAnimation();
			showTweener.PlayAnimation();
			UpdateMyInfo();
			UpdateFriends(true);
		}


		public void Hide()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			hideTweener.from = canvasGroup.alpha;
			hideTweener.to = 0f;
			hideTweener.StopAnimation();
			hideTweener.PlayAnimation();
		}
	}
}