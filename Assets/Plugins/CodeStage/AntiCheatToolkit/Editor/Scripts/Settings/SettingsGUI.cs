#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using Common;
	using Detectors;
#if UNITY_2018_1_OR_NEWER
	using Genuine.CodeHash;
#endif

	using UnityEditor;
	using UnityEngine;

	internal class SettingsGUI
	{
		private const string Homepage = "http://codestage.net/uas/actk";
		private const string APILink = "http://codestage.net/uas_files/actk/api/";
		private const string SupportLink = "http://codestage.net/contacts/";
		private const string ForumLink = "https://forum.unity.com/threads/anti-cheat-toolkit-stop-cheaters-easily.196578/";
		private const string ReviewURL = "https://assetstore.unity.com/packages/slug/152334/reviews?aid=1011lGBp&pubref=actk";

#if !UNITY_2018_3_OR_NEWER
		private static Vector2 scrollPosition;
#endif
		private static SerializedObject graphicsSettingsAsset;
		private static SerializedProperty includedShaders;

		private static SymbolsData symbolsData;
		 
		public static void OnGUI()
		{
#if !UNITY_2018_3_OR_NEWER
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
#endif
			GUITools.Separator();

			DrawSettingsHeader();

			GUILayout.Space(5f);

			DrawIL2CPPSection();
			
			EditorGUILayout.Space();

			DrawInjectionSection();

			EditorGUILayout.Space();
#if UNITY_2018_1_OR_NEWER
			DrawHashSection();

			EditorGUILayout.Space();
#endif
			DrawWallHackSection();

			EditorGUILayout.Space();

			DrawConditionalSection();

#if !UNITY_2018_3_OR_NEWER
			GUILayout.EndScrollView();
#endif
		}

		private static void DrawSettingsHeader()
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(5f);
				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label("Version: " + ACTkConstants.Version);
					
					using (new GUILayout.HorizontalScope())
					{
						if (GUITools.ImageButton("", "Visit Homepage", Icons.Home))
						{
							Application.OpenURL(Homepage);
						}

						if (GUITools.ImageButton("", "Ask at Forums", Icons.Forum))
						{
							Application.OpenURL(ForumLink);
						}

						if (GUITools.ImageButton("", "Get priority support", Icons.Support))
						{
							Application.OpenURL(SupportLink);
						}

						if (GUITools.ImageButton("", "Write a Review at the Asset Store, it will help a lot!", Icons.Star))
						{ 
							if (!Event.current.control)
							{
								Application.OpenURL(ReviewURL);
							}
							else
							{
								UnityEditorInternal.AssetStore.Open(ReviewURL);
							}
						}

						GUILayout.Space(10f);

						if (GUITools.ImageButton("", "Read Anti-Cheat Toolkit Manual (Readme.pdf)", Icons.Manual))
						{
							EditorTools.OpenReadme();
						}

						if (GUITools.ImageButton("", "Read API reference", Icons.API))
						{
							Application.OpenURL(APILink);
						}

						GUILayout.Space(10f);

						if (GUITools.ImageButton("", "About", Icons.Help))
						{
							EditorUtility.DisplayDialog("About Anti-Cheat Toolkit v" + ACTkConstants.Version,
								"Developer: Dmitriy Yukhanov\n" +
								"Logo: Daniele Giardini \\m/\n" +
								"Material Icons: Google\n" +
								"Support: my lovely wife, daughters and you! <3\n\n" +
								@"¯\_(ツ)_/¯", "Fine!");
						}
					}
					GUILayout.Space(1f);
				}

				GUILayout.FlexibleSpace();

				var logo = Images.Logo;
				if (logo != null)
				{
					logo.wrapMode = TextureWrapMode.Clamp;
					var logoRect = EditorGUILayout.GetControlRect(GUILayout.Width(logo.width), GUILayout.Height(logo.height));
					logoRect.y += 13;
					GUI.DrawTexture(logoRect, logo);
				}
			}
		}

		private static void DrawIL2CPPSection()
		{
			using (var changed = new EditorGUI.ChangeCheckScope())
			{
				var fold = GUITools.DrawFoldHeader("IL2CPP", ACTkEditorPrefsSettings.IL2CPPFoldout);
				if (changed.changed)
				{
					ACTkEditorPrefsSettings.IL2CPPFoldout = fold;
				}
			}

			if (!ACTkEditorPrefsSettings.IL2CPPFoldout)
			{
				return;
			}

			GUILayout.Space(-3f);

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				GUILayout.Label("IL2CPP prevents Mono injections and easy code decompilation. " +
				                "Also consider obfuscating your metadata to make cheaters cry, see <b>readme</b> for details.",
					GUITools.RichLabel);

				GUILayout.Label("<b>Note: IL2CPP is AOT and does not support JIT!</b>", GUITools.RichMiniLabel);

				GUILayout.Space(5f);

				var supported = SettingsUtils.IsIL2CPPSupported();
				var supportedColor = supported ? ColorTools.GetGreenString() : ColorTools.GetRedString();

				var enabled = SettingsUtils.IsIL2CPPEnabled();
				var enabledColor = enabled ? ColorTools.GetGreenString() : ColorTools.GetRedString();

				GUILayout.Label("IL2CPP Supported: <color=#" + supportedColor + ">" + supported + "</color>",
					GUITools.RichLabel);
				GUILayout.Label("IL2CPP Enabled: <color=#" + enabledColor + ">" + enabled + "</color>",
					GUITools.RichLabel);

				if (!SettingsUtils.IsIL2CPPEnabled() && SettingsUtils.IsIL2CPPSupported())
				{
					GUILayout.Space(5f);
					EditorGUILayout.HelpBox("Use IL2CPP to stop injections & easy code decompilation",
						MessageType.Warning, true);
					GUILayout.Space(5f);
					if (GUILayout.Button(new GUIContent("Switch to IL2CPP")))
					{
						PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup,
							ScriptingImplementation.IL2CPP);
					}
				}

				GUILayout.Space(3);
			}
		}

		private static void DrawInjectionSection()
		{
			using (var changed = new EditorGUI.ChangeCheckScope())
			{
				var fold = GUITools.DrawFoldHeader("Injection Detector", ACTkEditorPrefsSettings.InjectionFoldout);
				if (changed.changed)
				{
					ACTkEditorPrefsSettings.InjectionFoldout = fold;
				}
			}

			if (!ACTkEditorPrefsSettings.InjectionFoldout)
			{
				return;
			}

			GUILayout.Space(-3f);

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				var enableInjectionDetector = ACTkSettings.Instance.InjectionDetectorEnabled;

				if (SettingsUtils.IsIL2CPPEnabled())
				{
					EditorGUILayout.HelpBox("Injection is not possible in IL2CPP,\n" +
					                        "this detector is not needed in IL2CPP builds", MessageType.Info, true);
					GUILayout.Space(5f);
				}
				else if (!InjectionRoutines.IsTargetPlatformCompatible())
				{
					EditorGUILayout.HelpBox(
						"Injection Detection is only supported in non-IL2CPP Standalone and Android builds",
						MessageType.Warning, true);
					GUILayout.Space(5f);
				}

				using (new GUILayout.HorizontalScope())
				{
					EditorGUI.BeginChangeCheck();
					enableInjectionDetector = EditorGUILayout.ToggleLeft(new GUIContent(
							"Add mono injection detection support to build",
							"Injection Detector checks assemblies against whitelist. " +
							"Please enable this option if you're using Injection Detector " +
							"and default whitelist will be generated while Unity builds resulting build.\n" +
							"Has no effect for IL2CPP or unsupported platforms."), enableInjectionDetector
					);
					if (EditorGUI.EndChangeCheck())
					{
						ACTkSettings.Instance.InjectionDetectorEnabled = enableInjectionDetector;
					}
				}

				GUILayout.Space(3);

				if (GUILayout.Button(new GUIContent(
					"Edit Custom Whitelist (" + ACTkSettings.Instance.InjectionDetectorWhiteList.Count + ")",
					"Fill any external assemblies which are not included into the project to the user-defined whitelist to make Injection Detector aware of them."))
				)
				{
					UserWhitelistEditor.ShowWindow();
				}

				GUILayout.Space(3);
			}
		}

