using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShortcutSettingPage : BasePage
	{
		[SerializeField] private List<GameObject> list_MoreShortcut = default;


		[SerializeField] private Image[] imgArrows = default;


		[SerializeField] private Sprite[] arrowSprites = default;


		private readonly string actionPath = "ShortcutScrollRect/Contents/MoreShortcut/More_3";


		private readonly string emotionPath = "ShortcutScrollRect/Contents/MoreShortcut/More_5";


		private readonly string emotionPlatePath = "ShortcutScrollRect/Contents/MoreShortcut/More_5";


		private readonly string interfacePath = "ShortcutScrollRect/Contents/MoreShortcut/More_2";


		private readonly string learnPath = "ShortcutScrollRect/Contents/MoreShortcut/More_1";


		private readonly string markPath = "ShortcutScrollRect/Contents/MoreShortcut/More_4";


		private readonly string quickCastPath = "ShortcutScrollRect/Contents/Shortcut/ShortcutItems/QuickCast/Button";


		private readonly string shortcutItemPath = "ShortcutScrollRect/Contents/Shortcut/ShortcutItems/Item/Button";


		private readonly string shortcutSkillPath = "ShortcutScrollRect/Contents/Shortcut/ShortcutItems/Skill/Button";


		private Dictionary<GameInputEvent, ShortcutButton> castButtonMap;


		private Dictionary<GameInputEvent, Toggle> quickCastToggleMap;


		private Button setAllNormalCastBtn;


		private Button setAllQuickCastBtn;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			setAllQuickCastBtn = GameUtil.Bind<Button>(gameObject, quickCastPath + "/Btn1");
			setAllNormalCastBtn = GameUtil.Bind<Button>(gameObject, quickCastPath + "/Btn2");
			quickCastToggleMap = new Dictionary<GameInputEvent, Toggle>
			{
				{
					GameInputEvent.Active1,
					GameUtil.Bind<Toggle>(gameObject,
						string.Format("{0}/{1}", shortcutSkillPath, GameInputEvent.Active1))
				},
				{
					GameInputEvent.Active2,
					GameUtil.Bind<Toggle>(gameObject,
						string.Format("{0}/{1}", shortcutSkillPath, GameInputEvent.Active2))
				},
				{
					GameInputEvent.Active3,
					GameUtil.Bind<Toggle>(gameObject,
						string.Format("{0}/{1}", shortcutSkillPath, GameInputEvent.Active3))
				},
				{
					GameInputEvent.Active4,
					GameUtil.Bind<Toggle>(gameObject,
						string.Format("{0}/{1}", shortcutSkillPath, GameInputEvent.Active4))
				},
				{
					GameInputEvent.WeaponSkill,
					GameUtil.Bind<Toggle>(gameObject,
						string.Format("{0}/{1}", shortcutSkillPath, GameInputEvent.WeaponSkill))
				},
				{
					GameInputEvent.Alpha1,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha1))
				},
				{
					GameInputEvent.Alpha2,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha2))
				},
				{
					GameInputEvent.Alpha3,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha3))
				},
				{
					GameInputEvent.Alpha4,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha4))
				},
				{
					GameInputEvent.Alpha5,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha5))
				},
				{
					GameInputEvent.Alpha6,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha6))
				},
				{
					GameInputEvent.Alpha7,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha7))
				},
				{
					GameInputEvent.Alpha8,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha8))
				},
				{
					GameInputEvent.Alpha9,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha9))
				},
				{
					GameInputEvent.Alpha0,
					GameUtil.Bind<Toggle>(gameObject, string.Format("{0}/{1}", shortcutItemPath, GameInputEvent.Alpha0))
				}
			};
			castButtonMap = new Dictionary<GameInputEvent, ShortcutButton>
			{
				{
					GameInputEvent.Active1,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutSkillPath, GameInputEvent.Active1))
				},
				{
					GameInputEvent.Active2,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutSkillPath, GameInputEvent.Active2))
				},
				{
					GameInputEvent.Active3,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutSkillPath, GameInputEvent.Active3))
				},
				{
					GameInputEvent.Active4,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutSkillPath, GameInputEvent.Active4))
				},
				{
					GameInputEvent.WeaponSkill,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutSkillPath, GameInputEvent.WeaponSkill))
				},
				{
					GameInputEvent.Alpha1,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha1))
				},
				{
					GameInputEvent.Alpha2,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha2))
				},
				{
					GameInputEvent.Alpha3,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha3))
				},
				{
					GameInputEvent.Alpha4,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha4))
				},
				{
					GameInputEvent.Alpha5,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha5))
				},
				{
					GameInputEvent.Alpha6,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha6))
				},
				{
					GameInputEvent.Alpha7,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha7))
				},
				{
					GameInputEvent.Alpha8,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha8))
				},
				{
					GameInputEvent.Alpha9,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha9))
				},
				{
					GameInputEvent.Alpha0,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}/Button", shortcutItemPath, GameInputEvent.Alpha0))
				},
				{
					GameInputEvent.LearnActive1,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnActive1))
				},
				{
					GameInputEvent.LearnActive2,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnActive2))
				},
				{
					GameInputEvent.LearnActive3,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnActive3))
				},
				{
					GameInputEvent.LearnActive4,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnActive4))
				},
				{
					GameInputEvent.LearnPassive,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnPassive))
				},
				{
					GameInputEvent.LearnWeaponSkill,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", learnPath, GameInputEvent.LearnWeaponSkill))
				},
				{
					GameInputEvent.Escape,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.Escape))
				},
				{
					GameInputEvent.ChangeCameraMode,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.ChangeCameraMode))
				},
				{
					GameInputEvent.OpenCombineWindow,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.OpenCombineWindow))
				},
				{
					GameInputEvent.OpenScoreboard,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.OpenScoreboard))
				},
				{
					GameInputEvent.OpenMap,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.OpenMap))
				},
				{
					GameInputEvent.ResetCamera,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.ResetCamera))
				},
				{
					GameInputEvent.OpenCharacterMastery,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.OpenCharacterMastery))
				},
				{
					GameInputEvent.OpenCharacterStat,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.OpenCharacterStat))
				},
				{
					GameInputEvent.ShowObjectText,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.ShowObjectText))
				},
				{
					GameInputEvent.ShowRouteList,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.ShowRouteList))
				},
				{
					GameInputEvent.MaxChatWindow,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.MaxChatWindow))
				},
				{
					GameInputEvent.MinimapZoomIn,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.MinimapZoomIn))
				},
				{
					GameInputEvent.MinimapZoomOut,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.MinimapZoomOut))
				},
				{
					GameInputEvent.CameraTeam1,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.CameraTeam1))
				},
				{
					GameInputEvent.CameraTeam2,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.CameraTeam2))
				},
				{
					GameInputEvent.CameraTeam3,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.CameraTeam3))
				},
				{
					GameInputEvent.MoveExpandMap,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.MoveExpandMap))
				},
				{
					GameInputEvent.NormalMatchingSolo,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.NormalMatchingSolo))
				},
				{
					GameInputEvent.NormalMatchingDuo,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.NormalMatchingDuo))
				},
				{
					GameInputEvent.NormalMatchingSquad,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.NormalMatchingSquad))
				},
				{
					GameInputEvent.RankMatchingSolo,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.RankMatchingSolo))
				},
				{
					GameInputEvent.RankMatchingDuo,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.RankMatchingDuo))
				},
				{
					GameInputEvent.RankMatchingSquad,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", interfacePath, GameInputEvent.RankMatchingSquad))
				},
				{
					GameInputEvent.Attack,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.Attack))
				},
				{
					GameInputEvent.Stop,
					GameUtil.Bind<ShortcutButton>(gameObject, string.Format("{0}/{1}", actionPath, GameInputEvent.Stop))
				},
				{
					GameInputEvent.Hold,
					GameUtil.Bind<ShortcutButton>(gameObject, string.Format("{0}/{1}", actionPath, GameInputEvent.Hold))
				},
				{
					GameInputEvent.Rest,
					GameUtil.Bind<ShortcutButton>(gameObject, string.Format("{0}/{1}", actionPath, GameInputEvent.Rest))
				},
				{
					GameInputEvent.Reload,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.Reload))
				},
				{
					GameInputEvent.QuickCombine,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.QuickCombine))
				},
				{
					GameInputEvent.ThrowItem,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.ThrowItem))
				},
				{
					GameInputEvent.AddGuide,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.AddGuide))
				},
				{
					GameInputEvent.MoveToAttack,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", actionPath, GameInputEvent.MoveToAttack))
				},
				{
					GameInputEvent.ChangeEnableGameUI,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", markPath, GameInputEvent.ChangeEnableGameUI))
				},
				{
					GameInputEvent.ChangeEnableTrackerName,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", markPath, GameInputEvent.ChangeEnableTrackerName))
				},
				{
					GameInputEvent.ChangeEnableTrackerStatusBar,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", markPath, GameInputEvent.ChangeEnableTrackerStatusBar))
				},
				{
					GameInputEvent.Emotion1,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion1))
				},
				{
					GameInputEvent.Emotion2,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion2))
				},
				{
					GameInputEvent.Emotion3,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion3))
				},
				{
					GameInputEvent.Emotion4,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion4))
				},
				{
					GameInputEvent.Emotion5,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion5))
				},
				{
					GameInputEvent.Emotion6,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.Emotion6))
				},
				{
					GameInputEvent.PingTarget,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.PingTarget))
				},
				{
					GameInputEvent.MarkTarget,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.MarkTarget))
				},
				{
					GameInputEvent.DeleteMarkTarget,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.DeleteMarkTarget))
				},
				{
					GameInputEvent.ChatItem,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPath, GameInputEvent.ChatItem))
				},
				{
					GameInputEvent.EmotionPlate,
					GameUtil.Bind<ShortcutButton>(gameObject,
						string.Format("{0}/{1}", emotionPlatePath, GameInputEvent.EmotionPlate))
				}
			};
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			ResetUI();
			LoadSetting();
			RegisterEvent();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			UnRegisterEvent();
		}


		private void ResetUI()
		{
			foreach (Image image in imgArrows)
			{
				image.sprite = arrowSprites[0];
				image.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}

			foreach (GameObject gameObject in list_MoreShortcut)
			{
				gameObject.SetActive(false);
			}
		}


		public void RegisterEvent()
		{
			setAllQuickCastBtn.onClick.AddListener(delegate
			{
				foreach (Toggle toggle in quickCastToggleMap.Values)
				{
					toggle.isOn = true;
				}

				UpdateSetting();
			});
			setAllNormalCastBtn.onClick.AddListener(delegate
			{
				foreach (Toggle toggle in quickCastToggleMap.Values)
				{
					toggle.isOn = false;
				}

				UpdateSetting();
			});
			using (Dictionary<GameInputEvent, Toggle>.ValueCollection.Enumerator enumerator =
				quickCastToggleMap.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Toggle shortcutKey = enumerator.Current;
					shortcutKey.onValueChanged.AddListener(delegate(bool isOn)
					{
						shortcutKey.isOn = isOn;
						UpdateSetting();
					});
				}
			}

			using (Dictionary<GameInputEvent, ShortcutButton>.Enumerator enumerator2 = castButtonMap.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<GameInputEvent, ShortcutButton> kvp = enumerator2.Current;
					kvp.Value.GetButton().onClick.AddListener(delegate { OpenChangeKeyPopup(kvp.Key); });
					kvp.Value.SetInputEvent(kvp.Key);
				}
			}
		}


		private void OpenChangeKeyPopup(GameInputEvent gameInputEvent)
		{
			if (gameInputEvent.IsFixedKeyEvent())
			{
				return;
			}

			MonoBehaviourInstance<Popup>.inst.ChangeKeyPopup.Open(delegate { UpdateButtonKeys(); }, gameInputEvent);
			MonoBehaviourInstance<Popup>.inst.ChangeKeyPopup.UpdateBoard();
		}


		public void UpdateButtonKeys()
		{
			foreach (KeyValuePair<GameInputEvent, ShortcutButton> keyValuePair in castButtonMap)
			{
				foreach (ShortcutInputEvent shortcutInputEvent in Singleton<LocalSetting>.inst.setting.newCastKeys)
				{
					if (keyValuePair.Key == shortcutInputEvent.gameInputEvent)
					{
						KeyCode[] combinationKeys = shortcutInputEvent.combinationKeys;
						string combi =
							Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(combinationKeys.ToList<KeyCode>());
						List<KeyCode> list = new List<KeyCode>();
						list.Add(shortcutInputEvent.key);
						string key = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
						if (keyValuePair.Key >= GameInputEvent.Active1 &&
						    keyValuePair.Key <= GameInputEvent.WeaponSkill ||
						    keyValuePair.Key >= GameInputEvent.Alpha1 && keyValuePair.Key <= GameInputEvent.Alpha0)
						{
							keyValuePair.Value.SetTextStyleNormal(combi, key);
						}
						else
						{
							keyValuePair.Value.SetTitle(keyValuePair.Key);
							keyValuePair.Value.SetTextStyleMore(combi, key);
						}
					}
				}
			}
		}


		private void UnRegisterEvent()
		{
			setAllQuickCastBtn.onClick.RemoveAllListeners();
			setAllNormalCastBtn.onClick.RemoveAllListeners();
			foreach (Toggle toggle in quickCastToggleMap.Values)
			{
				toggle.onValueChanged.RemoveAllListeners();
			}

			foreach (ShortcutButton shortcutButton in castButtonMap.Values)
			{
				shortcutButton.GetButton().onClick.RemoveAllListeners();
			}
		}


		public void LoadSetting()
		{
			foreach (KeyValuePair<GameInputEvent, bool> keyValuePair in Singleton<LocalSetting>.inst.setting
				.quickCastKeys)
			{
				if (quickCastToggleMap.ContainsKey(keyValuePair.Key))
				{
					quickCastToggleMap[keyValuePair.Key].isOn = keyValuePair.Value;
					quickCastToggleMap[keyValuePair.Key].gameObject.SetActive(false);
					quickCastToggleMap[keyValuePair.Key].gameObject.SetActive(true);
				}
			}

			UpdateButtonKeys();
		}


		public void UpdateSetting()
		{
			foreach (KeyValuePair<GameInputEvent, Toggle> keyValuePair in quickCastToggleMap)
			{
				Singleton<LocalSetting>.inst.setting.quickCastKeys[keyValuePair.Key] = keyValuePair.Value.isOn;
			}

			Singleton<LocalSetting>.inst.Save();
		}


		private void OneMoreAskResetOKCallBack()
		{
			UnRegisterEvent();
			Singleton<LocalSetting>.inst.DefaultKeySetting();
			Singleton<LocalSetting>.inst.Save();
			LoadSetting();
			RegisterEvent();
			if (MonoBehaviourInstance<GameInput>.inst != null && MonoBehaviourInstance<GameUI>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.DefaultRegister();
				MonoBehaviourInstance<GameUI>.inst.SkillHud.SetSkillTextUI();
				MonoBehaviourInstance<GameUI>.inst.StatusHud.SetMasteryKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenCharacterMastery),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenCharacterMastery));
				MonoBehaviourInstance<GameUI>.inst.StatusHud.SetReloadKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.Reload),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.Reload));
				MonoBehaviourInstance<GameUI>.inst.HudButton.SetRestKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.Rest),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.Rest));
				MonoBehaviourInstance<GameUI>.inst.HudButton.SetCameraKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.ChangeCameraMode),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.ChangeCameraMode));
				MonoBehaviourInstance<GameUI>.inst.HudButton.SetCombineWindowKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenCombineWindow),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenCombineWindow));
				MonoBehaviourInstance<GameUI>.inst.HudButton.SetMapKeyCode(
					Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenMap),
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenMap));
				MonoBehaviourInstance<GameUI>.inst.InventoryHud.InvenSlots.ForEach(delegate(InvenItemSlot slot)
				{
					slot.SetInvenSlotKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(slot.GameInputEvent),
						Singleton<LocalSetting>.inst.GetCombinationKeyCode(slot.GameInputEvent));
				});
			}
		}


		public void ClickReset()
		{
			MonoBehaviourInstance<Popup>.inst.OneMoreAskPopup.Open(null, OneMoreAskResetOKCallBack, null);
		}


		private void UpdateMoreUI(int index)
		{
			bool activeSelf = list_MoreShortcut[index].activeSelf;
			list_MoreShortcut[index].SetActive(!activeSelf);
			if (activeSelf)
			{
				imgArrows[index].sprite = arrowSprites[0];
				imgArrows[index].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				return;
			}

			imgArrows[index].sprite = arrowSprites[1];
			imgArrows[index].transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}


		public void ClickMore1()
		{
			UpdateMoreUI(0);
		}


		public void ClickMore2()
		{
			UpdateMoreUI(1);
		}


		public void ClickMore3()
		{
			UpdateMoreUI(2);
		}


		public void ClickMore4()
		{
			UpdateMoreUI(3);
		}


		public void ClickMore5()
		{
			UpdateMoreUI(4);
		}
	}
}