using NLog;

namespace Blis.Common.Logs
{
	public class NLogLoggers
	{
		public static readonly Logger DefaultLogger = LogManager.GetLogger("Blis");


		public static readonly Logger LiteNetLibLogger = LogManager.GetLogger("LiteNetLib");
	}
}