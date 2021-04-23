#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#define ACTK_TIME_CHEATING_DETECTOR_ENABLED

#if ACTK_PREVENT_INTERNET_PERMISSION
#undef ACTK_TIME_CHEATING_DETECTOR_ENABLED
#endif

#if (UNITY_STANDALONE || UNITY_ANDROID) && !ENABLE_IL2CPP
#define ACTK_INJECTION_DETECTOR_ENABLED
#endif

namespace CodeStage.AntiCheat.Examples
{
	using Storage;
	using Detectors;
	using ObscuredTypes;

	using System;
	using System.Linq;
	using UnityEngine;
	using Random = UnityEngine.Random;

	[AddComponentMenu("")]
	internal class ExamplesGUI : MonoBehaviour
	{
		private enum ExamplePage
		{
			ObscuredTypes = 0,
			ObscuredPrefs = 1,
			Detectors = 2,
			CodeHashing = 3
		}

		private const string RedColor = "#FF4040";
		private const string YellowColor = "#E9D604";
		private const string GreenColor = "#02C85F";

#pragma warning disable 0649
		[Header("Examples")]
		public ObscuredTypesExamples obscuredTypesExamples;
		public ObscuredPrefsExamples obscuredPrefsExamples;
		public DetectorsExamples detectorsExamples;
		public CodeHashExample codeHashExample;
#pragma warning restore 0649

		private readonly string[] tabs = {"Variables protection", "Saves protection", "Cheating detectors", "Code Hashing"};
		private ExamplePage currentPage;

		private string allSimpleObscuredTypes;
		private ObscuredPrefs.DeviceLockLevel savesLock;

		private GUIStyle centeredStyle;

		private void OnGUI()
		{
			if (centeredStyle == null)
			{
				centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter};
			}

			GUILayout.BeginArea(new Rect(10, 5, Screen.width - 20, Screen.height - 10));

			GUILayout.Label("<color=\"#0287C8\"><b>Anti-Cheat Toolkit Sandbox</b></color>", centeredStyle);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", centeredStyle);
			GUILayout.Space(5);

			currentPage = (ExamplePage)GUILayout.Toolbar((int)currentPage, tabs);

			switch (currentPage)
			{
				case ExamplePage.ObscuredTypes:
				{
					DrawObscuredTypesPage();
					break;
				}
				case ExamplePage.ObscuredPrefs:
				{
					DrawObscuredPrefsPage();
					break;
				}
				case ExamplePage.Detectors:
				{
					DrawDetectorsPage();
					break;
				}
				case ExamplePage.CodeHashing:
				{
					DrawCodeHashingPage();
					break;
				}
			}
			GUILayout.EndArea();
		}

