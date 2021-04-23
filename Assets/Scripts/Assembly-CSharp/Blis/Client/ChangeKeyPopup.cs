using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ChangeKeyPopup : BasePopup
	{
		private const int maxKeyCombine = 2;


		public static bool changekeyPopupOpen;


		[SerializeField] private Sprite[] btnSaveSprites = default;


		[SerializeField] private OneMoreAskPopup oneMoreAskPopup = default;


		[SerializeField] private Text txt_Desc = default;


		[SerializeField] private Text txt_Key = default;


		[SerializeField] private Button btn_Save = default;


		private readonly KeyCode[] combineKeyCodes = default;


		private readonly List<KeyCode> currentKeyCodes = default;


		private readonly KeyCode[] overlapKeyCodes = default;


		private GameInputEvent currentGameInputEvent = default;


		private Image img_Save = default;


		private List<KeyCode> originalKeyCodes = default;


		private bool resetInput;


		private Text txt_Save = default;


		private Action updateCallback;


		private void Update()
		{
			if (!IsOpen)
			{
				return;
			}

			if (MonoBehaviourInstance<Popup>.inst.OneMoreAskPopup.IsOpen)
			{
				return;
			}

			foreach (object obj in Enum.GetValues(typeof(KeyCode)))
			{
				KeyCode keyCode = (KeyCode) obj;
				if (!GameInput.IgnoreInputKeyCode(keyCode) && Input.GetKeyDown(keyCode))
				{
					if (resetInput)
					{
						currentKeyCodes.Clear();
						resetInput = false;
					}

					currentKeyCodes.Add(keyCode);
					if (currentKeyCodes.Count == 2)
					{
						if (!combineKeyCodes.Contains(currentKeyCodes[0]))
						{
							currentKeyCodes.RemoveAt(0);
						}
						else if (currentKeyCodes[0] == currentKeyCodes[1])
						{
							currentKeyCodes.RemoveAt(1);
						}
					}
					else if (currentKeyCodes.Count > 2)
					{
						if (combineKeyCodes.Contains(keyCode))
						{
							currentKeyCodes.Clear();
							currentKeyCodes.Add(keyCode);
						}
						else
						{
							currentKeyCodes.RemoveAt(1);
						}
					}

					txt_Key.text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(currentKeyCodes);
					if (currentKeyCodes.Count == 1)
					{
						if (combineKeyCodes.Contains(keyCode))
						{
							SetButtonStyle(false);
							return;
						}
					}
					else if (currentKeyCodes.Count == 2 && overlapKeyCodes.Contains(keyCode))
					{
						resetInput = true;
						SetButtonStyle(false);
						return;
					}

					if (EqualKeyCodeList(originalKeyCodes, currentKeyCodes))
					{
						resetInput = true;
						SetButtonStyle(false);
						return;
					}

					SetButtonStyle(true);
				}
			}

			CheckResult();
		}

		private void Ref()
		{
			Reference.Use(oneMoreAskPopup);
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			enableBackShadeEvent = false;
			txt_Save = GameUtil.Bind<Text>(btn_Save.gameObject, "TXT_Save");
			GameUtil.Bind<Image>(btn_Save.gameObject, ref img_Save);
		}


		public void Open(Action updateCallback, GameInputEvent gameInputEvent)
		{
			base.Open();
			this.updateCallback = updateCallback;
			currentGameInputEvent = gameInputEvent;
			originalKeyCodes = Singleton<LocalSetting>.inst.GetKeyCodeList(gameInputEvent);
			currentKeyCodes.Clear();
			changekeyPopupOpen = true;
			SetButtonStyle(false);
		}


		public void UpdateBoard()
		{
			txt_Desc.text = Ln.Get(string.Format("Shortcut/{0}", currentGameInputEvent));
			txt_Key.text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(originalKeyCodes);
		}


		private void SetButtonStyle(bool enableSave)
		{
			if (enableSave)
			{
				txt_Save.color = Color.white;
				img_Save.sprite = btnSaveSprites[1];
				btn_Save.enabled = true;
				return;
			}

			txt_Save.color = new Color(0.678f, 0.678f, 0.678f, 1f);
			img_Save.sprite = btnSaveSprites[0];
			btn_Save.enabled = false;
		}


		private void CheckResult()
		{
			if (Input.anyKey)
			{
				return;
			}

			resetInput = true;
			if (EqualKeyCodeList(originalKeyCodes, currentKeyCodes))
			{
				SetButtonStyle(false);
				return;
			}

			if (CheckOpenOnemorePopup())
			{
				SetButtonStyle(false);
				return;
			}

			if (CheckOverlap())
			{
				SetButtonStyle(false);
				return;
			}

			if (currentKeyCodes.Count == 0)
			{
				return;
			}

			if (currentKeyCodes.Count == 1 && combineKeyCodes.Contains(currentKeyCodes[0]))
			{
				SetButtonStyle(false);
				return;
			}

			txt_Key.text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(currentKeyCodes);
		}


		private bool CheckOverlap()
		{
			int num = 0;
			foreach (KeyCode value in currentKeyCodes)
			{
				if (combineKeyCodes.Contains(value))
				{
					num++;
				}
			}

			return num == 2;
		}


		private bool EqualKeyCodeList(List<KeyCode> keyCodes1, List<KeyCode> keyCodes2)
		{
			int num = 0;
			foreach (KeyCode keyCode in keyCodes1)
			{
				foreach (KeyCode keyCode2 in keyCodes2)
				{
					if (keyCode == keyCode2)
					{
						num++;
						break;
					}
				}
			}

			return keyCodes1.Count == num && keyCodes1.Count == keyCodes2.Count;
		}


		private bool CheckOpenOnemorePopup()
		{
			using (List<ShortcutInputEvent>.Enumerator enumerator =
				Singleton<LocalSetting>.inst.setting.newCastKeys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ShortcutInputEvent sci = enumerator.Current;
					List<KeyCode> keyCodeList = Singleton<LocalSetting>.inst.GetKeyCodeList(sci.gameInputEvent);
					if (EqualKeyCodeList(keyCodeList, currentKeyCodes))
					{
						SetButtonStyle(false);
						MonoBehaviourInstance<Popup>.inst.OneMoreAskPopup.Open(sci,
							delegate { OneMoreAskOkCallBack(sci.gameInputEvent); }, OneMoreAskCancelCallBack);
						return true;
					}
				}
			}

			return false;
		}


		private void OneMoreAskOkCallBack(GameInputEvent gameInputEvent)
		{
			Singleton<LocalSetting>.inst.ClearShortcutInputEvent(gameInputEvent);
			SettingSuccess(true, gameInputEvent);
			Close();
		}


		private void OneMoreAskCancelCallBack()
		{
			currentKeyCodes.Clear();
			foreach (KeyCode item in originalKeyCodes)
			{
				currentKeyCodes.Add(item);
			}

			txt_Key.text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(currentKeyCodes);
		}


		private void SettingSuccess(bool existRemoveFlag = false, GameInputEvent existInputEvent = GameInputEvent.None)
		{
			if (currentKeyCodes.Count == 0)
			{
				return;
			}

			KeyCode keyCode = Singleton<LocalSetting>.inst.GetKeyCode(currentGameInputEvent);
			KeyCode[] combinationKeyCode = Singleton<LocalSetting>.inst.GetCombinationKeyCode(currentGameInputEvent);
			Singleton<LocalSetting>.inst.ClearShortcutInputEvent(existInputEvent);
			Singleton<LocalSetting>.inst.ClearShortcutInputEvent(currentGameInputEvent);
			ShortcutInputEvent shortcutInputEvent =
				Singleton<LocalSetting>.inst.GetShortcutInputEvent(currentGameInputEvent);
			if (currentKeyCodes.Count == 1 && currentKeyCodes[0] == KeyCode.LeftAlt)
			{
				shortcutInputEvent.key = KeyCode.LeftAlt;
			}
			else
			{
				foreach (KeyCode keyCode2 in currentKeyCodes)
				{
					if (combineKeyCodes.Contains(keyCode2))
					{
						shortcutInputEvent.combinationKeys = new[]
						{
							keyCode2
						};
					}
					else
					{
						shortcutInputEvent.key = keyCode2;
					}
				}
			}

			Singleton<LocalSetting>.inst.Save();
			if (MonoBehaviourInstance<GameInput>.inst != null && MonoBehaviourInstance<GameUI>.inst != null)
			{
				KeyCode keyCode3 = Singleton<LocalSetting>.inst.GetKeyCode(currentGameInputEvent);
				KeyCode[] combinationKeyCode2 =
					Singleton<LocalSetting>.inst.GetCombinationKeyCode(currentGameInputEvent);
				MonoBehaviourInstance<GameInput>.inst.ChangeKeyboardEvent(currentGameInputEvent, keyCode,
					combinationKeyCode, keyCode3, combinationKeyCode2, existRemoveFlag, existInputEvent);
				switch (currentGameInputEvent)
				{
					case GameInputEvent.Active1:
					case GameInputEvent.Active2:
					case GameInputEvent.Active3:
					case GameInputEvent.Active4:
					case GameInputEvent.WeaponSkill:
						MonoBehaviourInstance<GameUI>.inst.SkillHud.SetSkillTextUI();
						break;
					case GameInputEvent.OpenMap:
						MonoBehaviourInstance<GameUI>.inst.HudButton.SetMapKeyCode(keyCode3, combinationKeyCode2);
						break;
					case GameInputEvent.OpenCharacterMastery:
						MonoBehaviourInstance<GameUI>.inst.StatusHud.SetMasteryKeyCode(keyCode3, combinationKeyCode2);
						break;
					case GameInputEvent.OpenCombineWindow:
						MonoBehaviourInstance<GameUI>.inst.HudButton.SetCombineWindowKeyCode(keyCode3,
							combinationKeyCode2);
						break;
					case GameInputEvent.ChangeCameraMode:
						MonoBehaviourInstance<GameUI>.inst.HudButton.SetCameraKeyCode(keyCode3, combinationKeyCode2);
						break;
					case GameInputEvent.Alpha1:
					case GameInputEvent.Alpha2:
					case GameInputEvent.Alpha3:
					case GameInputEvent.Alpha4:
					case GameInputEvent.Alpha5:
					case GameInputEvent.Alpha6:
					case GameInputEvent.Alpha7:
					case GameInputEvent.Alpha8:
					case GameInputEvent.Alpha9:
					case GameInputEvent.Alpha0:
						MonoBehaviourInstance<GameUI>.inst.InventoryHud.InvenSlots
							.Find(x => x.GameInputEvent == currentGameInputEvent)
							.SetInvenSlotKeyCode(keyCode3, combinationKeyCode2);
						break;
					case GameInputEvent.Rest:
						MonoBehaviourInstance<GameUI>.inst.HudButton.SetRestKeyCode(keyCode3, combinationKeyCode2);
						break;
					case GameInputEvent.Reload:
						MonoBehaviourInstance<GameUI>.inst.StatusHud.SetReloadKeyCode(keyCode3, combinationKeyCode2);
						break;
				}
			}

			if (updateCallback != null)
			{
				updateCallback();
			}
		}


		public void ClickSave()
		{
			if (Input.anyKey)
			{
				return;
			}

			SettingSuccess();
			Close();
		}


		public void ClickCancel()
		{
			Close();
		}


		public new void Close()
		{
			base.Close();
			currentKeyCodes.Clear();
			changekeyPopupOpen = false;
			resetInput = true;
		}
	}
}