using System;
using Neptune.Http;

namespace Blis.Common
{
	public static class GameDBApi
	{
		private static readonly string dataServerUrl = ApiConstants.DataServerUrl + "/metaData";

		public static Func<HttpRequest> GetGameData(string data)
		{
			return HttpRequestFactory.Get(dataServerUrl + "/" + data + "/");
		}


		public static Func<HttpRequest> GetVersionData()
		{
			return HttpRequestFactory.Get(dataServerUrl + "/hash/");
		}
	}
}