		private void DrawObscuredTypesPage()
		{
			GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).");
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("<b>Obscured types:</b>\n<color=\"#75C4EB\">" + GetAllSimpleObscuredTypes() + "</color>", GUILayout.MinWidth(130));
				GUILayout.Space(10);
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):");

					#region int
					GUILayout.Space(10);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>int:</b> " + obscuredTypesExamples.regularInt, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.regularInt += Random.Range(1, 100);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.regularInt = 0;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>ObscuredInt:</b> " + obscuredTypesExamples.obscuredInt, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.obscuredInt += Random.Range(1, 100);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.obscuredInt = 0;
						}
					}
					#endregion

					#region float
					GUILayout.Space(10);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>float:</b> " + obscuredTypesExamples.regularFloat, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.regularFloat += Random.Range(1f, 100f);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.regularFloat = 0;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>ObscuredFloat:</b> " + obscuredTypesExamples.obscuredFloat, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.obscuredFloat += Random.Range(1f, 100f);
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.obscuredFloat = 0;
						}
					}
					#endregion

					#region Vector3
					GUILayout.Space(10);
					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>Vector3:</b> " + obscuredTypesExamples.regularVector3, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.regularVector3 += Random.insideUnitSphere;
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.regularVector3 = Vector3.zero;
						}
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("<b>ObscuredVector3:</b> " + obscuredTypesExamples.obscuredVector3, GUILayout.Width(250));
						if (GUILayout.Button("Add random value"))
						{
							obscuredTypesExamples.obscuredVector3 += Random.insideUnitSphere;
						}
						if (GUILayout.Button("Reset"))
						{
							obscuredTypesExamples.obscuredVector3 = Vector3.zero;
						}
					}
					#endregion
				}
			}
		}

		private void DrawObscuredPrefsPage()
		{
			GUILayout.Label("ACTk has secure layer for the PlayerPrefs: <color=\"#75C4EB\">ObscuredPrefs</color>. " +
			                "It protects data from view, detects any cheating attempts, " +
			                "optionally locks data to the current device and supports additional data types.");
			GUILayout.Space(5);
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("<b>Supported types:</b>\n" + GetAllObscuredPrefsDataTypes(), GUILayout.MinWidth(130));
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					GUILayout.Label("Below you can try to cheat both regular PlayerPrefs and secure ObscuredPrefs:");
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label("<color=\"" + RedColor + "\"><b>PlayerPrefs:</b></color>\neasy to cheat, only 3 supported types", centeredStyle);
						GUILayout.Space(5);
						if (string.IsNullOrEmpty(obscuredPrefsExamples.regularPrefs))
						{
							obscuredPrefsExamples.LoadRegularPrefs();
						}
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label(obscuredPrefsExamples.regularPrefs, GUILayout.Width(270));
							using (new GUILayout.VerticalScope())
							{
								using (new GUILayout.HorizontalScope())
								{
									if (GUILayout.Button("Save"))
									{
										obscuredPrefsExamples.SaveRegularPrefs();
									}
									if (GUILayout.Button("Load"))
									{
										obscuredPrefsExamples.LoadRegularPrefs();
									}
								}
								if (GUILayout.Button("Delete"))
								{
									obscuredPrefsExamples.DeleteRegularPrefs();
								}
							}
						}
					}
					GUILayout.Space(5);
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label("<color=\"" + GreenColor + "\"><b>ObscuredPrefs:</b></color>\nsecure, lot of additional types and extra options", centeredStyle);
						GUILayout.Space(5);
						if (string.IsNullOrEmpty(obscuredPrefsExamples.obscuredPrefs))
						{
							obscuredPrefsExamples.LoadObscuredPrefs();
						}

						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label(obscuredPrefsExamples.obscuredPrefs, GUILayout.Width(270));
							using (new GUILayout.VerticalScope())
							{
								using (new GUILayout.HorizontalScope())
								{
									if (GUILayout.Button("Save"))
									{
										obscuredPrefsExamples.SaveObscuredPrefs();
									}
									if (GUILayout.Button("Load"))
									{
										obscuredPrefsExamples.LoadObscuredPrefs();
									}
								}
								if (GUILayout.Button("Delete"))
								{
									obscuredPrefsExamples.DeleteObscuredPrefs();
								}

								using (new GUILayout.HorizontalScope())
								{
									GUILayout.Label("LockToDevice level");
								}

								savesLock = (ObscuredPrefs.DeviceLockLevel)GUILayout.SelectionGrid((int)savesLock, Enum.GetNames(typeof(ObscuredPrefs.DeviceLockLevel)), 3);
								obscuredPrefsExamples.LockObscuredPrefsToDevice(savesLock);

								GUILayout.Space(5);
								using (new GUILayout.HorizontalScope())
								{
									obscuredPrefsExamples.PreservePlayerPrefs = GUILayout.Toggle(obscuredPrefsExamples.PreservePlayerPrefs, "preservePlayerPrefs");
								}
								using (new GUILayout.HorizontalScope())
								{
									obscuredPrefsExamples.EmergencyMode = GUILayout.Toggle(obscuredPrefsExamples.EmergencyMode, "emergencyMode");
								}
								using (new GUILayout.HorizontalScope())
								{
									obscuredPrefsExamples.ReadForeignSaves = GUILayout.Toggle(obscuredPrefsExamples.ReadForeignSaves, "readForeignSaves");
								}
								GUILayout.Space(5);
								GUILayout.Label("<color=\"" + (obscuredPrefsExamples.savesAlterationDetected ? RedColor : GreenColor) + "\">Saves modification detected: " + obscuredPrefsExamples.savesAlterationDetected + "</color>");
								GUILayout.Label("<color=\"" + (obscuredPrefsExamples.foreignSavesDetected ? RedColor : GreenColor) + "\">Foreign saves detected: " + obscuredPrefsExamples.foreignSavesDetected + "</color>");
							}
						}
					}
					GUILayout.Space(5);
				}
			}
		}

		private void DrawDetectorsPage()
		{
			GUILayout.Label("ACTk is able to detect some types of cheating to let you take action on the cheating players. " +
			                "This example scene has all possible detectors and they do automatically start on scene start.");
			GUILayout.Space(5);
			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				GUILayout.Label("<b>" + SpeedHackDetector.ComponentName + "</b>");
				GUILayout.Label("Allows to detect speed hack applied from Cheat Engine, Game Guardian and such.");
				GUILayout.Label("Running: " + SpeedHackDetector.Instance.IsRunning + 
				                "\n<color=\"" + (detectorsExamples.speedHackDetected ? RedColor : GreenColor) + "\">" +
				                "Detected: " + detectorsExamples.speedHackDetected.ToString().ToLower() + 
				                "</color>");

#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
				GUILayout.Space(10);
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label("<b>" + TimeCheatingDetector.ComponentName + "</b> (updates once per 1 min by default)");
					if (GUILayout.Button("Force check", GUILayout.Width(100)))
					{
						detectorsExamples.ForceTimeCheatingDetectorCheck();
					}
				}
