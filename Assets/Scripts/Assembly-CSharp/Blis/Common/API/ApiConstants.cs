namespace Blis.Common
{
	public static class ApiConstants
	{
		public const int ResultCodeSuccess = 200;

		public static string MatchingUrl;
		public static string ChinaRestApiUrl;
		public static string ChinaMatchingUrl;

		public static ServerType serverType = ServerType.RELEASE;


		public static string RootHttpsUrl => ServerTypeExtension.GetApiServerURL();
		public static string DataServerUrl => ServerTypeExtension.GetDataServerURL();
		public static string EventLogServerUrl => ServerTypeExtension.GetEventLogServerURL();
		public static string CrashReportUrl => ServerTypeExtension.GetCrashReportURL();
		public static string MaintenaceUrl => ServerTypeExtension.GetMaintenanceURL();
		public static string ApiProxyUrl => ServerTypeExtension.GetApiProxyURL();

		public static string Url(string path, params object[] args)
		{
			if (args.Length != 0)
			{
				path = string.Format(path, args);
			}

			return RootHttpsUrl + (RootHttpsUrl.EndsWith("/") ? path.Substring(1) : path);
		}
	}
}