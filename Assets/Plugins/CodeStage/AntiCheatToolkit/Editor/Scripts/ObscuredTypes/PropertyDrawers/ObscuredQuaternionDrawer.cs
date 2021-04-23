#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	using ObscuredTypes;

	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ObscuredQuaternion))]
	internal class ObscuredQuaternionDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{

        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
}