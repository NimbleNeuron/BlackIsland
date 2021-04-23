using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Blis.Client
{
	public class GameInput : MonoBehaviourInstance<GameInput>
	{
		public delegate void KeyPressed(GameInputEvent inputEvent, Vector3 mousePosition);


		public delegate void KeyPressing(GameInputEvent inputEvent, Vector3 mousePosition);


		public delegate void KeyReleased(GameInputEvent inputEvent, Vector3 mousePosition);


		public delegate void MousePressed(GameInputEvent inputEvent, Vector3 mousePosition);


		public delegate void MousePressing(GameInputEvent inputEvent, Vector3 mousePosition);


		public delegate void MouseReleased(GameInputEvent inputEvent, Vector3 mousePressedPosition,
			Vector3 mousePosition);


		private readonly Dictionary<KeyCode, GameInputEvent> curDownInputEvents =
			new Dictionary<KeyCode, GameInputEvent>(SingletonComparerEnum<KeyCodeComparer, KeyCode>.Instance);


		private readonly Dictionary<KeyCode, List<InputEventCombination>> keyboardEvents =
			new Dictionary<KeyCode, List<InputEventCombination>>(SingletonComparerEnum<KeyCodeComparer, KeyCode>
				.Instance);


		private readonly MouseButton[] MouseButtons = (MouseButton[]) Enum.GetValues(typeof(MouseButton));


		private readonly Dictionary<MouseButton, List<InputEventCombination>> mouseEvents =
			new Dictionary<MouseButton, List<InputEventCombination>>(
				SingletonComparerEnum<MouseButtonComparer, MouseButton>.Instance);


		private readonly List<KeyCode> releaseKeysCache = new List<KeyCode>();


		private GameCursor gameCursor;


		private bool isActive = true;


		private bool isLockInputChat;


		private bool isLockInputSerach;


		private GameInputEvent lastEvent;


		private Vector3 mousePressedPosition;


		public GameCursor GameCursor => gameCursor;


		public GameInputEvent LastEvent => lastEvent;


		private void Update()
		{
			if (!isActive)
			{
				return;
			}

			GameInputEvent gameInputEvent = GetMouseEvent();
			if (EventSystem.current != null && SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene &&
			    EventSystem.current.IsPointerOverGameObject() && gameInputEvent != lastEvent)
			{
				gameInputEvent = GameInputEvent.None;
			}

			if (gameInputEvent != GameInputEvent.None)
			{
				if (lastEvent == gameInputEvent)
				{
					OnMousePressing(gameInputEvent, GetMousePosition());
				}
				else
				{
					if (lastEvent != GameInputEvent.None)
					{
						OnMouseReleased(lastEvent, mousePressedPosition, GetMousePosition());
					}

					mousePressedPosition = GetMousePosition();
					OnMousePressed(gameInputEvent, mousePressedPosition);
				}
			}
			else if (lastEvent != GameInputEvent.None)
			{
				OnMouseReleased(lastEvent, mousePressedPosition, GetMousePosition());
			}

			Vector3 mousePosition = GetMousePosition();
			CheckKeyPressedEvent(mousePosition);
			CheckKeyPressingAndReleaseEvent(mousePosition);
			lastEvent = gameInputEvent;
		}

		
		
		public event MousePressed OnMousePressed = delegate { };


		
		
		public event MousePressing OnMousePressing = delegate { };


		
		
		public event MouseReleased OnMouseReleased = delegate { };


		
		
		public event KeyPressed OnKeyPressed = delegate { };


		
		
		public event KeyPressing OnKeyPressing = delegate { };


		
		
		public event KeyReleased OnKeyRelease = delegate { };


		public void SetLockInputChat(bool lockInputChat)
		{
			isLockInputChat = lockInputChat;
		}


		public void SetLockInputSearch(bool lockInputSerach)
		{
			isLockInputSerach = lockInputSerach;
		}


		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
		}


		private void Register(InputEventCombination inputEventCombination)
		{
			if (inputEventCombination.type == InputEventCombination.Type.Mouse)
			{
				List<InputEventCombination> list = mouseEvents.ContainsKey(inputEventCombination.mouseButton)
					? mouseEvents[inputEventCombination.mouseButton]
					: null;
				if (list == null)
				{
					list = new List<InputEventCombination>();
					mouseEvents.Add(inputEventCombination.mouseButton, list);
				}

				list.Add(inputEventCombination);
				return;
			}

			if (inputEventCombination.type == InputEventCombination.Type.Keyboard)
			{
				List<InputEventCombination> list2 = keyboardEvents.ContainsKey(inputEventCombination.key)
					? keyboardEvents[inputEventCombination.key]
					: null;
				if (list2 == null)
				{
					list2 = new List<InputEventCombination>();
					keyboardEvents.Add(inputEventCombination.key, list2);
				}

				if (inputEventCombination.combinationKeys != null && inputEventCombination.combinationKeys.Length != 0)
				{
					list2.Insert(0, inputEventCombination);
					return;
				}

				list2.Add(inputEventCombination);
			}
		}


		private void RemoveRegisterKeyBoard(GameInputEvent gameInputEvent, KeyCode prevKey,
			InputEventCombination inputEventCombination)
		{
			if (inputEventCombination.type == InputEventCombination.Type.Keyboard)
			{
				foreach (KeyValuePair<KeyCode, List<InputEventCombination>> keyValuePair in keyboardEvents)
				{
					if (prevKey == keyValuePair.Key)
					{
						if (keyValuePair.Value.Count > 1)
						{
							int index = 0;
							for (int i = 0; i < keyValuePair.Value.Count; i++)
							{
								if (keyValuePair.Value[i].gameInputEvent == gameInputEvent)
								{
									index = i;
								}
							}

							keyValuePair.Value.RemoveAt(index);
							break;
						}

						keyboardEvents.Remove(prevKey);
						break;
					}
				}
			}
		}


		private void RemoveRegisterMouseButton(GameInputEvent gameInputEvent, MouseButton prevMouseButton,
			InputEventCombination.Type type)
		{
			if (type == InputEventCombination.Type.Mouse)
			{
				foreach (KeyValuePair<MouseButton, List<InputEventCombination>> keyValuePair in mouseEvents)
				{
					if (prevMouseButton == keyValuePair.Key)
					{
						if (keyValuePair.Value.Count == 1)
						{
							if (keyValuePair.Value[0].gameInputEvent == gameInputEvent)
							{
								mouseEvents.Remove(prevMouseButton);
							}
						}
						else if (keyValuePair.Value.Count > 1)
						{
							int index = 0;
							for (int i = 0; i < keyValuePair.Value.Count; i++)
							{
								if (keyValuePair.Value[i].gameInputEvent == gameInputEvent)
								{
									index = i;
								}
							}

							keyValuePair.Value.RemoveAt(index);
						}

						break;
					}
				}
			}
		}


		private void MouseEvent(GameInputEvent gameInputEvent, MouseButton mouseButton,
			params KeyCode[] combinationKeys)
		{
			Register(new InputEventCombination(gameInputEvent, InputEventCombination.Type.Mouse, mouseButton,
				KeyCode.None, combinationKeys));
		}


		public void MouseReverse(bool reverse)
		{
			if (reverse)
			{
				RemoveRegisterMouseButton(GameInputEvent.Select, MouseButton.LeftMouse,
					InputEventCombination.Type.Mouse);
				RemoveRegisterMouseButton(GameInputEvent.Move, MouseButton.RightMouse,
					InputEventCombination.Type.Mouse);
				RemoveRegisterMouseButton(GameInputEvent.PingTarget, MouseButton.LeftMouse,
					InputEventCombination.Type.Mouse);
				RemoveRegisterMouseButton(GameInputEvent.MarkTarget, MouseButton.LeftMouse,
					InputEventCombination.Type.Mouse);
				MouseEvent(GameInputEvent.MarkTarget, MouseButton.RightMouse, KeyCode.LeftControl);
				MouseEvent(GameInputEvent.PingTarget, MouseButton.RightMouse, KeyCode.LeftAlt);
				MouseEvent(GameInputEvent.PingTarget, MouseButton.MiddleMouse, KeyCode.LeftAlt);
				MouseEvent(GameInputEvent.Select, MouseButton.RightMouse, Array.Empty<KeyCode>());
				MouseEvent(GameInputEvent.Move, MouseButton.LeftMouse, Array.Empty<KeyCode>());
				return;
			}

			RemoveRegisterMouseButton(GameInputEvent.Select, MouseButton.RightMouse, InputEventCombination.Type.Mouse);
			RemoveRegisterMouseButton(GameInputEvent.Move, MouseButton.LeftMouse, InputEventCombination.Type.Mouse);
			RemoveRegisterMouseButton(GameInputEvent.PingTarget, MouseButton.RightMouse,
				InputEventCombination.Type.Mouse);
			RemoveRegisterMouseButton(GameInputEvent.MarkTarget, MouseButton.RightMouse,
				InputEventCombination.Type.Mouse);
			MouseEvent(GameInputEvent.MarkTarget, MouseButton.LeftMouse, KeyCode.LeftControl);
			MouseEvent(GameInputEvent.PingTarget, MouseButton.LeftMouse, KeyCode.LeftAlt);
			MouseEvent(GameInputEvent.PingTarget, MouseButton.MiddleMouse, KeyCode.LeftAlt);
			MouseEvent(GameInputEvent.Select, MouseButton.LeftMouse, Array.Empty<KeyCode>());
			MouseEvent(GameInputEvent.Move, MouseButton.RightMouse, Array.Empty<KeyCode>());
		}


		protected override void _Awake()
		{
			isActive = true;
			gameCursor = gameObject.AddComponent<GameCursor>();
			DefaultRegister();
			curDownInputEvents.Clear();
		}


		public void DefaultRegister()
		{
			mouseEvents.Clear();
			keyboardEvents.Clear();
			MouseEvent(GameInputEvent.Warp, MouseButton.RightMouse, KeyCode.LeftShift);
			MouseEvent(GameInputEvent.OpenMap, MouseButton.MiddleMouse, Array.Empty<KeyCode>());
			MouseReverse(Singleton<LocalSetting>.inst.setting.mouseReverse);
			SetKeyboard(GameInputEvent.Active1);
			SetKeyboard(GameInputEvent.Active2);
			SetKeyboard(GameInputEvent.Active3);
			SetKeyboard(GameInputEvent.Active4);
			SetKeyboard(GameInputEvent.WeaponSkill);
			SetKeyboard(GameInputEvent.LearnActive1);
			SetKeyboard(GameInputEvent.LearnActive2);
			SetKeyboard(GameInputEvent.LearnActive3);
			SetKeyboard(GameInputEvent.LearnActive4);
			SetKeyboard(GameInputEvent.LearnPassive);
			SetKeyboard(GameInputEvent.LearnWeaponSkill);
			SetKeyboard(GameInputEvent.ChangeCameraMode);
			SetKeyboard(GameInputEvent.OpenCombineWindow);
			SetKeyboard(GameInputEvent.OpenScoreboard);
			SetKeyboard(GameInputEvent.OpenMap);
			SetKeyboard(GameInputEvent.ResetCamera);
			SetKeyboard(GameInputEvent.OpenCharacterMastery);
			SetKeyboard(GameInputEvent.OpenCharacterStat);
			SetKeyboard(GameInputEvent.ShowObjectText);
			SetKeyboard(GameInputEvent.ShowRouteList);
			SetKeyboard(GameInputEvent.Attack);
			SetKeyboard(GameInputEvent.Stop);
			SetKeyboard(GameInputEvent.Hold);
			SetKeyboard(GameInputEvent.Rest);
			SetKeyboard(GameInputEvent.Reload);
			SetKeyboard(GameInputEvent.QuickCombine);
			SetKeyboard(GameInputEvent.MoveToAttack);
			SetKeyboard(GameInputEvent.CameraTeam1);
			SetKeyboard(GameInputEvent.CameraTeam2);
			SetKeyboard(GameInputEvent.CameraTeam3);
			SetKeyboard(GameInputEvent.MoveExpandMap);
			SetKeyboard(GameInputEvent.ChangeEnableGameUI);
			SetKeyboard(GameInputEvent.ChangeEnableTrackerName);
			SetKeyboard(GameInputEvent.ChangeEnableTrackerStatusBar);
			SetKeyboard(GameInputEvent.Emotion1);
			SetKeyboard(GameInputEvent.Emotion2);
			SetKeyboard(GameInputEvent.Emotion3);
			SetKeyboard(GameInputEvent.Emotion4);
			SetKeyboard(GameInputEvent.Emotion5);
			SetKeyboard(GameInputEvent.Emotion6);
			SetKeyboard(GameInputEvent.Alpha0);
			SetKeyboard(GameInputEvent.Alpha1);
			SetKeyboard(GameInputEvent.Alpha2);
			SetKeyboard(GameInputEvent.Alpha3);
			SetKeyboard(GameInputEvent.Alpha4);
			SetKeyboard(GameInputEvent.Alpha5);
			SetKeyboard(GameInputEvent.Alpha6);
			SetKeyboard(GameInputEvent.Alpha7);
			SetKeyboard(GameInputEvent.Alpha8);
			SetKeyboard(GameInputEvent.Alpha9);
			SetKeyboard(GameInputEvent.ChatActive);
			SetKeyboard(GameInputEvent.ChatActive2);
			SetKeyboard(GameInputEvent.AllChatActive);
			SetKeyboard(GameInputEvent.MaxChatWindow);
			SetKeyboard(GameInputEvent.ObserveNextPlayer);
			SetKeyboard(GameInputEvent.ObservePreviousPlayer);
			SetKeyboard(GameInputEvent.MinimapZoomIn);
			SetKeyboard(GameInputEvent.MinimapZoomOut);
			SetKeyboard(GameInputEvent.DeleteMarkTarget);
			SetKeyboard(GameInputEvent.EmotionPlate);
			KeyboardEvent(GameInputEvent.Escape, KeyCode.Escape, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.NormalMatchingSolo, KeyCode.F5, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.NormalMatchingDuo, KeyCode.F6, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.NormalMatchingSquad, KeyCode.F7, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.RankMatchingSolo, KeyCode.F9, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.RankMatchingDuo, KeyCode.F10, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.RankMatchingSquad, KeyCode.F11, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.UpArrow, KeyCode.UpArrow, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.DownArrow, KeyCode.DownArrow, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.RightArrow, KeyCode.RightArrow, Array.Empty<KeyCode>());
			KeyboardEvent(GameInputEvent.LeftArrow, KeyCode.LeftArrow, Array.Empty<KeyCode>());
		}


		private void SetKeyboard(GameInputEvent gameInputEvent)
		{
			KeyboardEvent(gameInputEvent, Singleton<LocalSetting>.inst.GetKeyCode(gameInputEvent),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(gameInputEvent));
		}


		private void KeyboardEvent(GameInputEvent gameInputEvent, KeyCode key, params KeyCode[] combinationKeys)
		{
			Register(new InputEventCombination(gameInputEvent, InputEventCombination.Type.Keyboard,
				MouseButton.LeftMouse, key, combinationKeys));
		}


		public void ChangeKeyboardEvent(GameInputEvent gameInputEvent, KeyCode prevKey, KeyCode[] prevCombinationKeys,
			KeyCode key, KeyCode[] combinationKeys, bool ExistRemoveFlag = false,
			GameInputEvent existInputEvent = GameInputEvent.None)
		{
			if (ExistRemoveFlag)
			{
				RemoveRegisterKeyBoard(existInputEvent, key,
					new InputEventCombination(existInputEvent, InputEventCombination.Type.Keyboard,
						MouseButton.LeftMouse, key, combinationKeys));
			}

			RemoveRegisterKeyBoard(gameInputEvent, prevKey,
				new InputEventCombination(gameInputEvent, InputEventCombination.Type.Keyboard, MouseButton.LeftMouse,
					prevKey, prevCombinationKeys));
			Register(new InputEventCombination(gameInputEvent, InputEventCombination.Type.Keyboard,
				MouseButton.LeftMouse, key, combinationKeys));
		}


		public Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}


		private bool CheckCombination(InputEventCombination combination)
		{
			if (combination.combinationKeys.Length == 0)
			{
				return true;
			}

			KeyCode[] combinationKeys = combination.combinationKeys;
			for (int i = 0; i < combinationKeys.Length; i++)
			{
				if (!Input.GetKey(combinationKeys[i]))
				{
					return false;
				}
			}

			return true;
		}


		public KeyCode GetKeyCode(GameInputEvent gameInputEvent)
		{
			foreach (KeyValuePair<KeyCode, List<InputEventCombination>> keyValuePair in keyboardEvents)
			{
				foreach (InputEventCombination inputEventCombination in keyValuePair.Value)
				{
					if (inputEventCombination.gameInputEvent == gameInputEvent)
					{
						return inputEventCombination.key;
					}
				}
			}

			return KeyCode.None;
		}


		public bool CheckCombination(GameInputEvent gameInputEvent)
		{
			foreach (KeyValuePair<KeyCode, List<InputEventCombination>> keyValuePair in keyboardEvents)
			{
				foreach (InputEventCombination inputEventCombination in keyValuePair.Value)
				{
					if (inputEventCombination.gameInputEvent == gameInputEvent &&
					    inputEventCombination.combinationKeys.Length != 0)
					{
						return true;
					}
				}
			}

			return false;
		}


		private GameInputEvent GetMouseEvent()
		{
			foreach (MouseButton mouseButton in MouseButtons)
			{
				List<InputEventCombination> list;
				if (Input.GetMouseButton((int) mouseButton) && mouseEvents.TryGetValue(mouseButton, out list))
				{
					foreach (InputEventCombination inputEventCombination in list)
					{
						if (CheckCombination(inputEventCombination))
						{
							return inputEventCombination.gameInputEvent;
						}
					}
				}
			}

			return GameInputEvent.None;
		}


		private void CheckKeyPressedEvent(Vector3 mousePosition)
		{
			foreach (KeyValuePair<KeyCode, List<InputEventCombination>> keyValuePair in keyboardEvents)
			{
				List<InputEventCombination> list;
				if (Input.GetKeyDown(keyValuePair.Key) && keyboardEvents.TryGetValue(keyValuePair.Key, out list))
				{
					GameInputEvent gameInputEvent = GameInputEvent.None;
					bool flag = false;
					foreach (InputEventCombination inputEventCombination in list)
					{
						if (!isLockInputChat || inputEventCombination.gameInputEvent.IsChatEvent())
						{
							if (isLockInputSerach)
							{
								if (inputEventCombination.gameInputEvent == GameInputEvent.Escape)
								{
									isLockInputSerach = false;
								}
							}
							else if (inputEventCombination.combinationKeys.Length == 0)
							{
								gameInputEvent = inputEventCombination.gameInputEvent;
							}
							else if (CheckCombination(inputEventCombination))
							{
								curDownInputEvents[keyValuePair.Key] = inputEventCombination.gameInputEvent;
								OnKeyPressed(curDownInputEvents[keyValuePair.Key], mousePosition);
								flag = true;
								break;
							}
						}
					}

					if (!flag && gameInputEvent != GameInputEvent.None)
					{
						curDownInputEvents[keyValuePair.Key] = gameInputEvent;
						OnKeyPressed(curDownInputEvents[keyValuePair.Key], mousePosition);
					}
				}
			}
		}


		private void CheckKeyPressingAndReleaseEvent(Vector3 mousePosition)
		{
			releaseKeysCache.Clear();
			foreach (KeyValuePair<KeyCode, GameInputEvent> keyValuePair in curDownInputEvents)
			{
				if (keyValuePair.Key == KeyCode.None)
				{
					releaseKeysCache.Add(keyValuePair.Key);
				}
				else if (!Input.GetKey(keyValuePair.Key))
				{
					releaseKeysCache.Add(keyValuePair.Key);
					OnKeyRelease(keyValuePair.Value, mousePosition);
				}
				else
				{
					OnKeyPressing(keyValuePair.Value, mousePosition);
				}
			}

			foreach (KeyCode key in releaseKeysCache)
			{
				curDownInputEvents.Remove(key);
			}
		}


		public GameInputEvent GetCastslotIndexToInputEvent(SkillSlotIndex skillSlotIndex)
		{
			switch (skillSlotIndex)
			{
				case SkillSlotIndex.Active1:
					return GameInputEvent.Active1;
				case SkillSlotIndex.Active2:
					return GameInputEvent.Active2;
				case SkillSlotIndex.Active3:
					return GameInputEvent.Active3;
				case SkillSlotIndex.Active4:
					return GameInputEvent.Active4;
				case SkillSlotIndex.WeaponSkill:
					return GameInputEvent.WeaponSkill;
				default:
					return GameInputEvent.None;
			}
		}


		public bool IsKeyHoldEvent(GameInputEvent gameInputEvent)
		{
			foreach (KeyValuePair<KeyCode, List<InputEventCombination>> keyValuePair in keyboardEvents)
			{
				List<InputEventCombination> list;
				if (Input.GetKey(keyValuePair.Key) && keyboardEvents.TryGetValue(keyValuePair.Key, out list))
				{
					foreach (InputEventCombination inputEventCombination in list)
					{
						if (inputEventCombination.gameInputEvent == gameInputEvent)
						{
							if (CheckCombination(inputEventCombination))
							{
								return true;
							}

							return false;
						}
					}
				}
			}

			return false;
		}


		public static bool IgnoreInputKeyCode(KeyCode keyCode)
		{
			if (keyCode != KeyCode.Pause && keyCode != KeyCode.Escape)
			{
				switch (keyCode)
				{
					case KeyCode.Keypad0:
					case KeyCode.Keypad1:
					case KeyCode.Keypad2:
					case KeyCode.Keypad3:
					case KeyCode.Keypad4:
					case KeyCode.Keypad5:
					case KeyCode.Keypad6:
					case KeyCode.Keypad7:
					case KeyCode.Keypad8:
					case KeyCode.Keypad9:
					case KeyCode.KeypadDivide:
					case KeyCode.KeypadMultiply:
					case KeyCode.KeypadEnter:
					case KeyCode.UpArrow:
					case KeyCode.DownArrow:
					case KeyCode.RightArrow:
					case KeyCode.LeftArrow:
					case KeyCode.F5:
					case KeyCode.F6:
					case KeyCode.F7:
					case KeyCode.F8:
					case KeyCode.F9:
					case KeyCode.F10:
					case KeyCode.F11:
					case KeyCode.F12:
					case KeyCode.Numlock:
					case KeyCode.ScrollLock:
					case KeyCode.RightShift:
					case KeyCode.RightControl:
					case KeyCode.RightAlt:
					case KeyCode.RightCommand:
					case KeyCode.LeftCommand:
					case KeyCode.SysReq:
					case KeyCode.Menu:
					case KeyCode.Mouse0:
					case KeyCode.Mouse1:
					case KeyCode.Mouse2:
						return true;
				}

				return false;
			}

			return true;
		}


		private class InputEventCombination
		{
			public enum Type
			{
				Keyboard,

				Mouse
			}


			public readonly KeyCode[] combinationKeys;


			public readonly GameInputEvent gameInputEvent;


			public readonly KeyCode key;


			public readonly MouseButton mouseButton;


			public readonly Type type;

			public InputEventCombination(GameInputEvent gameInputEvent, Type type, MouseButton mouseButton, KeyCode key,
				KeyCode[] combinationKeys)
			{
				this.gameInputEvent = gameInputEvent;
				this.type = type;
				this.mouseButton = mouseButton;
				this.key = key;
				this.combinationKeys = combinationKeys;
			}
		}


		public class MouseButtonComparer : SingletonComparerEnum<MouseButtonComparer, MouseButton>
		{
			public override bool Equals(MouseButton x, MouseButton y)
			{
				return x == y;
			}


			public override int GetHashCode(MouseButton obj)
			{
				return (int) obj;
			}
		}


		public class KeyCodeComparer : SingletonComparerEnum<KeyCodeComparer, KeyCode>
		{
			public override bool Equals(KeyCode x, KeyCode y)
			{
				return x == y;
			}


			public override int GetHashCode(KeyCode obj)
			{
				return (int) obj;
			}
		}
	}
}