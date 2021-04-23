#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using Common;
	using System;
	using System.Collections.Generic;
	using System.IO;
	
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	[Serializable]
	public class ACTkSettings : ScriptableObject
	{
		private const string Directory = "ProjectSettings";
		private const string Path = Directory + "/ACTkSettings.asset";

		[SerializeField]
		private bool injectionDetectorEnabled;

		[SerializeField]
		private bool preGenerateBuildHash;

		[SerializeField]
		private bool disableInjectionDetectorValidation;

		[SerializeField]
		private bool disableWallhackDetectorValidation;

		[SerializeField]
		private List<AllowedAssembly> injectionDetectorWhiteList = new List<AllowedAssembly>();

		[SerializeField]
		private string version = ACTkConstants.Version;

		private static ACTkSettings instance;
		public static ACTkSettings Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = LoadOrCreate();
				return instance;
			}
		}

		public static void Show()
		{
#if UNITY_2018_3_OR_NEWER
			SettingsService.OpenProjectSettings(ACTkEditorConstants.SettingsProviderPath);
#else
			try
			{
				SettingsPreferenceItem.OpenPreferences();
			}
			catch (Exception)
			{
				EditorApplication.ExecuteMenuItem("Edit/Preferences...");
			}
#endif
		}

		public bool InjectionDetectorEnabled
		{
			get { return injectionDetectorEnabled; }
			set
			{
				injectionDetectorEnabled = value;
				Save();
			}
		}
		
		public bool PreGenerateBuildHash
		{
			get { return preGenerateBuildHash; }
			set
			{
				preGenerateBuildHash = value;
				Save();
			}
		}
		
		public bool DisableInjectionDetectorValidation
		{
			get { return disableInjectionDetectorValidation; }
			set
			{
				disableInjectionDetectorValidation = value;
				Save();
			}
		}
		
		public bool DisableWallhackDetectorValidation
		{
			get { return disableWallhackDetectorValidation; }
			set
			{
				disableWallhackDetectorValidation = value;
				Save();
			}
		}
		
		public List<AllowedAssembly> InjectionDetectorWhiteList
		{
			get { return injectionDetectorWhiteList; }
			set
			{
				injectionDetectorWhiteList = value;
				Save();
			}
		}

		public static void Delete()
		{
			instance = null;
			EditorTools.DeleteFile(Path);
		}

		public static void Save()
		{
			SaveInstance(Instance);
		}

		private static ACTkSettings LoadOrCreate()
		{
			ACTkSettings settings;

			if (!File.Exists(Path))
			{
				settings = CreateNewSettingsFile();
			}
			else
			{
				settings = LoadInstance();

				if (settings == null)
				{
					EditorTools.DeleteFile(Path);
					settings = CreateNewSettingsFile();
				}

				if (settings.version != ACTkConstants.Version)
				{
					// for future migration reference
				}
			}

			settings.hideFlags = HideFlags.HideAndDontSave;
			settings.version = ACTkConstants.Version;

			return settings;
		}

		private static ACTkSettings CreateNewSettingsFile()
		{
			var settingsInstance = CreateInstance();
			SaveInstance(settingsInstance);
			return settingsInstance;
		}

		private static void SaveInstance(ACTkSettings settingsInstance)
		{
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

			try
			{
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { settingsInstance }, Path, true);
			}
			catch (Exception ex)
			{
				Debug.LogError(EditorTools.ConstructError("Can't save settings!\n" + ex));
			}
		}

		private static ACTkSettings LoadInstance()
		{
			ACTkSettings settingsInstance;

			try
			{
				settingsInstance = (ACTkSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex)
			{
				Debug.Log(ACTkConstants.LogPrefix + "Can't read settings, resetting them to defaults.\nThis is a harmless message in most cases and can be ignored.\n" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static ACTkSettings CreateInstance()
		{
			var newInstance = CreateInstance<ACTkSettings>();
			return newInstance;
		}
	}
}