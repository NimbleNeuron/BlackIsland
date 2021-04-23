using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class RecommendArea
	{
		public readonly int area1Code;


		public readonly int area2Code;


		public readonly int area3Code;


		public readonly int area4Code;


		public readonly int area5Code;


		public readonly int characterCode;


		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType mastery;


		public readonly bool recommend;

		[JsonConstructor]
		public RecommendArea(int code, int area1Code, int area2Code, int area3Code, int area4Code, int area5Code,
			int characterCode, MasteryType mastery, bool recommend)
		{
			this.code = code;
			this.area1Code = area1Code;
			this.area2Code = area2Code;
			this.area3Code = area3Code;
			this.area4Code = area4Code;
			this.area5Code = area5Code;
			this.characterCode = characterCode;
			this.mastery = mastery;
			this.recommend = recommend;
		}


		public List<int> GetAreaList()
		{
			List<int> list = new List<int>();
			if (0 < area1Code)
			{
				list.Add(area1Code);
			}

			if (0 < area2Code)
			{
				list.Add(area2Code);
			}

			if (0 < area3Code)
			{
				list.Add(area3Code);
			}

			if (0 < area4Code)
			{
				list.Add(area4Code);
			}

			if (0 < area5Code)
			{
				list.Add(area5Code);
			}

			return list;
		}
	}
}