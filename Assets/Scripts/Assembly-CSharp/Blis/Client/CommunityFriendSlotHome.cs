using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CommunityFriendSlotHome : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		private Image avatarPic;


		private Image background;


		private Button btnRollOver;


		private RectTransform empty;


		private RectTransform info;


		private Image rollOverBg;


		private Image stateImg;


		private Text stateText;


		private CSteamID steamID;


		private Text steamName;


		public void OnPointerEnter(PointerEventData eventData)
		{
			rollOverBg.enabled = true;
			btnRollOver.interactable = true;
			btnRollOver.image.enabled = true;
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			rollOverBg.enabled = false;
			btnRollOver.interactable = false;
			btnRollOver.image.enabled = false;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			empty = GameUtil.Bind<RectTransform>(gameObject, "Empty");
			info = GameUtil.Bind<RectTransform>(gameObject, "Info");
			avatarPic = GameUtil.Bind<Image>(info.gameObject, "AvatarBg/AvatarPic");
			steamName = GameUtil.Bind<Text>(info.gameObject, "SteamName");
			stateImg = GameUtil.Bind<Image>(info.gameObject, "StateImg");
			stateText = GameUtil.Bind<Text>(info.gameObject, "StateImg/StateText");
			background = GameUtil.Bind<Image>(info.gameObject, "EventBg");
			rollOverBg = GameUtil.Bind<Image>(info.gameObject, "RollOverBg");
			btnRollOver = GameUtil.Bind<Button>(rollOverBg.gameObject, "BtnRollOver");
			btnRollOver.onClick.AddListener(OnClickDetailInfo);
			rollOverBg.enabled = false;
			btnRollOver.interactable = false;
			btnRollOver.image.enabled = false;
		}


		public void Empty()
		{
			steamID = CSteamID.Nil;
			empty.localScale = Vector3.one;
			info.localScale = Vector3.zero;
		}


		public void SetSlot(CSteamID steamID, string steamName, int personaState, string gameStatus)
		{
			if (personaState == 2)
			{
				gameObject.SetActive(false);
				return;
			}

			this.steamID = steamID;
			SetAvatar(steamID);
			SetSteamName(steamName);
			SetState(personaState, gameStatus);
			empty.localScale = Vector3.zero;
			info.localScale = Vector3.one;
			transform.localScale = Vector3.one;
		}


		private void SetState(int personaState, string gameStatus)
		{
			stateImg.color = CommunityService.GetStateImgColor(personaState, gameStatus);
			stateText.color = CommunityService.GetStateTextColor(personaState, gameStatus);
			stateText.text = CommunityService.GetStateText(personaState, gameStatus);
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


		private void OnClickDetailInfo()
		{
			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.CommunityHud.ShowFriendPopup(steamID);
		}
	}
}