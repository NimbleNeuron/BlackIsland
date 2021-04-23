using System;
using UnityEngine;

namespace Neptune.Log
{
	
	public static class Logger
	{
		private static LogLevel logLevel;
		public static Logger.LogFunc Log;
		public static Logger.LogFunc Error;
		public static Logger.LogFunc Warning;
		public static Logger.ExceptionFunc Exception;

		public delegate void LogFunc(object message, params object[] args);
		public delegate void ExceptionFunc(Exception ex);
		
		static Logger()
		{
			if (Debug.isDebugBuild)
			{
				Logger.LogLevel = LogLevel.All;
			}
		}

		public static LogLevel LogLevel
		{
			get
			{
				return Logger.logLevel;
			}
			set
			{
				Logger.logLevel = value;
				
				if (Logger.IsEnabled(LogLevel.Log))
				{
					Logger.Log = new Logger.LogFunc(Logger.LogLog);
				}
				else
				{
					Logger.Log = new Logger.LogFunc(Logger.LogNone);
				}
				if (Logger.IsEnabled(LogLevel.Warning))
				{
					Logger.Warning = new Logger.LogFunc(Logger.LogWarning);
				}
				else
				{
					Logger.Warning = new Logger.LogFunc(Logger.LogNone);
				}
				if (Logger.IsEnabled(LogLevel.Error))
				{
					Logger.Error = new Logger.LogFunc(Logger.LogError);
				}
				else
				{
					Logger.Error = new Logger.LogFunc(Logger.LogNone);
				}
				if (Logger.IsEnabled(LogLevel.Exception))
				{
					Logger.Exception = new Logger.ExceptionFunc(Logger.LogException);
					return;
				}
				Logger.Exception = new Logger.ExceptionFunc(Logger.LogNoneEx);
			}
		}
		
		private static bool IsEnabled(LogLevel level)
		{
			return (Logger.logLevel & level) == level;
		}
		
		private static object Message(object message, params object[] args)
		{
			if (args.Length != 0 && message is string)
			{
				return string.Format(message as string, args);
			}
			return message;
		}
		
		private static void LogNone(object message, params object[] args)
		{
		}
		
		private static void LogNoneEx(Exception ex)
		{
		}
		
		private static void LogLog(object message, params object[] args)
		{
			Debug.Log(Logger.Message(message, args));
		}
		
		private static void LogError(object message, params object[] args)
		{
			Debug.LogError(Logger.Message(message, args));
		}
		
		private static void LogWarning(object message, params object[] args)
		{
			Debug.LogWarning(Logger.Message(message, args));
		}
		
		private static void LogException(Exception ex)
		{
			Debug.LogException(ex);
		}

		private static string GetLogTextColor(string _Text, byte _ColorR, byte _ColorG, byte _ColorB)
		{
			return Logger.GetLogTextColor(_Text, new Color32(_ColorR, _ColorG, _ColorB, byte.MaxValue));
		}
		
		private static string GetLogTextColor(string _Text, Color32 _Color)
		{
			_Text = string.Format("<color=#{1:x}{2:x}{3:x}>{0}</color>", new object[]
			{
				_Text,
				_Color.r,
				_Color.g,
				_Color.b
			});
			return _Text;
		}
	}
}
