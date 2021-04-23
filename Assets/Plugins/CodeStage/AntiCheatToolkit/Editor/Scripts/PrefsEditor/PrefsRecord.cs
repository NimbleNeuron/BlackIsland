namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;
	using Common;
	using ObscuredTypes;
	using Storage;
	using UnityEditor;
	using UnityEngine;
	using Utils;

	[Serializable]
	internal class PrefsRecord
	{
		protected const string DefaultString = "[^_; = ElinaKristinaMyGirlsLoveYou'16 = ;_^]";
		protected const float DefaultFloat = float.MinValue + 2016.0122f;
		protected const int DefaultInt = int.MinValue + 20130524;
		protected const string CantDecrypt = "Can't decrypt with specified key";

		internal PrefsType prefType = PrefsType.Unknown;
		internal ObscuredPrefs.DataType obscuredType = ObscuredPrefs.DataType.Unknown;

		internal bool dirtyKey;
		internal bool dirtyValue;

		[SerializeField]
		private string savedKey;

		[SerializeField]
		private string key;

		internal string Key
		{
			get { return key; }
			set
			{
				if (value == key) return;
				key = value;

				dirtyKey = true;
			}
		}

		[SerializeField]
		private string stringValue;

		internal string StringValue
		{
			get { return stringValue; }
			set
			{
				if (value == stringValue) return;

				stringValue = value;
				dirtyValue = true;
			}
		}

		[SerializeField]
		private int intValue;

		internal int IntValue
		{
			get { return intValue; }
			set
			{
				if (value == intValue) return;

				intValue = value;
				dirtyValue = true;
			}
		}

		[SerializeField]
		private float floatValue;

		internal float FloatValue
		{
			get { return floatValue; }
			set
			{
				if (Math.Abs(value - floatValue) < 0.0000001f) return;

				floatValue = value;
				dirtyValue = true;
			}
		}

		internal string DisplayValue
		{
			get
			{
				switch (prefType)
				{
					case PrefsType.Unknown:
						return ACTkPrefsEditor.UnknownValueDescription;
					case PrefsType.String:
						return IsEditableObscuredValue() || !Obscured ? stringValue : ACTkPrefsEditor.UnsupportedValueDescription;
					case PrefsType.Int:
						return intValue.ToString();
					case PrefsType.Float:
						return floatValue.ToString(CultureInfo.InvariantCulture);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		internal string DisplayType
		{
			get { return Obscured ? obscuredType.ToString() : prefType.ToString(); }
		}

		internal static int SortByNameAscending(PrefsRecord n1, PrefsRecord n2)
		{
			return string.CompareOrdinal(n1.key, n2.key);
		}

		internal static int SortByNameDescending(PrefsRecord n1, PrefsRecord n2)
		{
			var result = string.CompareOrdinal(n2.key, n1.key);
			return result;
		}

		internal static int SortByType(PrefsRecord n1, PrefsRecord n2)
		{
			var result = string.CompareOrdinal(n1.DisplayType, n2.DisplayType);
			if (result == 0)
			{
				return SortByNameAscending(n1, n2);
			}

			return result;
		}

		internal static int SortByObscurance(PrefsRecord n1, PrefsRecord n2)
		{
			var result = n1.Obscured.CompareTo(n2.Obscured);

			if (result == 0)
			{
				return SortByNameAscending(n1, n2);
			}

			return result;
		}

		internal bool Obscured { get; set; }

		internal PrefsRecord(string newKey, string value, bool encrypted)
		{
			key = savedKey = newKey;
			stringValue = value;

			prefType = PrefsType.String;

			if (encrypted)
			{
				obscuredType = ObscuredPrefs.DataType.String;
				Obscured = true;
			}
		}

		internal PrefsRecord(string newKey, int value, bool encrypted)
		{
			key = savedKey = newKey;
			intValue = value;

			if (encrypted)
			{
				prefType = PrefsType.String;
				obscuredType = ObscuredPrefs.DataType.Int;
				Obscured = true;
			}
			else
			{
				prefType = PrefsType.Int;
			}
		}

		internal PrefsRecord(string newKey, float value, bool encrypted)
		{
			key = savedKey = newKey;
			floatValue = value;

			if (encrypted)
			{
				prefType = PrefsType.String;
				obscuredType = ObscuredPrefs.DataType.Float;
				Obscured = true;
			}
			else
			{
				prefType = PrefsType.Float;
			}
		}

		internal PrefsRecord(string originalKey)
		{
			key = savedKey = originalKey;

			ReadValue();

			// only string prefs may be obscured
			if (prefType == PrefsType.String)
			{
				Obscured = IsValueObscured(stringValue);

				if (Obscured)
				{
					key = DecryptKey(key);

					if (obscuredType == ObscuredPrefs.DataType.String)
					{
						stringValue = ObscuredPrefs.DecryptValue(key, null, DefaultString, stringValue);
						if (stringValue == DefaultString) stringValue = CantDecrypt;
					}
					else if (obscuredType == ObscuredPrefs.DataType.Int)
					{
						intValue = ObscuredPrefs.DecryptValue(key, null, DefaultInt, stringValue);
						if (intValue == DefaultInt)
						{
							obscuredType = ObscuredPrefs.DataType.String;
							stringValue = CantDecrypt;
						}
					}
					else if (obscuredType == ObscuredPrefs.DataType.Float)
					{
						floatValue = ObscuredPrefs.DecryptValue(key, null, DefaultFloat, stringValue);
						if (Math.Abs(floatValue - DefaultFloat) < 0.00001f)
						{
							obscuredType = ObscuredPrefs.DataType.String;
							stringValue = CantDecrypt;
						}
					}
				}
			}
		}

		internal bool Save(bool newRecord = false)
		{
			var savedString = stringValue;
			string newSavedKey;

			if (Obscured)
			{
				savedString = GetEncryptedValue();
				newSavedKey = GetEncryptedKey();
			}
			else
			{
				newSavedKey = key;
			}

			if (newSavedKey != savedKey && PlayerPrefs.HasKey(newSavedKey))
			{
				if (!EditorUtility.DisplayDialog("Pref overwrite",
					"Pref with name " + key + " already exists!\n" + "Are you sure you wish to overwrite it?", "Yes",
					"No"))
				{
					return false;
				}
			}

			if (dirtyKey)
			{
				PlayerPrefs.DeleteKey(savedKey);
			}

			switch (prefType)
			{
				case PrefsType.Unknown:
					Debug.LogError(ACTkConstants.LogPrefix + "Can't save Pref of unknown type!");
					break;
				case PrefsType.String:
					PlayerPrefs.SetString(newSavedKey, savedString);
					break;
				case PrefsType.Int:
					PlayerPrefs.SetInt(newSavedKey, intValue);
					break;
				case PrefsType.Float:
					PlayerPrefs.SetFloat(newSavedKey, floatValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			savedKey = newSavedKey;

			dirtyKey = false;
			dirtyValue = false;

			PlayerPrefs.Save();

			return true;
		}

		internal void Delete()
		{
			PlayerPrefs.DeleteKey(savedKey);
			PlayerPrefs.Save();
		}

		internal void Encrypt()
		{
			if (Obscured) return;

			var success = true;

			switch (prefType)
			{
				case PrefsType.Unknown:
					success = false;
					Debug.LogError(ACTkConstants.LogPrefix + "Can't encrypt pref of unknown type!");
					break;
				case PrefsType.String:
					obscuredType = ObscuredPrefs.DataType.String;
					break;
				case PrefsType.Int:
					obscuredType = ObscuredPrefs.DataType.Int;
					break;
				case PrefsType.Float:
					obscuredType = ObscuredPrefs.DataType.Float;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (success)
			{
				prefType = PrefsType.String;
				Obscured = true;
				dirtyValue = dirtyKey = true;
			}
		}

		internal void Decrypt()
		{
			if (!Obscured) return;
			if (!IsEditableObscuredValue()) return;

			var success = true;

			switch (obscuredType)
			{
				case ObscuredPrefs.DataType.Int:
					prefType = PrefsType.Int;
					break;
				case ObscuredPrefs.DataType.String:
					prefType = PrefsType.String;
					break;
				case ObscuredPrefs.DataType.Float:
					prefType = PrefsType.Float;
					break;
				case ObscuredPrefs.DataType.UInt:
				case ObscuredPrefs.DataType.Double:
				case ObscuredPrefs.DataType.Long:
				case ObscuredPrefs.DataType.Bool:
				case ObscuredPrefs.DataType.ByteArray:
				case ObscuredPrefs.DataType.Vector2:
				case ObscuredPrefs.DataType.Vector3:
				case ObscuredPrefs.DataType.Quaternion:
				case ObscuredPrefs.DataType.Color:
				case ObscuredPrefs.DataType.Rect:
					ACTkPrefsEditor.instance.ShowNotification(new GUIContent("Type " + obscuredType + " isn't supported"));
					success = false;
					break;
				case ObscuredPrefs.DataType.Unknown:
					ACTkPrefsEditor.instance.ShowNotification(new GUIContent("Can't decrypt " + key));
					success = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (success)
			{
				Obscured = false;
				obscuredType = ObscuredPrefs.DataType.Unknown;
				dirtyValue = dirtyKey = true;
			}
		}

		internal string GetEncryptedKey()
		{
			return ObscuredPrefs.EncryptKey(key);
		}

		internal string GetEncryptedValue()
		{
			string savedString;

			switch (obscuredType)
			{
				case ObscuredPrefs.DataType.Int:
					savedString = ObscuredPrefs.EncryptValue(key, intValue);
					break;
				case ObscuredPrefs.DataType.String:
					savedString = ObscuredPrefs.EncryptValue(key, stringValue);
					break;
				case ObscuredPrefs.DataType.Float:
					savedString = ObscuredPrefs.EncryptValue(key, floatValue);
					break;
				case ObscuredPrefs.DataType.Unknown:
				case ObscuredPrefs.DataType.UInt:
				case ObscuredPrefs.DataType.Double:
				case ObscuredPrefs.DataType.Long:
				case ObscuredPrefs.DataType.Bool:
				case ObscuredPrefs.DataType.ByteArray:
				case ObscuredPrefs.DataType.Vector2:
				case ObscuredPrefs.DataType.Vector3:
				case ObscuredPrefs.DataType.Quaternion:
				case ObscuredPrefs.DataType.Color:
				case ObscuredPrefs.DataType.Rect:
				case ObscuredPrefs.DataType.Decimal:
				case ObscuredPrefs.DataType.ULong:
					savedString = stringValue;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return savedString;
		}

		internal bool IsEditableObscuredValue()
		{
			return (obscuredType == ObscuredPrefs.DataType.Int) || (obscuredType == ObscuredPrefs.DataType.String) ||
			       (obscuredType == ObscuredPrefs.DataType.Float);
		}

		internal string ToString(bool raw = false)
		{
			string result;

			if (raw)
			{
				result = "Key: " + GetEncryptedKey() + Environment.NewLine + "Value: " + GetEncryptedValue();
			}
			else
			{
				result = "Key: " + key + Environment.NewLine + "Value: " + DisplayValue;
			}

			return result;
		}

		private void ReadValue()
		{
			var stringTry = PlayerPrefs.GetString(key, DefaultString);
			if (stringTry != DefaultString)
			{
				prefType = PrefsType.String;
				stringValue = stringTry;
				return;
			}

			var floatTry = PlayerPrefs.GetFloat(key, DefaultFloat);
			if (Math.Abs(floatTry - DefaultFloat) > 0.0000001f)
			{
				prefType = PrefsType.Float;
				floatValue = floatTry;
				return;
			}

			var intTry = PlayerPrefs.GetInt(key, DefaultInt);
			if (intTry != DefaultInt)
			{
				prefType = PrefsType.Int;
				intValue = intTry;
			}
		}

		private string DecryptKey(string encryptedKey)
		{
			string decryptedKey;

			try
			{
				var decryptedKeyChars = Base64Utils.FromBase64ToChars(encryptedKey);
				decryptedKey = ObscuredString.Decrypt(decryptedKeyChars, ObscuredPrefs.GetCryptoKey());
			}
			catch
			{
				decryptedKey = string.Empty;
			}

			return decryptedKey;
		}

		private bool IsValueObscured(string value)
		{
			var validBase64String = (value.Length % 4 == 0) &&
			                        Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
			if (!validBase64String) return false;

			var dataType = ObscuredPrefs.GetRawValueType(value);
			if (!Enum.IsDefined(typeof(ObscuredPrefs.DataType), dataType) || dataType == ObscuredPrefs.DataType.Unknown)
			{
				return false;
			}

			obscuredType = dataType;

			return true;
		}

		internal enum PrefsType : byte
		{
			Unknown,
			String,
			Int,
			Float
		}
	}
}