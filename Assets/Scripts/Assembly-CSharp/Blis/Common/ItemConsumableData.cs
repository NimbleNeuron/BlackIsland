using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ItemConsumableData : ItemData
	{
		[JsonProperty] private readonly string consumableTag;


		[JsonIgnore] public readonly ItemConsumableTag consumableTagFlag;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemConsumableType consumableType;


		public readonly int heal;


		public readonly int hpRecover;


		public readonly int spRecover;

		[JsonConstructor]
		public ItemConsumableData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable,
			int initialCount, int makeMaterial1, int makeMaterial2, string craftAnimTrigger,
			ItemConsumableType consumableType, string consumableTag, int heal, int hpRecover, int spRecover) : base(
			code, name, itemType, itemGrade, stackable, initialCount, makeMaterial1, makeMaterial2, craftAnimTrigger)
		{
			this.consumableType = consumableType;
			this.consumableTag = consumableTag;
			this.heal = heal;
			this.hpRecover = hpRecover;
			this.spRecover = spRecover;
			if (consumableTag != "None")
			{
				string[] array = consumableTag.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					ItemConsumableTag itemConsumableTag;
					if (Enum.TryParse<ItemConsumableTag>(array[i], true, out itemConsumableTag))
					{
						consumableTagFlag |= itemConsumableTag;
					}
				}
			}
		}


		public override int GetSubType()
		{
			return (int) consumableType;
		}


		public override MasteryConditionType GetMasteryConditionType()
		{
			switch (consumableType)
			{
				case ItemConsumableType.None:
					return MasteryConditionType.None;
				case ItemConsumableType.Food:
					return MasteryConditionType.CraftFood;
				case ItemConsumableType.Beverage:
					return MasteryConditionType.CraftBeverage;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		public override bool IsShareCooldown(ItemData itemData)
		{
			if (itemData.itemType != itemType)
			{
				return false;
			}

			ItemConsumableData subTypeData = itemData.GetSubTypeData<ItemConsumableData>();
			return subTypeData != null && consumableType == subTypeData.consumableType;
		}
	}
}