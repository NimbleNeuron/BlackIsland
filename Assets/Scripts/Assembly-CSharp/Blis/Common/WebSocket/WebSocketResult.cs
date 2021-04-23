using Newtonsoft.Json;

namespace Blis.Common
{
	public class WebSocketResult<T>
	{
		[JsonProperty("rst")] public T result;
	}
}