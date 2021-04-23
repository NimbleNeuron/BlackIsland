using UnityEngine;

namespace SuperScrollView
{
	public class FPSDisplay : MonoBehaviour
	{
		private float deltaTime;


		private GUIStyle mStyle;


		private void Awake()
		{
			mStyle = new GUIStyle();
			mStyle.alignment = TextAnchor.UpperLeft;
			mStyle.normal.background = null;
			mStyle.fontSize = 25;
			mStyle.normal.textColor = new Color(0f, 1f, 0f, 1f);
		}


		private void Update()
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		}


		private void OnGUI()
		{
			int width = Screen.width;
			int height = Screen.height;
			Rect position = new Rect(0f, 0f, width, height * 2 / 100);
			float num = 1f / deltaTime;
			string text = string.Format("   {0:0.} FPS", num);
			GUI.Label(position, text, mStyle);
		}
	}
}