#if UNITY_2018_1_OR_NEWER
		private static void DrawHashSection()
		{
			using (var changed = new EditorGUI.ChangeCheckScope())
			{
				var betaColor = ColorTools.GetPurpleString();
				var fold = GUITools.DrawFoldHeader("Code Hash Generator <color=#" + betaColor + ">BETA</color>", ACTkEditorPrefsSettings.HashFoldout);
				if (changed.changed)
				{
					ACTkEditorPrefsSettings.HashFoldout = fold;
				}
			}

			if (!ACTkEditorPrefsSettings.HashFoldout)
			{
				return;
			}

			GUILayout.Space(-3f);

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				var option = ACTkSettings.Instance.PreGenerateBuildHash;

				EditorGUI.BeginChangeCheck();
				option = EditorGUILayout.ToggleLeft(new GUIContent("Generate code hash on build completion", "Generates hash after build is finished, prints it to the console & sends it via CodeHashGeneratorPostprocessor."), option);
				if (EditorGUI.EndChangeCheck())
				{
					ACTkSettings.Instance.PreGenerateBuildHash = option;
				}

				EditorGUILayout.Space();

				GUILayout.Label("Can differ from runtime hash if you post-process code in resulting build (e.g. obfuscate, compress, etc.).", GUITools.RichLabel);
				
				GUILayout.Space(5f);
				EditorGUILayout.HelpBox("Always make sure post-build hash equals runtime one if you're using it for later comparison", 
					MessageType.Info, true);

				if (!CodeHashGenerator.IsTargetPlatformCompatible())
				{
					EditorGUILayout.HelpBox("Current platform is not supported: Windows or Android required", 
						MessageType.Warning, true);
				}

				GUILayout.Space(3);
			}
		}
