using Newtonsoft.Json;

namespace Blis.Common
{
	public class BattleOptionParam
	{
		[JsonProperty("uob")] private bool useObserver;

		public bool IsUseObserver()
		{
			return useObserver;
		}


		public void SetUseObserver(bool useObserver)
		{
			this.useObserver = useObserver;
		}
	}
}