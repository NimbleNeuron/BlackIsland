using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Blis.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


public class ArchCrashlytics : SingletonMonoBehaviour<ArchCrashlytics>
{
	
	public void Init(string reportVersion)
	{
		this.reportVersion = reportVersion;
		this._historyKey = this.HISTORY_KEY + "_" + this.reportVersion;
	}

	
	private string GetHistoryKey()
	{
		if (this._historyKey != null)
		{
			return this._historyKey;
		}
		return this.HISTORY_KEY + "_" + this.reportVersion;
	}

	
	protected override void OnAwakeSingleton()
	{
		base.OnAwakeSingleton();
		AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
		Application.logMessageReceived += new Application.LogCallback(this.HandleLog);
		// Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
		this.LoadHistory();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (type != LogType.Exception)
		{
			return;
		}
		this.SendLog(new ArchCrashlytics.ExceptionEntry(message, stackTrace, SystemInfo.deviceUniqueIdentifier));
	}

	
	public void SendLog(string message, string stackTrace, LogType type)
	{
		this.SendLog(new ArchCrashlytics.ExceptionEntry(message, stackTrace, SystemInfo.deviceUniqueIdentifier));
	}

	
	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (!(e.ExceptionObject is Exception))
		{
			return;
		}
		Exception ex = e.ExceptionObject as Exception;
		this.HandleLog(ex.Message, ex.StackTrace, LogType.Exception);
	}

	
	private void SendLog(ArchCrashlytics.ExceptionEntry exception)
	{
		if (exception.msg.Contains("GameException") || this.reportVersion.Contains("Editor"))
		{
			return;
		}
		ArchCrashlytics.SendData sendData = this.CreateReport(exception);
		if (sendData != null)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(ApiConstants.CrashReportUrl, "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sendData));
			unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			base.StartCoroutine(this.Send(unityWebRequest));
		}
	}

	
	private IEnumerator Send(UnityWebRequest request)
	{
		yield return request.SendWebRequest();
		if (request.responseCode != 200L)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Log: ",
				request.responseCode,
				", ",
				request.error
			}));
		}
	}

	private ArchCrashlytics.SendData CreateReport(ArchCrashlytics.ExceptionEntry exception)
	{
		string crashId = "";
		string callStack = exception.callStack;
		try
		{
			crashId = callStack.Substring(0, callStack.IndexOf(".")) + " : " + exception.msg.Split(new char[]
			{
				':'
			})[0];
		}
		catch
		{
			crashId = exception.msg;
		}
		string str = string.Empty;
		if (exception.callStack.IndexOf('<') >= 0)
		{
			str = exception.callStack.Substring(0, exception.callStack.IndexOf('<'));
		}
		else if (exception.callStack.IndexOf('(') >= 0)
		{
			str = exception.callStack.Substring(0, exception.callStack.IndexOf('('));
		}
		string text = BitConverter.ToString(new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(exception.msg + str)));
		if (this.exceptionHistory.Contains(text))
		{
			return null;
		}
		this.exceptionHistory.Add(text);
		return new ArchCrashlytics.SendData
		{
			userNum = exception.userNum,
			reportVersion = this.reportVersion,
			identityKey = text,
			crashId = crashId,
			crashMsg = exception.msg,
			callstack = callStack,
			os = Environment.OSVersion.VersionString,
			phase = "release"
		};
	}

	
	protected override void OnDestroySingleton()
	{
		this.SaveHistory();
		base.OnDestroySingleton();
		
		AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomain_UnhandledException;
		Application.logMessageReceived -= new Application.LogCallback(this.HandleLog);
	}

	
	private void SaveHistory()
	{
		StringBuilder stringBuilder = GameUtil.StringBuilder;
		stringBuilder.Clear();
		foreach (string value in this.exceptionHistory)
		{
			stringBuilder.AppendLine(value);
		}
		PlayerPrefs.SetString(this.GetHistoryKey(), stringBuilder.ToString());
	}

	
	private void LoadHistory()
	{
		try
		{
			if (PlayerPrefs.HasKey(this.GetHistoryKey()))
			{
				using (StringReader stringReader = new StringReader(PlayerPrefs.GetString(this.GetHistoryKey(), string.Empty)))
				{
					while (stringReader.Peek() > -1)
					{
						string text = stringReader.ReadLine();
						if (!string.IsNullOrEmpty(text))
						{
							this.exceptionHistory.Add(text);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	
	private readonly string SAVE_KEY = "ArchCrashlytics";

	
	private readonly string HISTORY_KEY = "ArchCrashlyticsHistory";

	
	private string reportVersion = "NULL";

	
	private string _historyKey;

	
	private readonly HashSet<string> exceptionHistory = new HashSet<string>();

	
	[Serializable]
	public class ExceptionEntry
	{
		public ExceptionEntry(string msg, string callStack, string userNum)
		{
			this.msg = msg;
			this.callStack = callStack;
			this.userNum = userNum;
		}
		
		public string msg;
		public string callStack;
		public string userNum;
	}

	
	private class SendData
	{
		public string userNum;
		public string reportVersion;
		public string identityKey;
		public string crashId;
		public string crashMsg;
		public string callstack;
		public string os;
		public string phase;
	}
	
	private void Ref()
	{
		Reference.Use(SAVE_KEY);
	}
}
