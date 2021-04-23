#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using Common;
	using Storage;

	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEngine;
#if UNITY_EDITOR_WIN
	using Microsoft.Win32;
#elif UNITY_EDITOR_OSX
	using System.IO;
#else // LINUX
	using System.IO;
	using System.Xml;
#endif

	internal class ACTkPrefsEditor : EditorWindow
	{
		protected const int RecordsPerPage = 50;

		internal const string UnknownValueDescription = "Value corrupted / wrong Unity version";
		internal const string UnsupportedValueDescription = "Not editable value";
		protected const string StringTooLong = "String is too long";

		internal static ACTkPrefsEditor instance;

		// 180, 255, 180, 255
		// note the 2 alpha - it's to make disabled components look as usual
		private readonly Color obscuredColor = new Color(0.706f, 1f, 0.706f, 2f);

		[SerializeField]
		private SortingType sortingType = SortingType.KeyAscending;

		[SerializeField]
		private string searchPattern;

		[SerializeField]
		private List<PrefsRecord> allRecords;

		[SerializeField]
		private List<PrefsRecord> filteredRecords;

		[SerializeField]
		private Vector2 scrollPosition;

		[SerializeField]
		private int recordsCurrentPage;

		[SerializeField]
		private int recordsTotalPages;

		[SerializeField]
		private bool addingNewRecord;

		[SerializeField]
		private int newRecordType;

		[SerializeField]
		private bool newRecordEncrypted;

		[SerializeField]
		private string newRecordKey;

		[SerializeField]
		private string newRecordStringValue;

		[SerializeField]
		private int newRecordIntValue;

		[SerializeField]
		private float newRecordFloatValue;

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Prefs Editor as Tab...", false, 500)]
		internal static void ShowWindow()
		{
			var myself = GetWindow<ACTkPrefsEditor>(false, "Prefs Editor", true);
			myself.minSize = new Vector2(500, 300);
			myself.RefreshData();
		}

		[MenuItem(ACTkEditorConstants.ToolsMenuPath + "Prefs Editor as Utility...", false, 501)]
		internal static void ShowWindowUtility()
		{
			var myself = GetWindow<ACTkPrefsEditor>(true, "Prefs Editor", true);
			myself.minSize = new Vector2(500, 300);
			myself.RefreshData();
		}

		[DidReloadScripts]
		private static void OnRecompile()
		{ 
			if (instance) instance.Repaint();
		}
		 
		private void OnEnable()
		{
			instance = this;
			//RefreshData();
		}

		#region GUI

		// ------------------------------------------------------------------
		// GUI
		// ------------------------------------------------------------------

		private void OnGUI()
		{
			if (allRecords == null) allRecords = new List<PrefsRecord>();
			if (filteredRecords == null) filteredRecords = new List<PrefsRecord>();

			using (GUITools.Horizontal(GUITools.Toolbar))
			{
				if (GUILayout.Button(new GUIContent("+", "Create new prefs record."), EditorStyles.toolbarButton, GUILayout.Width(20)))
				{
					addingNewRecord = true;
				}

				if (GUILayout.Button(new GUIContent("Refresh", "Re-read and re-parse all prefs."), EditorStyles.toolbarButton, GUILayout.Width(50)))
				{
					RefreshData();
					GUIUtility.keyboardControl = 0;
					scrollPosition = Vector2.zero;
					recordsCurrentPage = 0;
				}

				EditorGUI.BeginChangeCheck();
				sortingType = (SortingType)EditorGUILayout.EnumPopup(sortingType, EditorStyles.toolbarDropDown, GUILayout.Width(110));
				if (EditorGUI.EndChangeCheck())
				{
					ApplySorting();
				}

				GUILayout.Space(10);

				EditorGUI.BeginChangeCheck();
				searchPattern = GUITools.SearchToolbar(searchPattern);
				if (EditorGUI.EndChangeCheck())
				{
					ApplyFiltering();
				}
			}

			if (addingNewRecord)
			{
				using (GUITools.Horizontal(GUITools.PanelWithBackground))
				{
					string[] types = {"String", "Int", "Float"};
					newRecordType = EditorGUILayout.Popup(newRecordType, types, GUILayout.Width(50));

					newRecordEncrypted = GUILayout.Toggle(newRecordEncrypted, new GUIContent("E", "Create new pref as encrypted ObscuredPref?"), GUITools.CompactButton, GUILayout.Width(25));

					var guiColor = GUI.color;
					if (newRecordEncrypted)
					{
						GUI.color = obscuredColor;
					}

					GUILayout.Label("Key:", GUILayout.ExpandWidth(false));
					newRecordKey = EditorGUILayout.TextField(newRecordKey);
					GUILayout.Label("Value:", GUILayout.ExpandWidth(false));

					if (newRecordType == 0)
					{
						newRecordStringValue = EditorGUILayout.TextField(newRecordStringValue);
					}
					else if (newRecordType == 1)
					{
						newRecordIntValue = EditorGUILayout.IntField(newRecordIntValue);
					}
					else
					{
						newRecordFloatValue = EditorGUILayout.FloatField(newRecordFloatValue);
					}

					GUI.color = guiColor;

					if (GUILayout.Button("OK", GUITools.CompactButton, GUILayout.Width(30)))
					{
						if (string.IsNullOrEmpty(newRecordKey) ||
						    (newRecordType == 0 && string.IsNullOrEmpty(newRecordStringValue)) ||
						    (newRecordType == 1 && newRecordIntValue == 0) ||
						    (newRecordType == 2 && Math.Abs(newRecordFloatValue) < 0.00000001f))
						{
							ShowNotification(new GUIContent("Please fill in the pref first!"));
						}
						else
						{
							PrefsRecord newRecord;

							if (newRecordType == 0)
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordStringValue, newRecordEncrypted);
							}
							else if (newRecordType == 1)
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordIntValue, newRecordEncrypted);
							}
							else
							{
								newRecord = new PrefsRecord(newRecordKey, newRecordFloatValue, newRecordEncrypted);
							}

							if (newRecord.Save())
							{
								allRecords.Add(newRecord);
								ApplySorting();
								CloseNewRecordPanel();
							}
						}
					}

					if (GUILayout.Button("Cancel", GUITools.CompactButton, GUILayout.Width(60)))
					{
						CloseNewRecordPanel();
					}
				}
			}

			using (GUITools.Vertical(GUITools.PanelWithBackground))
			{
				GUILayout.Space(5);

				DrawRecordsPages();

				GUILayout.Space(5);

				GUI.enabled = filteredRecords.Count > 0;
				using (GUITools.Horizontal())
				{
					if (GUILayout.Button("Encrypt ALL", GUITools.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Obscure ALL prefs in list?", "This will apply obscuration to ALL unobscured prefs in the list.\nAre you sure you wish to do this?", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Encrypt();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Decrypt ALL", GUITools.CompactButton))
					{
						if (EditorUtility.DisplayDialog("UnObscure ALL prefs in list?", "This will remove obscuration from ALL obscured prefs in the list if possible.\nAre you sure you wish to do this?", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Decrypt();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Save ALL", GUITools.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Save changes to ALL prefs in list?", "Are you sure you wish to save changes to ALL prefs in the list? This can't be undone!", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Save();
							}
							GUIUtility.keyboardControl = 0;
							ApplySorting();
						}
					}

					if (GUILayout.Button("Delete ALL", GUITools.CompactButton))
					{
						if (EditorUtility.DisplayDialog("Delete ALL prefs in list?", "Are you sure you wish to delete the ALL prefs in the list? This can't be undone!", "Yep", "Oh, no!"))
						{
							foreach (var record in filteredRecords)
							{
								record.Delete();
							}
							
							RefreshData();
							GUIUtility.keyboardControl = 0;
						}
					}
				}
				GUI.enabled = true;
			}
		}

		private void CloseNewRecordPanel()
		{
			addingNewRecord = false;
			newRecordKey = string.Empty;
			newRecordStringValue = string.Empty;
			newRecordIntValue = 0;
			newRecordFloatValue = 0;
			GUIUtility.keyboardControl = 0;
		}

		private void DrawRecordsPages()
		{
			recordsTotalPages = Math.Max(1,(int)Math.Ceiling((double)filteredRecords.Count / RecordsPerPage));

			if (recordsCurrentPage < 0) recordsCurrentPage = 0;
			if (recordsCurrentPage + 1 > recordsTotalPages) recordsCurrentPage = recordsTotalPages - 1;

			var fromRecord = recordsCurrentPage * RecordsPerPage;
			var toRecord = fromRecord + Math.Min(RecordsPerPage, filteredRecords.Count - fromRecord);

			if (recordsTotalPages > 1)
			{
				GUILayout.Label("Prefs " + fromRecord + " - " + toRecord + " from " + filteredRecords.Count);
			}

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for (var i = fromRecord; i < toRecord; i++)
			{
				bool recordRemoved;
				DrawRecord(i, out recordRemoved);
				if (recordRemoved)
				{
					break;
				}
			}
			GUILayout.EndScrollView();

			if (recordsTotalPages <= 1) return;

			GUILayout.Space(5);
			using (GUITools.Horizontal())
			{
				GUILayout.FlexibleSpace();

				GUI.enabled = recordsCurrentPage > 0;
				if (GUILayout.Button("<<", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage = 0;
					scrollPosition = Vector2.zero;
				}
				if (GUILayout.Button("<", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage--;
					scrollPosition = Vector2.zero;
				}
				GUI.enabled = true;
				GUILayout.Label(recordsCurrentPage + 1 + " of " + recordsTotalPages, GUITools.CenteredLabel, GUILayout.Width(100));
				GUI.enabled = recordsCurrentPage < recordsTotalPages - 1;
				if (GUILayout.Button(">", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage++;
					scrollPosition = Vector2.zero;
				}
				if (GUILayout.Button(">>", GUILayout.Width(50)))
				{
					RemoveNotification();
					recordsCurrentPage = recordsTotalPages - 1;
					scrollPosition = Vector2.zero;
				}
				GUI.enabled = true;

				GUILayout.FlexibleSpace();
			}
		}

		protected void DrawRecord(int recordIndex, out bool recordRemoved)
		{
			recordRemoved = false;
			var record = filteredRecords[recordIndex];

			GUITools.Separator();

			using (GUITools.Horizontal(GUITools.PanelWithBackground))
			{
				if (GUILayout.Button(new GUIContent("X", "Delete this pref."), GUITools.CompactButton, GUILayout.Width(20)))
				{
					record.Delete();
					allRecords.Remove(record);
					filteredRecords.Remove(record);
					recordRemoved = true;
					return;
				}

				GUI.enabled = (record.dirtyValue || record.dirtyKey) && record.prefType != PrefsRecord.PrefsType.Unknown;
				if (GUILayout.Button(new GUIContent("S", "Save changes in this pref."), GUITools.CompactButton, GUILayout.Width(20)))
				{
					record.Save();
					GUIUtility.keyboardControl = 0;
				}

				GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;

				if (record.Obscured)
				{
					GUI.enabled &= record.obscuredType == ObscuredPrefs.DataType.String ||
								   record.obscuredType == ObscuredPrefs.DataType.Int ||
								   record.obscuredType == ObscuredPrefs.DataType.Float;
					if (GUILayout.Button(new GUIContent("D", "Decrypt this pref using ObscuredPrefs"), GUITools.CompactButton, GUILayout.Width(25)))
					{
						record.Decrypt();
						GUIUtility.keyboardControl = 0;
					}
				}
				else
				{
					if (GUILayout.Button(new GUIContent("E", "Encrypt this pref using ObscuredPrefs"), GUITools.CompactButton, GUILayout.Width(25)))
					{
						record.Encrypt();
						GUIUtility.keyboardControl = 0;
					}
				}
				GUI.enabled = true;

				if (GUILayout.Button(new GUIContent("...", "Other operations"), GUITools.CompactButton, GUILayout.Width(25)))
				{
					ShowOtherMenu(record);
				}

				var guiColor = GUI.color;
				if (record.Obscured)
				{
					GUI.color = obscuredColor;
				}

				GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;

				if (record.Obscured && !(record.obscuredType == ObscuredPrefs.DataType.String ||
				                         record.obscuredType == ObscuredPrefs.DataType.Int ||
				                         record.obscuredType == ObscuredPrefs.DataType.Float))
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(record.Key, GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				else
				{
					record.Key = EditorGUILayout.TextField(record.Key, GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
				}
				
				if ((record.prefType == PrefsRecord.PrefsType.String && !record.Obscured) || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.String))
				{
					// to avoid TextMeshGenerator error because of too much characters
					if (record.StringValue.Length > 16382)
					{
						GUI.enabled = false;
						EditorGUILayout.TextField(StringTooLong, GUILayout.MinWidth(150));
						GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
					}
					else
					{
						record.StringValue = EditorGUILayout.TextField(record.StringValue, GUILayout.MinWidth(150));
					}
				}
				else if (record.prefType == PrefsRecord.PrefsType.Int || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.Int))
				{
					record.IntValue = EditorGUILayout.IntField(record.IntValue, GUILayout.MinWidth(150));
				}
				else if (record.prefType == PrefsRecord.PrefsType.Float || (record.Obscured && record.obscuredType == ObscuredPrefs.DataType.Float))
				{
					record.FloatValue = EditorGUILayout.FloatField(record.FloatValue, GUILayout.MinWidth(150));
				}
				else if (record.Obscured)
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(UnsupportedValueDescription, GUILayout.MinWidth(150));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				else
				{
					GUI.enabled = false;
					EditorGUILayout.TextField(UnknownValueDescription, GUILayout.MinWidth(150));
					GUI.enabled = record.prefType != PrefsRecord.PrefsType.Unknown;
				}
				GUI.color = guiColor;
				GUI.enabled = true;

				EditorGUILayout.LabelField(record.DisplayType, GUILayout.Width(70));
			}
		}

		private void ShowOtherMenu(PrefsRecord record)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Copy to clipboard"), false, () =>
			{
				EditorGUIUtility.systemCopyBuffer = record.ToString();
			});

			if (record.Obscured)
			{
				menu.AddItem(new GUIContent("Copy obscured raw data to clipboard"), false, () =>
				{
					EditorGUIUtility.systemCopyBuffer = record.ToString(true);
				});
			}

			var valueToPaste = EditorGUIUtility.systemCopyBuffer;
			switch (record.prefType)
			{
				case PrefsRecord.PrefsType.Unknown:
					break;
				case PrefsRecord.PrefsType.String:
					if (!record.Obscured || record.IsEditableObscuredValue())
					{
						menu.AddItem(new GUIContent("Paste string value from clipboard"), false, () =>
						{
							record.StringValue = valueToPaste;
						});
					}
					break;
				case PrefsRecord.PrefsType.Int:
					menu.AddItem(new GUIContent("Paste int value from clipboard"), false, () =>
					{
						int pastedInt;
						if (int.TryParse(valueToPaste, out pastedInt))
						{
							record.IntValue = pastedInt;
						}
						else
						{
							Debug.LogWarning(ACTkConstants.LogPrefix + "Can't paste this value to Int pref:\n" + valueToPaste);
						}
					});
					break;
				case PrefsRecord.PrefsType.Float:
					menu.AddItem(new GUIContent("Paste float value from clipboard"), false, () =>
					{
						float pastedFloat;
						if (float.TryParse(valueToPaste, out pastedFloat))
						{
							record.FloatValue = pastedFloat;
						}
						else
						{
							Debug.LogWarning(ACTkConstants.LogPrefix + "Can't paste this value to Float pref:\n" + valueToPaste);
						}
					});
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			menu.ShowAsContext();
		}

		#endregion

		private void RefreshData()
		{
			var keys = new List<string>();
#if UNITY_EDITOR_WIN
			keys.AddRange(ReadKeysWin());
#elif UNITY_EDITOR_OSX
			keys.AddRange(ReadKeysOSX());
#else // LINUX
			keys.AddRange(ReadKeysLinux());
#endif
			keys.RemoveAll(IgnoredPrefsKeys);

			if (allRecords == null) allRecords = new List<PrefsRecord>();
			if (filteredRecords == null) filteredRecords = new List<PrefsRecord>();

			allRecords.Clear();
			filteredRecords.Clear();

			var keysCount = keys.Count;
			var showProgress = keysCount >= 500;

			for (var i = 0; i < keysCount; i++)
			{
				var keyName = keys[i];
				if (showProgress)
				{
					if (EditorUtility.DisplayCancelableProgressBar("Reading PlayerPrefs [" + (i + 1) + " of " + keysCount + "]", "Reading " + keyName, (float)i/keysCount))
					{
						break;
					}
				}
				allRecords.Add(new PrefsRecord(keyName));
			}

			if (showProgress) EditorUtility.ClearProgressBar();

			ApplySorting();
		}

		private void ApplyFiltering()
		{
			filteredRecords.Clear();
			if (string.IsNullOrEmpty(searchPattern))
			{
				filteredRecords.AddRange(allRecords);
			}
			else
			{
				foreach (var record in allRecords)
				{
					if (record.Key.ToLowerInvariant().Contains(searchPattern.Trim().ToLowerInvariant()))
					{
						filteredRecords.Add(record);
					}
				}
			}
		}

		private void ApplySorting()
		{
			switch (sortingType)
			{
				case SortingType.KeyAscending:
					allRecords.Sort(PrefsRecord.SortByNameAscending);
					break;
				case SortingType.KeyDescending:
					allRecords.Sort(PrefsRecord.SortByNameDescending);
					break;
				case SortingType.Type:
					allRecords.Sort(PrefsRecord.SortByType);
					break;
				case SortingType.Obscurance:
					allRecords.Sort(PrefsRecord.SortByObscurance);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			ApplyFiltering();
		}

		private bool IgnoredPrefsKeys(string key)
		{
			return key == ObscuredPrefs.PrefsKey ||
			       key == "OwsfBQ8qISsHHho6ACAJAiAAKRI2GjIDFh4EIQ0o" ||
			       key == "UnityGraphicsQuality" ||
			       key == "UnitySelectMonitor" ||
			       key == "Screenmanager Resolution Width" ||
			       key == "Screenmanager Resolution Height" ||
			       key == "Screenmanager Is Fullscreen mode" ||
			       key == "unity.cloud_userid" ||
			       key == "unity.player_session_background_time" ||
			       key == "unity.player_session_elapsed_time" ||
			       key == "unity.player_sessionid" ||
			       key == "unity.player_session_count" ||
			       key == "UnityUdpSdkImported" ||
			       key.StartsWith("PackageUpdaterLastChecked");
		}

#if UNITY_EDITOR_WIN

		private string[] ReadKeysWin()
		{
			var registryLocation = Registry.CurrentUser.CreateSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
			if (registryLocation == null)
			{
				return new string[0];
			}

			var names = registryLocation.GetValueNames();
			var result = new string[names.Length];

			for (var i = 0; i < names.Length; i++)
			{
				var key = names[i];

				if (key.IndexOf('_') > 0)
				{
					result[i] = key.Substring(0, key.LastIndexOf('_'));
				}
				else
				{
					result[i] = key;
				}
			}

			return result;
		}

#elif UNITY_EDITOR_OSX

		private string[] ReadKeysOSX()
		{
			var plistPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + 
				PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";

			if (!File.Exists (plistPath)) 
			{
				return new string[0];
			}

			var parsedPlist = (Dictionary<string, object>)Plist.readPlist(plistPath);

			var keys = new string[parsedPlist.Keys.Count];
			parsedPlist.Keys.CopyTo (keys, 0);

			return keys;
		}

#else // LINUX!

		private string[] ReadKeysLinux()
		{
			var prefsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.config/unity3d/" + 
				PlayerSettings.companyName + "/" + PlayerSettings.productName + "/prefs";

			if (!File.Exists(prefsPath)) 
			{
				return new string[0];
			}

			var prefsXML = new XmlDocument();
			prefsXML.Load(prefsPath);
			var prefsList = prefsXML.SelectNodes("/unity_prefs/pref");

			var keys = new string[prefsList.Count];

			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = prefsList[i].Attributes["name"].Value;
			}

			return keys;
		}

#endif
		private enum SortingType : byte
		{
			KeyAscending = 0,
			KeyDescending = 2,
			Type = 5,
			Obscurance = 10
		}
	}
}