using Newtonsoft.Json;

namespace Blis.Common
{
	public class ApiResultContainer
	{
		[JsonProperty("cod")] public int code;


		[JsonProperty("msg")] public string msg;
	}
}