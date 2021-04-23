using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BSER.Integration
{
	[CreateAssetMenu(menuName = "BSER/Create GUIDReplacer", fileName = "GUIDReplacer", order = 20)]
	public class GUIDReplacer : ScriptableObject
	{
		public bool isActive = true;

		public Object willBeRemoved = default;
		public Object newGUIDObject = default;

		public string[] types = new[] {"ScriptableObject"};
		
		private void TryProcess()
		{
			if (!isActive) return;
			
			bool result = DoProcess();
			// isActive = !result;
		}
		
		private string OldGuid {
			get => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(willBeRemoved));
		}

		private string NewGuid
		{
			get => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newGUIDObject));
		}

		private bool DoProcess()
		{
			string oldGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(willBeRemoved));
			string newGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newGUIDObject));

			foreach (string type in types)
			{
				string[] soGroup = AssetDatabase.FindAssets($"t:{type}")
					.Select(AssetDatabase.GUIDToAssetPath)
					.ToArray();

				Debug.Log($"Old guid : {oldGUID}");
				Debug.Log($"new guid : {newGUID}");

				try
				{
					int length = soGroup.Length;
					for (int i = 0; i < length; i++)
					{
						string so = soGroup[i];

						EditorUtility.DisplayProgressBar("Processing...",
							$"{so}", (float) i / length);

						string read = File.ReadAllText(so);
						if (read.Contains(oldGUID))
						{
							Debug.Log(so);
							
							// string replaced = read.Replace(oldGUID, newGUID);
							// File.WriteAllText(so, replaced);
							//
							// AssetDatabase.ImportAsset(so);
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					return false;
				}
			}
			
			EditorUtility.SetDirty(this);

			return true;
		}

		[MenuItem("Editor/ERBS/Process ALL GUID Replacer")]
		private static void ProcessAll()
		{
			var objects = AssetDatabase.FindAssets("t:GUIDReplacer")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<GUIDReplacer>)
				.Where(e => e.isActive)
				.ToArray();
			
			Debug.Log($"Found profile : {objects.Length}");

			AssetDatabase.StartAssetEditing();

			Dictionary<string[], List<GUIDReplacer>> map = new Dictionary<string[], List<GUIDReplacer>>();
			
			foreach (GUIDReplacer profile in objects)
			{
				if (!map.TryGetValue(profile.types, out var list))
				{
					list = new List<GUIDReplacer>();
				}
				
				list.Add(profile);
				map[profile.types] = list;
			}

			foreach (var pair in map)
			{
				string[] types = pair.Key;

				foreach (string type in types)
				{
					string[] soGroup = AssetDatabase.FindAssets($"t:{type}")
						.Select(AssetDatabase.GUIDToAssetPath)
						.ToArray();
					
					int length = soGroup.Length;
					for (int i = 0; i < length; i++)
					{
						string so = soGroup[i];

						EditorUtility.DisplayProgressBar($"{type}...",
							$"{so}", (float) i / length);

						bool isDirty = false;

						foreach (GUIDReplacer profile in pair.Value)
						{
							if (!profile.isActive) continue;
							
							string read = File.ReadAllText(so);
							if (read.Contains(profile.OldGuid))
							{
								string replaced = read.Replace(profile.OldGuid, profile.NewGuid);
								File.WriteAllText(so, replaced);

								profile.isActive = false;
								EditorUtility.SetDirty(profile);
								isDirty = true;
							}
						}

						if (isDirty)
							AssetDatabase.ImportAsset(so);
					}
				}
			}
			
			AssetDatabase.StopAssetEditing();
			EditorUtility.ClearProgressBar();
			
			AssetDatabase.SaveAssets();
		}

		[MenuItem("Editor/ERBS/Auto Generate")]
		private static void Generate()
		{
			Object selected = Selection.activeObject;
			if (selected is DefaultAsset folder)
			{
				string rootPath = AssetDatabase.GetAssetPath(folder);
				var entry = new List<FileInfo>();
				
				Find(entry, new DirectoryInfo(rootPath));

				var mono = new List<FileInfo>();
				var so = new List<FileInfo>();

				foreach (FileInfo script in entry)
				{
					string read = File.ReadAllText(script.FullName);

					if (read.Contains(": MonoBehaviour"))
					{
						mono.Add(script);
					}
					else if (read.Contains(": ScriptableObject"))
					{
						so.Add(script);
					}
				}
				
				AssetDatabase.StartAssetEditing();

				foreach (FileInfo mInfo in mono)
				{
					Debug.Log($"MONO : {mInfo.FullName}");

					GUIDReplacer instance = CreateInstance<GUIDReplacer>();
					instance.willBeRemoved = AssetDatabase.LoadAssetAtPath<Object>(mInfo.FullName);
					
					AssetDatabase.CreateAsset(instance, $"Assets/Integration/MLAgents/{mInfo.Name}".Replace(".cs", ".asset"));
				}
				
				foreach (FileInfo mInfo in so)
				{
					Debug.Log($"SO : {mInfo.FullName}");
					
					GUIDReplacer instance = CreateInstance<GUIDReplacer>();
					instance.willBeRemoved = AssetDatabase.LoadAssetAtPath<Object>(mInfo.FullName);
					
					AssetDatabase.CreateAsset(instance, $"Assets/Integration/MLAgents/{mInfo.Name}".Replace(".cs", ".asset"));
				}
				
				AssetDatabase.StopAssetEditing();
				AssetDatabase.SaveAssets();
			}
		}

		private static void Find(List<FileInfo> entry, DirectoryInfo dir)
		{
			var files = dir.GetFiles("*.cs");
			entry.AddRange(files);

			var subDirs = dir.GetDirectories();
			foreach (DirectoryInfo subDir in subDirs)
			{
				Find(entry, subDir);
			}
		}
	}
}