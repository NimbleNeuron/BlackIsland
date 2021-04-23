using System;
using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomObserverSlot : BaseUI
	{
		private readonly Color emptyAvatarColor = new Color(0.13f, 0.13f, 0.13f, 1f);


		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


		private Button btnBan;


		private Button btnEmpty;


		private CustomGameSlot customGameSlot;


		private EventTrigger eventTrigger;


		private Image imgCharacter;


		private Image imgMySlot;


		private bool isUser;


		private GameObject menu;


		private Text txtUserNickName;


		private GameObject userSlot;


		public bool IsEmpty => customGameSlot.IsEmptySlot();


		public bool IsUser => isUser;

		
		
		public event Action OnRequestMoveSlot = delegate { };


		
		
		public event Action<long, int, string, bool> OnRequestBanUser = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			userSlot = transform.FindRecursively("UserSlot").gameObject;
			imgMySlot = GameUtil.Bind<Image>(userSlot, "IMG_MySlot");
			imgCharacter = GameUtil.Bind<Image>(userSlot, "IMG_Character");
			txtUserNickName = GameUtil.Bind<Text>(userSlot, "TXT_UserNickName");
			btnEmpty = GameUtil.Bind<Button>(userSlot, "BTN_Empty");
			menu = transform.FindRecursively("Menu").gameObject;
			menu.SetActive(false);
			btnBan = GameUtil.Bind<Button>(menu, "BTN_Ban");
			btnBan.gameObject.SetActive(false);
			btnEmpty.onClick.AddListener(RequestMoveSlot);
			btnBan.onClick.AddListener(delegate
			{
				OnRequestBanUser(customGameSlot.userNum, customGameSlot.slotIndex, customGameSlot.nickname,
					customGameSlot.isBot);
			});
			GameUtil.BindOrAdd<EventTrigger>(userSlot, ref eventTrigger);
			onEnterEvent.AddListener(OnPointerEnter);
			onExitEvent.AddListener(OnPointerExit);
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerEnter,
				callback = onEnterEvent
			});
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerExit,
				callback = onExitEvent
			});
			Clear();
		}


		private void OnPointerEnter(BaseEventData eventData)
		{
			if (customGameSlot.IsEmptySlot())
			{
				return;
			}

			if (Lobby.inst.User.UserNum == customGameSlot.userNum && !customGameSlot.isBot)
			{
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.OnPointerEnterMenuHider();
			menu.SetActive(true);
		}


		private void OnPointerExit(BaseEventData eventData) { }


		public void SetEmpty(CustomGameSlot customGameSlot)
		{
			this.customGameSlot = customGameSlot;
			isUser = false;
			Clear();
		}


		private void Clear()
		{
			imgMySlot.transform.localScale = Vector3.zero;
			imgCharacter.transform.localScale = Vector3.zero;
			txtUserNickName.transform.localScale = Vector3.zero;
			btnEmpty.gameObject.SetActive(true);
		}


		public void SetCustomUser(CustomGameSlot customGameSlot)
		{
			this.customGameSlot = customGameSlot;
			isUser = !customGameSlot.isBot;
			SetSlotBG();
			Sprite characterImage = null;
			if (customGameSlot.isBot)
			{
				characterImage =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterCommunitySprite(customGameSlot
						.characterCode);
			}
			else if (!string.IsNullOrEmpty(customGameSlot.steamId))
			{
				Texture2D avatar = CommunityService.GetAvatar(new CSteamID(ulong.Parse(customGameSlot.steamId)));
				if (avatar != null)
				{
					Rect rect = new Rect(0f, 0f, avatar.width, avatar.height);
					characterImage = Sprite.Create(avatar, rect, new Vector2(0.5f, 0.5f));
				}
			}

			SetCharacterImage(characterImage);
			SetUserNickName(customGameSlot.nickname);
			SetMenuButtons();
		}


		private void SetCharacterImage(Sprite sprite)
		{
			imgCharacter.sprite = sprite;
			imgCharacter.color = sprite == null ? emptyAvatarColor : Color.white;
			imgCharacter.transform.localScale = Vector3.one;
		}


		private void SetUserNickName(string userNickName)
		{
			bool flag = userNickName != string.Empty;
			btnEmpty.gameObject.SetActive(!flag);
			txtUserNickName.transform.localScale = flag ? Vector3.one : Vector3.zero;
			txtUserNickName.text = userNickName;
		}


		private void SetMenuButtons()
		{
			bool active = MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum);
			btnBan.gameObject.SetActive(active);
		}


		public void HideMenu()
		{
			menu.SetActive(false);
		}


		private void SetSlotBG()
		{
			bool flag = Lobby.inst.User.UserNum == customGameSlot.userNum;
			imgMySlot.transform.localScale = flag ? Vector3.one : Vector3.zero;
		}


		private void RequestMoveSlot()
		{
			if (!customGameSlot.IsEmptySlot())
			{
				return;
			}

			OnRequestMoveSlot();
		}
	}
}