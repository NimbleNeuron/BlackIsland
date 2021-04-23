#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System.IO;

	internal static class InjectionConstants
	{
		public const string LegacyWhitelistRelativePath = "InjectionDetectorData/UserWhitelist.bytes";

		public const string WhitelistFileName = "ACTkInjectionDetectorWhitelist.asset";
		public const string PrefsKey = "ACTDIDEnabledGlobal";
		public const string DataFileName = "fndid.bytes";
		public const string DataSeparator = ":";
		public const string ScriptAssembliesFolderRelative = "Library/ScriptAssemblies";

		public static readonly string ResourcesFolder = Path.Combine(ACTkEditorConstants.AssetsFolder, "Resources");
		public static readonly string ScriptAssembliesFolder = Path.Combine(ACTkEditorConstants.ProjectFolder, ScriptAssembliesFolderRelative);
		public static readonly string DataFilePath = Path.Combine(ResourcesFolder, DataFileName);
		public static readonly string DataFileMetaPath = DataFilePath + ".meta";
		public static readonly string DataFilePathRelative = DataFilePath.Replace(ACTkEditorConstants.ProjectFolder, "");
		public static readonly string DataFileMetaPathRelative = DataFilePathRelative + ".meta";
		public static readonly string WhitelistPath = Path.Combine(ACTkEditorConstants.ProjectSettingsFolder, WhitelistFileName);
	}
}