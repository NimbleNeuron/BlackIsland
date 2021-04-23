using System;
using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityGroupMemberSlot : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		private Image avatar;


		private Image avatarMyBg;


		private Image avatarPic;


		private Button button;


		private Action<CSteamID> clickEventCallback;


		private Image host;


		private CSteamID steamID;


		public CSteamID SteamID => steamID;


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (steamID.IsValid())
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(CommunityService.GetFriendSteamName(steamID));
			}
			else
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("친구와 팀 만들기"));
			}

			Vector2 vector = transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(15f, 75f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
			host = GameUtil.Bind<Image>(gameObject, "Crown");
			avatar = GameUtil.Bind<Image>(gameObject, "Avatar");
			avatarMyBg = GameUtil.Bind<Image>(avatar.gameObject, "MyBg");
			avatarPic = GameUtil.Bind<Image>(avatar.gameObject, "AvatarPic");
			SetEmpty();
		}


		public void SetClickEventCallback(Action<CSteamID> callback)
		{
			clickEventCallback = callback;
		}


		public void SetMember(CSteamID steamID)
		{
			if (steamID.IsValid())
			{
				SetSteamId(steamID);
				SetHost(CommunityService.IsLobbyOwner(steamID));
				SetPortrait(steamID);
				SetMyBg(CommunityService.IsMe(steamID));
				return;
			}

			SetEmpty();
		}


		private void SetPortrait(CSteamID steamID)
		{
			SetAvatar(CommunityService.GetAvatar(steamID));
		}


		private void SetEmpty()
		{
			SetSteamId(CSteamID.Nil);
			SetHost(false);
			SetAvatar(null);
			SetMyBg(false);
			avatar.enabled = false;
		}


		private void SetSteamId(CSteamID steamID)
		{
			this.steamID = steamID;
		}


		private void SetHost(bool enable)
		{
			host.enabled = enable;
		}


		private void SetAvatar(Texture2D avatar)
		{
			this.avatar.enabled = true;
			if (avatar == null)
			{
				avatarPic.enabled = false;
				return;
			}

			avatarPic.enabled = true;
			Rect rect = new Rect(0f, 0f, avatar.width, avatar.height);
			avatarPic.sprite = Sprite.Create(avatar, rect, new Vector2(0.5f, 0.5f));
		}


		private void SetCharacterPortrait(int characterCode)
		{
			avatar.enabled = true;
			avatarPic.enabled = true;
			avatarPic.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterCommunitySprite(characterCode);
		}


		private void SetMyBg(bool enable)
		{
			avatarMyBg.enabled = enable;
		}


		private void OnClick()
		{
			Action<CSteamID> action = clickEventCallback;
			if (action == null)
			{
				return;
			}

			action(steamID);
		}
	}
}