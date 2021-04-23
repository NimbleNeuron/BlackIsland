using Newtonsoft.Json;

namespace Blis.Common
{
	public class StartItem
	{
		public readonly int code;
		public readonly int count;
		public readonly int groupCode;
		public readonly int itemCode;

		[JsonConstructor]
		public StartItem(int code, int groupCode, int itemCode, int count)
		{
			this.code = code;
			this.groupCode = groupCode;
			this.itemCode = itemCode;
			this.count = count;
		}
	}
}