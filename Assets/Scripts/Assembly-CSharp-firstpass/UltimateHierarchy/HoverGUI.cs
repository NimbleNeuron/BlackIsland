using System.Collections.Generic;
using UnityEngine;

namespace UltimateHierarchy
{
	public static class HoverGUI
	{
		public static void draw(UltimateHierarchyPro script, List<Slot> list)
		{
			script.setStyles();
			int num = 18;
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i].obj == null) && !(Camera.main == null))
				{
					Vector3 vector = Camera.main.WorldToScreenPoint(list[i].obj.transform.position);
					Color black = Color.black;
					black.a = UltimateHierarchyPro.overlayButtonTransparency;
					if (list[i].objectType == Slot.ObjectType.Gameobject)
					{
						Color colorGO = script.colorGO;
						colorGO.a = UltimateHierarchyPro.overlayButtonTransparency;
						if (script.getActive(i))
						{
							GUI.color = colorGO;
						}
						else
						{
							GUI.color = (colorGO + black) / 2f;
						}

						Vector2 vector2 = new GUIStyle().CalcSize(new GUIContent(list[i].obj.name + " (GO)"));
						vector2.x *= 1.25f;
						if (GUI.Button(
							new Rect(-(vector2.x / 2f) + vector.x, Screen.height - vector.y - vector2.y / 2f, vector2.x,
								num), list[i].obj.name + " (GO)"))
						{
							script.toggleActive(i);
						}
					}
					else if (list[i].objectType == Slot.ObjectType.Script)
					{
						Color colorS = script.colorS;
						colorS.a = UltimateHierarchyPro.overlayButtonTransparency;
						if (script.getUpdate(i))
						{
							GUI.color = colorS;
						}
						else
						{
							GUI.color = (colorS + black) / 2f;
						}

						int num2 = Mathf.Clamp(list[i].type, 0, list[i].type);
						string shortString = UltimateHierarchyPro.getShortString(list[i].componentNames[num2]);
						Vector2 vector3 =
							new GUIStyle().CalcSize(new GUIContent(list[i].obj.name + " (" + shortString + ")"));
						vector3.x *= 1.25f;
						if (GUI.Button(
							new Rect(-(vector3.x / 2f) + vector.x, Screen.height - vector.y - vector3.y / 2f, vector3.x,
								num), list[i].obj.name + " (" + shortString + ")"))
						{
							script.toggleUpdate(i);
						}
					}
				}
			}
		}
	}
}