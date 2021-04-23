using System;
using Neptune.Http;

namespace Blis.Common
{
	public class TutorialApi
	{
		public static Func<HttpRequest> GetTutorialResult(int tutorialRewardCode)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/users/tutorial/result", Array.Empty<object>()),
				tutorialRewardCode);
		}


		public class TutorialResult
		{
			public bool result;
		}
	}
}