using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabCharacterDetail : BasePage, ILnEventHander
	{
		public delegate void BuySuccessCallback(int code);


		public delegate void NoMoneyOpenShopCallback();


		private Button btn_GoBuy;


		public BuySuccessCallback buySuccessCallback;


		private CanvasGroup canvasGroup;


		private CharacterDetailInfomation characterDetailInfomation;


		private CharacterDetailSkill characterDetailSkill;


		private CharacterDetailSkin characterDetailSkin;


		private CharacterDetailSummary characterDetailSummary;


		private Text characterName;


		private DetailInfoMenu currentMenu;


		private Toggle menuInfomation;


		private Toggle menuSkill;


		private Toggle menuSkin;


		private Toggle menuSummary;


		public NoMoneyOpenShopCallback noMoneyOpenShopCallback;


		private int selectedCharacterCode = -1;


		public void OnLnDataChange()
		{
			if (selectedCharacterCode >= 0)
			{
				SetCharacterCode(selectedCharacterCode);
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			menuSummary = GameUtil.Bind<Toggle>(gameObject, "Submenu/Summary");
			menuSkill = GameUtil.Bind<Toggle>(gameObject, "Submenu/Skills");
			menuInfomation = GameUtil.Bind<Toggle>(gameObject, "Submenu/Info");
			menuSkin = GameUtil.Bind<Toggle>(gameObject, "Submenu/Skin");
			characterName = GameUtil.Bind<Text>(gameObject, "CharacterName");
			btn_GoBuy = GameUtil.Bind<Button>(gameObject, "Btn_GoBuy");
			characterDetailSummary = GameUtil.Bind<CharacterDetailSummary>(gameObject, "CharacterDetailSummary");
			characterDetailSkill = GameUtil.Bind<CharacterDetailSkill>(gameObject, "CharacterDetailSkill");
			characterDetailInfomation = GameUtil.Bind<CharacterDetailInfomation>(gameObject, "CharacterDetailInfo");
			characterDetailSkin = GameUtil.Bind<CharacterDetailSkin>(gameObject, "CharacterDetailSkin");
			menuSummary.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, DetailInfoMenu.SUMMARY);
			});
			menuSkill.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, DetailInfoMenu.SKILL); });
			menuInfomation.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, DetailInfoMenu.INFOMATION);
			});
			menuSkin.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, DetailInfoMenu.SKIN); });
			btn_GoBuy.onClick.AddListener(OnClickGoBuyBtn);
		}


		public void Open()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			OpenPage();
			menuSummary.isOn = true;
			OpenDetailInfo(DetailInfoMenu.SUMMARY);
		}


		public void Close()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			MonoBehaviourInstance<LobbyUI>.inst.StopLobbySound();
			ClosePage();
		}


		public void SetCharacterCode(int characterCode)
		{
			selectedCharacterCode = characterCode;
			characterName.text = LnUtil.GetCharacterName(characterCode);
			btn_GoBuy.gameObject.SetActive(!Lobby.inst.IsHaveCharacter(characterCode));
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
			characterDetailSummary.SetCharacterData(characterCode, characterMasteryData);
			characterDetailSkill.SetCharacterData(characterCode, characterMasteryData);
			characterDetailInfomation.SetCharacterData(characterCode, characterMasteryData);
		}


		private void OnToggleChange(bool isOn, DetailInfoMenu menu)
		{
			if (isOn)
			{
				OpenDetailInfo(menu);
			}
		}


		private void OpenDetailInfo(DetailInfoMenu currentMenu)
		{
			this.currentMenu = currentMenu;
			switch (currentMenu)
			{
				case DetailInfoMenu.SUMMARY:
					characterDetailSummary.OpenPage();
					characterDetailSkill.ClosePage();
					characterDetailInfomation.ClosePage();
					characterDetailSkin.ClosePage();
					return;
				case DetailInfoMenu.SKILL:
					characterDetailSummary.ClosePage();
					characterDetailSkill.OpenPage();
					characterDetailInfomation.ClosePage();
					characterDetailSkin.ClosePage();
					return;
				case DetailInfoMenu.INFOMATION:
					characterDetailSummary.ClosePage();
					characterDetailSkill.ClosePage();
					characterDetailInfomation.OpenPage();
					characterDetailSkin.ClosePage();
					return;
				case DetailInfoMenu.SKIN:
					characterDetailSkin.SetData(selectedCharacterCode, -1);
					characterDetailSummary.ClosePage();
					characterDetailSkill.ClosePage();
					characterDetailInfomation.ClosePage();
					characterDetailSkin.OpenPage();
					return;
				default:
					return;
			}
		}


		public void OnClickGoBuyBtn()
		{
			ShopProductService.RequestShopCharacter(delegate(ShopProduct shopCharacterData)
			{
				if (shopCharacterData.purchaseType == PurchaseType.TUTORIAL)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("튜토리얼 보상으로 획득가능합니다."), new Popup.Button
					{
						text = Ln.Get("확인")
					});
					return;
				}

				MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Open();
				MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.SetProduct(shopCharacterData);
				ShopProductWindow shopProductWindow = MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow;
				shopProductWindow.buySuccessCallback = (ShopProductWindow.BuySuccessCallback) Delegate.Combine(
					shopProductWindow.buySuccessCallback, new ShopProductWindow.BuySuccessCallback(delegate(object code)
					{
						btn_GoBuy.gameObject.SetActive(false);
						buySuccessCallback((int) code);
					}));
				MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.noMoneyOpenShopCallback = delegate
				{
					MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyShopTab>(LobbyTab.ShopTab)
						.NoMoneyOpenShopCallback();
				};
			}, selectedCharacterCode);
		}


		public void OnCloseCharacterDetailSkill()
		{
			characterDetailSkill.ClosePage();
		}


		public void OnShowSkin(int characterCode, int skinCode)
		{
			menuSkin.isOn = true;
			OnToggleChange(true, DetailInfoMenu.SKIN);
			characterDetailSkin.SetData(characterCode, skinCode);
			characterDetailSkin.Refresh();
		}


		private enum DetailInfoMenu
		{
			SUMMARY,

			SKILL,

			INFOMATION,

			SKIN
		}


		public class MasteryIcon
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly Image icon;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private readonly Toggle toggle;


			private MasteryType masteryType;


			private SkillData skillData;


			public MasteryIcon(GameObject gameObject)
			{
				this.gameObject = gameObject;
				GameUtil.Bind<Toggle>(gameObject, ref toggle);
				icon = GameUtil.Bind<Image>(gameObject, "Icon");
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
				eventTrigger.triggers.Clear();
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


			public Toggle Toggle => toggle;


			public SkillData SkillData => skillData;


			public void SetMastery(MasteryType masteryType)
			{
				if (masteryType != MasteryType.None)
				{
					skillData = GameDB.skill.GetSkillData(masteryType, 1, 0);
					icon.sprite = masteryType.GetIcon();
					gameObject.SetActive(true);
					this.masteryType = masteryType;
					return;
				}

				skillData = null;
				icon.sprite = null;
				gameObject.SetActive(false);
				masteryType = MasteryType.None;
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(string.Format("WeaponType/{0}", masteryType)));
				Vector2 vector = gameObject.transform.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(-20.5f, 72f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}