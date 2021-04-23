#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.Validation
{
	using Detectors;
	using UnityEditor;

	[InitializeOnLoad]
	internal static class SettingsValidator
	{
		private static bool injectionValidated;
		private static bool wallhackValidated;

		static SettingsValidator()
		{
#if UNITY_2018_1_OR_NEWER
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
#else
			EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
#endif
		}

		private static void OnHierarchyChanged()
		{
			if (!injectionValidated && !ACTkSettings.Instance.DisableInjectionDetectorValidation)
			{
				var instance = InjectionDetector.Instance;
				if (instance != null)
				{
					if (InjectionRoutines.IsInjectionPossible())
					{
						if (!ACTkSettings.Instance.InjectionDetectorEnabled)
						{
							var result = EditorUtility.DisplayDialogComplex("Anti-Cheat Toolkit Validation",
								"ACTk noticed you're using Injection Detector but you have build detection support disabled.\n" +
								"Injection Detector needs it enabled in order to work properly.\nWould you like to enable it now?",
								"Yes", "Open Settings", "No, never ask again");

							if (result == 0)
							{
								ACTkSettings.Instance.InjectionDetectorEnabled = true;
							}
							else if (result == 1)
							{
								ACTkSettings.Show();
								return;
							}
							else
							{
								ACTkSettings.Instance.DisableInjectionDetectorValidation = true;
							}
						}
					}
				}
				injectionValidated = true;
			}

			if (!wallhackValidated && !ACTkSettings.Instance.DisableWallhackDetectorValidation)
			{
				var instance = WallHackDetector.Instance;
				if (instance != null && instance.CheckWireframe)
				{
						if (!SettingsGUI.IsWallhackDetectorShaderIncluded())
						{
							var result = EditorUtility.DisplayDialog("Anti-Cheat Toolkit Validation",
								"ACTk noticed you're using Wallhack Detector with Wireframe option enabled but you have no required shader added" +
								" to the Always Included Shaders.\n" +
								"Would you like to exit Play Mode and open Settings to include it now?",
								"Yes", "No, never ask again");

							if (result)
							{
								EditorApplication.isPlaying = false;
								ACTkSettings.Show();
							}
							else
							{
								ACTkSettings.Instance.DisableWallhackDetectorValidation = true;
							}
						}
				}
				wallhackValidated = true;
			}
		}
	}
}