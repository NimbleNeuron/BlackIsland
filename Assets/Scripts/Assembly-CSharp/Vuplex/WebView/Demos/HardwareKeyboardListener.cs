using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vuplex.WebView.Demos
{
	
	internal class HardwareKeyboardListener : MonoBehaviour
	{
		
		
		
		[Obsolete("The HardwareKeyboardListener.InputReceived event is now deprecated. Please use the HardwareKeyboardListener.KeyDownReceived event instead.")]
		public event EventHandler<KeyboardInputEventArgs> InputReceived
		{
			add
			{
				this.KeyDownReceived += value;
			}
			remove
			{
				this.KeyDownReceived -= value;
			}
		}

		
		
		
		public event EventHandler<KeyboardInputEventArgs> KeyDownReceived;

		
		
		
		public event EventHandler<KeyboardInputEventArgs> KeyUpReceived;

		
		public static HardwareKeyboardListener Instantiate()
		{
			return new GameObject("HardwareKeyboardListener").AddComponent<HardwareKeyboardListener>();
		}

		
		private bool _areKeysUndetectableThroughInputStringPressed()
		{
			foreach (string keyValue in HardwareKeyboardListener._keyValuesUndetectableThroughInputString)
			{
				if (Input.GetKey(this._getUnityKeyNameForJsKeyValue(keyValue)))
				{
					return true;
				}
			}
			return false;
		}

		
		private KeyModifier _getModifiers()
		{
			KeyModifier keyModifier = KeyModifier.None;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				keyModifier |= KeyModifier.Shift;
			}
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				keyModifier |= KeyModifier.Control;
			}
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
			{
				keyModifier |= KeyModifier.Alt;
			}
			if (Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows))
			{
				keyModifier |= KeyModifier.Meta;
			}
			return keyModifier;
		}

		
		private string _getUnityKeyNameForJsKeyValue(string keyValue)
		{
			if (keyValue == " ")
			{
				return "space";
			}
			if (keyValue == "ArrowUp")
			{
				return "up";
			}
			if (keyValue == "ArrowDown")
			{
				return "down";
			}
			if (keyValue == "ArrowRight")
			{
				return "right";
			}
			if (!(keyValue == "ArrowLeft"))
			{
				return keyValue.ToLower();
			}
			return "left";
		}

		
		private void _processKeysReleased(KeyModifier modifiers)
		{
			if (this._keysDown.Count == 0)
			{
				return;
			}
			foreach (string text in new List<string>(this._keysDown))
			{
				bool flag = false;
				try
				{
					flag = Input.GetKeyUp(this._getUnityKeyNameForJsKeyValue(text));
				}
				catch (ArgumentException arg)
				{
					Debug.LogError("Invalid key value passed to Input.GetKeyUp: " + arg);
					this._keysDown.Remove(text);
					break;
				}
				if (flag)
				{
					EventHandler<KeyboardInputEventArgs> keyUpReceived = this.KeyUpReceived;
					if (keyUpReceived != null)
					{
						keyUpReceived(this, new KeyboardInputEventArgs(text, modifiers));
					}
					this._keysDown.Remove(text);
				}
			}
		}

		
		private void _processKeysPressed(KeyModifier modifiers)
		{
			if (!Input.anyKeyDown && Input.inputString.Length <= 0)
			{
				return;
			}
			EventHandler<KeyboardInputEventArgs> keyDownReceived = this.KeyDownReceived;
			bool flag = modifiers != KeyModifier.None && modifiers != KeyModifier.Shift;
			bool flag2 = this._areKeysUndetectableThroughInputStringPressed();
			if (flag || flag2)
			{
				foreach (string text in HardwareKeyboardListener._keyValues)
				{
					if (Input.GetKeyDown(this._getUnityKeyNameForJsKeyValue(text)))
					{
						if (keyDownReceived != null)
						{
							keyDownReceived(this, new KeyboardInputEventArgs(text, modifiers));
						}
						this._keysDown.Add(text);
					}
				}
				return;
			}
			string inputString = Input.inputString;
			int i = 0;
			while (i < inputString.Length)
			{
				char c = inputString[i];
				string text2;
				if (c <= '\n')
				{
					if (c != '\b')
					{
						if (c != '\n')
						{
							goto IL_D0;
						}
						goto IL_BE;
					}
					else
					{
						text2 = "Backspace";
					}
				}
				else
				{
					if (c == '\r')
					{
						goto IL_BE;
					}
					if (c != '')
					{
						goto IL_D0;
					}
					text2 = "Delete";
				}
				IL_D9:
				if (keyDownReceived != null)
				{
					keyDownReceived(this, new KeyboardInputEventArgs(text2, modifiers));
				}
				this._keysDown.Add(text2);
				i++;
				continue;
				IL_BE:
				text2 = "Enter";
				goto IL_D9;
				IL_D0:
				text2 = c.ToString();
				goto IL_D9;
			}
		}

		
		private void Update()
		{
			KeyModifier modifiers = this._getModifiers();
			this._processKeysPressed(modifiers);
			this._processKeysReleased(modifiers);
		}

		
		private List<string> _keysDown = new List<string>();

		
		private static readonly string[] _keyValues = new string[]
		{
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"i",
			"j",
			"k",
			"l",
			"m",
			"n",
			"o",
			"p",
			"q",
			"r",
			"s",
			"t",
			"u",
			"v",
			"w",
			"x",
			"y",
			"z",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"0",
			"`",
			"-",
			"=",
			"[",
			"]",
			"\\",
			";",
			"'",
			",",
			".",
			"/",
			" ",
			"Enter",
			"Backspace",
			"Tab",
			"ArrowUp",
			"ArrowDown",
			"ArrowRight",
			"ArrowLeft"
		};

		
		private static readonly string[] _keyValuesUndetectableThroughInputString = new string[]
		{
			"Tab",
			"ArrowUp",
			"ArrowDown",
			"ArrowRight",
			"ArrowLeft"
		};
	}
}
