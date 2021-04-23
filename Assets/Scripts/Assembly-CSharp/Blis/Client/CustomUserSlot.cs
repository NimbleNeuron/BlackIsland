using System;
using Blis.Common;
using Blis.Common.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomUserSlot : BaseUI
	{
		private readonly Color emptyAvatarColor = new Color(0.13f, 0.13f, 0.13f, 1f);


		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


		private Button btnAddAI;


		private Button btnBan;


		private Button btnEmpty;


		private Button btnObserver;


		private CustomGameSlot customGameSlot;


		private EventTrigger eventTrigger;


		private Image imgCharacter;


		private Image imgMySlot;


		private Image imgOwner;


		private bool isUser;


		private GameObject menu;


		private int teamNumber;


		private Text txtUserNickName;


		private GameObject userSlot;


		public int TeamNumber => customGameSlot.teamNumber;


		public bool IsEmpty => customGameSlot.IsEmptySlot();


		public bool IsUser => isUser;

		
		
		public event Action<int> OnRequestMoveSlot = delegate { };


		
		
		public event Action<int> OnRequestAddAI = delegate { };


		
		
		public event Action<long, int, string, bool> OnRequestBanUser = delegate { };


		
		
		public event Action<long> OnRequestMoveToObserverUser = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			userSlot = transform.FindRecursively("UserSlot").gameObject;
			imgMySlot = GameUtil.Bind<Image>(userSlot, "IMG_MySlot");
			imgCharacter = GameUtil.Bind<Image>(userSlot, "IMG_Character");
			imgOwner = GameUtil.Bind<Image>(userSlot, "IMG_Owner");
			txtUserNickName = GameUtil.Bind<Text>(userSlot, "TXT_UserNickName");
			btnEmpty = GameUtil.Bind<Button>(userSlot, "BTN_Empty");
			btnAddAI = GameUtil.Bind<Button>(gameObject, "BTN_AddAI");
			menu = transform.FindRecursively("Menu").gameObject;
			btnObserver = GameUtil.Bind<Button>(menu, "BTN_Observer");
			btnBan = GameUtil.Bind<Button>(menu, "BTN_Ban");
			btnEmpty.onClick.AddListener(delegate { RequestMoveSlot(); });
			btnAddAI.onClick.AddListener(delegate { OnRequestAddAI(customGameSlot.slotIndex); });
			btnObserver.onClick.AddListener(delegate { OnRequestMoveToObserverUser(customGameSlot.userNum); });
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
			menu.gameObject.SetActive(true);
		}


		private void OnPointerExit(BaseEventData eventData) { }


		public void SetCustomUser(CustomGameSlot customGameSlot)
		{
			this.customGameSlot = customGameSlot;
			isUser = !customGameSlot.isBot;
			SetSlotBG();
			SetOwner();
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


		public void SetEmpty(CustomGameSlot customGameSlot)
		{
			this.customGameSlot = customGameSlot;
			isUser = false;
			Clear();
		}


		private void Clear()
		{
			imgMySlot.transform.localScale = Vector3.zero;
			imgOwner.transform.localScale = Vector3.zero;
			imgCharacter.transform.localScale = Vector3.zero;
			txtUserNickName.transform.localScale = Vector3.zero;
			btnEmpty.gameObject.SetActive(true);
			btnAddAI.gameObject.SetActive(true);
			HideMenu();
		}


		private void SetSlotBG()
		{
			bool flag = Lobby.inst.User.UserNum == customGameSlot.userNum && !customGameSlot.isBot;
			imgMySlot.transform.localScale = flag ? Vector3.one : Vector3.zero;
		}


		private void SetOwner()
		{
			bool flag = MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(customGameSlot.userNum) &&
			            !customGameSlot.isBot;
			imgOwner.transform.localScale = flag ? Vector3.one : Vector3.zero;
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


		public void ShowAddAIButton(bool active)
		{
			btnAddAI.gameObject.SetActive(active);
		}


		private void SetMenuButtons()
		{
			bool flag = MonoBehaviourInstance<MatchingService>.inst.IsCustomGameRoomOwner(Lobby.inst.User.UserNum);
			btnBan.gameObject.SetActive(flag);
			bool active = false;
			if (!customGameSlot.isBot)
			{
				active = flag && !MonoBehaviourInstance<MatchingService>.inst.IsFullObserverInCustomGameRoom();
			}

			btnObserver.gameObject.SetActive(active);
		}


		public void HideMenu()
		{
			menu.gameObject.SetActive(false);
		}


		private void RequestMoveSlot()
		{
			if (!customGameSlot.IsEmptySlot())
			{
				return;
			}

			OnRequestMoveSlot(customGameSlot.slotIndex);
		}
	}
}