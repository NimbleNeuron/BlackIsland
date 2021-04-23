using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using Blis.Common.Logs;
using NLog;
using NLog.Config;
using NLog.Layouts;
using UnityEngine;
using Logger = NLog.Logger;

namespace Blis.Common
{
	public static class Log
	{
		public delegate void LogEvent(LogType type, string message);


		private static string _hostName;


		private static readonly object[] formatParam_1;


		private static readonly object[] formatParam_2;


		static Log()
		{
			OnLog = delegate { };
			formatParam_1 = new object[1];
			formatParam_2 = new object[2];
			ConfigureLogging();
		}

		
		
		public static event LogEvent OnLog;


		private static void LogImpl(LogLevel logLevel, string msg, int callstackOffset = 0)
		{
			Logger logger = NLogLoggers.DefaultLogger;
			StackFrame frame = new StackTrace().GetFrame(2 + callstackOffset);
			if (frame != null)
			{
				string hostName = GetHostName();
				MethodBase method = frame.GetMethod();
				Type reflectedType = method.ReflectedType;
				string text = reflectedType != null ? reflectedType.ToString() : null;
				StringBuilder stringBuilderSafety = GameUtil.StringBuilderSafety;
				stringBuilderSafety.Clear();
				stringBuilderSafety.Append("[");
				stringBuilderSafety.Append(hostName);
				stringBuilderSafety.Append("] [");
				stringBuilderSafety.Append(DateTime.Now.ToString("HH:mm:ss.fff"));
				stringBuilderSafety.Append("] ");
				stringBuilderSafety.Append(text);
				stringBuilderSafety.Append(":");
				stringBuilderSafety.Append(method.Name);
				stringBuilderSafety.Append(" | ");
				stringBuilderSafety.Append(msg);
				if (text != null && text.Equals("Blis.Common.LiteNetLogger"))
				{
					logger = NLogLoggers.LiteNetLibLogger;
				}

				msg = stringBuilderSafety.ToString();
			}

			LogType logType = logLevel.GetLogType();
			switch (logLevel)
			{
				case LogLevel.LOG_TRACE:
					logger.Trace(msg);
					break;
				case LogLevel.LOG_DEBUG:
					logger.Debug(msg);
					break;
				case LogLevel.LOG_VERBOSE:
				case LogLevel.LOG_HIGHLIGHT:
					logger.Info(msg);
					break;
				case LogLevel.LOG_WARNING:
					logger.Warn(msg);
					break;
				case LogLevel.LOG_ERROR:
				case LogLevel.LOG_EXCEPTION:
					logger.Error(msg);
					break;
				default:
					throw new ArgumentOutOfRangeException("logLevel", logLevel, null);
			}

			OnLog(logType, msg);
		}


		private static void LogFmtImpl(LogLevel logLevel, string fmt, params object[] param)
		{
			try
			{
				LogImpl(logLevel, string.Format(fmt, param), 1);
			}
			catch (Exception)
			{
				LogImpl(logLevel, fmt, 1);
			}
		}


		[Conditional("LOG_TRACE")]
		public static void T(string msg)
		{
			LogImpl(LogLevel.LOG_TRACE, msg);
		}


