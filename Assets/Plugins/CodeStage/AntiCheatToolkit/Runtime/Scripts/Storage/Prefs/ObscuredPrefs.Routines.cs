#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Storage
{
	using System;
	using System.Collections.Generic;
	using Common;
	using ObscuredTypes;
	using UnityEngine;
	using Utils;

	public static partial class ObscuredPrefs
	{
		private static readonly Type IntType = typeof(int);
		private static readonly Type UIntType = typeof(uint);
		private static readonly Type StringType = typeof(string);
		private static readonly Type FloatType = typeof(float);
		private static readonly Type DoubleType = typeof(double);
		private static readonly Type DecimalType = typeof(decimal);
		private static readonly Type LongType = typeof(long);
		private static readonly Type ULongType = typeof(ulong);
		private static readonly Type BoolType = typeof(bool);

		private static char[] generatedCryptoKey;

		private static bool migratingFromACTkV1;

		internal static char[] GetCryptoKey(string dynamicSuffix = null)
		{
			if (generatedCryptoKey == null)
			{
				var savedKey = PlayerPrefs.GetString(PrefsKey);
				if (!string.IsNullOrEmpty(savedKey))
				{
					generatedCryptoKey = Base64Utils.FromBase64ToChars(savedKey);
				}
				else
				{
					generatedCryptoKey = ObscuredString.GenerateKey();
					var b64 = Base64Utils.ToBase64(generatedCryptoKey);
					PlayerPrefs.SetString(PrefsKey, b64);
					PlayerPrefs.Save();
				}
			}

			if (string.IsNullOrEmpty(dynamicSuffix))
			{
				return generatedCryptoKey;
			}
			
			var suffixChars = dynamicSuffix.ToCharArray();
			var result = new char[generatedCryptoKey.Length + suffixChars.Length];
			Buffer.BlockCopy(generatedCryptoKey, 0, result, 0, generatedCryptoKey.Length);
			Buffer.BlockCopy(suffixChars, 0, result, generatedCryptoKey.Length + 1, suffixChars.Length);

			return result;
		}

		internal static string EncryptKey(string key)
		{
			var keyChars = ObscuredString.Encrypt(key.ToCharArray(), GetCryptoKey());
			key = Base64Utils.ToBase64(keyChars);
			return key;
		}

		internal static string EncryptData(string key, byte[] cleanBytes, DataType type)
		{
			var dataLength = cleanBytes.Length;
			var encryptedBytes = EncryptDecryptBytes(cleanBytes, dataLength, GetCryptoKey(key));

			var dataHash = xxHash.CalculateHash(cleanBytes, dataLength, 0);
			var dataHashBytes = new byte[4]; // replaces BitConverter.GetBytes(hash);
			dataHashBytes[0] = (byte)(dataHash & 0xFF);
			dataHashBytes[1] = (byte)((dataHash >> 8) & 0xFF);
			dataHashBytes[2] = (byte)((dataHash >> 16) & 0xFF);
			dataHashBytes[3] = (byte)((dataHash >> 24) & 0xFF);

			byte[] deviceHashBytes = null;
			int finalBytesLength;
			if (lockToDevice != DeviceLockLevel.None)
			{
				// 4 device id hash + 1 data type + 1 device lock mode + 1 version + 4 data hash
				finalBytesLength = dataLength + 11;
				var deviceHash = DeviceIdHash;
				deviceHashBytes = new byte[4]; // replaces BitConverter.GetBytes(hash);
				deviceHashBytes[0] = (byte)(deviceHash & 0xFF);
				deviceHashBytes[1] = (byte)((deviceHash >> 8) & 0xFF);
				deviceHashBytes[2] = (byte)((deviceHash >> 16) & 0xFF);
				deviceHashBytes[3] = (byte)((deviceHash >> 24) & 0xFF);
			}
			else
			{
				// 1 data type + 1 device lock mode + 1 version + 4 data hash
				finalBytesLength = dataLength + 7;
			}

			var finalBytes = new byte[finalBytesLength];

			Buffer.BlockCopy(encryptedBytes, 0, finalBytes, 0, dataLength);
			if (deviceHashBytes != null)
			{
				Buffer.BlockCopy(deviceHashBytes, 0, finalBytes, dataLength, 4);
			}

			finalBytes[finalBytesLength - 7] = (byte)type;
			finalBytes[finalBytesLength - 6] = Version;
			finalBytes[finalBytesLength - 5] = (byte)lockToDevice;
			Buffer.BlockCopy(dataHashBytes, 0, finalBytes, finalBytesLength - 4, 4);

			return Convert.ToBase64String(finalBytes);
		}

		internal static byte[] DecryptData(string key, string encryptedInput)
		{
			byte[] inputBytes;

			try
			{
				inputBytes = Convert.FromBase64String(encryptedInput);
			}
			catch (Exception)
			{
				SavesTampered();
				return null;
			}

			if (inputBytes.Length <= 0)
			{
				SavesTampered();
				return null;
			}

			var inputLength = inputBytes.Length;

			// reserved for future use
			// type = (DataType)inputBytes[inputLength - 7];

			var inputVersion = inputBytes[inputLength - 6];
			if (inputVersion != Version)
			{
				if (inputVersion == 2)
				{
					/*if (string.IsNullOrEmpty(cryptoKeyObsolete))
					{
						Debug.LogError(FinalLogPrefix + "Data encrypted with obsolete version found but CryptoKey is not set! " +
						               "Can't decrypt it without a key.");
						return null;
					}*/
				}
				else
				{
					SavesTampered();
					return null;
				}
			}

			var inputLockToDevice = (DeviceLockLevel)inputBytes[inputLength - 5];

			var dataHashBytes = new byte[4];
			Buffer.BlockCopy(inputBytes, inputLength - 4, dataHashBytes, 0, 4);
			var inputDataHash = (uint)(dataHashBytes[0] | dataHashBytes[1] << 8 | dataHashBytes[2] << 16 | dataHashBytes[3] << 24);

			int dataBytesLength;
			uint inputDeviceHash = 0;

			if (inputLockToDevice != DeviceLockLevel.None)
			{
				dataBytesLength = inputLength - 11;
				if (lockToDevice != DeviceLockLevel.None)
				{
					var deviceHashBytes = new byte[4];
					Buffer.BlockCopy(inputBytes, dataBytesLength, deviceHashBytes, 0, 4);
					inputDeviceHash = (uint)(deviceHashBytes[0] | deviceHashBytes[1] << 8 | deviceHashBytes[2] << 16 | deviceHashBytes[3] << 24);
				}
			}
			else
			{
				dataBytesLength = inputLength - 7;
			}

			var encryptedBytes = new byte[dataBytesLength];
			Buffer.BlockCopy(inputBytes, 0, encryptedBytes, 0, dataBytesLength);

			byte[] cleanBytes;
			if (!migratingFromACTkV1)
			{
				cleanBytes = EncryptDecryptBytes(encryptedBytes, dataBytesLength, GetCryptoKey(key));
			}
			else
			{
				cleanBytes = EncryptDecryptBytesObsolete(encryptedBytes, dataBytesLength, key + cryptoKeyObsoleteForMigration);
			}

			var realDataHash = xxHash.CalculateHash(cleanBytes, dataBytesLength, 0);
			if (realDataHash != inputDataHash)
			{
				SavesTampered();
				return null;
			}

			if (lockToDevice == DeviceLockLevel.Strict && inputDeviceHash == 0 && !emergencyMode &&!readForeignSaves)
			{
				return null;
			}

			if (inputDeviceHash != 0 && !emergencyMode)
			{
				var realDeviceHash = DeviceIdHash;
				if (inputDeviceHash != realDeviceHash)
				{
					PossibleForeignSavesDetected();
					if (!readForeignSaves) return null;
				}
			}

			return cleanBytes;
		}

		internal static DataType GetRawValueType(string value)
		{
			var result = DataType.Unknown;
			byte[] inputBytes;

			try
			{
				inputBytes = Convert.FromBase64String(value);
			}
			catch (Exception)
			{
				return result;
			}

			if (inputBytes.Length < 7)
			{
				return result;
			}

			var inputLength = inputBytes.Length;

			result = (DataType)inputBytes[inputLength - 7];

			var version = inputBytes[inputLength - 6];
			if (version > 10)
			{
				result = DataType.Unknown;
			}

			return result;
		}

		internal static string EncryptValue<T>(string key, T value) where T : IConvertible
		{
			var cleanBytes = default(byte[]);
			var dataType = DataType.Unknown;

			var genericType = typeof(T);

			if (genericType == IntType)
			{
				dataType = DataType.Int;
				cleanBytes = BitConverter.GetBytes(value.ToInt32(null));
			}
			else if (genericType == UIntType)
			{
				dataType = DataType.UInt;
				cleanBytes = BitConverter.GetBytes(value.ToUInt32(null));
			}
			else if (genericType == StringType)
			{
				dataType = DataType.String;
				cleanBytes = StringUtils.StringToBytes(value.ToString());
			}
			else if (genericType == FloatType)
			{
				dataType = DataType.Float;
				cleanBytes = BitConverter.GetBytes(value.ToSingle(null));
			}
			else if (genericType == DoubleType)
			{
				dataType = DataType.Double;
				cleanBytes = BitConverter.GetBytes(value.ToDouble(null));
			}
			else if (genericType == DecimalType)
			{
				dataType = DataType.Decimal;
				cleanBytes = DecimalToBytes(value.ToDecimal(null));
			}
			else if (genericType == LongType)
			{
				dataType = DataType.Long;
				cleanBytes = BitConverter.GetBytes(value.ToInt64(null));
			}
			else if (genericType == ULongType)
			{
				dataType = DataType.ULong;
				cleanBytes = BitConverter.GetBytes(value.ToUInt64(null));
			}
			else if (genericType == BoolType)
			{
				dataType = DataType.Bool;
				cleanBytes = BitConverter.GetBytes(value.ToBoolean(null));
			}

			return EncryptData(key, cleanBytes, dataType);
		}

		internal static T DecryptValue<T>(string key, string encryptedKey, T defaultValue, string encryptedInput = null)
		{
			if (encryptedInput == null)
			{
				encryptedInput = GetEncryptedPrefsString(key, encryptedKey);
			}

			if (encryptedInput == RawNotFound)
			{
				return defaultValue;
			}

			var cleanBytes = DecryptData(key, encryptedInput);
			if (cleanBytes == null)
			{
				return defaultValue;
			}

			var cleanValue = default(T);
			var genericType = typeof(T);

			if (genericType == IntType)
			{
				cleanValue = (T)(object)BitConverter.ToInt32(cleanBytes, 0);
			}
			else if (genericType == UIntType)
			{
				cleanValue = (T)(object)BitConverter.ToUInt32(cleanBytes, 0);
			}
			else if (genericType == StringType)
			{
				cleanValue = (T)(object)StringUtils.BytesToString(cleanBytes, 0, cleanBytes.Length);
			}
			else if (genericType == FloatType)
			{
				cleanValue = (T)(object)BitConverter.ToSingle(cleanBytes, 0);
			}
			else if (genericType == DoubleType)
			{
				cleanValue = (T)(object)BitConverter.ToDouble(cleanBytes, 0);
			}
			else if (genericType == DecimalType)
			{
				cleanValue = (T)(object)BytesToDecimal(cleanBytes);
			}
			else if (genericType == LongType)
			{
				cleanValue = (T)(object)BitConverter.ToInt64(cleanBytes, 0);
			}
			else if (genericType == ULongType)
			{
				cleanValue = (T)(object)BitConverter.ToUInt64(cleanBytes, 0);
			}
			else if (genericType == BoolType)
			{
				cleanValue = (T)(object)BitConverter.ToBoolean(cleanBytes, 0);
			}

			return cleanValue;
		}

		private static byte[] EncryptDecryptBytes(byte[] bytes, int dataLength, char[] key)
		{
			var encryptionKeyLength = key.Length;

			var result = new byte[dataLength];

			for (var i = 0; i < dataLength; i++)
			{
				result[i] = (byte)(bytes[i] ^ key[i % encryptionKeyLength]);
			}

			return result;
		}

		private static byte[] DecimalToBytes(decimal dec)
		{
			var bits = decimal.GetBits(dec);
			var bytes = new List<byte>();
			foreach (var i in bits)
			{
				bytes.AddRange(BitConverter.GetBytes(i));
			}
			return bytes.ToArray();
		}

		private static decimal BytesToDecimal(byte[] bytes)
		{
			if (bytes.Length != 16)
				throw new Exception(ACTkConstants.LogPrefix + "A decimal must be created from exactly 16 bytes");
			var bits = new int[4];
			for (var i = 0; i <= 15; i += 4)
			{
				bits[i / 4] = BitConverter.ToInt32(bytes, i);
			}
			return new decimal(bits);
		}

		private static string GetEncryptedPrefsString(string key, string encryptedKey)
		{
			var result = PlayerPrefs.GetString(encryptedKey, RawNotFound);

			if (result == RawNotFound)
			{
				if (PlayerPrefs.HasKey(key))
				{
					Debug.LogWarning(FinalLogPrefix + "Are you trying to read regular PlayerPrefs data using ObscuredPrefs (key = " + key + ")?");
				}
				else
				{
					MigrateFromACTkV1Internal(key, cryptoKeyObsolete);
					result = PlayerPrefs.GetString(encryptedKey, RawNotFound);
				}
			}
			return result;
		}

		private static uint CalculateChecksum(string input)
		{
			byte[] inputBytes;

			if (!migratingFromACTkV1)
			{
				inputBytes = StringUtils.CharsToBytes(GetCryptoKey(input));
			}
			else
			{
				inputBytes = StringUtils.StringToBytes(input + cryptoKeyObsoleteForMigration);
			}
			
			var hash = xxHash.CalculateHash(inputBytes, inputBytes.Length, 0);
			return hash;
		}

		private static Color32 DecodeColor(uint encodedColor)
		{
			var a = (byte)(encodedColor >> 24);
			var r = (byte)(encodedColor >> 16);
			var g = (byte)(encodedColor >> 8);
			var b = (byte)(encodedColor >> 0);
			return new Color32(r, g, b, a);
		}

		private static bool MigrateFromACTkV1Internal(string key, string cryptoKey)
		{
			var oldPrefsKey = Base64Utils.ToBase64(ObscuredString.EncryptDecryptObsolete(key, cryptoKey));
			if (!PlayerPrefs.HasKey(oldPrefsKey))
			{
				return false;
			}

			SetMigrationMode(true);
			cryptoKeyObsoleteForMigration = cryptoKey;

			var encrypted = PlayerPrefs.GetString(oldPrefsKey);
			var type = GetRawValueType(encrypted);

			switch (type)
			{
				case DataType.Int:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0, encrypted);
					SetMigrationMode(false);
					SetInt(key, decrypted);
					break;
				}
				case DataType.UInt:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0u, encrypted);
					SetMigrationMode(false);
					SetUInt(key, decrypted);
					break;
				}
				case DataType.String:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, string.Empty, encrypted);
					SetMigrationMode(false);
					SetString(key, decrypted);
					break;
				}
				case DataType.Float:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0f, encrypted);
					SetMigrationMode(false);
					SetFloat(key, decrypted);
					break;
				}
				case DataType.Double:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0d, encrypted);
					SetMigrationMode(false);
					SetDouble(key, decrypted);
					break;
				}
				case DataType.Decimal:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0m, encrypted);
					SetMigrationMode(false);
					SetDecimal(key, decrypted);
					break;
				}
				case DataType.Long:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0L, encrypted);
					SetMigrationMode(false);
					SetLong(key, decrypted);
					break;
				}
				case DataType.ULong:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, 0ul, encrypted);
					SetMigrationMode(false);
					SetULong(key, decrypted);
					break;
				}
				case DataType.Bool:
				{
					var decrypted = DecryptValue(key, oldPrefsKey, false, encrypted);
					SetMigrationMode(false);
					SetBool(key, decrypted);
					break;
				}
				case DataType.ByteArray:
				{
					var decrypted = DecryptByteArrayValue(key, encrypted, 0, 0);
					SetMigrationMode(false);
					SetByteArray(key, decrypted);
					break;
				}
				case DataType.Vector2:
				{
					var decrypted = DecryptVector2Value(key, encrypted, Vector2.zero);
					SetMigrationMode(false);
					SetVector2(key, decrypted);
					break;
				}
				case DataType.Vector3:
				{
					var decrypted = DecryptVector3Value(key, encrypted, Vector3.zero);
					SetMigrationMode(false);
					SetVector3(key, decrypted);
					break;
				}
				case DataType.Quaternion:
				{
					var decrypted = DecryptQuaternionValue(key, encrypted, Quaternion.identity);
					SetMigrationMode(false);
					SetQuaternion(key, decrypted);
					break;
				}
				case DataType.Color:
				{
					var encodedColor = DecryptValue(key, oldPrefsKey, 16777216u, encrypted);
					var decrypted = DecodeColor(encodedColor);
					SetMigrationMode(false);
					SetColor(key, decrypted);
					break;
				}
				case DataType.Rect:
				{
					var decrypted = DecryptRectValue(key, encrypted, new Rect(0,0,0,0));
					SetMigrationMode(false);
					SetRect(key, decrypted);
					break;
				}
				default:
					Debug.LogWarning(FinalLogPrefix + "Couldn't migrate " + key + " key from ACTk v1 prefs since its type is unknown!");
					return false;
			}

			Debug.Log(FinalLogPrefix + "Obscured pref " + key + " successfully migrated to the newer format.");

			cryptoKeyObsoleteForMigration = null;
			PlayerPrefs.DeleteKey(oldPrefsKey);
			return true;
		}

		private static void SetMigrationMode(bool enabled)
		{
			migratingFromACTkV1 = enabled;
			deviceIdHash = 0; // to force hash recalculation
		}

		private static byte[] EncryptDecryptBytesObsolete(byte[] bytes, int dataLength, string key)
		{
			var encryptionKeyLength = key.Length;

			var result = new byte[dataLength];

			for (var i = 0; i < dataLength; i++)
			{
				result[i] = (byte)(bytes[i] ^ key[i % encryptionKeyLength]);
			}

			return result;
		}
	}
}