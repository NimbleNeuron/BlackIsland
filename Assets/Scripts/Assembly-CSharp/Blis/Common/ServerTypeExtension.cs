namespace Blis.Common
{
	public static class ServerTypeExtension
	{
		public static string GetApiServerURL()
		{
			if (GlobalUserData.gaap)
			{
				return "http://link-2b0ual6d.gaapqcloud.com/api";
			}

			return "https://bser-rest-release.bser.io/api";
		}

		public static string GetDataServerURL()
		{
#if LOCAL_SERVER
			return "https://localhost:5000/";
#endif
			
			if (GlobalUserData.gaap)
			{
				return "http://link-ehuwazdr.gaapqcloud.com";
			}

			return "https://bser-gamedb-release.bser.io";
		}

		public static string GetEventLogServerURL()
		{
			return "";
		}

		public static string GetCrashReportURL()
		{
			if (GlobalUserData.gaap)
			{
				return "http://link-ehuwazdr.gaapqcloud.com/logging/crash";
			}

			return "https://bser-gamedb-release.bser.io/logging/crash";
		}
		
		public static string GetMaintenanceURL()
		{
			return "https://bser-gamedb-release.bser.io/maintenance";
		}

		public static string GetApiProxyURL()
		{
			return "https://api-proxy.eternalreturn.io";
		}
	}
}