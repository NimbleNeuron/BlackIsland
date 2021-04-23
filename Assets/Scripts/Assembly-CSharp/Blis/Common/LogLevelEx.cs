using UnityEngine;

namespace Blis.Common
{
	public static class LogLevelEx
	{
		public static LogType GetLogType(this LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.LOG_TRACE:
					return LogType.Log;
				case LogLevel.LOG_DEBUG:
					return LogType.Log;
				case LogLevel.LOG_VERBOSE:
					return LogType.Log;
				case LogLevel.LOG_WARNING:
					return LogType.Warning;
				case LogLevel.LOG_HIGHLIGHT:
					return LogType.Log;
				case LogLevel.LOG_ERROR:
					return LogType.Error;
				case LogLevel.LOG_EXCEPTION:
					return LogType.Exception;
				default:
					return LogType.Log;
			}
		}
	}
}