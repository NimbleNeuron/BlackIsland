#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Utils
{
	using System;
	using System.Text;

	internal static class StringUtils
	{
		public static byte[] CharsToBytes(char[] input)
		{
			return Encoding.UTF8.GetBytes(input);
		}

		public static byte[] StringToBytes(string input)
		{
			return Encoding.UTF8.GetBytes(input);
		}

		public static char[] BytesToChars(byte[] input)
		{
			return Encoding.UTF8.GetChars(input);
		}

		public static string BytesToString(byte[] input)
		{
			return Encoding.UTF8.GetString(input);
		}

		public static string BytesToString(byte[] input, int index, int count)
		{
			return Encoding.UTF8.GetString(input, index, count);
		}

		public static string HashBytesToHexString(byte[] input)
		{
			return BitConverter.ToString(input).Replace("-", "");
		}
	}
}