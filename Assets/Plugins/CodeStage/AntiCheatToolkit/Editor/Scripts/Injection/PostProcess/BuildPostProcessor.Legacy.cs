#region copyright

// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------

#endregion

#if !UNITY_2018_3_OR_NEWER
namespace CodeStage.AntiCheat.EditorCode.PostProcessors
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Common;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.Compilation;
	using UnityEngine;

	internal class BuildPostProcessor
	{
		private static bool hookBeforeResourcesPackingExecuted;
		private static bool hookBeforeResourcesPackingError;

		[DidReloadScripts(int.MaxValue - 1)]
		private static void DidReloadScripts()
		{
			InjectionRoutines.Cleanup();
		}

		internal class FolderFilter
		{
			public string path;
			public bool recursive;

			public FolderFilter(string path, bool recursive = false)
			{
				this.path = path;
				this.recursive = recursive;
			}
		}

		public static string[] GetGuessedLibrariesForBuild()
		{
			var foldersWithLibraries = GetGuessedFoldersWithLibrariesForBuild();
			var libsInFolders = new List<string>();

			foreach (var folder in foldersWithLibraries)
			{
				var allLibraries = EditorTools.FindLibrariesAt(folder.path, folder.recursive);
				libsInFolders.AddRange(allLibraries);
			}

			var paths = new HashSet<string>();

			foreach (var path in libsInFolders)
			{
				paths.Add(path);
			}

#if UNITY_2018_1_OR_NEWER
			var compiledAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
#else
			var compiledAssemblies = CompilationPipeline.GetAssemblies();
#endif

			foreach (var compiledAssembly in compiledAssemblies)
			{
				var path = Path.GetFullPath(compiledAssembly.outputPath);
				paths.Add(path);

				try
				{
					var compiledAssemblyReferences = compiledAssembly.compiledAssemblyReferences;

					foreach (var reference in compiledAssemblyReferences)
					{
						path = Path.GetFullPath(reference);
						paths.Add(path);
					}
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return paths.ToArray();
		}

		private static FolderFilter[] GetGuessedFoldersWithLibrariesForBuild()
		{
#if UNITY_EDITOR_WIN
			const string editorSubfolder = "Data";
#elif UNITY_EDITOR_OSX
			const string editorSubfolder = "Unity.app/Contents";
#endif

#if !(UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)
			Debug.LogError(ACTkConstants.LogPrefix + "Only Windows and Mac OS are supported in the legacy post processor! Please update Unity to 2018.3 or newer to use this feature on Linux.");
			return new FolderFilter[0];
#else
			var editorFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
			if (string.IsNullOrEmpty(editorFolder))
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Can't obtain Editor directory path!");
				return new FolderFilter[0];
			}

			editorFolder = Path.Combine(editorFolder, editorSubfolder);

			var folders = new List<FolderFilter>
			{
				new FolderFilter(Path.Combine(editorFolder, "Managed")),
				new FolderFilter(Path.Combine(editorFolder, "Managed/UnityEngine")),
				new FolderFilter(Path.Combine(editorFolder, "Mono/lib/mono/2.0")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/4.5")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/unityjit")),
				new FolderFilter(Path.Combine(editorFolder, "Mono/lib/mono/micro")),
				new FolderFilter(Path.Combine(editorFolder, "Mono/lib/mono/unity")),
				new FolderFilter(Path.Combine(editorFolder, "Mono/lib/mono/unity_web")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/2.0")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/4.0")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/unity")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/unity_aot")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/unityaot")),
				new FolderFilter(Path.Combine(editorFolder, "MonoBleedingEdge/lib/mono/unity_web")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/Advertisements")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/GUISystem")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/GUISystem/Standalone")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/Networking")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/Networking/Standalone")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/TestRunner")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/TestRunner/net35")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/TestRunner/standalone")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/TestRunner/portable")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/Timeline/Runtime")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/Timeline/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UIAutomation")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UIAutomation/Standalone")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityGoogleAudioSpatializer/Runtime")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityGoogleAudioSpatializer/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityAnalytics")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityHoloLens/Runtime")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityHoloLens/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnitySpatialTracking/Runtime")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnitySpatialTracking/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityPurchasing")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityVR/Runtime")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/UnityVR/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "UnityExtensions/Unity/VR/RuntimeEditor")),
				new FolderFilter(Path.Combine(editorFolder, "NetStandard/compat/2.0.0/shims/netfx")),
				new FolderFilter(Path.Combine(editorFolder, "NetStandard/compat/2.0.0/shims/netstandard")),
				new FolderFilter(Path.Combine(editorFolder, "NetStandard/Extensions/2.0.0")),
				new FolderFilter(Path.Combine(editorFolder, "NetStandard/ref/2.0.0")),
#if UNITY_ANDROID
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/AndroidPlayer/Variations/mono/Managed")),
#else
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/windowsstandalonesupport/Managed")),
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/windowsstandalonesupport/Variations/win32_development_mono/Data/Managed")),
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/windowsstandalonesupport/Variations/win32_nondevelopment_mono/Data/Managed")),
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/windowsstandalonesupport/Variations/win64_development_mono/Data/Managed")),
				new FolderFilter(Path.Combine(editorFolder, "PlaybackEngines/windowsstandalonesupport/Variations/win64_nondevelopment_mono/Data/Managed")),
#endif
				new FolderFilter(ACTkEditorConstants.AssetsFolder, true),
				new FolderFilter(InjectionConstants.ScriptAssembliesFolder)
			};
			
			return folders.ToArray();
#endif
		}

		[PostProcessScene(int.MaxValue - 1)]
		private static void HookBeforeResourcesPacking()
		{
			if (!ACTkSettings.Instance.InjectionDetectorEnabled || 
			    !InjectionRoutines.IsInjectionPossible())
			{
				return;
			}

			if (EditorApplication.isPlayingOrWillChangePlaymode || 
			    hookBeforeResourcesPackingExecuted ||
			    hookBeforeResourcesPackingError || 
			    !BuildPipeline.isBuildingPlayer)
			{
				return;
			}

			try
			{
				hookBeforeResourcesPackingExecuted = true;
				EditorApplication.LockReloadAssemblies();

				InjectionWhitelistBuilder.GenerateWhitelist();
			}
			catch (Exception e)
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Error at HookBeforeResourcesPacking:\n" + e);
				hookBeforeResourcesPackingError = true;
			}
			finally
			{
				EditorApplication.UnlockReloadAssemblies();
			}
		}

		[PostProcessBuild(int.MaxValue - 1)]
		private static void PostBuildHook(BuildTarget buildTarget, string pathToBuildProject)
		{
			InjectionRoutines.Cleanup();
		}
	}
}
#endif