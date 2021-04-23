using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemMiscData : ItemData
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MiscItemType miscItemType;

		[JsonConstructor]
		public ItemMiscData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable,
			int initialCount, int makeMaterial1, int makeMaterial2, string craftAnimTrigger, MiscItemType miscItemType)
			: base(code, name, itemType, itemGrade, stackable, initialCount, makeMaterial1, makeMaterial2,
				craftAnimTrigger)
		{
			this.miscItemType = miscItemType;
		}


		public override int GetSubType()
		{
			return (int) miscItemType;
		}


		public override MasteryConditionType GetMasteryConditionType()
		{
			MiscItemType miscItemType = this.miscItemType;
			if (miscItemType == MiscItemType.None)
			{
				return MasteryConditionType.None;
			}

			if (miscItemType != MiscItemType.Material)
			{
				throw new ArgumentOutOfRangeException();
			}

			return MasteryConditionType.CraftMaterial;
		}
	}
}