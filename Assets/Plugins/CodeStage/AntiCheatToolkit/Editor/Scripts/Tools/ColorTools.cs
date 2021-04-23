#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using UnityEditor;
	using UnityEngine;

	internal static class ColorTools
	{
		public static string GetGreenString()
		{
			return EditorGUIUtility.isProSkin ? "02C85F" : "02981F";
		}

		public static string GetPurpleString()
		{
			return EditorGUIUtility.isProSkin ? "A76ED1" : "7030A0";
		}

		public static string GetRedString()
		{
			return EditorGUIUtility.isProSkin ? "FF6060" : "FF1010";
		}

		public static Color GetGreen()
		{
			return EditorGUIUtility.isProSkin ? new Color32(2, 200, 95, 255) : new Color32(2, 152, 31, 255);
		}

		public static Color GetPurple()
		{
			return EditorGUIUtility.isProSkin ? new Color32(167, 110, 209, 255) : new Color32(112, 48, 160, 255);
		}

		public static Color GetRed()
		{
			return EditorGUIUtility.isProSkin ? new Color32(255, 96, 96, 255) : new Color32(255, 16, 16, 255);
		}
	}
}