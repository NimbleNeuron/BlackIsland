﻿#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ACTk.Examples.Editor")]

namespace CodeStage.AntiCheat.EditorCode
{
	using System.IO;
	using System.Linq;
	using UnityEngine;

	internal static class ACTkEditorConstants
	{
		internal static class Conditionals
		{
			public const string InjectionDebug = "ACTK_INJECTION_DEBUG";
			public const string InjectionDebugVerbose = "ACTK_INJECTION_DEBUG_VERBOSE";
			public const string InjectionDebugParanoid = "ACTK_INJECTION_DEBUG_PARANOID";
			public const string WallhackDebug = "ACTK_WALLHACK_DEBUG";
			public const string ExcludeObfuscation = "ACTK_EXCLUDE_OBFUSCATION";
			public const string PreventReadPhoneState = "ACTK_PREVENT_READ_PHONE_STATE";
			public const string PreventInternetPermission = "ACTK_PREVENT_INTERNET_PERMISSION";
			public const string ObscuredAutoMigration = "ACTK_OBSCURED_AUTO_MIGRATION";
			public const string DetectionBacklogs = "ACTK_DETECTION_BACKLOGS";
			public const string ThirdPartyIntegration = "ACTK_IS_HERE";
			public const string UsExportCompatible = "ACTK_US_EXPORT_COMPATIBLE";
		}

#if UNITY_2018_3_OR_NEWER
		public const string SettingsProviderPath = "Code Stage/Anti-Cheat Toolkit";
#endif
		public const string MenuPath = "Code Stage/🕵 Anti-Cheat Toolkit/";
		public const string ToolsMenuPath = "Tools/" + MenuPath;
		public const string GameObjectMenuPath = "GameObject/Create Other/" + MenuPath;
		public const string SupportEmail = "support@codestage.net";

		public static readonly string ProjectFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));
		public static readonly string ProjectTempFolder = Path.Combine(ProjectFolder, "Temp");
		public static readonly string ProjectLibraryFolder = Path.Combine(ProjectFolder, "Library");
		public static readonly string ProjectSettingsFolder = Path.Combine(ProjectFolder, "ProjectSettings");
		public static readonly string AssetsFolder = Path.Combine(ProjectFolder, "Assets");
		
		public static readonly string[] HexTable = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();
	}
}