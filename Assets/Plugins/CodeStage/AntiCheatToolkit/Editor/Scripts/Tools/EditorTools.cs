#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("CodeStage.ACTk.Service")]

namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Common;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	internal static class EditorTools
	{
		#region files and directories

		private static string directory;

		public static void DeleteFile(string path)
		{
			if (!File.Exists(path)) return;
			RemoveReadOnlyAttribute(path);
			File.Delete(path);
		}

		public static void RemoveDirectoryIfEmpty(string directoryName)
		{
			if (Directory.Exists(directoryName) && IsDirectoryEmpty(directoryName))
			{
				FileUtil.DeleteFileOrDirectory(directoryName);
				var metaFile = directoryName + ".meta";
				if (File.Exists(metaFile))
				{
					FileUtil.DeleteFileOrDirectory(metaFile);
				}
			}
		}

		public static bool IsDirectoryEmpty(string path)
		{
			var dirs = Directory.GetDirectories(path);
			var files = Directory.GetFiles(path);
			return dirs.Length == 0 && files.Length == 0;
		}

		public static string GetACTkDirectory()
		{
			if (!string.IsNullOrEmpty(directory))
			{
				return directory;
			}

			directory = ACTkMarker.GetAssetPath();

			if (!string.IsNullOrEmpty(directory))
			{
				if (directory.IndexOf("Editor/Scripts/ACTkMarker.cs", StringComparison.Ordinal) >= 0)
				{
					directory = directory.Replace("Editor/Scripts/ACTkMarker.cs", "");
				}
				else
				{
					directory = null;
					Debug.LogError(ConstructError("Looks like Anti-Cheat Toolkit is placed in project incorrectly!"));
				}
			}
			else
			{
				directory = null;
				Debug.LogError(ConstructError("Can't locate the Anti-Cheat Toolkit directory!"));
			}
			return directory;
		}
		#endregion

		public static string ConstructError(string errorText)
		{
			return ACTkConstants.LogPrefix + errorText + "\nPlease report to " + ACTkEditorConstants.SupportEmail;
		}

		public static bool CheckUnityEventHasActivePersistentListener(SerializedProperty unityEvent)
		{
			if (unityEvent == null) return false;

			var calls = unityEvent.FindPropertyRelative("m_PersistentCalls.m_Calls");
			if (calls == null)
			{
				ConstructError("Can't find Unity Event calls!");
				return false;
			}
			if (!calls.isArray)
			{
				ConstructError("Looks like Unity Event calls are not array anymore!");
				return false;
			}

			var result = false;

			var callsCount = calls.arraySize;
			for (var i = 0; i < callsCount; i++)
			{
				var call = calls.GetArrayElementAtIndex(i);

				var targetProperty = call.FindPropertyRelative("m_Target");
				var methodNameProperty = call.FindPropertyRelative("m_MethodName");
				var callStateProperty = call.FindPropertyRelative("m_CallState");

				if (targetProperty != null && methodNameProperty != null && callStateProperty != null &&
                    targetProperty.propertyType == SerializedPropertyType.ObjectReference &&
					methodNameProperty.propertyType == SerializedPropertyType.String &&
					callStateProperty.propertyType == SerializedPropertyType.Enum)
				{
					var target = targetProperty.objectReferenceValue;
					var methodName = methodNameProperty.stringValue;
					var callState = (UnityEventCallState)callStateProperty.enumValueIndex;

					if (target != null && !string.IsNullOrEmpty(methodName) && callState != UnityEventCallState.Off)
					{
						result = true;
						break;
					}
				}
				else
				{
					ConstructError("Can't parse Unity Event call!");
				}
			}
			return result;
		}

		public static void RemoveReadOnlyAttribute(string path)
		{
			var attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
		}

		public static string[] FindLibrariesAt(string folder, bool recursive = true)
		{
			folder = folder.Replace('\\', '/');

			if (!Directory.Exists(folder))
			{
				return new string[0];
			}

			var allFiles = Directory.GetFiles(folder, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			var result = new List<string>();

#if !UNITY_2018_3_OR_NEWER
			var assembliesPathLowerCase = InjectionConstants.ScriptAssembliesFolder.ToLowerInvariant().Replace('\\', '/');
			var assetsPathLowerCase = ACTkEditorConstants.AssetsFolder.ToLowerInvariant().Replace('\\', '/');
#endif

			foreach (var file in allFiles)
			{
				var extension = Path.GetExtension(file);
				if (string.IsNullOrEmpty(extension))
				{
					continue;
				}

				if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
				{
					var path = file.Replace('\\', '/');

#if !UNITY_2018_3_OR_NEWER
					var fileName = Path.GetFileName(path);
					if (string.IsNullOrEmpty(fileName))
					{
						continue;
					}

					if (fileName.StartsWith("UnityEditor", StringComparison.Ordinal))
					{
						continue;
					}

					var pathLowerCase = path.ToLowerInvariant();

					if (pathLowerCase.Contains("/editor/") && pathLowerCase.IndexOf(assetsPathLowerCase, StringComparison.Ordinal) != -1)
					{
						continue;
					}

					if (pathLowerCase.EndsWith("-editor.dll") && pathLowerCase.IndexOf(assembliesPathLowerCase, StringComparison.Ordinal) != -1)
					{
						continue;
					}
#endif

					result.Add(path);
				}
			}

			return result.ToArray();
		}

		public static void OpenReadme()
		{
			var defaultReadmePath = Path.Combine(GetACTkDirectory(), "Readme.pdf");
			var loadedReadme = AssetDatabase.LoadMainAssetAtPath(defaultReadmePath);
			AssetDatabase.OpenAsset(loadedReadme);
		}
	}
}