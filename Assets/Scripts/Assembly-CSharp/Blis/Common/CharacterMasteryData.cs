using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class CharacterMasteryData
	{
		
		[JsonConstructor]
		public CharacterMasteryData(int code, MasteryType weapon1, MasteryType weapon2, MasteryType weapon3, MasteryType weapon4, MasteryType exploration1, MasteryType exploration2, MasteryType exploration3, MasteryType exploration4, MasteryType survival1, MasteryType survival2, MasteryType survival3, MasteryType survival4)
		{
			this.code = code;
			this.weapon1 = weapon1;
			this.weapon2 = weapon2;
			this.weapon3 = weapon3;
			this.weapon4 = weapon4;
			this.exploration1 = exploration1;
			this.exploration2 = exploration2;
			this.exploration3 = exploration3;
			this.exploration4 = exploration4;
			this.survival1 = survival1;
			this.survival2 = survival2;
			this.survival3 = survival3;
			this.survival4 = survival4;
		}

		
		public List<MasteryType> GetMasteries()
		{
			List<MasteryType> list = new List<MasteryType>();
			if (this.weapon1 != MasteryType.None)
			{
				list.Add(this.weapon1);
			}
			if (this.weapon2 != MasteryType.None)
			{
				list.Add(this.weapon2);
			}
			if (this.weapon3 != MasteryType.None)
			{
				list.Add(this.weapon3);
			}
			if (this.weapon4 != MasteryType.None)
			{
				list.Add(this.weapon4);
			}
			return list;
		}

		
		public readonly int code;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType weapon1;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType weapon2;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType weapon3;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType weapon4;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType exploration1;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType exploration2;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType exploration3;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType exploration4;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType survival1;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType survival2;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType survival3;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MasteryType survival4;
	}
}
