using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemSpecialData : ItemData
	{
		public readonly int consumeCount;


		public readonly int summonCode;


		[JsonConverter(typeof(StringEnumConverter))]
		public SpecialItemType specialItemType;

		[JsonConstructor]
		public ItemSpecialData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable,
			int initialCount, int makeMaterial1, int makeMaterial2, string craftAnimTrigger,
			SpecialItemType specialItemType, int consumeCount, int summonCode) : base(code, name, itemType, itemGrade,
			stackable, initialCount, makeMaterial1, makeMaterial2, craftAnimTrigger)
		{
			this.specialItemType = specialItemType;
			this.consumeCount = consumeCount;
			this.summonCode = summonCode;
		}


		public override int GetSubType()
		{
			return (int) specialItemType;
		}


		public override MasteryConditionType GetMasteryConditionType()
		{
			switch (specialItemType)
			{
				case SpecialItemType.None:
				case SpecialItemType.Ammo:
					return MasteryConditionType.None;
				case SpecialItemType.Special:
				case SpecialItemType.Control:
					return MasteryConditionType.CraftSpecial;
				case SpecialItemType.Summon:
					return MasteryConditionType.CraftTrap;
			}

			throw new ArgumentOutOfRangeException();
		}
	}
}