using LiteNetLib;

namespace Blis.Common
{
	public class LiteNetLogger : Singleton<LiteNetLogger>, INetLogger
	{
		public void WriteNet(NetLogLevel level, string str, params object[] args)
		{
			switch (level)
			{
				case NetLogLevel.Warning:
					Log.W(str, args);
					return;
				case NetLogLevel.Error:
					Log.E(str, args);
					return;
				case NetLogLevel.Trace:
					break;
				case NetLogLevel.Info:
					Log.V(str, args);
					break;
				default:
					return;
			}
		}

		public void Enable()
		{
			NetDebug.Logger = this;
		}


		public void Disable()
		{
			NetDebug.Logger = null;
		}
	}
}