#else
				GUILayout.Space(10);
				GUILayout.Label("<b>" + TimeCheatingDetector.ComponentName + "</b>");
#endif
				GUILayout.Label("Allows to detect system time change to cheat " +
				                "some long-term processes (building progress, etc.).");

#if ACTK_TIME_CHEATING_DETECTOR_ENABLED
				if (detectorsExamples.wrongTimeDetected && !detectorsExamples.timeCheatingDetected)
				{
					GUILayout.Label("Running: " + TimeCheatingDetector.Instance.IsRunning + 
					                "\n<color=\"" + YellowColor + "\">" +
					                "Wrong time detected (diff more than: " + 
					                TimeCheatingDetector.Instance.wrongTimeThreshold.ToString().ToLower() + 
					                " minutes)</color>");
				}
				else
				{
					GUILayout.Label("Running: " + TimeCheatingDetector.Instance.IsRunning + 
					                "\n<color=\"" + (detectorsExamples.timeCheatingDetected ? RedColor : GreenColor) + "\">" +
					                "Detected: " + detectorsExamples.timeCheatingDetected.ToString().ToLower() + 
					                "</color>");
				}
#else
				GUILayout.Label("Was disabled with ACTK_PREVENT_INTERNET_PERMISSION compilation symbol.");
#endif
				GUILayout.Space(10);
				GUILayout.Label("<b>" + ObscuredCheatingDetector.ComponentName + "</b>");

				GUILayout.Label("Detects cheating of any Obscured type (except ObscuredPrefs, " +
				                "it has own detection features) used in project.");
				GUILayout.Label("Running: " + ObscuredCheatingDetector.Instance.IsRunning + 
				                "\n<color=\"" + (detectorsExamples.obscuredTypeCheatDetected ? RedColor : GreenColor) + "\">" +
				                "Detected: " + detectorsExamples.obscuredTypeCheatDetected.ToString().ToLower() + 
				                "</color>");

				GUILayout.Space(10);
				GUILayout.Label("<b>" + WallHackDetector.ComponentName + "</b>");
				GUILayout.Label("Detects common types of wall hack cheating: walking through the walls " +
				                "(Rigidbody and CharacterController modules), shooting through the walls " +
				                "(Raycast module), looking through the walls (Wireframe module).");
				GUILayout.Label("Running: " + WallHackDetector.Instance.IsRunning + 
				                "\n<color=\"" + (detectorsExamples.wallHackCheatDetected ? RedColor : GreenColor) + "\">" +
				                "Detected: " + detectorsExamples.wallHackCheatDetected.ToString().ToLower() + 
				                "</color>");

				GUILayout.Space(10);
				GUILayout.Label("<b>" + InjectionDetector.ComponentName + "</b>");
				GUILayout.Label("Allows to detect foreign managed assemblies in your application.");

