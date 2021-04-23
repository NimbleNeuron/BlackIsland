#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if !UNITY_2018_3_OR_NEWER

namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using System.Collections;
	using System.Reflection;
	using UnityEditor;

	internal class SettingsPreferenceItem
	{
		[PreferenceItem("Anti-Cheat Toolkit")]
		public static void PreferencesGUI()
		{
			SettingsGUI.OnGUI();
		}

		public static void OpenPreferences()
		{
			OpenPreference(typeof(SettingsPreferenceItem));
		}

		// original code by ChemiKhazi:
		// https://gist.github.com/ChemiKhazi/3363ddd715c6479f539cc6e34ccb8f42
		// modified by focus, Code Stage
		private static void OpenPreference(Type targetClass)
		{
			// Find the assemblies needed to access internal Unity classes
			Type tEditorAssembly = null;
			Type tAssemblyHelper = null;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (tEditorAssembly == null)
				{
					var tempEditorAssembly = assembly.GetType("UnityEditor.EditorAssemblies");
					if (tempEditorAssembly != null)
						tEditorAssembly = tempEditorAssembly;
				}
				if (tAssemblyHelper == null)
				{
					var tempAssemblyHelper = assembly.GetType("UnityEditor.AssemblyHelper");
					if (tempAssemblyHelper != null)
						tAssemblyHelper = tempAssemblyHelper;
				}
				if (tEditorAssembly != null && tAssemblyHelper != null)
					break;
			}

			if (tEditorAssembly == null || tAssemblyHelper == null)
				return;

			var nonPublicStatic = BindingFlags.Static | BindingFlags.NonPublic;

			var loadedAssemblyProp = tEditorAssembly.GetProperty("loadedAssemblies", nonPublicStatic);
			
			var loadedAssemblies = loadedAssemblyProp.GetValue(null, null) as IList;
			var methodGetTypes = tAssemblyHelper.GetMethod("GetTypesFromAssembly", nonPublicStatic);

			if (loadedAssemblies == null || methodGetTypes == null)
				return;

			var targetIndex = -1;
			var totalCustomSections = 0;

			// This reconstructs PreferenceWindow.AddCustomSections() as reflection calls
			foreach (var loadedAssemblyObj in loadedAssemblies)
			{
				var assembly = loadedAssemblyObj as Assembly;
				if (assembly == null)
					continue;
				var types = methodGetTypes.Invoke(null, new object[]{assembly}) as IList;
				if (types == null)
					continue;

				foreach (var typeObj in types)
				{
					var type = typeObj as Type;
					if (type == null)
						continue;
					foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						var preferenceItem = Attribute.GetCustomAttribute(method, typeof(PreferenceItem)) as PreferenceItem;
						if (preferenceItem != null)
						{
							if (type == targetClass)
								targetIndex = totalCustomSections;
							totalCustomSections++;
						}
					}
				}
			}

			if (targetIndex < 0)
				return;

			// Opening preference window, taken from here
			// http://answers.unity3d.com/questions/473949/open-editpreferences-from-code.html
			var asm = Assembly.GetAssembly(typeof(EditorWindow));
			var tPrefsWindow = asm.GetType("UnityEditor.PreferencesWindow");
			var methodShow = tPrefsWindow.GetMethod("ShowPreferencesWindow", nonPublicStatic);
			methodShow.Invoke(null, null);

			// Need to wait a few frames to let preference window build itself
			var frameWait = 3;
			EditorApplication.CallbackFunction waitCallback = null;
			waitCallback = () =>
			{
				frameWait--;
				if (frameWait > 0)
					return;
				EditorApplication.update -= waitCallback;

				var prefWindow = EditorWindow.GetWindow(tPrefsWindow);

				var nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

				var fieldSections = tPrefsWindow.GetField("m_Sections", nonPublicInstance);
				var sectionCount = (fieldSections.GetValue(prefWindow) as IList).Count;
				var startIdx = sectionCount - totalCustomSections;
				
				var propSectionIndex = tPrefsWindow.GetProperty("selectedSectionIndex", nonPublicInstance);
				propSectionIndex.SetValue(prefWindow, startIdx + targetIndex, null);
			};

			EditorApplication.update += waitCallback;
		}
	}
}

#endif