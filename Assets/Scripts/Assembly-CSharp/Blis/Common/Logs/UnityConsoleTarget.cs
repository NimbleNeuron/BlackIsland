using NLog;
using NLog.Targets;
using UnityEngine;

namespace Blis.Common.Logs
{
	public class UnityConsoleTarget : TargetWithLayout
	{
		public UnityConsoleTarget() { }


		public UnityConsoleTarget(string name) : this()
		{
			Name = name;
		}


		protected override void Write(LogEventInfo logEvent)
		{
			string str = Layout.Render(logEvent);
			if (logEvent.Level >= NLog.LogLevel.Error)
			{
				Debug.LogError(str);
			}
			else if (logEvent.Level >= NLog.LogLevel.Warn)
			{
				Debug.LogWarning(str);
			}
			else if (logEvent.Level >= NLog.LogLevel.Info)
			{
				Debug.Log(str);
			}
			else
			{
				Debug.Log(str);
			}
		}
	}
}