using Blis.Common;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityUserSlot : BaseUI
	{
		private Image avatarPic;


		private Text nickName;


		private Image stateImg;


		private Text stateText;


		private Text steamName;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			avatarPic = GameUtil.Bind<Image>(gameObject, "AvatarBg/AvatarPic");
			nickName = GameUtil.Bind<Text>(gameObject, "NickName");
			steamName = GameUtil.Bind<Text>(gameObject, "SteamName");
			stateImg = GameUtil.Bind<Image>(gameObject, "StateImg");
			stateText = GameUtil.Bind<Text>(gameObject, "StateImg/StateText");
		}


		public void SetSlot(CSteamID steamID)
		{
			SetAvatar(steamID);
			SetNickName(CommunityService.GetFriendNickName(steamID));
			SetSteamName(CommunityService.GetFriendSteamName(steamID));
			SetState(CommunityService.GetPersonaState(steamID), CommunityService.GetGameStatus(steamID));
		}


		private void SetAvatar(CSteamID steamID)
		{
			Texture2D avatar = CommunityService.GetAvatar(steamID);
			if (avatar == null)
			{
				avatarPic.transform.localScale = Vector3.zero;
				return;
			}

			avatarPic.transform.localScale = Vector3.one;
			Rect rect = new Rect(0f, 0f, avatar.width, avatar.height);
			avatarPic.sprite = Sprite.Create(avatar, rect, new Vector2(0.5f, 0.5f));
		}


		private void SetNickName(string nickName)
		{
			this.nickName.text = nickName;
		}


		private void SetSteamName(string steamName)
		{
			this.steamName.text = steamName;
		}


		private void SetState(int personaState, string gameStatus)
		{
			stateImg.color = CommunityService.GetStateImgColor(personaState, gameStatus);
			stateText.color = CommunityService.GetStateTextColor(personaState, gameStatus);
			stateText.text = CommunityService.GetStateText(personaState, gameStatus);
		}
	}
}