		[Conditional("LOG_TRACE")]
		public static void T(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_TRACE, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		public static void T(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_TRACE, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		public static void T(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_TRACE, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		public static void D(string msg)
		{
			LogImpl(LogLevel.LOG_DEBUG, msg);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		public static void D(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_DEBUG, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		public static void D(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_DEBUG, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		public static void D(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_DEBUG, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string msg)
		{
			LogImpl(LogLevel.LOG_VERBOSE, msg);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string fmt, int param, int param_1)
		{
			formatParam_2[0] = param.ToString();
			formatParam_2[1] = param_1.ToString();
			LogFmtImpl(LogLevel.LOG_VERBOSE, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string fmt, string param, string param_1)
		{
			formatParam_2[0] = param;
			formatParam_2[1] = param_1;
			LogFmtImpl(LogLevel.LOG_VERBOSE, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_VERBOSE, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_VERBOSE, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		public static void V(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_VERBOSE, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string msg)
		{
			LogImpl(LogLevel.LOG_WARNING, msg);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string fmt, int param, int param_1)
		{
			formatParam_2[0] = param.ToString();
			formatParam_2[1] = param_1.ToString();
			LogFmtImpl(LogLevel.LOG_WARNING, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string fmt, string param, string param_1)
		{
			formatParam_2[0] = param;
			formatParam_2[1] = param_1;
			LogFmtImpl(LogLevel.LOG_WARNING, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_WARNING, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_WARNING, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		public static void W(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_WARNING, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string msg)
		{
			LogImpl(LogLevel.LOG_HIGHLIGHT, msg);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string fmt, string param, string param_1)
		{
			formatParam_2[0] = param;
			formatParam_2[1] = param_1;
			LogFmtImpl(LogLevel.LOG_HIGHLIGHT, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string fmt, int param, int param_1)
		{
			formatParam_2[0] = param.ToString();
			formatParam_2[1] = param_1.ToString();
			LogFmtImpl(LogLevel.LOG_HIGHLIGHT, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_HIGHLIGHT, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_HIGHLIGHT, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		public static void H(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_HIGHLIGHT, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string msg)
		{
			LogImpl(LogLevel.LOG_ERROR, msg);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string fmt, string param, string param_1)
		{
			formatParam_2[0] = param;
			formatParam_2[1] = param_1;
			LogFmtImpl(LogLevel.LOG_ERROR, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string fmt, int param, int param_1)
		{
			formatParam_2[0] = param.ToString();
			formatParam_2[1] = param_1.ToString();
			LogFmtImpl(LogLevel.LOG_ERROR, fmt, formatParam_2);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string fmt, string param)
		{
			formatParam_1[0] = param;
			LogFmtImpl(LogLevel.LOG_ERROR, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string fmt, int param)
		{
			formatParam_1[0] = param.ToString();
			LogFmtImpl(LogLevel.LOG_ERROR, fmt, formatParam_1);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		public static void E(string fmt, params object[] param)
		{
			LogFmtImpl(LogLevel.LOG_ERROR, fmt, param);
		}


		[Conditional("LOG_TRACE")]
		[Conditional("LOG_DEBUG")]
		[Conditional("LOG_VERBOSE")]
		[Conditional("LOG_WARNING")]
		[Conditional("LOG_HIGHLIGHT")]
		[Conditional("LOG_ERROR")]
		[Conditional("LOG_EXCEPTION")]
		public static void Exception(Exception e)
		{
			GameException ex;
			if ((ex = e as GameException) != null)
			{
				LogImpl(LogLevel.LOG_ERROR, ex.msg);
			}

			LogImpl(LogLevel.LOG_EXCEPTION, e.ToString());
		}


		private static void ConfigureLogging()
		{
			string logPath = GetLogPath();
			XmlLoggingConfiguration loggingConfiguration =
				new XmlLoggingConfiguration(Application.streamingAssetsPath + "/NLog.config");
			loggingConfiguration.Variables.Add("instanceName", GetHostName());
			loggingConfiguration.Variables.Add("logPath", logPath);
			UnityConsoleTarget unityConsoleTarget = new UnityConsoleTarget("unityConsoleTarget");
			unityConsoleTarget.Layout = Layout.FromString("[${level:uppercase=true}] ${message}");
			loggingConfiguration.AddTarget(unityConsoleTarget);
			loggingConfiguration.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, unityConsoleTarget, "LiteNetLib",
				true);
			loggingConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, unityConsoleTarget, "Blis");
			LogManager.Configuration = loggingConfiguration;
			Singleton<LiteNetLogger>.inst.Enable();

			// co: dotPeek
			// string logPath = Log.GetLogPath();
			// XmlLoggingConfiguration xmlLoggingConfiguration = new XmlLoggingConfiguration(Application.streamingAssetsPath + "/NLog.config");
			// xmlLoggingConfiguration.Variables.Add("instanceName", Log.GetHostName());
			// xmlLoggingConfiguration.Variables.Add("logPath", logPath);
			// UnityConsoleTarget unityConsoleTarget = new UnityConsoleTarget("unityConsoleTarget");
			// unityConsoleTarget.Layout = Layout.FromString("[${level:uppercase=true}] ${message}");
			// xmlLoggingConfiguration.AddTarget(unityConsoleTarget);
			// xmlLoggingConfiguration.AddRule(LogLevel.Info, LogLevel.Fatal, unityConsoleTarget, "LiteNetLib", true);
			// xmlLoggingConfiguration.AddRule(LogLevel.Trace, LogLevel.Fatal, unityConsoleTarget, "Blis");
			// LogManager.Configuration = xmlLoggingConfiguration;
			// Singleton<LiteNetLogger>.inst.Enable();
		}


		private static string GetLogPath()
		{
			return Application.dataPath + "/Logs/bser";
		}


		private static string GetHostName()
		{
			string result;
			try
			{
				if (_hostName == null)
				{
					_hostName = Dns.GetHostName();
				}

				result = _hostName;
			}
			catch (Exception)
			{
				result = "";
			}

			return result;
		}


		public static void Gauge(double value, string bucket) { }
	}
}