#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_2018_3_OR_NEWER

namespace CodeStage.AntiCheat.EditorCode
{
	using System.Collections.Generic;
	using UnityEditor;

	internal class SettingsProviderWrapper
	{
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			var provider = new SettingsProvider(ACTkEditorConstants.SettingsProviderPath, SettingsScope.Project)
			{
				label = "Anti-Cheat Toolkit",
				guiHandler = searchContext =>
				{
					SettingsGUI.OnGUI();
				},

				keywords = new HashSet<string>(new[] {"focus", "codestage", "Anti", "Cheat", "Toolkit", "Injection", "Hash", "Wall", "Hack", "ACTk" })
			};

			return provider;
		}
	}
}

#endif