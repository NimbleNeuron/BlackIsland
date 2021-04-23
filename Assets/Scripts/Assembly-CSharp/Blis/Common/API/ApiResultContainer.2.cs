using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class ApiResultContainer<T> : ApiResultContainer
	{
		
		[JsonProperty("rst")] public T results;
	}
}