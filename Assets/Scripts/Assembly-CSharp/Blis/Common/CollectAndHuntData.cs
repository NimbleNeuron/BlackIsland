using Newtonsoft.Json;

namespace Blis.Common
{
	public class CollectAndHuntData
	{
		public readonly int code;


		public readonly int itemCode;

		[JsonConstructor]
		public CollectAndHuntData(int code, int itemCode)
		{
			this.code = code;
			this.itemCode = itemCode;
		}
	}
}