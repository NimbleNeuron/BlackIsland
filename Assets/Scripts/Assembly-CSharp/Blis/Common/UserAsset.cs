using Newtonsoft.Json;

namespace Blis.Common
{
	public class UserAsset
	{
		[JsonProperty("ac")] public int aCoin;
		[JsonProperty("np")] public int np;

		public void UpdateAsset(UserAsset userAsset)
		{
			aCoin = userAsset.aCoin;
			np = userAsset.np;
		}
	}
}