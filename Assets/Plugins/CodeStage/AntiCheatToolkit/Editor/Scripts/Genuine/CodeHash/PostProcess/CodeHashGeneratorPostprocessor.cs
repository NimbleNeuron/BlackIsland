#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_2018_1_OR_NEWER

namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Security.Cryptography;
	using Common;
	using Genuine.CodeHash;
	using ICSharpCode.SharpZipLib.Zip;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;
	using Utils;
	using Debug = UnityEngine.Debug;

	/// <summary>
	/// Does calculates code hash after build if you use option "Generate code hash".
	/// Listen to HashesGenerate or look for hash for each build in the Editor Console.
	/// </summary>
	/// Resulting hash in most cases should match value you get from the \ref CodeStage.AntiCheat.Genuine.CodeHash.CodeHashGenerator "CodeHashGenerator"
	public class CodeHashGeneratorPostprocessor : IPostprocessBuildWithReport
	{
		/// <summary>
		/// Use to subscribe to the HashesGenerate event.
		/// </summary>
		public static CodeHashGeneratorPostprocessor Instance { get; private set; }

		/// <summary>
		/// HashesGenerate event delegate.
		/// </summary>
		/// <param name="report">Standard post-build report from Unity.</param>
		/// <param name="buildHashes">Dictionary where keys are paths to the resulting builds and values are code hashes for each resulting build.
		/// You may generate multiple actual builds within single build operation,
		/// like multiple apks when you use "Split APKs by target architecture" option
		/// and build for multiple target architectures (e.g. for ARM and x86)</param>
		public delegate void OnHashesGenerate(BuildReport report, Dictionary<string, string> buildHashes);

		/// <summary>
		/// You may listen to this event if you wish to post-process resulting code hash,
		/// e.g. upload it to the server for the later runtime check with CodeHashGenerator.
		/// </summary>
		public event OnHashesGenerate HashesGenerate;

		public CodeHashGeneratorPostprocessor()
		{
			Instance = this;
		}

		~CodeHashGeneratorPostprocessor()
		{
			if (Instance == this)
			{
				Instance = null;
			}

			HashesGenerate = null;
		}

		public int callbackOrder
		{
			get { return int.MaxValue; }
		}

		/// <summary>
		/// Calls selection dialog and calculates hash for the selected build.
		/// </summary>
		/// <param name="selectedBuildPath">Selected build path or null if selection was cancelled.</param>
		/// <returns>Calculated hash or null in case of error / user cancellation.</returns>
		public static string CalculateExternalBuildHash(out string selectedBuildPath)
		{
			var buildPath = EditorUtility.OpenFilePanel("Select Standalone Windows build exe or Android build apk / aab", "", "exe,apk,aab");
			selectedBuildPath = buildPath;
			if (string.IsNullOrEmpty(buildPath))
			{
				return null;
			}

			var extension = Path.GetExtension(selectedBuildPath);
			if (extension == null)
			{
				return null;
			}

			extension = extension.ToLower(CultureInfo.InvariantCulture);

			string result = null;
			var sha1 = new SHA1Managed();

			try
			{
				var il2Cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) ==
				             ScriptingImplementation.IL2CPP;

				if (extension == ".apk" || extension == ".aab")
				{
					result = GetApkHash(buildPath, CodeHashGenerator.GetFileFiltersAndroid(il2Cpp), sha1);
				}
				else
				{
					var buildFolder = Path.GetDirectoryName(selectedBuildPath);
					result = StandaloneWindowsWorker.GetBuildHash(buildFolder,
						CodeHashGenerator.GetFileFiltersStandaloneWindows(il2Cpp), sha1);

				}
			}
			catch (Exception e)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Error while trying to hash build: " + e);
			}
			finally
			{
				sha1.Clear();
				EditorUtility.ClearProgressBar();
			}

			return result;
		}

		// called by Unity
		public void OnPostprocessBuild(BuildReport report)
		{
			if (!ACTkSettings.Instance.PreGenerateBuildHash || !CodeHashGenerator.IsTargetPlatformCompatible())
			{
				return;
			}

			try
			{
				EditorUtility.DisplayProgressBar("ACTk: Generating code hash", "Preparing...", 0);
				var hashes = GetHashes(report);

				if (hashes == null || hashes.Count == 0)
				{
					Debug.Log(ACTkConstants.LogPrefix + "Couldn't pre-generate code hash. " +
					          "Please run your build and generate hash with CodeHashGenerator.");
					return;
				}

				foreach (var hash in hashes)
				{
					Debug.Log(ACTkConstants.LogPrefix + "Pre-generated code hash: " + hash.Value + "\nBuild: " + hash.Key);
				}

				if (HashesGenerate != null)
				{
					HashesGenerate.Invoke(report, hashes);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}

		private Dictionary<string, string> GetHashes(BuildReport report)
		{
			var fileFilters = GetFileFilters();
			var sha1 = new SHA1Managed();
			Dictionary<string, string> result = null;
#if UNITY_ANDROID
			result = GetAndroidBuildHashes(report, fileFilters, sha1);
#elif UNITY_STANDALONE_WIN
			result = GetStandaloneWindowsBuildHashes(report, fileFilters, sha1);
#endif
			sha1.Clear();
			return result;
		}

		private static FileFilter[] GetFileFilters()
		{
			var il2Cpp = false;
#if UNITY_EDITOR
			il2Cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;
#elif ENABLE_IL2CPP
			il2Cpp = true;
#endif

#if UNITY_ANDROID
			return CodeHashGenerator.GetFileFiltersAndroid(il2Cpp);
#elif UNITY_STANDALONE_WIN
			return CodeHashGenerator.GetFileFiltersStandaloneWindows(il2Cpp);
#else
			return null;
#endif
		}

		// --------------------------------------------------------------
		// Android build post-processing
		// --------------------------------------------------------------

		private Dictionary<string, string> GetAndroidBuildHashes(BuildReport report, FileFilter[] fileFilters, SHA1Managed sha1)
		{
			var result = new Dictionary<string, string>();

			foreach (var reportFile in report.files)
			{
				var path = reportFile.path;
				if (path.EndsWith(".apk") || path.EndsWith(".aab"))
				{
					var hash = GetApkHash(path, fileFilters, sha1);
					result.Add(path, hash);
				}
			}

			if (result.Count == 0)
			{
				Debug.LogWarning(ACTkConstants.LogPrefix + "Couldn't find compiled APK or AAB build.\n" +
				                 "This is fine if you use Export Project feature. " +
				                 "Otherwise, please report to " + ACTkEditorConstants.SupportEmail);
			}

			return result;
		}

		private static string GetApkHash(string path, FileFilter[] fileFilters, SHA1Managed sha1)
		{
			ZipFile zf = null;
			var fileHashes = new List<string>();

			try
			{
				var fs = File.OpenRead(path);
				zf = new ZipFile(fs); 

				var i = 0f;

				foreach (ZipEntry zipEntry in zf)
				{
					i++;

					if (!zipEntry.IsFile)
					{
						continue;
					}

					var entryFileName = zipEntry.Name;
					var suitableFile = false;

					foreach (var fileFilter in fileFilters)
					{
						if (fileFilter.MatchesPath(entryFileName))
						{
							suitableFile = true;
							break;
						}
					}

					if (!suitableFile) continue;
					
					EditorUtility.DisplayProgressBar("ACTk: Generating code hash", "Hashing files...", (i) / zf.Count);

					var zipStream = zf.GetInputStream(zipEntry);
					var hash = sha1.ComputeHash(zipStream);
					var hashString = StringUtils.HashBytesToHexString(hash);
					fileHashes.Add(hashString);
					//Debug.Log("Path: " + zipEntry.Name + "\nHash: " + hashString);
					zipStream.Close();
				}
			}
			catch (Exception e)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Error while calculating code hash: " + e);
				return null;
			}
			finally 
			{
				if (zf != null) 
				{
					zf.IsStreamOwner = true; // Makes close also shut the underlying stream
					zf.Close(); // Ensure we release resources
				}
			}

			fileHashes.Sort();
			var hashesString = string.Join("", fileHashes.ToArray());
			var hashesBytes = StringUtils.StringToBytes(hashesString);

			var codeHashBytes = sha1.ComputeHash(hashesBytes);
			var result = StringUtils.HashBytesToHexString(codeHashBytes);

			return result;
		}

		// --------------------------------------------------------------
		// Standalone Windows build post-processing
		// --------------------------------------------------------------
		private Dictionary<string, string> GetStandaloneWindowsBuildHashes(BuildReport report, FileFilter[] fileFilters, SHA1Managed sha1)
		{
			var result = new Dictionary<string, string>();
			var folder = Path.GetDirectoryName(report.summary.outputPath);
			if (folder == null)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Could not found build folder for this file: ");
				return result;
			}

			var codeHash = StandaloneWindowsWorker.GetBuildHash(folder, fileFilters, sha1);
			if (codeHash == null)
			{
				return result;
			}

			var buildPath = report.files[0].path;

			result.Add(buildPath, codeHash);

			return result;
		}

		private IEnumerable<string> FindFilesInFolder(string folder, string fileName)
		{
			return Directory.GetFiles(folder, fileName, SearchOption.AllDirectories);
		}

	}
}

#endif