#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	using Common;
	using ObscuredTypes;

	using System.Runtime.InteropServices;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ObscuredFloat))]
	internal class ObscuredFloatDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			var hiddenValue = prop.FindPropertyRelative("hiddenValue");

			var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			var inited = prop.FindPropertyRelative("inited");
			var fakeValue = prop.FindPropertyRelative("fakeValue");
			var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

			var currentCryptoKey = cryptoKey.intValue;

			var union = new IntBytesUnion();
			float val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = ObscuredFloat.GenerateKey();
				}

				inited.boolValue = true;

				union.i = ObscuredFloat.Encrypt(0, currentCryptoKey);
				hiddenValue.intValue = union.i;
			}
			else
			{
				union.i = hiddenValue.intValue;
				val = ObscuredFloat.Decrypt(union.i, currentCryptoKey);
			}

			label = EditorGUI.BeginProperty(position, label, prop);

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.FloatField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				union.i = ObscuredFloat.Encrypt(val, currentCryptoKey);
				hiddenValue.intValue = union.i;

				fakeValue.floatValue = val;
				fakeValueActive.boolValue = true;
			}			
			EditorGUI.EndProperty();
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct IntBytesUnion
		{
			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public ACTkByte4 b4;
		}
	}
}