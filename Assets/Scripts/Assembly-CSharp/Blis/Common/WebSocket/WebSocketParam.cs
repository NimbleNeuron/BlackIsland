using Newtonsoft.Json;

namespace Blis.Common
{
	public class WebSocketParam<T>
	{
		[JsonProperty("prm")] public T param;
	}
}