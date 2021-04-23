using System;
using System.Collections.Generic;
using UnityEngine;


public class DebugConsole : MonoBehaviour
{
	
	private void OnEnable()
	{
		Application.logMessageReceived += this.HandleLog;
	}

	
	private void OnDisable()
	{
		Application.logMessageReceived -= this.HandleLog;
	}

	
	private void Update()
	{
		if (Input.GetKeyDown(this.toggleKey))
		{
			this.show = !this.show;
		}
	}

	
	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		this.windowRect = GUILayout.Window(123456, this.windowRect, new GUI.WindowFunction(this.ConsoleWindow), "Console", Array.Empty<GUILayoutOption>());
	}

	
	private void ConsoleWindow(int windowID)
	{
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, Array.Empty<GUILayoutOption>());
		for (int i = 0; i < this.logs.Count; i++)
		{
			DebugConsole.Log log = this.logs[i];
			if (!this.collapse || i <= 0 || !(log.message == this.logs[i - 1].message))
			{
				GUI.contentColor = DebugConsole.logTypeColors[log.type];
				GUILayout.Label(log.message, Array.Empty<GUILayoutOption>());
			}
		}
		GUILayout.EndScrollView();
		GUI.contentColor = Color.white;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button(this.clearLabel, Array.Empty<GUILayoutOption>()))
		{
			this.logs.Clear();
		}
		this.collapse = GUILayout.Toggle(this.collapse, this.collapseLabel, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(false)
		});
		GUILayout.EndHorizontal();
		GUI.DragWindow(this.titleBarRect);
	}

	
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (this.logs.Count > 20)
		{
			this.logs.RemoveAt(0);
		}
		this.logs.Add(new DebugConsole.Log
		{
			message = message,
			stackTrace = stackTrace,
			type = type
		});
	}

	
	public KeyCode toggleKey = KeyCode.BackQuote;

	
	private List<DebugConsole.Log> logs = new List<DebugConsole.Log>();

	
	private Vector2 scrollPosition;

	
	private bool show;

	
	private bool collapse;

	
	private static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
	{
		{
			LogType.Assert,
			Color.white
		},
		{
			LogType.Error,
			Color.red
		},
		{
			LogType.Exception,
			Color.red
		},
		{
			LogType.Log,
			Color.white
		},
		{
			LogType.Warning,
			Color.yellow
		}
	};

	
	private const int margin = 20;

	
	private Rect windowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 40));

	
	private Rect titleBarRect = new Rect(0f, 0f, 10000f, 20f);

	
	private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");

	
	private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	
	private struct Log
	{
		
		public string message;

		
		public string stackTrace;

		
		public LogType type;
	}
}