#if ACTK_INJECTION_DETECTOR_ENABLED
				GUILayout.Label("Running: " + (InjectionDetector.Instance != null && InjectionDetector.Instance.IsRunning) + "\n" +
				                "<color=\"" + (detectorsExamples.injectionDetected ? RedColor : GreenColor) + "\">" +
				                "Detected: " + detectorsExamples.injectionDetected.ToString().ToLower() + 
				                "</color>");
#else
				GUILayout.Label("Injection detection is not available on current platform");
#endif
			}
		}

		private void DrawCodeHashingPage()
		{
			GUILayout.Label("ACTk is able to generate hash signature of your code included into the build. " +
			                "You can compare current hash with genuine hash to figure out if your code was altered. " +
			                "It's better to do this comparison on the server side so cheater couldn't alter it as well.");
			GUILayout.Space(5);

			using (new GUILayout.VerticalScope(GUI.skin.box))
			{
				if (codeHashExample.IsSupported)
				{
#if UNITY_2018_1_OR_NEWER
					// just to make sure it's added to the scene and Instance will be not empty
					codeHashExample.Init();

					if (!codeHashExample.IsBusy)
					{
						if (codeHashExample.LastResult != null)
						{
							if (codeHashExample.LastResult.Success)
							{
								GUILayout.Label("Generated Hash: " + codeHashExample.LastResult.CodeHash);
								if (codeHashExample.IsGenuineValueSetInInspector)
								{
									GUILayout.Label("Hash matches value from inspector: " + codeHashExample.HashMatches());
								}
								else
								{
									GUILayout.Label("No genuine hash was set in inspector.");
								}
							}
							else
							{
								GUILayout.Label("Error: " + codeHashExample.LastResult.ErrorMessage);
							}
						}
						else
						{
							if (GUILayout.Button("Generate Hash"))
							{
								codeHashExample.StartGeneration();
							}
						}
					}
					else
					{
						GUILayout.Label("Generating...");
					}
#endif
				}
				else
				{
					GUILayout.Label("Code Hash Generator works only in Standalone Windows and Android builds, starting from Unity 2018.1.");
				}
			}
		}

		private string GetAllSimpleObscuredTypes()
		{
			var result = "Can't use reflection here, sorry :(";
#if UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0
			return result;
#else
			var types = "";

			if (string.IsNullOrEmpty(allSimpleObscuredTypes))
			{
				var csharpAssembly = typeof(ObscuredInt).Assembly;
				var q = from t in csharpAssembly.GetTypes()
						where t.IsPublic && t.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && t.Name != "IObscuredType"
						select t;
				q.ToList().ForEach(t =>
				{
					if (types.Length > 0)
					{
						types += "\n" + (t.Name);
					}
					else
					{
						types += (t.Name);
					}
				});

				if (!string.IsNullOrEmpty(types))
				{
					result = types;
					allSimpleObscuredTypes = types;
				}
				else
				{
					allSimpleObscuredTypes = result;
				}
			}
			else
			{
				result = allSimpleObscuredTypes;
			}
			return result;
#endif
		}

		private string GetAllObscuredPrefsDataTypes()
		{
            return "int\n" +
				   "float\n" +
				   "string\n" +
				   "<color=\"#75C4EB\">" +
                   "uint\n" +
			       "double\n" +
			       "decimal\n" +
			       "long\n" +
			       "ulong\n" +
			       "bool\n" +
			       "byte[]\n" +
			       "Vector2\n" +
			       "Vector3\n" +
			       "Quaternion\n" +
			       "Color\n" +
			       "Rect" + 
				   "</color>";
		}
    }
}