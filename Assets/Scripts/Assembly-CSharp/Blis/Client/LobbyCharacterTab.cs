using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyCharacterTab : LobbyTabBaseUI, ILobbyTab, ICharacterSelectCardListener
	{
		private Button backButton;


		private CharacterTabCharacterDetail characterInfo;


		private CharacterTabCharacterList characterList;


		private CharacterTabSkinList characterSkinList;


		private InventorySubTabState currentTabState;


		private CharacterTabEmoticonList emoticonList;


		private Toggle menuCharacter;


		private Toggle menuEmoticon;


		private Toggle menuSkin;


		private LobbyTab prevTab;


		public void OnClickCharacterCard(int characterCode)
		{
			SingletonMonoBehaviour<GameAnalytics>.inst.SetSelectCharacter(characterCode);
			MonoBehaviourInstance<LobbyService>.inst.SelectCharacter(characterCode);
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.LoadCharacter(characterCode, 0);
			characterInfo.SetCharacterCode(characterCode);
			characterInfo.buySuccessCallback = delegate(int code) { characterList.buySuccessCallback(code); };
			characterList.Close();
			characterInfo.Open();
		}


		public void OnOpen(LobbyTab from)
		{
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.SetCameraRendering3D(true);
			EnableCanvas(true);
			menuCharacter.isOn = true;
			characterList.Open();
			characterInfo.Close();
			characterSkinList.ClosePage();
			emoticonList.ClosePage();
		}


		public TabCloseResult OnClose(LobbyTab to)
		{
			if (currentTabState == InventorySubTabState.EMOTICON && emoticonList.IsChangeDB())
			{
				CheckEmotionSave(null, delegate { MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(to); });
				return TabCloseResult.Fail;
			}

			characterInfo.Close();
			EnableCanvas(false);
			SingletonMonoBehaviour<LobbyCharacterStation>.inst.SetCameraRendering3D(false);
			MonoBehaviourInstance<LobbyUI>.inst.StopLobbySound();
			return TabCloseResult.Success;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			characterList = GameUtil.Bind<CharacterTabCharacterList>(gameObject, "CharacterList");
			characterInfo = GameUtil.Bind<CharacterTabCharacterDetail>(gameObject, "CharacterDetail");
			characterSkinList = GameUtil.Bind<CharacterTabSkinList>(gameObject, "SkinList");
			emoticonList = GameUtil.Bind<CharacterTabEmoticonList>(gameObject, "EmotionList");
			menuCharacter = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuCharacter");
			menuSkin = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuSkin");
			menuEmoticon = GameUtil.Bind<Toggle>(gameObject, "Submenu/MenuEmoticon");
			backButton = GameUtil.Bind<Button>(gameObject, "CharacterDetail/Btn_Back");
			menuCharacter.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, InventorySubTabState.CHARACTER);
			});
			menuSkin.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, InventorySubTabState.SKIN);
			});
			menuEmoticon.onValueChanged.AddListener(delegate(bool isOn)
			{
				OnToggleChange(isOn, InventorySubTabState.EMOTICON);
			});
			backButton.onClick.AddListener(OnClickBack);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		private void OnToggleChange(bool isOn, InventorySubTabState tabState)
		{
			if (isOn)
			{
				OpenInventoryTab(tabState);
			}
		}


		private void OpenInventoryTab(InventorySubTabState tabState)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab != LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(LobbyTab.InventoryTab);
			}

			characterList.Close();
			characterInfo.Close();
			characterSkinList.ClosePage();
			emoticonList.ClosePage();
			prevTab = LobbyTab.InventoryTab;
			currentTabState = tabState;
			switch (currentTabState)
			{
				case InventorySubTabState.CHARACTER:
					characterList.Open();
					if (emoticonList.IsChangeDB())
					{
						CheckEmotionSave(delegate { menuEmoticon.isOn = true; },
							delegate { menuCharacter.isOn = true; });
					}

					break;
				case InventorySubTabState.SKIN:
					characterSkinList.OpenPage();
					if (emoticonList.IsChangeDB())
					{
						CheckEmotionSave(delegate { menuEmoticon.isOn = true; }, delegate { menuSkin.isOn = true; });
					}

					break;
				case InventorySubTabState.EMOTICON:
					emoticonList.OpenPage();
					break;
				default:
					return;
			}
		}


		private void OnClickSkinCard(int skinCode, LobbyTab prev)
		{
			CharacterSkinData skinData = GameDB.character.GetSkinData(skinCode);
			if (skinData == null)
			{
				return;
			}

			prevTab = prev;
			LobbyTab lobbyTab = prevTab;
			if (lobbyTab != LobbyTab.InventoryTab)
			{
				if (lobbyTab == LobbyTab.ShopTab)
				{
					menuSkin.isOn = true;
					prevTab = LobbyTab.ShopTab;
				}
			}
			else
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetCurrentTab = LobbyTab.InventoryTab;
			}

			characterList.Open();
			characterInfo.Close();
			characterSkinList.ClosePage();
			emoticonList.ClosePage();
			OnClickCharacterCard(skinData.characterCode);
			characterInfo.OnShowSkin(skinData.characterCode, skinCode);
		}


		private void OnClickBack()
		{
			LobbyTab lobbyTab = prevTab;
			if (lobbyTab == LobbyTab.InventoryTab)
			{
				OpenInventoryTab(currentTabState);
				return;
			}

			if (lobbyTab != LobbyTab.ShopTab)
			{
				OpenInventoryTab(InventorySubTabState.CHARACTER);
				return;
			}

			MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(prevTab);
		}


		public void OnClickShowCharacterInfo(int characterCode)
		{
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab != LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(LobbyTab.InventoryTab);
			}

			MonoBehaviourInstance<LobbyUI>.inst.PlayLobbySound();
			MonoBehaviourInstance<LobbyUI>.inst.StopLobbySound();
			OnClickCharacterCard(characterCode);
		}


		public void OnClickShowSkinInfo(int skinCode)
		{
			LobbyTab currentTab = MonoBehaviourInstance<LobbyUI>.inst.CurrentTab;
			if (MonoBehaviourInstance<LobbyUI>.inst.CurrentTab != LobbyTab.InventoryTab)
			{
				MonoBehaviourInstance<LobbyUI>.inst.SetLobbyTab(LobbyTab.InventoryTab);
			}

			MonoBehaviourInstance<LobbyUI>.inst.PlayLobbySound();
			MonoBehaviourInstance<LobbyUI>.inst.StopLobbySound();
			OnClickSkinCard(skinCode, currentTab);
		}


		private void CheckEmotionSave(Action startCallback, Action completeCallback)
		{
			if (startCallback != null)
			{
				startCallback();
			}

			MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("변경 사항이 있습니다. 저장하시겠습니까?"), new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("저장"),
				callback = (Action) (() => emoticonList.OnSave(() => completeCallback()))
			}, new Popup.Button
			{
				type = Popup.ButtonType.Cancel,
				text = Ln.Get("아니오"),
				callback = (Action) (() =>
				{
					emoticonList.OnClickCancel();
					completeCallback();
				})
			});

			// co: dotPeek
			// if (startCallback != null)
			// {
			// 	startCallback();
			// }
			// Action <>9__1;
			// MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("변경 사항이 있습니다. 저장하시겠습니까?"), new Popup.Button[]
			// {
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Confirm,
			// 		text = Ln.Get("저장"),
			// 		callback = delegate()
			// 		{
			// 			CharacterTabEmoticonList characterTabEmoticonList = this.emoticonList;
			// 			Action callback;
			// 			if ((callback = <>9__1) == null)
			// 			{
			// 				callback = (<>9__1 = delegate()
			// 				{
			// 					completeCallback();
			// 				});
			// 			}
			// 			characterTabEmoticonList.OnSave(callback);
			// 		}
			// 	},
			// 	new Popup.Button
			// 	{
			// 		type = Popup.ButtonType.Cancel,
			// 		text = Ln.Get("아니오"),
			// 		callback = delegate()
			// 		{
			// 			this.emoticonList.OnClickCancel();
			// 			completeCallback();
			// 		}
			// 	}
			// });
		}


		private enum InventorySubTabState
		{
			CHARACTER,

			SKIN,

			EMOTICON
		}
	}
}