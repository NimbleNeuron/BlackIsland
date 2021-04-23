#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_2018_1_OR_NEWER

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Security.Cryptography;
	using System.Threading;
	using Common;
	using UnityEngine;
	using Utils;

	internal class StandaloneWindowsWorker : BaseWorker
	{
		public static string GetBuildHash(string buildPath, FileFilter[] fileFilters, SHA1Managed sha1)
		{
			var files = Directory.GetFiles(buildPath, "*", SearchOption.AllDirectories);
			var count = files.Length;
			if (count == 0)
			{
				return null;
			}

			var fileHashes = new List<string>();
			for (var i = 0; i < count; i++)
			{
				var filePath = files[i];

				// skip folders since we can't hash them
				if (Directory.Exists(filePath))
				{
					continue;
				}

				foreach (var fileFilter in fileFilters)
				{
					if (fileFilter.MatchesPath(filePath, buildPath))
					{
#if UNITY_EDITOR
						UnityEditor.EditorUtility.DisplayProgressBar("ACTk: Generating code hash", "Hashing files...", (i + 1f) / count);
#endif
						using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
						using (var bs = new BufferedStream(fs))
						{
							var hash = sha1.ComputeHash(bs);
							var hashString = StringUtils.HashBytesToHexString(hash);
							//Debug.Log("Path: " + filePath + "\nHash: " + hashString);
							fileHashes.Add(hashString);
						}
					}
				}
			}

			if (fileHashes.Count == 0)
			{
				return null;
			}

			fileHashes.Sort();
			var hashesString = string.Join("", fileHashes.ToArray());
			var hashesBytes = StringUtils.StringToBytes(hashesString);

			var codeHashBytes = sha1.ComputeHash(hashesBytes);
			var codeHash = StringUtils.HashBytesToHexString(codeHashBytes);

			return codeHash;
		}

		public override void Execute()
		{
			base.Execute();

		    try
		    {
			    var buildFolder = Path.GetFullPath(Application.dataPath + @"\..\");
			    var t = new Thread(GenerateHashThread);
				t.Start(buildFolder);
		    }
		    catch (Exception e)
		    {
			    Debug.LogError(ACTkConstants.LogPrefix + "Something went wrong while calculating hash!\n" + e);
			    Complete(HashGeneratorResult.FromError(e.ToString()));
		    }
		}

		private void GenerateHashThread(object folder)
		{
			var buildFolder = (string)folder;

			try
			{
				var sha1 = new SHA1Managed();
#if ENABLE_IL2CPP
				var il2cpp = true;
#else
				var il2cpp = false;
#endif
				var hash = GetBuildHash(buildFolder, CodeHashGenerator.GetFileFiltersStandaloneWindows(il2cpp), sha1);
				sha1.Clear();
				Complete(HashGeneratorResult.FromCodeHash(hash));
			}
			catch (Exception e)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Something went wrong in thread: " + e);
				Complete(HashGeneratorResult.FromError(e.ToString()));
			}
		}
			
	}
}

#endif