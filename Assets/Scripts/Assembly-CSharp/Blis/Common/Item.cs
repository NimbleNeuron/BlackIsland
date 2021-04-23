using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	public class Item
	{
		[Key(0)] public readonly int id;
		[Key(1)] public readonly int itemCode;
		[IgnoreMember] public readonly ItemSpecialData itemSpecialData;
		[Key(4)] public int addRecovery;
		private bool isDirty;
		private ItemData itemData;
		[Key(5)] public ItemMadeType madeType;
		private int? maxBulletCount;
		private bool? weaponConsumable;
		private WeaponTypeInfoData weaponTypeInfoData;

		[SerializationConstructor]
		public Item(int id, int itemCode, int amount, int bullet)
		{
			this.id = id;
			this.itemCode = itemCode;
			Amount = amount;
			Bullet = bullet;
			itemData = null;
			addRecovery = 0;
		}


		public Item(int id, int itemCode, int amount, int bullet, ItemData itemData) : this(id, itemCode, amount,
			bullet)
		{
			this.itemData = itemData;
		}


		[IgnoreMember] public virtual bool IsAmmo => false;


		[IgnoreMember]
		public ItemData ItemData {
			get
			{
				if (itemData == null)
				{
					itemData = GameDB.item.FindItemByCode(itemCode);
				}

				return itemData;
			}
		}


		[IgnoreMember]
		public WeaponTypeInfoData WeaponTypeInfoData {
			get
			{
				if (weaponTypeInfoData == null)
				{
					weaponTypeInfoData =
						GameDB.mastery.GetWeaponTypeInfoData(ItemData.GetSubTypeData<ItemWeaponData>().weaponType);
				}

				return weaponTypeInfoData;
			}
		}


		[IgnoreMember]
		public bool WeaponConsumable {
			get
			{
				if (weaponConsumable == null)
				{
					ItemWeaponData subTypeData = ItemData.GetSubTypeData<ItemWeaponData>();
					if (subTypeData != null)
					{
						weaponConsumable = subTypeData.consumable;
					}
					else
					{
						weaponConsumable = false;
					}
				}

				return weaponConsumable.Value;
			}
		}


		[IgnoreMember]
		public int MaxBulletCount {
			get
			{
				if (maxBulletCount == null)
				{
					maxBulletCount = GameDB.item.GetBulletCapacity(itemData.code);
				}

				return maxBulletCount.Value;
			}
		}


		
		[IgnoreMember] public float RemainCoolTime { get; set; }


		[Key(2)] public int Amount { get; private set; }


		[Key(3)] public int Bullet { get; private set; }


		public T GetItemData<T>() where T : ItemData
		{
			if (ItemData is T)
			{
				return ItemData as T;
			}

			return default;
		}


		public void Merge(Item item)
		{
			if (ItemData.code.Equals(item.ItemData.code) && madeType.Equals(item.madeType))
			{
				int num = ItemData.stackable - Amount;
				if (num > 0)
				{
					int count = Mathf.Min(num, item.Amount);
					item.SubAmount(count);
					AddAmount(count);
				}
			}
		}


		public void AddAmount(int count)
		{
			if (count + Amount > ItemData.stackable)
			{
				throw new GameException(ErrorType.Internal);
			}

			isDirty = true;
			Amount += count;
		}


		public void ForceAddAmount(int count)
		{
			isDirty = true;
			Amount += count;
		}


		public void SubAmount(int count)
		{
			if (count > Amount)
			{
				throw new GameException(ErrorType.NotEnoughItem);
			}

			isDirty = true;
			Amount -= count;
		}


		public bool IsEmpty()
		{
			return Amount <= 0;
		}


		public bool IsFull()
		{
			return Amount >= ItemData.stackable;
		}


		public void AddBullet(int count)
		{
			int num = count + Bullet;
			int? num2 = maxBulletCount;
			if ((num > num2.GetValueOrDefault()) & (num2 != null))
			{
				throw new GameException(ErrorType.Internal);
			}

			isDirty = true;
			Bullet += count;
		}


		public void SubBullet(int count)
		{
			if (count > Amount)
			{
				throw new GameException(ErrorType.NotEnoughItem);
			}

			isDirty = true;
			Bullet -= count;
		}


		public bool IsEmptyBullet()
		{
			return Bullet <= 0;
		}


		public bool IsFullBullet()
		{
			return Bullet >= MaxBulletCount;
		}


		public bool FlushDirty()
		{
			bool result = isDirty;
			isDirty = false;
			return result;
		}


		public EquipSlotType GetEquipSlotType()
		{
			return itemData.GetEquipSlotType();
		}


		public bool IsEquipItem()
		{
			return itemData.itemType == ItemType.Weapon || itemData.itemType == ItemType.Armor;
		}


		public void SetItemSpecialType(ItemMadeType madeType)
		{
			this.madeType = madeType;
		}


		public void AddRecoveryItem(int addRecovery)
		{
			if (itemData == null || itemData.itemType != ItemType.Consume)
			{
				return;
			}

			this.addRecovery = addRecovery;
		}


		public int GetRecovery(bool isSp)
		{
			ItemConsumableData itemConsumableData = GetItemData<ItemConsumableData>();
			if (itemConsumableData == null)
			{
				return 0;
			}

			int num = isSp ? itemConsumableData.spRecover : itemConsumableData.hpRecover;
			return num + (int) (num * (float) addRecovery / 100f);
		}


		public bool IsRecoveryItem()
		{
			if (itemData.itemType != ItemType.Consume)
			{
				return false;
			}

			ItemConsumableData itemConsumableData = GetItemData<ItemConsumableData>();
			return itemConsumableData != null && (itemConsumableData.hpRecover > 0f || itemConsumableData.heal > 0f ||
			                                      itemConsumableData.spRecover > 0f);
		}
	}
}