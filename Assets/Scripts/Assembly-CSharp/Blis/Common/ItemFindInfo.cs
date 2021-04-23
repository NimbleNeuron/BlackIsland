using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemFindInfo
	{
		public readonly bool airSupply;


		public readonly int code;


		public readonly int collectibleCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntBat;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntBear;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntBoar;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntChicken;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntWickline;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntWildDog;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly DropFrequency huntWolf;


		public readonly int itemCode;

		[JsonConstructor]
		public ItemFindInfo(int code, int itemCode, DropFrequency huntChicken, DropFrequency huntBat,
			DropFrequency huntBoar, DropFrequency huntWildDog, DropFrequency huntWolf, DropFrequency huntBear,
			DropFrequency huntWickline, int collectibleCode, bool airSupply)
		{
			this.code = code;
			this.itemCode = itemCode;
			this.huntChicken = huntChicken;
			this.huntBat = huntBat;
			this.huntBoar = huntBoar;
			this.huntWildDog = huntWildDog;
			this.huntWolf = huntWolf;
			this.huntBear = huntBear;
			this.huntWickline = huntWickline;
			this.collectibleCode = collectibleCode;
			this.airSupply = airSupply;
		}


		public bool IsNeedHunt()
		{
			return huntChicken != DropFrequency.Never || huntBat != DropFrequency.Never ||
			       huntBoar != DropFrequency.Never || huntWildDog != DropFrequency.Never ||
			       huntWolf != DropFrequency.Never || huntBear != DropFrequency.Never ||
			       huntWickline > DropFrequency.Never;
		}
	}
}