using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Blis.Common
{
	public class ItemData
	{
		public readonly int code;


		public readonly string craftAnimTrigger;


		public readonly int initialCount;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemGrade itemGrade;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly ItemType itemType;


		public readonly int makeMaterial1;


		public readonly int makeMaterial2;


		public readonly string name;


		private readonly string spriteName;


		public readonly int stackable;

		[JsonConstructor]
		public ItemData(int code, string name, ItemType itemType, ItemGrade itemGrade, int stackable, int initialCount,
			int makeMaterial1, int makeMaterial2, string craftAnimTrigger)
		{
			this.code = code;
			this.name = name;
			this.itemType = itemType;
			this.itemGrade = itemGrade;
			this.stackable = stackable;
			this.initialCount = initialCount;
			this.makeMaterial1 = makeMaterial1;
			this.makeMaterial2 = makeMaterial2;
			this.craftAnimTrigger = craftAnimTrigger;
			spriteName = "ItemIcon_" + code.ToString().PadLeft(6, '0');
		}


		public Sprite GetSprite()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetItemSprite(spriteName);
		}


		public Sprite GetGradeSprite()
		{
			return itemGrade.GetGradeBgSprite();
		}


		public T GetSubTypeData<T>() where T : ItemData
		{
			if (this is T)
			{
				return this as T;
			}

			Log.E("ItemData Cast Miss");
			return default;
		}


		public virtual int GetSubType()
		{
			return 0;
		}


		public virtual MasteryConditionType GetMasteryConditionType()
		{
			return MasteryConditionType.None;
		}


		public virtual MasteryType GetMasteryType()
		{
			return MasteryType.None;
		}


		public Sprite GetMapSprite()
		{
			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Map_" +
				code.ToString().PadLeft(6, '0'));
		}


		public virtual bool IsShareCooldown(ItemData itemData)
		{
			return code == itemData.code;
		}


		public virtual bool IsGunType()
		{
			return false;
		}


		public virtual bool IsThrowType()
		{
			return false;
		}


		public virtual bool IsLeafNodeItem()
		{
			return makeMaterial1 <= 0 && makeMaterial2 <= 0;
		}


		public bool IsEquipItem()
		{
			return itemType == ItemType.Weapon || itemType == ItemType.Armor;
		}


		public virtual float GetStatValue(StatType stateType)
		{
			return 0f;
		}


		public EquipSlotType GetEquipSlotType()
		{
			ItemType itemType = this.itemType;
			if (itemType == ItemType.Weapon)
			{
				return EquipSlotType.Weapon;
			}

			if (itemType != ItemType.Armor)
			{
				return EquipSlotType.None;
			}

			return GetSubTypeData<ItemArmorData>().armorType.GetEquipSlotType();
		}
	}
}