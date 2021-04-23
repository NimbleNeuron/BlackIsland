using System.Collections;
using Neptune.Http;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class WebSocketRequest
	{
		[JsonProperty("rid")] public long id;
		[JsonProperty("mtd")] public string method;
		[JsonProperty("prm")] public Hashtable param;
		[JsonProperty("tme")] public long time;

		public WebSocketRequest(string method, long id, long time, Hashtable param)
		{
			this.method = method;
			this.id = id;
			this.time = time;
			this.param = param;
		}

		public WebSocketRequest(string method, long id, long time, params object[] param)
		{
			this.method = method;
			this.id = id;
			this.time = time;
			this.param = new KeyValueList(param).ToHashtable();
		}
	}
}