#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	internal class TextureTools
	{
		private static readonly Dictionary<string, Texture2D> CachedTextures = new Dictionary<string, Texture2D>();

		public static Texture2D GetTexture(string fileName)
		{
			return GetTexture(fileName, false, false);
		}

		public static Texture2D GetIconTexture(string fileName, bool fromEditor = false)
		{
			return GetTexture(fileName, true, fromEditor);
		}

		private static Texture2D GetTexture(string fileName, bool icon, bool fromEditor)
		{
			Texture2D result;
			var isDark = EditorGUIUtility.isProSkin;

			var path = fileName;

			if (!fromEditor)
			{
				path = Path.Combine(EditorTools.GetACTkDirectory(), "Editor/Textures/For" + (isDark ? "Dark/" : "Bright/") + (icon ? "Icons/" : "") + fileName);
			}

			if (CachedTextures.ContainsKey(path))
			{
				result = CachedTextures[path];
			}
			else
			{
				if (!fromEditor)
				{
					result = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
				}
				else
				{
					result = EditorGUIUtility.FindTexture(path);
				}

				if (result == null)
				{
					Debug.LogError(EditorTools.ConstructError("Some error occurred while looking for image\n" + path));
				}
				else
				{
					CachedTextures[path] = result;
				}
			}
			return result;
		}
	}

	internal class Icons
	{
		public static Texture API { get { return TextureTools.GetIconTexture("API.png"); } }
		public static Texture Forum { get { return TextureTools.GetIconTexture("Forum.png"); } }
		public static Texture Manual { get { return TextureTools.GetIconTexture("Manual.png"); } }
		public static Texture Help { get { return TextureTools.GetIconTexture("Help.png"); } }
		public static Texture Home { get { return TextureTools.GetIconTexture("Home.png"); } }
		public static Texture Support { get { return TextureTools.GetIconTexture("Support.png"); } }
		public static Texture Star { get { return TextureTools.GetIconTexture("Star.png"); } }
	}

	internal class Images
	{
		public static Texture Logo { get { return TextureTools.GetTexture("Logo.png"); } }
	}
}