using System.Diagnostics;

namespace LiteNetLib
{
	
	public static class NetDebug
	{
		
		private static void WriteLogic(NetLogLevel logLevel, string str, params object[] args)
		{
			object debugLogLock = NetDebug.DebugLogLock;
			lock (debugLogLock)
			{
				if (NetDebug.Logger == null)
				{
					UnityEngine.Debug.Log(string.Format(str, args));
				}
				else
				{
					NetDebug.Logger.WriteNet(logLevel, str, args);
				}
			}
		}

		
		[Conditional("DEBUG_MESSAGES")]
		internal static void Write(string str, params object[] args)
		{
			NetDebug.WriteLogic(NetLogLevel.Trace, str, args);
		}

		
		[Conditional("DEBUG_MESSAGES")]
		internal static void Write(NetLogLevel level, string str, params object[] args)
		{
			NetDebug.WriteLogic(level, str, args);
		}

		
		[Conditional("DEBUG_MESSAGES")]
		[Conditional("DEBUG")]
		internal static void WriteForce(string str, params object[] args)
		{
			NetDebug.WriteLogic(NetLogLevel.Trace, str, args);
		}

		
		[Conditional("DEBUG_MESSAGES")]
		[Conditional("DEBUG")]
		internal static void WriteForce(NetLogLevel level, string str, params object[] args)
		{
			NetDebug.WriteLogic(level, str, args);
		}

		
		internal static void WriteError(string str, params object[] args)
		{
			NetDebug.WriteLogic(NetLogLevel.Error, str, args);
		}

		
		public static INetLogger Logger = null;

		
		private static readonly object DebugLogLock = new object();
	}
}
