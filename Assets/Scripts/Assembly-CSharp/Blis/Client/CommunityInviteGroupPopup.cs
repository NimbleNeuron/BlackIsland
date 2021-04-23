using Blis.Common;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityInviteGroupPopup : BaseUI
	{
		private Image avatarPic;


		private Button btnAccept;


		private Button btnReject;


		private CanvasAlphaTweener canvasAlphaTweener;


		private CanvasGroup canvasGroup;


		private ScaleTweener canvasScaleTweener;


		private bool isOpen;


		private Text message;


		private Text steamName;


		public bool IsOpen => isOpen;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			canvasGroup = GameUtil.Bind<CanvasGroup>(gameObject, "Anchor");
			canvasGroup.alpha = 0f;
			canvasGroup.transform.localScale = Vector3.zero;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "Anchor");
			canvasScaleTweener = GameUtil.Bind<ScaleTweener>(gameObject, "Anchor");
			avatarPic = GameUtil.Bind<Image>(canvasGroup.gameObject, "AvatarBg/AvatarPic");
			steamName = GameUtil.Bind<Text>(canvasGroup.gameObject, "SteamName");
			message = GameUtil.Bind<Text>(canvasGroup.gameObject, "Message");
			btnAccept = GameUtil.Bind<Button>(canvasGroup.gameObject, "BtnAccept");
			btnReject = GameUtil.Bind<Button>(canvasGroup.gameObject, "BtnReject");
		}


		public void SetButtonEvents(UnityAction accept, UnityAction reject)
		{
			btnAccept.onClick.AddListener(accept);
			btnReject.onClick.AddListener(reject);
		}


		public void Open(CSteamID steamID, string steamName, string nickName)
		{
			isOpen = true;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasAlphaTweener.from = 0f;
			canvasAlphaTweener.to = 1f;
			canvasAlphaTweener.StopAnimation();
			canvasAlphaTweener.PlayAnimation();
			canvasScaleTweener.from = Vector3.zero;
			canvasScaleTweener.to = Vector3.one;
			canvasScaleTweener.StopAnimation();
			canvasScaleTweener.PlayAnimation();
			SetAvatar(steamID);
			SetSteamName(steamName);
			SetMessage(steamName, nickName);
		}


		public void Close()
		{
			isOpen = false;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasAlphaTweener.from = canvasGroup.alpha;
			canvasAlphaTweener.to = 0f;
			canvasAlphaTweener.StopAnimation();
			canvasAlphaTweener.PlayAnimation();
			canvasScaleTweener.from = canvasGroup.transform.localScale;
			canvasScaleTweener.to = Vector3.zero;
			canvasScaleTweener.StopAnimation();
			canvasScaleTweener.PlayAnimation();
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


		private void SetSteamName(string steamName)
		{
			this.steamName.text = steamName;
		}


		private void SetMessage(string steamName, string nickName)
		{
			string param_ = "<color=#F3D7A3>" + (string.IsNullOrEmpty(nickName) ? steamName : nickName) + "</color>";
			message.text = Ln.Format("{0}님이 당신을 초대합니다.", param_);
		}
	}
}