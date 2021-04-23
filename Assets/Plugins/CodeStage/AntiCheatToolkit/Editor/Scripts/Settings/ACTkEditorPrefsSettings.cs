#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using UnityEditor;

	internal static class ACTkEditorPrefsSettings
	{
		private const string PrefsPrefix = "ACTkSettings_";

		private const string IL2CPPFoldoutPref = PrefsPrefix + "IL2CPPFoldout";
		private const string InjectionFoldoutPref = PrefsPrefix + "injectionFoldout";
		private const string HashFoldoutPref = PrefsPrefix + "hashFoldout";
		private const string WallHackFoldoutPref = PrefsPrefix + "wallHackFoldout";
		private const string ConditionalFoldoutPref = PrefsPrefix + "conditionalFoldout";

		private static bool? il2cppFoldout;
		private static bool? injectionFoldout;
		private static bool? hashFoldout;
		private static bool? wallHackFoldout;
		private static bool? conditionalFoldout;

		public static bool IL2CPPFoldout
		{
			get
			{
				if (il2cppFoldout == null)
				{
					il2cppFoldout = EditorPrefs.GetBool(IL2CPPFoldoutPref);
				}
				return (bool)il2cppFoldout;
			}
			set
			{
				il2cppFoldout = value;
				EditorPrefs.SetBool(IL2CPPFoldoutPref, (bool)il2cppFoldout);
			}
		}

		public static bool InjectionFoldout
		{
			get
			{
				if (injectionFoldout == null)
				{
					injectionFoldout = EditorPrefs.GetBool(InjectionFoldoutPref);
				}
				return (bool)injectionFoldout;
			}
			set
			{
				injectionFoldout = value;
				EditorPrefs.SetBool(InjectionFoldoutPref, (bool)injectionFoldout);
			}
		}
		
		public static bool HashFoldout
		{
			get
			{
				if (hashFoldout == null)
				{
					hashFoldout = EditorPrefs.GetBool(HashFoldoutPref);
				}
				return (bool)hashFoldout;
			}
			set
			{
				hashFoldout = value;
				EditorPrefs.SetBool(HashFoldoutPref, (bool)hashFoldout);
			}
		}
		
		public static bool WallHackFoldout
		{
			get
			{
				if (wallHackFoldout == null)
				{
					wallHackFoldout = EditorPrefs.GetBool(WallHackFoldoutPref);
				}
				return (bool)wallHackFoldout;
			}
			set
			{
				wallHackFoldout = value;
				EditorPrefs.SetBool(WallHackFoldoutPref, (bool)wallHackFoldout);
			}
		}
		
		public static bool ConditionalFoldout
		{
			get
			{
				if (conditionalFoldout == null)
				{
					conditionalFoldout = EditorPrefs.GetBool(ConditionalFoldoutPref);
				}
				return (bool)conditionalFoldout;
			}
			set
			{
				conditionalFoldout = value;
				EditorPrefs.SetBool(ConditionalFoldoutPref, (bool)conditionalFoldout);
			}
		}
	}
}