#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using Common;
	using UnityEditor;
	using UnityEngine;

	internal static class GUITools
	{
		private static GUIStyle iconButtonStyle;
		public static GUIStyle IconButtonStyle
		{
			get
			{
				Init();
				return iconButtonStyle;
			}
		}

		private static GUIStyle lineStyle;
		public static GUIStyle LineStyle
		{
			get
			{
				Init();
				return lineStyle;
			}
		}

		private static GUIStyle richFoldoutStyle;
		public static GUIStyle RichFoldoutStyle
		{
			get
			{
				Init();
				return richFoldoutStyle;
			}
		}

		private static GUIStyle richMiniLabel;
		public static GUIStyle RichMiniLabel
		{
			get
			{
				Init();
				return richMiniLabel;
			}
		}

		private static GUIStyle richLabel;
		public static GUIStyle RichLabel
		{
			get
			{
				Init();
				return richLabel;
			}
		}

		private static GUIStyle boldLabel;
		public static GUIStyle BoldLabel
		{
			get
			{
				Init();
				return boldLabel;
			}
		}
		
		private static GUIStyle largeBoldLabel;
		internal static GUIStyle LargeBoldLabel
		{
			get
			{
				Init();
				return largeBoldLabel;
			}
		}

		private static GUIStyle centeredLabel;
		internal static GUIStyle CenteredLabel
		{
			get
			{
				Init();
				return centeredLabel;
			}
		}
		
		private static GUIStyle panelWithBackground;
		internal static GUIStyle PanelWithBackground
		{
			get
			{
				Init();
				return panelWithBackground;
			}
		}
		
		private static GUIStyle compactButton;
		internal static GUIStyle CompactButton
		{
			get
			{
				Init();
				return compactButton;
			}
		}

		private static GUIStyle toolbarSeachTextField;
		internal static GUIStyle ToolbarSeachTextField
		{
			get
			{
				Init();
				return toolbarSeachTextField;
			}
		}

		private static GUIStyle toolbarSeachTextFieldPopup;
		internal static GUIStyle ToolbarSeachTextFieldPopup
		{
			get
			{
				Init();
				return toolbarSeachTextFieldPopup;
			}
		}
		
		private static GUIStyle toolbarSeachCancelButton;
		internal static GUIStyle ToolbarSeachCancelButton
		{
			get
			{
				Init();
				return toolbarSeachCancelButton;
			}
		}

		private static GUIStyle toolbarSeachCancelButtonEmpty;
		internal static GUIStyle ToolbarSeachCancelButtonEmpty
		{
			get
			{
				Init();
				return toolbarSeachCancelButtonEmpty;
			}
		}

		private static GUIStyle toolbar;
		internal static GUIStyle Toolbar
		{
			get
			{
				Init();
				return toolbar;
			}
		}

		private static GUIStyle toolbarLabel;
		internal static GUIStyle ToolbarLabel
		{
			get
			{
				Init();
				return toolbarLabel;
			}
		}

		private static bool inited;

		private static void Init()
		{
			if (inited)
			{
				return;
			}

			richMiniLabel = new GUIStyle(EditorStyles.miniLabel)
			{
				wordWrap = true,
				richText = true
			};

			richLabel = new GUIStyle(EditorStyles.label)
			{
				wordWrap = true, 
				richText = true
			};

			boldLabel = new GUIStyle(richLabel)
			{
				fontStyle = FontStyle.Bold
			};

			largeBoldLabel = new GUIStyle(EditorStyles.largeLabel)
			{
				wordWrap = true, fontStyle = FontStyle.Bold, richText = true
			};

			compactButton = new GUIStyle(GUI.skin.button)
			{
				//margin = richLabel.margin, 
				overflow = richLabel.overflow, 
				padding = new RectOffset(5, 5, 1, 4),
				margin = new RectOffset(2, 2, 3, 2),
				richText = true
			};

			//compactButton.margin = new RectOffset(2, 2, 3, 2);

			iconButtonStyle = new GUIStyle(compactButton)
			{
				padding = new RectOffset(0, 0, EditorGUIUtility.isProSkin ? -5 : -4, -2),
				overflow = EditorGUIUtility.isProSkin ? new RectOffset(1, 1, 1, 1) : new RectOffset(0, 0, 2, 1),
				fixedHeight = 18,
				fixedWidth = 22
			};

			centeredLabel = new GUIStyle(richLabel)
			{
				alignment = TextAnchor.MiddleCenter
			};

			panelWithBackground = new GUIStyle(GUI.skin.box)
			{
				padding = new RectOffset()
			};

			lineStyle = new GUIStyle(GUI.skin.box);
			lineStyle.border.top = lineStyle.border.bottom = 1;
			lineStyle.margin.top = lineStyle.margin.bottom = 1;
			lineStyle.padding.top = lineStyle.padding.bottom = 1;

			richFoldoutStyle = new GUIStyle(EditorStyles.foldout);
			richFoldoutStyle.active = richFoldoutStyle.focused = richFoldoutStyle.normal;
			richFoldoutStyle.onActive = richFoldoutStyle.onFocused = richFoldoutStyle.onNormal;
			richFoldoutStyle.richText = true;

			toolbar = new GUIStyle(EditorStyles.toolbar);
			toolbar.margin.top++;

			toolbarLabel = new GUIStyle(EditorStyles.miniLabel)
			{
				richText = true
			};
			toolbarLabel.padding.top--;

			toolbarSeachTextField = GetBuiltinStyle("ToolbarSeachTextField");
			if (toolbarSeachTextField == null)
			{
				toolbarSeachTextField = GetBuiltinStyle("ToolbarSearchTextField");
			}

			toolbarSeachTextFieldPopup = GetBuiltinStyle("ToolbarSeachTextFieldPopup");
			if (toolbarSeachTextFieldPopup == null)
			{
				toolbarSeachTextFieldPopup = GetBuiltinStyle("ToolbarSearchTextFieldPopup");
			}

			toolbarSeachCancelButton = GetBuiltinStyle("ToolbarSeachCancelButton");
			if (toolbarSeachCancelButton == null)
			{
				toolbarSeachCancelButton = GetBuiltinStyle("ToolbarSearchCancelButton");
			}

			toolbarSeachCancelButtonEmpty = GetBuiltinStyle("ToolbarSeachCancelButtonEmpty");
			if (toolbarSeachCancelButtonEmpty == null)
			{
				toolbarSeachCancelButtonEmpty = GetBuiltinStyle("ToolbarSearchCancelButtonEmpty");
			}

			inited = true;
		}

		
		internal static void Separator()
		{
			GUILayout.Box(GUIContent.none, LineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
		}

		public static bool IconButton(Texture icon, params GUILayoutOption[] options)
		{
			return IconButton(icon, null, options);
		}

		public static bool IconButton(Texture icon, string hint, params GUILayoutOption[] options)
		{
			return ImageButton(null, hint, icon, IconButtonStyle, options);
		}

		public static bool ImageButton(string label, Texture image, params GUILayoutOption[] options)
		{
			return ImageButton(label, null, image, options);
		}

		public static bool ImageButton(string label, string hint, Texture image, params GUILayoutOption[] options)
		{
			return ImageButton(label, hint, image, CompactButton, options);
		}

		public static bool ImageButton(string label, string hint, Texture image, GUIStyle style, params GUILayoutOption[] options)
		{
			var content = new GUIContent();

			if (!string.IsNullOrEmpty(label))
			{
				content.text = label;
			}

			if (!string.IsNullOrEmpty(hint))
			{
				content.tooltip = hint;
			}

			content.image = image;
			if (!string.IsNullOrEmpty(label))
			{
				content.text = " " + label;
			}
			
			return GUILayout.Button(content, style, options);
		}

		internal static bool Foldout(bool foldout, string caption)
		{
			return Foldout(foldout, new GUIContent(caption));
		}

		internal static bool Foldout(bool foldout, GUIContent caption)
		{
			return EditorGUI.Foldout(EditorGUILayout.GetControlRect(), foldout, caption, true, RichFoldoutStyle);
		}

		internal static string SearchToolbar(string searchPattern)
		{
			var searchFieldRect = EditorGUILayout.GetControlRect(false, ToolbarSeachTextField.lineHeight, ToolbarSeachTextField);
			var searchFieldTextRect = searchFieldRect;
			searchFieldTextRect.width -= 14f;

			searchPattern = EditorGUI.TextField(searchFieldTextRect, searchPattern, ToolbarSeachTextField);

			GUILayout.Space(10);

			var searchFieldButtonRect = searchFieldRect;
			searchFieldButtonRect.x += searchFieldRect.width - 14f;
			searchFieldButtonRect.width = 14f;

			var buttonStyle = string.IsNullOrEmpty(searchPattern) ? ToolbarSeachCancelButtonEmpty : ToolbarSeachCancelButton;
			if (GUI.Button(searchFieldButtonRect, GUIContent.none, buttonStyle) && !string.IsNullOrEmpty(searchPattern))
			{
				searchPattern = string.Empty;
				GUIUtility.keyboardControl = 0;
			}

			return searchPattern;
		}

		internal static void DrawHeader(string text)
		{
			using (Horizontal(PanelWithBackground, GUILayout.Height(20), GUILayout.ExpandHeight(false)))
			{
				EditorGUILayout.LabelField(text, LargeBoldLabel);
			}
			GUILayout.Space(3);
		}

		internal static bool DrawFoldHeader(string text, bool fold)
		{
			var result = fold;
			text = "<b>" + text + "</b>";
			using (Horizontal(PanelWithBackground, GUILayout.Height(20), GUILayout.ExpandHeight(false)))
			{
				result = EditorGUILayout.Foldout(fold, new GUIContent(text), true, RichFoldoutStyle);
			}
			GUILayout.Space(3);

			return result;
		}

		private static GUIStyle GetBuiltinStyle(string name)
		{
			var style = GUI.skin.FindStyle(name);
			if (style == null)
			{
				style = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(name);
			}

			if (style == null)
			{
				style = GUIStyle.none;
				Debug.LogError(ACTkConstants.LogPrefix + "Can't find builtin style " + name);
			}

			return style;
		}

		// ------------------------------------------------------------------
		// tooling for "using" keyword
		// ------------------------------------------------------------------

		internal static GUILayout.HorizontalScope Horizontal()
		{
			return Horizontal(GUIStyle.none);
		}

		internal static GUILayout.HorizontalScope Horizontal(GUIStyle style)
		{
			return Horizontal(style, null);
		}

		internal static GUILayout.HorizontalScope Horizontal(params GUILayoutOption[] options)
		{
			return Horizontal(GUIStyle.none, options);
		}

		internal static GUILayout.HorizontalScope Horizontal(GUIStyle style, params GUILayoutOption[] options)
		{
			return new GUILayout.HorizontalScope(style, options);
		}

		internal static GUILayout.VerticalScope Vertical(params GUILayoutOption[] options)
		{
			return Vertical(GUIStyle.none, options);
		}

		internal static GUILayout.VerticalScope Vertical(GUIStyle style, params GUILayoutOption[] options)
		{
			return new GUILayout.VerticalScope(style, options);
		}
	}
}