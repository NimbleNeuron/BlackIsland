using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityGroup : BaseUI
	{
		private readonly List<CommunityGroupMemberSlot> groupMembers = new List<CommunityGroupMemberSlot>();


		private Button btnLeaveGroup;


		private Button btnShowFriends;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			btnShowFriends = GameUtil.Bind<Button>(gameObject, "BtnShowFriends");
			btnShowFriends.onClick.AddListener(OnClickFriendList);
			btnLeaveGroup = GameUtil.Bind<Button>(gameObject, "BtnLeaveGroup");
			btnLeaveGroup.onClick.AddListener(OnClickLeaveGroup);
			btnLeaveGroup.gameObject.SetActive(false);
			GetComponentsInChildren<CommunityGroupMemberSlot>(groupMembers);
			Hide();
		}


		public void SetClickEventCallback(Action<CSteamID> callback)
		{
			foreach (CommunityGroupMemberSlot communityGroupMemberSlot in groupMembers)
			{
				communityGroupMemberSlot.SetClickEventCallback(callback);
			}
		}


		private void OnClickFriendList()
		{
			CommunityService.RefreshFriendInfo();
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.CommunityHud.Show();
		}


		private void OnClickLeaveGroup()
		{
			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("팀을 떠나시겠습니까?"), new Popup.Button
			{
				text = Ln.Get("나가기"),
				callback = CommunityService.LeaveLobby
			}, new Popup.Button
			{
				text = Ln.Get("취소")
			});
		}


		public void UpdateFriendInfo(CSteamID steamID)
		{
			if (groupMembers == null)
			{
				return;
			}

			foreach (CommunityGroupMemberSlot communityGroupMemberSlot in groupMembers)
			{
				if (steamID.Equals(communityGroupMemberSlot.SteamID))
				{
					communityGroupMemberSlot.SetMember(steamID);
					break;
				}
			}
		}


		public void UpdateGroup()
		{
			for (int i = 0; i < groupMembers.Count; i++)
			{
				groupMembers[i].SetMember(CommunityService.GetLobbyMemberSteamID(i));
			}

			btnLeaveGroup.gameObject.SetActive(1 <= CommunityService.LobbyMemberCount);
		}


		public void Show()
		{
			transform.localScale = Vector3.one;
		}


		public void Hide()
		{
			transform.localScale = Vector3.zero;
		}
	}
}