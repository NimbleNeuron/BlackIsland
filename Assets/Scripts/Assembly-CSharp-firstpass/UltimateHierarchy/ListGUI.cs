using System.Collections.Generic;
using UnityEngine;

namespace UltimateHierarchy
{
	public static class ListGUI
	{
		public static void draw(UltimateHierarchyPro script, List<Slot> list, bool isSceneOverlay)
		{
			script.setStyles();
			int num = UltimateHierarchyPro.toggles.IndexOf(script);
			int num2 = 10;
			int num3 = 10;
			if (!isSceneOverlay)
			{
				for (int i = 0; i < num; i++)
				{
					num3 += (int) UltimateHierarchyPro.toggles[i].GUIwidth + 10;
				}
			}

			int num4 = 0;
			int num5 = 18;
			int num6 = 1;
			float num7 = new GUIStyle().CalcSize(new GUIContent(script.toggleName)).x;
			GUI.color = Color.white;
			GUIStyle guistyle = new GUIStyle();
			guistyle.alignment = TextAnchor.MiddleCenter;
			guistyle.fontStyle = FontStyle.Bold;
			if (isSceneOverlay)
			{
				guistyle.fontSize = 10;
			}
			else
			{
				guistyle.fontSize = 12;
			}

			guistyle.normal.textColor = Color.white;
			Vector2 vector = new GUIStyle().CalcSize(new GUIContent(script.toggleName));
			num7 = updateGUIwidth(num7, vector.x);
			GUI.Label(new Rect(num3, num2 + num4 * num5, script.GUIwidth, num5 - num6), script.toggleName, guistyle);
			num4++;
			Color black = Color.black;
			black.a = UltimateHierarchyPro.overlayButtonTransparency;
			for (int j = 0; j < list.Count; j++)
			{
				if (!(list[j].obj == null))
				{
					if (list[j].objectType == Slot.ObjectType.Gameobject)
					{
						Color colorGO = script.colorGO;
						colorGO.a = UltimateHierarchyPro.overlayButtonTransparency;
						if (script.getActive(j))
						{
							GUI.color = colorGO;
						}
						else
						{
							GUI.color = (colorGO + black) / 2f;
						}

						vector = new GUIStyle().CalcSize(new GUIContent(list[j].obj.name + " (GO)"));
						vector.x *= 1.25f;
						num7 = updateGUIwidth(num7, vector.x);
						if (GUI.Button(
							new Rect(num3 + script.GUIwidth / 2f - vector.x / 2f, num2 + num4 * num5, vector.x,
								num5 - num6), list[j].obj.name + " (GO)"))
						{
							script.toggleActive(j);
						}
					}
					else if (list[j].objectType == Slot.ObjectType.Prefab)
					{
						Color colorP = script.colorP;
						colorP.a = UltimateHierarchyPro.overlayButtonTransparency;
						if (script.getActive(j))
						{
							GUI.color = colorP;
						}
						else
						{
							GUI.color = (colorP + black) / 2f;
						}

						vector = new GUIStyle().CalcSize(new GUIContent(list[j].obj.name + " (P)"));
						vector.x *= 1.25f;
						num7 = updateGUIwidth(num7, vector.x);
						if (GUI.Button(
							new Rect(num3 + script.GUIwidth / 2f - vector.x / 2f, num2 + num4 * num5, vector.x,
								num5 - num6), list[j].obj.name + " (P)"))
						{
							script.toggleActive(j);
						}
					}
					else if (list[j].objectType == Slot.ObjectType.Script)
					{
						Color colorS = script.colorS;
						colorS.a = UltimateHierarchyPro.overlayButtonTransparency;
						if (script.getUpdate(j))
						{
							GUI.color = colorS;
						}
						else
						{
							GUI.color = (colorS + black) / 2f;
						}

						int num8 = Mathf.Clamp(list[j].type, 0, list[j].type);
						string shortString = UltimateHierarchyPro.getShortString(list[j].componentNames[num8]);
						vector = new GUIStyle().CalcSize(new GUIContent(list[j].obj.name + " (" + shortString + ")"));
						vector.x *= 1.25f;
						num7 = updateGUIwidth(num7, vector.x);
						if (GUI.Button(
							new Rect(num3 + script.GUIwidth / 2f - vector.x / 2f, num2 + num4 * num5, vector.x,
								num5 - num6), list[j].obj.name + " (" + shortString + ")"))
						{
							script.toggleUpdate(j);
						}
					}

					num4++;
				}
			}

			script.GUIwidth = num7;
		}


		private static float updateGUIwidth(float newWidth, float checkWidth)
		{
			if (checkWidth > newWidth)
			{
				newWidth = checkWidth;
			}

			return newWidth;
		}
	}
}