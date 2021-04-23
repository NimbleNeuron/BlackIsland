using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomPortraitCharacterCard : BaseUI
	{
		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();
		private CustomGameSlot _customGameSlot;


		private Button btnAddFriend;


		private Button btnBan;


		private Button btnManDate;


		private Button btnViewInfo;


		private GameObject character;


		private GameObject empty;


		private EventTrigger eventTrigger;


		private Image imgCharacter;


		private Image imgCharacterNameBG;


		private Image imgHead;


		private GameObject menu;


		private Outline outLineCharacterName;


		private long ownerNum;


		private Text txtCharacterName;


		private Text txtUserName;

		
		
		public event Action<long, string, bool> OnRequestBanUser = delegate { };


		
		
		public event Action<long> OnRequestManDate = delegate { };


		
		
		public event Action<long> OnRequestAddFriend = delegate { };


		
		
		public event Action<long> OnRequestViewInfo = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			empty = transform.FindRecursively("Empty").gameObject;
			character = transform.FindRecursively("Character").gameObject;
			txtUserName = GameUtil.Bind<Text>(gameObject, "Character/TXT_UserName");
			imgHead = GameUtil.Bind<Image>(gameObject, "Character/TXT_UserName/IMG_Head");
			imgCharacter = GameUtil.Bind<Image>(gameObject, "Character/IMG_CharacterBG/IMG_Character");
			imgCharacterNameBG = GameUtil.Bind<Image>(gameObject, "Character/TXT_CharacterNameBG");
			txtCharacterName = GameUtil.Bind<Text>(gameObject, "Character/TXT_CharacterNameBG/TXT_CharacterName");
			outLineCharacterName = GameUtil.Bind<Outline>(txtCharacterName.gameObject, ref outLineCharacterName);
			menu = transform.FindRecursively("Menu").gameObject;
			btnBan = GameUtil.Bind<Button>(gameObject, "Menu/BTN_Ban");
			btnBan.onClick.AddListener(delegate
			{
				OnRequestBanUser(_customGameSlot.userNum, _customGameSlot.nickname, _customGameSlot.isBot);
			});
			btnManDate = GameUtil.Bind<Button>(gameObject, "Menu/BTN_ManDate");
			btnManDate.onClick.AddListener(delegate { OnRequestManDate(_customGameSlot.userNum); });
			btnAddFriend = GameUtil.Bind<Button>(gameObject, "Menu/BTN_AddFriend");
			btnAddFriend.onClick.AddListener(delegate { OnRequestAddFriend(_customGameSlot.userNum); });
			btnViewInfo = GameUtil.Bind<Button>(gameObject, "Menu/BTN_ViewInfo");
			btnViewInfo.onClick.AddListener(delegate { OnRequestViewInfo(_customGameSlot.userNum); });
			GameUtil.BindOrAdd<EventTrigger>(character, ref eventTrigger);
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
		}


		private void OnPointerEnter(BaseEventData eventData)
		{
			if (Lobby.inst.User.UserNum == _customGameSlot.userNum && !_customGameSlot.isBot)
			{
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.CustomModeWindow.OnPointerEnterMenuHider();
			menu.gameObject.SetActive(true);
		}


		private void OnPointerExit(BaseEventData eventData) { }


		public void SetUserInfo(CustomGameSlot customGameSlot, long ownerNum)
		{
			_customGameSlot = customGameSlot;
			txtUserName.text = customGameSlot.nickname;
			Sprite characterLobbyPortraitSprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(customGameSlot
					.characterCode);
			imgCharacter.sprite = characterLobbyPortraitSprite;
			txtCharacterName.text = Ln.Get(LnType.Character_Name, customGameSlot.characterCode.ToString());
			this.ownerNum = ownerNum;
			bool active = ownerNum == customGameSlot.userNum && !customGameSlot.isBot;
			imgHead.gameObject.SetActive(active);
			SetCharacterNameColor();
			SetMenuButtons();
			character.SetActive(true);
			empty.SetActive(false);
		}


		public void SetEmpty()
		{
			_customGameSlot = null;
			character.SetActive(false);
			empty.SetActive(true);
		}


		private void SetCharacterNameColor()
		{
			if (Lobby.inst.User.UserNum == _customGameSlot.userNum && !_customGameSlot.isBot)
			{
				imgCharacterNameBG.color = new Color(0.816f, 0.518f, 0.118f, 1f);
				txtCharacterName.color = new Color(0f, 0f, 0f, 1f);
				outLineCharacterName.enabled = false;
				return;
			}

			imgCharacterNameBG.color = new Color(0.404f, 0.404f, 0.404f, 1f);
			txtCharacterName.color = new Color(1f, 1f, 1f, 1f);
			outLineCharacterName.enabled = true;
		}


		private void SetMenuButtons()
		{
			bool active = Lobby.inst.User.UserNum == ownerNum;
			btnBan.gameObject.SetActive(active);
		}


		public void HideMenu()
		{
			menu.gameObject.SetActive(false);
		}
	}
}