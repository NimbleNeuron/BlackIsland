using System;
using Neptune.Http;

namespace Blis.Common
{
	public static class KakaoPcApi
	{
		public static Func<HttpRequest> RequestKakaoPc()
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/external/pcCafe", Array.Empty<object>()), null);
		}


		public class KakaoApiResult { }
	}
}