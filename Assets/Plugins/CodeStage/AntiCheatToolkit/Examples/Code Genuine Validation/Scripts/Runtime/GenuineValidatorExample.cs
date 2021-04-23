namespace CodeStage.AntiCheat.Examples.Genuine
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;
	using Utils;
	using UnityEngine;
	using ObscuredTypes;

#if UNITY_2018_1_OR_NEWER
	using CodeStage.AntiCheat.Genuine.CodeHash;
#endif

	// use this to check hash generated with CodeHashGeneratorListener.cs example file
	// note: this is an example for the Windows Standalone platform only
	public class GenuineValidatorExample : MonoBehaviour
	{
		public static readonly char[] StringKey = {'\x674', '\x345', '\x856', '\x968', '\x322'};

		// let's choose some non-obvious file name which will not be hashed (not .dll or .exe)
		public const string FileName = "Textures.asset";

		// let's JFF use 💝 as a pseudo-salt for hashing and get hash of the build hash =D
		public const string HashSalt = "💝";		
		
		// 💖 looks like a really lovely separator =)
		public const string Separator = "💖";

		public static readonly int SeparatorLength = Separator.Length;

		private string status;

		// just an unoptimized example of SHA1 hashing
		public static string GetHash(string firstBuildHash)
		{
			var stringBytes = StringUtils.StringToBytes(firstBuildHash);
			var sha1 = new SHA1Managed();
			var hash = sha1.ComputeHash(stringBytes);
			sha1.Clear();
			return StringUtils.HashBytesToHexString(hash);
		}

		private void Awake()
		{
#if UNITY_2018_1_OR_NEWER
			CodeHashGenerator.HashGenerated += OnGotHash;
			status = "Press Check";
#else
			status = "Unity 2018.1 or newer required!";
#endif
		}

		private void OnGUI()
		{
#if UNITY_2018_1_OR_NEWER
			if (GUILayout.Button("Check"))
			{
				OnCheckHashClick();
			}
#endif
			GUILayout.Label(status);
		}

#if UNITY_2018_1_OR_NEWER
		private void OnCheckHashClick()
		{
			status = "Checking...";
			CodeHashGenerator.Generate();
		}

		private void OnGotHash(HashGeneratorResult result)
		{
			if (!result.Success)
			{
				status = "Error: " + result.ErrorMessage;
				return;
			}

			var resultingHash = result.CodeHash;
			var filePath = Path.Combine(Path.GetFullPath(Application.dataPath + @"\..\"), FileName);
			if (!File.Exists(filePath))
			{
				status = "No super secret file found, you're cheater!\n" + filePath;
				return;
			}

			var allBytes = File.ReadAllBytes(filePath);
			var allChars = BytesToUnicodeChars(allBytes);
			var decrypted = ObscuredString.Decrypt(allChars, StringKey);
			var separatorIndex = decrypted.IndexOf(Separator, StringComparison.InvariantCulture);
			if (separatorIndex == -1)
			{
				status = "Super secret file is corrupted, you're cheater!";
				return;
			}

			var buildHash = decrypted.Substring(0, separatorIndex);
			var fileHash = decrypted.Substring(separatorIndex + SeparatorLength);
			var currentFileHash = GetHash(buildHash + HashSalt);
			if (currentFileHash != fileHash)
			{
				status = "Super secret file is corrupted, you're cheater!";
				return;
			}

			if (buildHash != resultingHash)
			{
				status = "Code hash differs, you're cheater!\n" + resultingHash + "\n" + buildHash;
				return;
			}

			status = "All fine!";
		}

		public static char[] BytesToUnicodeChars(byte[] input)
		{
			return Encoding.Unicode.GetChars(input);
		}

		public static byte[] UnicodeCharsToBytes(char[] input)
		{
			return Encoding.Unicode.GetBytes(input);
		}
#endif
	}
}