#endif

		private static void DrawWallHackSection()
		{
			using (var changed = new EditorGUI.ChangeCheckScope())
			{
				var fold = GUITools.DrawFoldHeader("WallHack Detector", ACTkEditorPrefsSettings.WallHackFoldout);
				if (changed.changed)
				{
					ACTkEditorPrefsSettings.WallHackFoldout = fold;
				}
			}

			if (!ACTkEditorPrefsSettings.WallHackFoldout)
			{
				return;
			}

			GUILayout.Space(-3f);

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				GUILayout.Label(
					"Wireframe module uses own shader under the hood and it should be included into the build.",
					EditorStyles.wordWrappedLabel);

				ReadGraphicsAsset();

				if (graphicsSettingsAsset != null && includedShaders != null)
				{
					// outputs whole included shaders list, use for debug
					//EditorGUILayout.PropertyField(includedShaders, true);

					var shaderIndex = GetWallhackDetectorShaderIndex();

					EditorGUI.BeginChangeCheck();

					var status = shaderIndex != -1 ? ColorTools.GetGreenString() + ">included" : ColorTools.GetRedString() + ">not included";
					GUILayout.Label("Shader status: <color=#" + status + "</color>", GUITools.RichLabel);

					GUILayout.Space(5f);
					EditorGUILayout.HelpBox("You don't need to include it if you're not going to use Wireframe module", 
						MessageType.Info, true);
					GUILayout.Space(5f);

					if (shaderIndex != -1)
					{
						if (GUILayout.Button("Remove shader"))
						{
							includedShaders.DeleteArrayElementAtIndex(shaderIndex);
							includedShaders.DeleteArrayElementAtIndex(shaderIndex);
						}

						GUILayout.Space(3);
					}
					else
					{
						using (GUITools.Horizontal())
						{
							if (GUILayout.Button("Auto Include"))
							{
								var shader = Shader.Find(WallHackDetector.WireframeShaderName);
								if (shader != null)
								{
									includedShaders.InsertArrayElementAtIndex(includedShaders.arraySize);
									var newItem = includedShaders.GetArrayElementAtIndex(includedShaders.arraySize - 1);
									newItem.objectReferenceValue = shader;
								}
								else
								{
									Debug.LogError(ACTkConstants.LogPrefix + "Can't find " + WallHackDetector.WireframeShaderName +
									               " shader! Please report this to the  " +
									               ACTkEditorConstants.SupportEmail +
									               " including your Unity version number.");
								}
							}

							if (GUILayout.Button("Include manually (see readme.pdf)"))
							{
#if UNITY_2018_3_OR_NEWER
								SettingsService.OpenProjectSettings("Project/Graphics");
#else
								EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
#endif
							}
						}

						GUILayout.Space(3);
					}

					if (EditorGUI.EndChangeCheck())
					{
						graphicsSettingsAsset.ApplyModifiedProperties();
					}
				}
				else
				{
					GUILayout.Label("Can't automatically control " + WallHackDetector.WireframeShaderName +
					                " shader existence at the Always Included Shaders list. Please, manage this manually in Graphics Settings.");
					if (GUILayout.Button("Open Graphics Settings"))
					{
						EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
					}
				}
			}
		}

		private static void DrawConditionalSection()
		{
			var header = "Conditional Compilation Symbols";
			if (EditorApplication.isCompiling)
			{
				var redColor = ColorTools.GetRedString();
				header += " [<color=#" + redColor + ">compiling</color>]";
			}

			using (var changed = new EditorGUI.ChangeCheckScope())
			{
				var fold = GUITools.DrawFoldHeader(header, ACTkEditorPrefsSettings.ConditionalFoldout);
				if (changed.changed)
				{
					ACTkEditorPrefsSettings.ConditionalFoldout = fold;
				}
			}

			if (EditorApplication.isCompiling)
			{
				GUI.enabled = false;
			}

			if (!ACTkEditorPrefsSettings.ConditionalFoldout)
			{
				return;
			}

			GUILayout.Space(-3f);

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				GUILayout.Label("Here you may switch conditional compilation symbols used in ACTk.\n" +
				                "Check Readme for more details on each symbol.", EditorStyles.wordWrappedLabel);
				EditorGUILayout.Space();
				if (symbolsData == null)
				{
					symbolsData = SettingsUtils.GetSymbolsData();
				}

				/*if (GUILayout.Button("Reset"))
				{
					var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
					foreach (BuildTargetGroup buildTargetGroup in groups)
					{
						PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Empty);
					}
				}*/

				//using (GUITools.Horizontal())
				GUILayout.Label("Debug Symbols", GUITools.LargeBoldLabel);
				GUITools.Separator();

				DrawSymbol(ref symbolsData.injectionDebug, 
					ACTkEditorConstants.Conditionals.InjectionDebug,
					"Switches the Injection Detector debug.");
				DrawSymbol(ref symbolsData.injectionDebugVerbose,
					ACTkEditorConstants.Conditionals.InjectionDebugVerbose,
					"Switches the Injection Detector verbose debug level.");
				DrawSymbol(ref symbolsData.injectionDebugParanoid,
					ACTkEditorConstants.Conditionals.InjectionDebugParanoid,
					"Switches the Injection Detector paranoid debug level.");
				DrawSymbol(ref symbolsData.wallhackDebug, 
					ACTkEditorConstants.Conditionals.WallhackDebug,
					"Switches the WallHack Detector debug - you'll see the WallHack objects in scene and get extra information in console.");
				DrawSymbol(ref symbolsData.detectionBacklogs, 
					ACTkEditorConstants.Conditionals.DetectionBacklogs,
					"Enables additional logs in some detectors to make it easier to debug false positives.");

				EditorGUILayout.Space();

				GUILayout.Label("Compatibility Symbols", GUITools.LargeBoldLabel);
				GUITools.Separator();

				DrawSymbol(ref symbolsData.exposeThirdPartyIntegration,
					ACTkEditorConstants.Conditionals.ThirdPartyIntegration,
					"Enable to let other third-party code in project know you have ACTk added.");
				DrawSymbol(ref symbolsData.excludeObfuscation, 
					ACTkEditorConstants.Conditionals.ExcludeObfuscation,
					"Enable if you use Unity-unaware obfuscators which support ObfuscationAttribute to help avoid names corruption.");
				DrawSymbol(ref symbolsData.preventReadPhoneState,
					ACTkEditorConstants.Conditionals.PreventReadPhoneState,
					"Disables ObscuredPrefs Lock To Device functionality.");
				DrawSymbol(ref symbolsData.preventInternetPermission,
					ACTkEditorConstants.Conditionals.PreventInternetPermission,
					"Disables TimeCheatingDetector functionality.");
				DrawSymbol(ref symbolsData.obscuredAutoMigration,
					ACTkEditorConstants.Conditionals.ObscuredAutoMigration,
					"Enables automatic migration of ObscuredFloat and ObscuredDouble instances from the ACTk 1.5.2.0-1.5.8.0 to the 1.5.9.0+ format. Reduces these types performance a bit.");
				DrawSymbol(ref symbolsData.usExportCompatible,
					ACTkEditorConstants.Conditionals.UsExportCompatible,
					"Enables US Encryption Export Regulations compatibility mode so ACTk do not force you to declare you're using encryption when publishing your application to the Apple App Store.");

				GUILayout.Space(3);
			}

			GUI.enabled = true;
		}

		public static void ReadGraphicsAsset()
		{
			if (graphicsSettingsAsset != null) return;

			var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
			if (assets.Length > 0)
			{
				graphicsSettingsAsset = new SerializedObject(assets[0]);
			}

			if (graphicsSettingsAsset != null)
			{
				includedShaders = graphicsSettingsAsset.FindProperty("m_AlwaysIncludedShaders");
				if (includedShaders == null)
				{
					Debug.LogError(ACTkConstants.LogPrefix + "Couldn't find m_AlwaysIncludedShaders property, please report to " +
					               ACTkEditorConstants.SupportEmail);
				}
			}
			else
			{
				Debug.LogError(ACTkConstants.LogPrefix + "Couldn't find GraphicsSettings asset, please report to " + ACTkEditorConstants.SupportEmail);
			}
		}

		public static int GetWallhackDetectorShaderIndex()
		{
			if (graphicsSettingsAsset == null || includedShaders == null) return -1;

			var result = -1;
			graphicsSettingsAsset.Update();

			var itemsCount = includedShaders.arraySize;
			for (var i = 0; i < itemsCount; i++)
			{
				var arrayItem = includedShaders.GetArrayElementAtIndex(i);
				if (arrayItem.objectReferenceValue != null)
				{
					var shader = (Shader)(arrayItem.objectReferenceValue);

					if (shader.name == WallHackDetector.WireframeShaderName)
					{
						result = i;
						break;
					}
				}
			}

			return result;
		}

		public static bool IsWallhackDetectorShaderIncluded()
		{
			var result = false;

			ReadGraphicsAsset();
			if (GetWallhackDetectorShaderIndex() != -1)
				result = true;

			return result;
		}

		private static void DrawSymbol(ref bool field, string symbol, string hint)
		{
			EditorGUI.BeginChangeCheck();
			field = EditorGUILayout.ToggleLeft(new GUIContent(symbol, hint), field);
			if (EditorGUI.EndChangeCheck())
			{
				if (field)
				{
					SettingsUtils.SetSymbol(symbol);
				}
				else
				{
					SettingsUtils.RemoveSymbol(symbol);
				}

				symbolsData = SettingsUtils.GetSymbolsData();
			}
		}
	}
}