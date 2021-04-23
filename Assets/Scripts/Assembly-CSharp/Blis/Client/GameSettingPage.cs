using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class GameSettingPage : BasePage
	{
		private readonly Color FIXED_COLOR_FOCUS = new Color(0.39f, 0.39f, 0.39f, 255f);


		private readonly Color FIXED_COLOR_TEXT = new Color(0.47f, 0.47f, 0.47f, 1f);


		private Toggle extendMinimapMove;


		private Toggle hideNameFromEnemy;


		private Image hideNameFromEnemyFocus;


		private Text hideNameFromEnemyText;


		private Toggle ignoreEmotionFromEnemy;


		private Toggle minimapMove;


		private Toggle mouseReverse;


		private Toggle moveExpandMap;


		private Toggle viewAllChatting;


		private Toggle viewTeamChatting;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			extendMinimapMove = GameUtil.Bind<Toggle>(gameObject, "MiniMapMove/Sub/ExtendMinimapMove/Toggle");
			minimapMove = GameUtil.Bind<Toggle>(gameObject, "MiniMapMove/Sub/MinimapMove/Toggle");
			viewTeamChatting = GameUtil.Bind<Toggle>(gameObject, "Chatting/Sub/TeamChatting/Toggle");
			viewAllChatting = GameUtil.Bind<Toggle>(gameObject, "Chatting/Sub/AllChatting/Toggle");
			mouseReverse = GameUtil.Bind<Toggle>(gameObject, "DetailSetting/Sub/MouseReverse/Toggle");
			moveExpandMap = GameUtil.Bind<Toggle>(gameObject, "DetailSetting/Sub/MoveExpandMap/Toggle");
			hideNameFromEnemy = GameUtil.Bind<Toggle>(gameObject, "Communication/Sub/HideNameFromEnemy/Toggle");
			ignoreEmotionFromEnemy =
				GameUtil.Bind<Toggle>(gameObject, "Communication/Sub/IgnoreEmotionFromEnemy/Toggle");
			hideNameFromEnemyFocus = GameUtil.Bind<Image>(hideNameFromEnemy.gameObject, "Focus");
			hideNameFromEnemyText = GameUtil.Bind<Text>(gameObject, "Communication/Sub/HideNameFromEnemy/TXT_SubTitle");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			LoadSetting();
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			RegisterEvent();
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				SetHideNameInteractable(MonoBehaviourInstance<LobbyService>.inst.LobbyState == LobbyState.Ready);
				return;
			}

			SetHideNameInteractable(false);
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			UnRegisterEvent();
		}


		private void LoadSetting()
		{
			extendMinimapMove.isOn = Singleton<LocalSetting>.inst.setting.extendMinimapMove;
			minimapMove.isOn = Singleton<LocalSetting>.inst.setting.minimapMove;
			viewTeamChatting.isOn = Singleton<LocalSetting>.inst.setting.viewTeamChatting;
			viewAllChatting.isOn = Singleton<LocalSetting>.inst.setting.viewAllChatting;
			mouseReverse.isOn = Singleton<LocalSetting>.inst.setting.mouseReverse;
			hideNameFromEnemy.isOn = Singleton<LocalSetting>.inst.setting.hideNameFromEnemy;
			ignoreEmotionFromEnemy.isOn = Singleton<LocalSetting>.inst.setting.ignoreEmotionFromEnemy;
			moveExpandMap.isOn = Singleton<LocalSetting>.inst.setting.moveExpandMap;
		}


		private void RegisterEvent()
		{
			extendMinimapMove.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.extendMinimapMove = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			minimapMove.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.minimapMove = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			viewTeamChatting.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.viewTeamChatting = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			viewAllChatting.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.viewAllChatting = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			mouseReverse.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.mouseReverse = isOn;
				Singleton<LocalSetting>.inst.Save();
				if (!SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
				{
					MonoBehaviourInstance<GameInput>.inst.MouseReverse(isOn);
				}
			});
			hideNameFromEnemy.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.hideNameFromEnemy = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			ignoreEmotionFromEnemy.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.ignoreEmotionFromEnemy = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
			moveExpandMap.onValueChanged.AddListener(delegate(bool isOn)
			{
				Singleton<LocalSetting>.inst.setting.moveExpandMap = isOn;
				Singleton<LocalSetting>.inst.Save();
			});
		}


		private void UnRegisterEvent()
		{
			extendMinimapMove.onValueChanged.RemoveAllListeners();
			minimapMove.onValueChanged.RemoveAllListeners();
			viewTeamChatting.onValueChanged.RemoveAllListeners();
			viewAllChatting.onValueChanged.RemoveAllListeners();
			mouseReverse.onValueChanged.RemoveAllListeners();
			hideNameFromEnemy.onValueChanged.RemoveAllListeners();
			ignoreEmotionFromEnemy.onValueChanged.RemoveAllListeners();
			moveExpandMap.onValueChanged.RemoveAllListeners();
		}


		public void ResetAll()
		{
			MonoBehaviourInstance<Popup>.inst.OneMoreAskPopup.Open(null, OneMoreAskResetOKCallBack, null);
		}


		private void OneMoreAskResetOKCallBack()
		{
			extendMinimapMove.isOn = true;
			minimapMove.isOn = true;
			viewTeamChatting.isOn = true;
			viewAllChatting.isOn = false;
			mouseReverse.isOn = false;
			moveExpandMap.isOn = true;
			if (hideNameFromEnemy.interactable)
			{
				hideNameFromEnemy.isOn = false;
			}

			if (ignoreEmotionFromEnemy.interactable)
			{
				ignoreEmotionFromEnemy.isOn = false;
			}
		}


		public void SetHideNameInteractable(bool interactable)
		{
			hideNameFromEnemy.interactable = interactable;
			hideNameFromEnemyFocus.color = interactable ? Color.white : FIXED_COLOR_FOCUS;
			hideNameFromEnemyText.color = interactable ? Color.white : FIXED_COLOR_TEXT;
		}


		public void OnPointerEnter(BaseEventData eventData)
		{
			PointerEventData pointerEventData = eventData as PointerEventData;
			if (pointerEventData != null)
			{
				Transform parent = pointerEventData.pointerEnter.transform.parent;
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("GameSetting/" + parent.name));
				Vector2 vector = parent.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(-330f, -14f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}
		}


		public void OnPointerExit(BaseEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}