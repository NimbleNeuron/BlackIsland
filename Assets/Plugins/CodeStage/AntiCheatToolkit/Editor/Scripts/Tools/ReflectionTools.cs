#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	internal static class ReflectionTools
	{
		private static readonly Type ScriptingImplementationType = typeof(ScriptingImplementation);
		private static readonly Type BuildTargetGroupType = typeof(BuildTargetGroup);
		private static readonly Type InspectorWindowType = ScriptingImplementationType.Assembly.GetType("UnityEditor.Modules.ModuleManager", false);
		private static readonly Type ScriptingImplementationsType = ScriptingImplementationType.Assembly.GetType("UnityEditor.Modules.IScriptingImplementations", false);

		private delegate object GetScriptingImplementations(BuildTargetGroup target);

		private static GetScriptingImplementations getScriptingImplementationsDelegate;
		private static MethodInfo scriptingImplementationsTypeEnabledMethodInfo;

		public static bool IsScriptingImplementationSupported(ScriptingImplementation implementation, BuildTargetGroup target)
		{
			if (InspectorWindowType == null)
			{
				Debug.LogError(EditorTools.ConstructError("Couldn't find ModuleManager type!"));
				return false;
			}
			
			if (ScriptingImplementationsType == null)
			{
				Debug.LogError(EditorTools.ConstructError("Couldn't find IScriptingImplementationsType type!"));
				return false;
			}

			if (getScriptingImplementationsDelegate == null)
			{
				var mi = InspectorWindowType.GetMethod("GetScriptingImplementations", BindingFlags.Static | BindingFlags.NonPublic, Type.DefaultBinder, new []{BuildTargetGroupType}, null);
				if (mi == null)
				{
					Debug.LogError(EditorTools.ConstructError("Couldn't find GetScriptingImplementations method!"));
					return false;
				}
				getScriptingImplementationsDelegate = (GetScriptingImplementations)Delegate.CreateDelegate(typeof(GetScriptingImplementations), mi);
			}

			var result = getScriptingImplementationsDelegate.Invoke(target);
			if (result == null) // happens for default platform support module
			{
#if UNITY_2018_1_OR_NEWER
				return PlayerSettings.GetDefaultScriptingBackend(target) == implementation;
#else
				return true;
#endif
			}

			if (scriptingImplementationsTypeEnabledMethodInfo == null)
			{
				scriptingImplementationsTypeEnabledMethodInfo = ScriptingImplementationsType.GetMethod("Enabled", BindingFlags.Public | BindingFlags.Instance);
				if (scriptingImplementationsTypeEnabledMethodInfo == null)
				{
					Debug.LogError(EditorTools.ConstructError("Couldn't find IScriptingImplementations.Enabled() method!"));
					return false;
				}
			}

			var enabledImplementations = (ScriptingImplementation[])scriptingImplementationsTypeEnabledMethodInfo.Invoke(result, null);
			return Array.IndexOf(enabledImplementations, implementation) != -1;
		}
	}
}