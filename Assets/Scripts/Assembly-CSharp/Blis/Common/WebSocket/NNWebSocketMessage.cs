using Newtonsoft.Json;

namespace Blis.Common
{
	public class NNWebSocketMessage
	{
		[JsonProperty("cod")] public int code;
		[JsonProperty("rid")] public long id;
		[JsonIgnore] public string json = "";
		[JsonProperty("mtd")] public string method;
		[JsonProperty("msg")] public string msg;
		[JsonProperty("tme")] public long time;
		public bool IsNotification => !string.IsNullOrEmpty(method);
	}
}