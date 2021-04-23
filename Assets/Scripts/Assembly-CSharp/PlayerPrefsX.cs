using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPrefsX
{
	
	public static bool SetBool(string name, bool value)
	{
		try
		{
			PlayerPrefs.SetInt(name, value ? 1 : 0);
		}
		catch
		{
			return false;
		}
		return true;
	}

	
	public static bool GetBool(string name)
	{
		return PlayerPrefs.GetInt(name) == 1;
	}

	
	public static bool GetBool(string name, bool defaultValue)
	{
		return 1 == PlayerPrefs.GetInt(name, defaultValue ? 1 : 0);
	}

	
	public static long GetLong(string key, long defaultValue)
	{
		int @int;
		int int2;
		PlayerPrefsX.SplitLong(defaultValue, out @int, out int2);
		var @uint = (uint)PlayerPrefs.GetInt(key + "_lowBits", @int);
		var uint2 = (uint)PlayerPrefs.GetInt(key + "_highBits", int2);
		return (long)((ulong)uint2 << 32 | (ulong)@uint);
	}

	
	public static long GetLong(string key)
	{
		uint @int = (uint)PlayerPrefs.GetInt(key + "_lowBits");
		return (long)((ulong)PlayerPrefs.GetInt(key + "_highBits") << 32 | (ulong)@int);
	}

	
	private static void SplitLong(long input, out int lowBits, out int highBits)
	{
		lowBits = (int)((uint)input);
		highBits = (int)((uint)(input >> 32));
	}

	
	public static void SetLong(string key, long value)
	{
		int value2;
		int value3;
		PlayerPrefsX.SplitLong(value, out value2, out value3);
		PlayerPrefs.SetInt(key + "_lowBits", value2);
		PlayerPrefs.SetInt(key + "_highBits", value3);
	}

	
	public static bool SetVector2(string key, Vector2 vector)
	{
		return PlayerPrefsX.SetFloatArray(key, new float[]
		{
			vector.x,
			vector.y
		});
	}

	
	private static Vector2 GetVector2(string key)
	{
		float[] floatArray = PlayerPrefsX.GetFloatArray(key);
		if (floatArray.Length < 2)
		{
			return Vector2.zero;
		}
		return new Vector2(floatArray[0], floatArray[1]);
	}

	
	public static Vector2 GetVector2(string key, Vector2 defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetVector2(key);
		}
		return defaultValue;
	}

	
	public static bool SetVector3(string key, Vector3 vector)
	{
		return PlayerPrefsX.SetFloatArray(key, new float[]
		{
			vector.x,
			vector.y,
			vector.z
		});
	}

	
	public static Vector3 GetVector3(string key)
	{
		float[] floatArray = PlayerPrefsX.GetFloatArray(key);
		if (floatArray.Length < 3)
		{
			return Vector3.zero;
		}
		return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
	}

	
	public static Vector3 GetVector3(string key, Vector3 defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetVector3(key);
		}
		return defaultValue;
	}

	
	public static bool SetQuaternion(string key, Quaternion vector)
	{
		return PlayerPrefsX.SetFloatArray(key, new float[]
		{
			vector.x,
			vector.y,
			vector.z,
			vector.w
		});
	}

	
	public static Quaternion GetQuaternion(string key)
	{
		float[] floatArray = PlayerPrefsX.GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return Quaternion.identity;
		}
		return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	
	public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetQuaternion(key);
		}
		return defaultValue;
	}

	
	public static bool SetColor(string key, Color color)
	{
		return PlayerPrefsX.SetFloatArray(key, new float[]
		{
			color.r,
			color.g,
			color.b,
			color.a
		});
	}

	
	public static Color GetColor(string key)
	{
		float[] floatArray = PlayerPrefsX.GetFloatArray(key);
		if (floatArray.Length < 4)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
	}

	
	public static Color GetColor(string key, Color defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetColor(key);
		}
		return defaultValue;
	}

	
	public static bool SetBoolArray(string key, bool[] boolArray)
	{
		byte[] array = new byte[(boolArray.Length + 7) / 8 + 5];
		array[0] = Convert.ToByte(PlayerPrefsX.ArrayType.Bool);
		new BitArray(boolArray).CopyTo(array, 5);
		PlayerPrefsX.Initialize();
		PlayerPrefsX.ConvertInt32ToBytes(boolArray.Length, array);
		return PlayerPrefsX.SaveBytes(key, array);
	}

	
	public static bool[] GetBoolArray(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return new bool[0];
		}
		byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
		if (array.Length < 5)
		{
			Debug.LogError("Corrupt preference file for " + key);
			return new bool[0];
		}
		if (array[0] != 2)
		{
			Debug.LogError(key + " is not a boolean array");
			return new bool[0];
		}
		PlayerPrefsX.Initialize();
		byte[] array2 = new byte[array.Length - 5];
		Array.Copy(array, 5, array2, 0, array2.Length);
		BitArray bitArray = new BitArray(array2);
		bitArray.Length = PlayerPrefsX.ConvertBytesToInt32(array);
		bool[] array3 = new bool[bitArray.Count];
		bitArray.CopyTo(array3, 0);
		return array3;
	}

	
	public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetBoolArray(key);
		}
		bool[] array = new bool[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static bool SetStringArray(string key, string[] stringArray)
	{
		byte[] array = new byte[stringArray.Length + 1];
		array[0] = Convert.ToByte(PlayerPrefsX.ArrayType.String);
		PlayerPrefsX.Initialize();
		for (int i = 0; i < stringArray.Length; i++)
		{
			if (stringArray[i] == null)
			{
				Debug.LogError("Can't save null entries in the string array when setting " + key);
				return false;
			}
			if (stringArray[i].Length > 255)
			{
				Debug.LogError("Strings cannot be longer than 255 characters when setting " + key);
				return false;
			}
			array[PlayerPrefsX.idx++] = (byte)stringArray[i].Length;
		}
		try
		{
			PlayerPrefs.SetString(key, Convert.ToBase64String(array) + "|" + string.Join("", stringArray));
		}
		catch
		{
			return false;
		}
		return true;
	}

	
	public static string[] GetStringArray(string key)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return new string[0];
		}
		string @string = PlayerPrefs.GetString(key);
		int num = @string.IndexOf("|"[0]);
		if (num < 4)
		{
			Debug.LogError("Corrupt preference file for " + key);
			return new string[0];
		}
		byte[] array = Convert.FromBase64String(@string.Substring(0, num));
		if (array[0] != 3)
		{
			Debug.LogError(key + " is not a string array");
			return new string[0];
		}
		PlayerPrefsX.Initialize();
		int num2 = array.Length - 1;
		string[] array2 = new string[num2];
		int num3 = num + 1;
		for (int i = 0; i < num2; i++)
		{
			int num4 = (int)array[PlayerPrefsX.idx++];
			if (num3 + num4 > @string.Length)
			{
				Debug.LogError("Corrupt preference file for " + key);
				return new string[0];
			}
			array2[i] = @string.Substring(num3, num4);
			num3 += num4;
		}
		return array2;
	}

	
	public static string[] GetStringArray(string key, string defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetStringArray(key);
		}
		string[] array = new string[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static bool SetIntArray(string key, int[] intArray)
	{
		return PlayerPrefsX.SetValue<int[]>(key, intArray, PlayerPrefsX.ArrayType.Int32, 1, new Action<int[], byte[], int>(PlayerPrefsX.ConvertFromInt));
	}

	
	public static bool SetFloatArray(string key, float[] floatArray)
	{
		return PlayerPrefsX.SetValue<float[]>(key, floatArray, PlayerPrefsX.ArrayType.Float, 1, new Action<float[], byte[], int>(PlayerPrefsX.ConvertFromFloat));
	}

	
	public static bool SetVector2Array(string key, Vector2[] vector2Array)
	{
		return PlayerPrefsX.SetValue<Vector2[]>(key, vector2Array, PlayerPrefsX.ArrayType.Vector2, 2, new Action<Vector2[], byte[], int>(PlayerPrefsX.ConvertFromVector2));
	}

	
	public static bool SetVector3Array(string key, Vector3[] vector3Array)
	{
		return PlayerPrefsX.SetValue<Vector3[]>(key, vector3Array, PlayerPrefsX.ArrayType.Vector3, 3, new Action<Vector3[], byte[], int>(PlayerPrefsX.ConvertFromVector3));
	}

	
	public static bool SetQuaternionArray(string key, Quaternion[] quaternionArray)
	{
		return PlayerPrefsX.SetValue<Quaternion[]>(key, quaternionArray, PlayerPrefsX.ArrayType.Quaternion, 4, new Action<Quaternion[], byte[], int>(PlayerPrefsX.ConvertFromQuaternion));
	}

	
	public static bool SetColorArray(string key, Color[] colorArray)
	{
		return PlayerPrefsX.SetValue<Color[]>(key, colorArray, PlayerPrefsX.ArrayType.Color, 4, new Action<Color[], byte[], int>(PlayerPrefsX.ConvertFromColor));
	}

	
	private static bool SetValue<T>(string key, T array, PlayerPrefsX.ArrayType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList
	{
		byte[] array2 = new byte[4 * array.Count * vectorNumber + 1];
		array2[0] = Convert.ToByte(arrayType);
		PlayerPrefsX.Initialize();
		for (int i = 0; i < array.Count; i++)
		{
			convert(array, array2, i);
		}
		return PlayerPrefsX.SaveBytes(key, array2);
	}

	
	private static void ConvertFromInt(int[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertInt32ToBytes(array[i], bytes);
	}

	
	private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertFloatToBytes(array[i], bytes);
	}

	
	private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertFloatToBytes(array[i].x, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].y, bytes);
	}

	
	private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertFloatToBytes(array[i].x, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].y, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].z, bytes);
	}

	
	private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertFloatToBytes(array[i].x, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].y, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].z, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].w, bytes);
	}

	
	private static void ConvertFromColor(Color[] array, byte[] bytes, int i)
	{
		PlayerPrefsX.ConvertFloatToBytes(array[i].r, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].g, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].b, bytes);
		PlayerPrefsX.ConvertFloatToBytes(array[i].a, bytes);
	}

	
	public static int[] GetIntArray(string key)
	{
		List<int> list = new List<int>();
		PlayerPrefsX.GetValue<List<int>>(key, list, PlayerPrefsX.ArrayType.Int32, 1, new Action<List<int>, byte[]>(PlayerPrefsX.ConvertToInt));
		return list.ToArray();
	}

	
	public static int[] GetIntArray(string key, int defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetIntArray(key);
		}
		int[] array = new int[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static float[] GetFloatArray(string key)
	{
		List<float> list = new List<float>();
		PlayerPrefsX.GetValue<List<float>>(key, list, PlayerPrefsX.ArrayType.Float, 1, new Action<List<float>, byte[]>(PlayerPrefsX.ConvertToFloat));
		return list.ToArray();
	}

	
	public static float[] GetFloatArray(string key, float defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetFloatArray(key);
		}
		float[] array = new float[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static Vector2[] GetVector2Array(string key)
	{
		List<Vector2> list = new List<Vector2>();
		PlayerPrefsX.GetValue<List<Vector2>>(key, list, PlayerPrefsX.ArrayType.Vector2, 2, new Action<List<Vector2>, byte[]>(PlayerPrefsX.ConvertToVector2));
		return list.ToArray();
	}

	
	public static Vector2[] GetVector2Array(string key, Vector2 defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetVector2Array(key);
		}
		Vector2[] array = new Vector2[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static Vector3[] GetVector3Array(string key)
	{
		List<Vector3> list = new List<Vector3>();
		PlayerPrefsX.GetValue<List<Vector3>>(key, list, PlayerPrefsX.ArrayType.Vector3, 3, new Action<List<Vector3>, byte[]>(PlayerPrefsX.ConvertToVector3));
		return list.ToArray();
	}

	
	public static Vector3[] GetVector3Array(string key, Vector3 defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetVector3Array(key);
		}
		Vector3[] array = new Vector3[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static Quaternion[] GetQuaternionArray(string key)
	{
		List<Quaternion> list = new List<Quaternion>();
		PlayerPrefsX.GetValue<List<Quaternion>>(key, list, PlayerPrefsX.ArrayType.Quaternion, 4, new Action<List<Quaternion>, byte[]>(PlayerPrefsX.ConvertToQuaternion));
		return list.ToArray();
	}

	
	public static Quaternion[] GetQuaternionArray(string key, Quaternion defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetQuaternionArray(key);
		}
		Quaternion[] array = new Quaternion[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	public static Color[] GetColorArray(string key)
	{
		List<Color> list = new List<Color>();
		PlayerPrefsX.GetValue<List<Color>>(key, list, PlayerPrefsX.ArrayType.Color, 4, new Action<List<Color>, byte[]>(PlayerPrefsX.ConvertToColor));
		return list.ToArray();
	}

	
	public static Color[] GetColorArray(string key, Color defaultValue, int defaultSize)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefsX.GetColorArray(key);
		}
		Color[] array = new Color[defaultSize];
		for (int i = 0; i < defaultSize; i++)
		{
			array[i] = defaultValue;
		}
		return array;
	}

	
	private static void GetValue<T>(string key, T list, PlayerPrefsX.ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
	{
		if (PlayerPrefs.HasKey(key))
		{
			byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
			if ((array.Length - 1) % (vectorNumber * 4) != 0)
			{
				Debug.LogError("Corrupt preference file for " + key);
				return;
			}
			if ((PlayerPrefsX.ArrayType)array[0] != arrayType)
			{
				Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
				return;
			}
			PlayerPrefsX.Initialize();
			int num = (array.Length - 1) / (vectorNumber * 4);
			for (int i = 0; i < num; i++)
			{
				convert(list, array);
			}
		}
	}

	
	private static void ConvertToInt(List<int> list, byte[] bytes)
	{
		list.Add(PlayerPrefsX.ConvertBytesToInt32(bytes));
	}

	
	private static void ConvertToFloat(List<float> list, byte[] bytes)
	{
		list.Add(PlayerPrefsX.ConvertBytesToFloat(bytes));
	}

	
	private static void ConvertToVector2(List<Vector2> list, byte[] bytes)
	{
		list.Add(new Vector2(PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes)));
	}

	
	private static void ConvertToVector3(List<Vector3> list, byte[] bytes)
	{
		list.Add(new Vector3(PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes)));
	}

	
	private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes)
	{
		list.Add(new Quaternion(PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes)));
	}

	
	private static void ConvertToColor(List<Color> list, byte[] bytes)
	{
		list.Add(new Color(PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes), PlayerPrefsX.ConvertBytesToFloat(bytes)));
	}

	
	public static void ShowArrayType(string key)
	{
		byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
		if (array.Length != 0)
		{
			PlayerPrefsX.ArrayType arrayType = (PlayerPrefsX.ArrayType)array[0];
			Debug.Log(key + " is a " + arrayType.ToString() + " array");
		}
	}

	
	private static void Initialize()
	{
		if (BitConverter.IsLittleEndian)
		{
			PlayerPrefsX.endianDiff1 = 0;
			PlayerPrefsX.endianDiff2 = 0;
		}
		else
		{
			PlayerPrefsX.endianDiff1 = 3;
			PlayerPrefsX.endianDiff2 = 1;
		}
		if (PlayerPrefsX.byteBlock == null)
		{
			PlayerPrefsX.byteBlock = new byte[4];
		}
		PlayerPrefsX.idx = 1;
	}

	
	private static bool SaveBytes(string key, byte[] bytes)
	{
		try
		{
			PlayerPrefs.SetString(key, Convert.ToBase64String(bytes));
		}
		catch
		{
			return false;
		}
		return true;
	}

	
	private static void ConvertFloatToBytes(float f, byte[] bytes)
	{
		PlayerPrefsX.byteBlock = BitConverter.GetBytes(f);
		PlayerPrefsX.ConvertTo4Bytes(bytes);
	}

	
	private static float ConvertBytesToFloat(byte[] bytes)
	{
		PlayerPrefsX.ConvertFrom4Bytes(bytes);
		return BitConverter.ToSingle(PlayerPrefsX.byteBlock, 0);
	}

	
	private static void ConvertInt32ToBytes(int i, byte[] bytes)
	{
		PlayerPrefsX.byteBlock = BitConverter.GetBytes(i);
		PlayerPrefsX.ConvertTo4Bytes(bytes);
	}

	
	private static int ConvertBytesToInt32(byte[] bytes)
	{
		PlayerPrefsX.ConvertFrom4Bytes(bytes);
		return BitConverter.ToInt32(PlayerPrefsX.byteBlock, 0);
	}

	
	private static void ConvertTo4Bytes(byte[] bytes)
	{
		bytes[PlayerPrefsX.idx] = PlayerPrefsX.byteBlock[PlayerPrefsX.endianDiff1];
		bytes[PlayerPrefsX.idx + 1] = PlayerPrefsX.byteBlock[1 + PlayerPrefsX.endianDiff2];
		bytes[PlayerPrefsX.idx + 2] = PlayerPrefsX.byteBlock[2 - PlayerPrefsX.endianDiff2];
		bytes[PlayerPrefsX.idx + 3] = PlayerPrefsX.byteBlock[3 - PlayerPrefsX.endianDiff1];
		PlayerPrefsX.idx += 4;
	}

	
	private static void ConvertFrom4Bytes(byte[] bytes)
	{
		PlayerPrefsX.byteBlock[PlayerPrefsX.endianDiff1] = bytes[PlayerPrefsX.idx];
		PlayerPrefsX.byteBlock[1 + PlayerPrefsX.endianDiff2] = bytes[PlayerPrefsX.idx + 1];
		PlayerPrefsX.byteBlock[2 - PlayerPrefsX.endianDiff2] = bytes[PlayerPrefsX.idx + 2];
		PlayerPrefsX.byteBlock[3 - PlayerPrefsX.endianDiff1] = bytes[PlayerPrefsX.idx + 3];
		PlayerPrefsX.idx += 4;
	}

	
	private static int endianDiff1;

	
	private static int endianDiff2;

	
	private static int idx;

	
	private static byte[] byteBlock;

	
	private enum ArrayType
	{
		
		Float,
		
		Int32,
		
		Bool,
		
		String,
		
		Vector2,
		
		Vector3,
		
		Quaternion,
		
		Color
	}
}
