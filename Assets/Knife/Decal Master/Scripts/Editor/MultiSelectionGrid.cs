using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace Knife.Tools
{
    public static class MultiSelectionGrid
    {
        static List<int> selectedList;

        public static int Draw(int[] selected, float elementSize, int xCount, GUIContent[] content, Color[] colors, out bool addButtonPressed, string b1, string b2, string b3, out bool b1p, out bool b2p, out bool b3p)
        {
            int currentPressed = -1;
            addButtonPressed = false;
            b1p = false;
            b2p = false;
            b3p = false;

            selectedList = selected.ToList();

            float maxWidth = elementSize * xCount;
            bool addButtonDrawed = false;
            int endIndex = 0;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.MaxWidth(maxWidth), GUILayout.MinHeight(200));
            var rect = EditorGUILayout.GetControlRect();
            rect.x -= 5 + 5;
            rect.y += 15 - 5;
            rect.width = maxWidth + 15 + xCount * 3;
            rect.height = (content.Length / xCount + 1) * (elementSize + EditorGUIUtility.singleLineHeight + 5) + 50;
            GUI.Box(rect, "");
            var buttonsRect = EditorGUILayout.GetControlRect();

            float buttonWidth = 50;
            buttonsRect.width = buttonWidth;
            b1p = GUI.Button(buttonsRect, b1, EditorStyles.miniButtonLeft);
            buttonsRect.x += buttonWidth;
            b2p = GUI.Button(buttonsRect, b2, EditorStyles.miniButtonMid);
            buttonsRect.x += buttonWidth;
            b3p = GUI.Button(buttonsRect, b3, EditorStyles.miniButtonRight);

            GUILayout.Space(10);
            for (int i = 0; i < content.Length / xCount + 1; i++)
            {
                int startIndex = i * xCount;
                endIndex = i * xCount + xCount;

                GUILayout.BeginHorizontal();
                for (int j = startIndex; j < endIndex; j++)
                {
                    if (j > content.Length)
                        GUILayout.Button(DPTResourcesLoader.EmptyImage, DPTResourcesLoader.EmptyButtonGridStyle, GUILayout.Width(elementSize), GUILayout.Height(elementSize));
                    else if (j == content.Length)
                    {
                        if (GUILayout.Button(DPTResourcesLoader.AddTemplateIcon, DPTResourcesLoader.AddTemplateButtonGridStyle, GUILayout.Width(elementSize), GUILayout.Height(elementSize)))
                        {
                            addButtonPressed = true;
                        }
                        addButtonDrawed = true;
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                        if(GUILayout.Button(content[j], selectedList.Contains(j) ? DPTResourcesLoader.SelectedGridButtonStyle : DPTResourcesLoader.DefaultGridButtonStyle, GUILayout.Width(elementSize), GUILayout.Height(elementSize)))
                        {
                            currentPressed = j;
                        }
                        colors[j] = EditorGUILayout.ColorField(colors[j]);
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();

                if (endIndex >= content.Length)
                {
                    if(!addButtonDrawed)
                    {
                        GUILayout.BeginVertical();
                        if (GUILayout.Button(DPTResourcesLoader.AddTemplateIcon, DPTResourcesLoader.AddTemplateButtonGridStyle, GUILayout.Width(elementSize), GUILayout.Height(elementSize)))
                        {
                            addButtonPressed = true;
                        }
                        GUILayout.Space(EditorGUIUtility.singleLineHeight + 5);
                        addButtonDrawed = true;
                        GUILayout.EndVertical();
                    }
                    break;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            return currentPressed;
        }
    }
}