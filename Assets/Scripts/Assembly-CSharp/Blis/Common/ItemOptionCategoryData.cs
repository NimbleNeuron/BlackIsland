using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class ItemOptionCategoryData
	{
		
		[JsonConstructor]
		public ItemOptionCategoryData(int code, ItemOptionCategory itemOptionCategory, int itemCode)
		{
			this.code = code;
			this.itemOptionCategory = itemOptionCategory;
			this.itemCode = itemCode;
		}

		
		public readonly int code;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemOptionCategory itemOptionCategory;

		
		public readonly int itemCode;
	}
}
