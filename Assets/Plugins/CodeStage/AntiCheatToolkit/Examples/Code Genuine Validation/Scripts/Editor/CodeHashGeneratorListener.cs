#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

// please comment next line to see CodeHashGeneratorListener example in action
#define ACTK_MUTE_EXAMPLES

#if UNITY_2018_1_OR_NEWER && !ACTK_MUTE_EXAMPLES
namespace CodeStage.AntiCheat.Examples.Genuine
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Common;
	using EditorCode;
	using EditorCode.PostProcessors;
	using ObscuredTypes;
	using UnityEditor;
	using UnityEditor.Build;
	using UnityEditor.Build.Reporting;
	using UnityEngine;

	// please check GenuineValidatorExample.cs to see runtime hash validation example

	public class CodeHashGeneratorListener : IPostprocessBuildWithReport
	{
		// using CodeHashGeneratorPostprocessor's 'order - 1' to subscribe before it finishes its job
		public int callbackOrder
		{
			get { return CodeHashGeneratorPostprocessor.Instance.callbackOrder - 1; }
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			// make sure example scene is built as a first scene, feel free to remove this in your real production code

			var exampleBuilt = true;
			foreach (var editorBuildSettingsScene in EditorBuildSettings.scenes)
			{
				if (!editorBuildSettingsScene.enabled)
				{
					continue;
				}

				if (!editorBuildSettingsScene.path.EndsWith("Code Genuine Validation/GenuineValidator.unity"))
				{
					exampleBuilt = false;
					break;
				}
			}

			if (!exampleBuilt)
			{
				return;
			}

			// make sure current platform is Windows Standalone
#if !UNITY_STANDALONE_WIN
			Debug.LogError("Please switch to Standalone Windows platform in order to use full GenuineValidator example.");
			return;
#endif

			// make sure hash generation enabled in settings
			if (!ACTkSettings.Instance.PreGenerateBuildHash)
			{
				Debug.LogError("Please enable code hash generation on build in the ACTk Settings in order to use full GenuineValidator example.");
				return;
			}

			// just subscribing to the hash generation event
			CodeHashGeneratorPostprocessor.Instance.HashesGenerate += OnHashesGenerate;
		}

		private static void OnHashesGenerate(BuildReport report, Dictionary<string, string> buildHashes)
		{
			Debug.Log("CodeHashGeneratorListener example listener saying hello.");

			// Upload hashes to the server or do anything you would like to.
			// 
			// Note, you may have multiple builds each with own hash in some cases after build,
			// e.g. when using "Split APKs by target architecture" option.
			foreach (var buildHash in buildHashes)
			{
				Debug.Log("Build: " + buildHash.Key + "\n" +
				          "Hash: " + buildHash.Value);
			}

			// for example, you may put hash next to the standalone build to compare it offline
			// just as a proof of concept, please consider uploading your hash to the server
			// and make comparison on the server-side to add some more pain to the cheaters\

			var firstBuildHash = buildHashes.FirstOrDefault().Value;
			if (string.IsNullOrEmpty(firstBuildHash))
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Couldn't find first build hash!");
				return;
			}

			var outputFolder = Path.GetDirectoryName(report.summary.outputPath);
			if (string.IsNullOrEmpty(outputFolder) || !Directory.Exists(outputFolder))
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Couldn't find build folder!");
				return;
			}

			var filePath = Path.Combine(outputFolder, GenuineValidatorExample.FileName);
			var hashOfTheHashHaha = GenuineValidatorExample.GetHash(firstBuildHash + GenuineValidatorExample.HashSalt);

			// let's put together build hash with its hash and encrypt it using constant key
			var encryptedValue = ObscuredString.Encrypt(firstBuildHash + GenuineValidatorExample.Separator + hashOfTheHashHaha, GenuineValidatorExample.StringKey);

			// now just get raw bytes and write them to the file to compare hash in runtime
			var bytes = GenuineValidatorExample.UnicodeCharsToBytes(encryptedValue);
			File.WriteAllBytes(filePath, bytes);
		}
	}
}
#endif