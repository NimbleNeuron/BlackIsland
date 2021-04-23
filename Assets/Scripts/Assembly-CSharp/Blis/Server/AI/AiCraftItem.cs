using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("입력된 난이도와 SetPoint에 맞는 아이템을 제작하고, AutoIncrementSetPoint가 켜져있으면 SetPoint를 1 증가시킨다.")]
	public class AiCraftItem : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsTypeOf<WorldBotPlayerCharacter>())
			{
				base.EndAction(false);
				return;
			}
			WorldBotPlayerCharacter worldBotPlayerCharacter = (WorldBotPlayerCharacter)base.agent;
			if (worldBotPlayerCharacter == null)
			{
				base.EndAction(false);
				return;
			}
			this.craftSlotNumber = 0;
			foreach (BotCraft data in GameDB.bot.GetBotCraftBySetPoint(this.craftSetPoint.value))
			{
				this.CraftItem(worldBotPlayerCharacter, data);
			}
			worldBotPlayerCharacter.SendEquipmentUpdate();
			if (this.autoIncrementSetPoint)
			{
				BBParameter<int> bbparameter = this.craftSetPoint;
				int value = bbparameter.value;
				bbparameter.value = value + 1;
			}
			base.EndAction(true);
		}

		
		private void CraftItem(WorldBotPlayerCharacter character, BotCraft data)
		{
			ItemData itemData = null;
			switch (data.type)
			{
			case BotCraftType.Weapon:
			{
				Item weapon = character.GetWeapon();
				itemData = GameDB.item.GetRandomWeaponItem(weapon.ItemData.GetSubTypeData<ItemWeaponData>().weaponType, data.GetItemGrade(this.difficulty.value));
				break;
			}
			case BotCraftType.Head:
			case BotCraftType.Chest:
			case BotCraftType.Arm:
			case BotCraftType.Leg:
			case BotCraftType.Trinket:
				itemData = GameDB.item.GetRandomArmorItem(this.GetEquipSlotType(data.type), data.GetItemGrade(this.difficulty.value));
				break;
			case BotCraftType.Beverage:
			case BotCraftType.Food:
				itemData = GameDB.item.GetRandomConsumableItem(this.GetConsumableType(data.type), data.GetItemGrade(this.difficulty.value));
				break;
			case BotCraftType.Summon:
				itemData = GameDB.item.GetRandomSummonItem(data.GetItemGrade(this.difficulty.value));
				break;
			}
			if (itemData == null)
			{
				return;
			}
			Item item = MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData, itemData.initialCount);
			if (itemData.IsEquipItem())
			{
				Item item2;
				character.Equipment.Equip(item, out item2);
				if (item != null && item.ItemData.IsGunType())
				{
					character.UpdateGunReloadTime(item.itemCode);
					character.GunReload(true);
				}
			}
			else
			{
				Inventory inventory = character.Inventory;
				int num = this.craftSlotNumber;
				this.craftSlotNumber = num + 1;
				inventory.ForceAddItem(num, item);
			}
		}

		
		private EquipSlotType GetEquipSlotType(BotCraftType craftType)
		{
			switch (craftType)
			{
			case BotCraftType.Weapon:
				return EquipSlotType.Weapon;
			case BotCraftType.Head:
				return EquipSlotType.Head;
			case BotCraftType.Chest:
				return EquipSlotType.Chest;
			case BotCraftType.Arm:
				return EquipSlotType.Arm;
			case BotCraftType.Leg:
				return EquipSlotType.Leg;
			case BotCraftType.Trinket:
				return EquipSlotType.Trinket;
			default:
				return EquipSlotType.None;
			}
		}

		
		private ItemConsumableType GetConsumableType(BotCraftType craftType)
		{
			if (craftType == BotCraftType.Beverage)
			{
				return ItemConsumableType.Beverage;
			}
			if (craftType == BotCraftType.Food)
			{
				return ItemConsumableType.Food;
			}
			return ItemConsumableType.None;
		}

		
		public BBParameter<BotDifficulty> difficulty;

		
		public BBParameter<int> craftSetPoint;

		
		public bool autoIncrementSetPoint;

		
		private int craftSlotNumber;
	}
}
