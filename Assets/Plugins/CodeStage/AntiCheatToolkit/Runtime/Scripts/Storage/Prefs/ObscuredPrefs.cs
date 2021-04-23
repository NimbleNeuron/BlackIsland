#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Storage
{
	using Common;
	using Utils;

	using System;
	using ObscuredTypes;
	using UnityEngine;

	/// <summary>
	/// This is an Obscured analogue of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/PlayerPrefs.html">PlayerPrefs</a> class.
	/// </summary>
	/// Saves data in encrypted state, optionally locking it to the current device.<br/>
	/// Automatically encrypts PlayerPrefs on first read (auto migration), has tampering detection and more.
	public static partial class ObscuredPrefs
	{
		// JFF: MD2 for "ElonShotMarsWithACar" (yes, MD2, not MD5)
		internal const string PrefsKey = "9978e9f39c218d674463dab9dc728bd6";
		private const string RawNotFound = "{not_found}";
		private const string FinalLogPrefix = ACTkConstants.LogPrefix + "ObscuredPrefs: ";
		private const byte Version = 3;

		private static bool alterationReported;
		private static bool foreignSavesReported;
		private static string deviceId;

		private static string cryptoKeyObsolete = "e806f6";
		private static string cryptoKeyObsoleteForMigration;

		[Obsolete("Custom crypto key is now obsolete, use only for data recovery from prefs saved with previous version. " +
		          "This property will be removed in future versions.")]
		public static string CryptoKey
		{
			set { cryptoKeyObsolete = value; }
			get { return cryptoKeyObsolete; }
		}

		/// <summary>
		/// Allows to get current device ID or set custom device ID to lock saves to the device.
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly All data saved with previous device ID will be considered foreign!</strong>
		/// \sa lockToDevice
		public static string DeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(deviceId))
				{
					deviceId = GetDeviceId();
				}
				return deviceId;
			}

			set { deviceId = value; }
		}

		internal static uint deviceIdHash;
		internal static uint DeviceIdHash
		{
			get
			{
				if (deviceIdHash == 0)
				{
					deviceIdHash = CalculateChecksum(DeviceId);
				}
				return deviceIdHash;
			}
		}

		/// <summary>
		/// Allows reacting on saves alteration. May be helpful for banning potential cheaters.
		/// </summary>
		/// Fires only once.
		public static event Action OnAlterationDetected;

		/// <summary>
		/// Allows reacting on detection of possible saves from some other device. 
		/// </summary>
		/// May be helpful to ban potential cheaters, trying to use someone's purchased in-app goods for example.<br/>
		/// May fire on same device in case cheater manipulates saved data in some special way.<br/>
		/// Fires only once.
		/// 
		/// <strong>\htmlonly<font color="7030A0">NOTE:</font>\endhtmlonly May be called if same device ID was changed (pretty rare case though).</strong>
		public static event Action OnPossibleForeignSavesDetected;

		/// <summary>
		/// Allows saving original PlayerPrefs values while migrating to ObscuredPrefs.
		/// </summary>
		/// In such case, original value still will be readable after switching from PlayerPrefs to 
		/// ObscuredPrefs and it should be removed manually as it became unneeded.<br/>
		/// Original PlayerPrefs value will be automatically removed after read by default.
		public static bool preservePlayerPrefs = false;

		/// <summary>
		/// Allows locking saved data to the current device.
		/// </summary>
		/// Use it to prevent cheating via 100% game saves sharing or sharing purchased in-app items for example.<br/>
		/// Set to \link ObscuredPrefs::Soft DeviceLockLevel.Soft \endlink to allow reading of not locked data.<br/>
		/// Set to \link ObscuredPrefs::Strict DeviceLockLevel.Strict \endlink to disallow reading of not locked data (any not locked data will be lost).<br/>
		/// Set to \link ObscuredPrefs::None DeviceLockLevel.None \endlink to disable data lock feature and to read both previously locked and not locked data.<br/>
		/// Read more in #DeviceLockLevel description.
		/// 
		/// Relies on <a href="http://docs.unity3d.com/Documentation/ScriptReference/SystemInfo-deviceUniqueIdentifier.html">SystemInfo.deviceUniqueIdentifier</a>.
		/// Please note, it may change in some rare cases, so one day all locked data may became inaccessible on same device, and here comes #emergencyMode and #readForeignSaves to rescue.<br/>
		/// 
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly On iOS use at your peril! There is no reliable way to get persistent device ID on iOS. So avoid using it or use in conjunction with ForceLockToDeviceInit() to set own device ID (e.g. user email).<br/></strong>
		/// <strong>\htmlonly<font color="7030A0">NOTE #1:</font>\endhtmlonly On iOS it tries to receive vendorIdentifier in first place, to avoid device id change while updating from iOS6 to iOS7. It leads to device ID change while updating from iOS5, but such case is lot rarer.<br/></strong>
		/// <strong>\htmlonly<font color="7030A0">NOTE #2:</font>\endhtmlonly You may use own device id via #DeviceId property. It may be useful to lock saves to the specified email for example.<br/></strong>
		/// <strong>\htmlonly<font color="7030A0">NOTE #3:</font>\endhtmlonly Main thread may lock up for a noticeable time while obtaining device ID first time on some devices (~ sec on my PC)! Consider using ForceLockToDeviceInit() to prevent undesirable behavior in such cases.</strong>
		/// \sa readForeignSaves, emergencyMode, ForceLockToDeviceInit(), DeviceId
		public static DeviceLockLevel lockToDevice = DeviceLockLevel.None;

		/// <summary>
		/// Allows reading saves locked to other device. #OnPossibleForeignSavesDetected still will be fired.
		/// </summary>
		/// \sa lockToDevice, emergencyMode
		public static bool readForeignSaves = false;

		/// <summary>
		/// Allows ignoring #lockToDevice to recover saved data in case of some unexpected issues, like unique device ID change for the same device.<br/>
		/// Similar to readForeignSaves, but doesn't fires #OnPossibleForeignSavesDetected on foreign saves detection.
		/// </summary>
		/// \sa lockToDevice, readForeignSaves
		public static bool emergencyMode = false;

		/// <summary>
		/// Allows forcing device id obtaining on demand. Otherwise, it will be obtained automatically on first usage.
		/// </summary>
		/// Device id obtaining process may be noticeably slow when called first time on some devices.<br/>
		/// This method allows you to force this process at comfortable time (while splash screen is showing for example).
		/// \sa lockToDevice
		public static void ForceLockToDeviceInit()
		{
			if (deviceIdHash == 0)
			{
				deviceId = GetDeviceId();
				deviceIdHash = CalculateChecksum(deviceId);
			}
			else
			{
				Debug.LogWarning(FinalLogPrefix + "ForceLockToDeviceInit() is called, but device ID is already obtained!");
			}
		}

		/// <summary>
		/// Allows to set the raw encrypted key and value.
		/// </summary>
		public static void SetRawValue(string encryptedKey, string encryptedValue)
		{
			PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}

		/// <summary>
		/// Allows to get the raw encrypted key and value for the specified key.
		/// </summary>
		/// <returns>True if key was found and false otherwise.</returns>
		public static bool GetRawValue(string key, out string encryptedKey, out string encryptedValue)
		{
			encryptedValue = null;
			encryptedKey = EncryptKey(key);

			if (!PlayerPrefs.HasKey(encryptedKey))
			{
				return false;
			}

			encryptedValue = PlayerPrefs.GetString(encryptedKey);
			return true;
		}

		/// <summary>
		/// Returns true if <c>key</c> exists in the ObscuredPrefs or in regular PlayerPrefs.
		/// </summary>
		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(EncryptKey(key));
		}

		/// <summary>
		/// Removes <c>key</c> and its corresponding value from the ObscuredPrefs and regular PlayerPrefs.
		/// </summary>
		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(EncryptKey(key));
			if (!preservePlayerPrefs) PlayerPrefs.DeleteKey(key);
		}

		/// <summary>
		/// Removes saved crypto key. Use only when you wish to completely remove all obscured prefs!
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Any existing obscured prefs will be lost after this action.</strong>
		public static void DeleteCryptoKey()
		{
			PlayerPrefs.DeleteKey(PrefsKey);
			
			generatedCryptoKey = null;
			deviceIdHash = 0;
		}

		/// <summary>
		/// Removes all keys and values from the preferences, including anything saved with regular PlayerPrefs. <strong>Use with caution!</strong>
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Please use ObscuredPrefs.DeleteAll() to remove all prefs instead of PlayerPrefs.DeleteAll() to properly clear internals and avoid any data loss when saving new obscured prefs after DeleteAll() call.</strong>
		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
			
			generatedCryptoKey = null;
			deviceIdHash = 0;
		}

		/// <summary>
		/// Writes all modified preferences to disk.
		/// </summary>
		/// By default, Unity writes preferences to disk on Application Quit.<br/>
		/// In case when the game crashes or otherwise prematurely exits, you might want to write the preferences at sensible 'checkpoints' in your game.<br/>
		/// This function will write to disk potentially causing a small hiccup, therefore it is not recommended to call during actual game play.
		public static void Save()
		{
			PlayerPrefs.Save();
		}

		#region int
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static int GetInt(string key, int defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);

			if (!PlayerPrefs.HasKey(encryptedKey))
			{
				if (PlayerPrefs.HasKey(key))
				{
					var unencrypted = PlayerPrefs.GetInt(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetInt(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
				MigrateFromACTkV1Internal(key, cryptoKeyObsolete);
			}

			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region uint
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetUInt(string key, uint value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static uint GetUInt(string key, uint defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region string
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetString(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			if (value == null)
			{
				value = string.Empty;
			}

			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static string GetString(string key, string defaultValue = "")
		{
			var encryptedKey = EncryptKey(key);

			if (!PlayerPrefs.HasKey(encryptedKey))
			{
				if (PlayerPrefs.HasKey(key))
				{
					var unencrypted = PlayerPrefs.GetString(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetString(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
				MigrateFromACTkV1Internal(key, cryptoKeyObsolete);
			}

			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region float
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static float GetFloat(string key, float defaultValue = 0f)
		{
			var encryptedKey = EncryptKey(key);

			if (!PlayerPrefs.HasKey(encryptedKey))
			{
				if (PlayerPrefs.HasKey(key))
				{
					var unencrypted = PlayerPrefs.GetFloat(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetFloat(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
				MigrateFromACTkV1Internal(key, cryptoKeyObsolete);
			}

			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region double
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetDouble(string key, double value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static double GetDouble(string key, double defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region decimal
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetDecimal(string key, decimal value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static decimal GetDecimal(string key, decimal defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region long
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetLong(string key, long value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static long GetLong(string key, long defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region ulong
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetULong(string key, ulong value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static ulong GetULong(string key, ulong defaultValue = 0)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region bool
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static bool GetBool(string key, bool defaultValue = false)
		{
			var encryptedKey = EncryptKey(key);
			return DecryptValue(key, encryptedKey, defaultValue);
		}
		#endregion

		#region byte[]
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetByteArray(string key, byte[] value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptByteArrayValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>byte[defaultLength]</c> filled with <c>defaultValue</c>.
		/// </summary>
		public static byte[] GetByteArray(string key, byte defaultValue = 0, int defaultLength = 0)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);

			if (encrypted == RawNotFound)
			{
				return ConstructByteArray(defaultValue, defaultLength);
			}

			return DecryptByteArrayValue(key, encrypted, defaultValue, defaultLength);
		}

		private static string EncryptByteArrayValue(string key, byte[] value)
		{
			return EncryptData(key, value, DataType.ByteArray);
		}

		private static byte[] DecryptByteArrayValue(string key, string encryptedInput, byte defaultValue, int defaultLength)
		{
			var cleanBytes = DecryptData(key, encryptedInput);
			return cleanBytes ?? ConstructByteArray(defaultValue, defaultLength);
		}

		private static byte[] ConstructByteArray(byte value, int length)
		{
			var bytes = new byte[length];
			for (var i = 0; i < length; i++)
			{
				bytes[i] = value;
			}
			return bytes;
		}
		#endregion

		#region Vector2
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetVector2(string key, Vector2 value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptVector2Value(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return Vector2.zero.
		/// </summary>
		public static Vector2 GetVector2(string key)
		{
			return GetVector2(key, Vector2.zero);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Vector2 GetVector2(string key, Vector2 defaultValue)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);
			return encrypted == RawNotFound ? defaultValue : DecryptVector2Value(key, encrypted, defaultValue);
		}

		private static string EncryptVector2Value(string key, Vector2 value)
		{
			var cleanBytes = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, cleanBytes, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, cleanBytes, 4, 4);
			return EncryptData(key, cleanBytes, DataType.Vector2);
		}

		private static Vector2 DecryptVector2Value(string key, string encryptedInput, Vector2 defaultValue)
		{
			var cleanBytes = DecryptData(key, encryptedInput);
			if (cleanBytes == null)
			{
				return defaultValue;
			}

			Vector2 cleanValue;
			cleanValue.x = BitConverter.ToSingle(cleanBytes, 0);
			cleanValue.y = BitConverter.ToSingle(cleanBytes, 4);
			return cleanValue;
		}
		#endregion

		#region Vector3
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetVector3(string key, Vector3 value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptVector3Value(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return Vector3.zero.
		/// </summary>
		public static Vector3 GetVector3(string key)
		{
			return GetVector3(key, Vector3.zero);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);
			return encrypted == RawNotFound ? defaultValue : DecryptVector3Value(key, encrypted, defaultValue);
		}

		private static string EncryptVector3Value(string key, Vector3 value)
		{
			var cleanBytes = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, cleanBytes, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, cleanBytes, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, cleanBytes, 8, 4);
			return EncryptData(key, cleanBytes, DataType.Vector3);
		}

		private static Vector3 DecryptVector3Value(string key, string encryptedInput, Vector3 defaultValue)
		{
			var cleanBytes = DecryptData(key, encryptedInput);
			if (cleanBytes == null)
			{
				return defaultValue;
			}

			Vector3 cleanValue;
			cleanValue.x = BitConverter.ToSingle(cleanBytes, 0);
			cleanValue.y = BitConverter.ToSingle(cleanBytes, 4);
			cleanValue.z = BitConverter.ToSingle(cleanBytes, 8);
			return cleanValue;
		}
		#endregion

		#region Quaternion
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetQuaternion(string key, Quaternion value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptQuaternionValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return Quaternion.identity.
		/// </summary>
		public static Quaternion GetQuaternion(string key)
		{
			return GetQuaternion(key, Quaternion.identity);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);
			return encrypted == RawNotFound ? defaultValue : DecryptQuaternionValue(key, encrypted, defaultValue);
		}

		private static string EncryptQuaternionValue(string key, Quaternion value)
		{
			var cleanBytes = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, cleanBytes, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, cleanBytes, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, cleanBytes, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, cleanBytes, 12, 4);
			return EncryptData(key, cleanBytes, DataType.Quaternion);
		}

		private static Quaternion DecryptQuaternionValue(string key, string encryptedInput, Quaternion defaultValue)
		{
			var cleanBytes = DecryptData(key, encryptedInput);
			if (cleanBytes == null)
			{
				return defaultValue;
			}

			Quaternion cleanValue;
			cleanValue.x = BitConverter.ToSingle(cleanBytes, 0);
			cleanValue.y = BitConverter.ToSingle(cleanBytes, 4);
			cleanValue.z = BitConverter.ToSingle(cleanBytes, 8);
			cleanValue.w = BitConverter.ToSingle(cleanBytes, 12);
			return cleanValue;
		}
		#endregion

		#region Color
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetColor(string key, Color32 value)
		{
			var encodedColor = (uint)((value.a << 24) | (value.r << 16) | (value.g << 8) | value.b);
			PlayerPrefs.SetString(EncryptKey(key), EncryptColorValue(key, encodedColor));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return Color.black.
		/// </summary>
		public static Color32 GetColor(string key)
		{
			return GetColor(key, new Color32(0,0,0,1));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Color32 GetColor(string key, Color32 defaultValue)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);
			if (encrypted == RawNotFound)
			{
				return defaultValue;
			}

			var encodedColor = DecryptValue(key, encryptedKey, 16777216u, encrypted); // 16777216u == Color32(0,0,0,1);
			return DecodeColor(encodedColor);
		}

		private static string EncryptColorValue(string key, uint value)
		{
			var cleanBytes = BitConverter.GetBytes(value);
			return EncryptData(key, cleanBytes, DataType.Color);
		}
		#endregion

		#region Rect
		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetRect(string key, Rect value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptRectValue(key, value));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return (0,0,0,0) rect.
		/// </summary>
		public static Rect GetRect(string key)
		{
			return GetRect(key, new Rect(0,0,0,0));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Rect GetRect(string key, Rect defaultValue)
		{
			var encryptedKey = EncryptKey(key);
			var encrypted = GetEncryptedPrefsString(key, encryptedKey);
			return encrypted == RawNotFound ? defaultValue : DecryptRectValue(key, encrypted, defaultValue);
		}

		private static string EncryptRectValue(string key, Rect value)
		{
			var cleanBytes = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, cleanBytes, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, cleanBytes, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.width), 0, cleanBytes, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.height), 0, cleanBytes, 12, 4);
			return EncryptData(key, cleanBytes, DataType.Rect);
		}

		private static Rect DecryptRectValue(string key, string encryptedInput, Rect defaultValue)
		{
			var cleanBytes = DecryptData(key, encryptedInput);
			if (cleanBytes == null)
			{
				return defaultValue;
			}

			var cleanValue = new Rect
			{
				x = BitConverter.ToSingle(cleanBytes, 0),
				y = BitConverter.ToSingle(cleanBytes, 4),
				width = BitConverter.ToSingle(cleanBytes, 8),
				height = BitConverter.ToSingle(cleanBytes, 12)
			};
			return cleanValue;
		}
		#endregion

		/// <summary>
		/// Use to migrate ACTk v1.* prefs to the newer format.
		/// </summary>
		/// <param name="key">Prefs key you wish to migrate.</param>
		/// <param name="cryptoKey">Custom crypto key you used for ObscuredPrefs, if any.
		/// Don't use this argument to utilize default key from ACTk v1.</param>
		/// <returns>True if migration was successful, false otherwise.</returns>
		public static bool MigrateFromACTkV1(string key, string cryptoKey = "e806f6")
		{
			return MigrateFromACTkV1Internal(key, cryptoKey);
		}

		/// <summary>
		/// Use to encrypt ACTkv1's value key for later use with SetRawValue to let it migrate.
		/// </summary>
		/// <param name="key">Prefs key.</param>
		/// <param name="cryptoKey">Crypto key you used with ACTk v1, if any.</param>
		/// <returns>Prefs key, encrypted with old ACTk v1 encryption.</returns>
		public static string EncryptKeyWithACTkV1Algorithm(string key, string cryptoKey = "e806f6")
		{
			return Base64Utils.ToBase64(ObscuredString.EncryptDecryptObsolete(key, cryptoKey));
		}

		private static void SavesTampered()
		{
			if (OnAlterationDetected != null && !alterationReported)
			{
				alterationReported = true;
				OnAlterationDetected.Invoke();
			}
		}

		private static void PossibleForeignSavesDetected()
		{
			if (OnPossibleForeignSavesDetected != null && !foreignSavesReported)
			{
				foreignSavesReported = true;
				OnPossibleForeignSavesDetected.Invoke();
			}
		}

		private static string GetDeviceId()
		{
			var id = "";
#if UNITY_IPHONE
			id = UnityEngine.iOS.Device.vendorIdentifier;
#endif

#if !ACTK_PREVENT_READ_PHONE_STATE
			if (string.IsNullOrEmpty(id)) id = SystemInfo.deviceUniqueIdentifier;
#else
			Debug.LogError(FinalLogPrefix + "Looks like you forced ACTK_PREVENT_READ_PHONE_STATE flag, but still use LockToDevice feature. It will work incorrect!");
#endif
			return id;
		}

		internal enum DataType: byte
		{
			Unknown = 0,
			Int = 5,
			UInt = 10,
			String = 15,
			Float = 20,
			Double = 25,
			Decimal = 27,
			Long = 30,
			ULong = 32,
			Bool = 35,
			ByteArray = 40,
			Vector2 = 45,
			Vector3 = 50,
			Quaternion = 55,
			Color = 60,
			Rect = 65,
		}

		/// <summary>
		/// Used to specify level of the device lock feature strictness.
		/// </summary>
		public enum DeviceLockLevel : byte
		{
			/// <summary>
			/// Both locked and not locked to any device data can be read (default one).
			/// </summary>
			None,

			/// <summary>
			/// Performs checks for locked data and still allows reading not locked data (useful when you decided to lock your saves in one of app updates and wish to keep user data).
			/// </summary>
			Soft,

			/// <summary>
			/// Only locked to the current device data can be read. This is a preferred mode, but it should be enabled right from the first app release. If you released app without data lock consider using Soft lock or all previously saved data will not be accessible.
			/// </summary>
			Strict
		}
	}
}