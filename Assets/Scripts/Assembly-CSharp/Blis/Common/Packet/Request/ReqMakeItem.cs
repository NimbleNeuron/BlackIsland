using System;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqMakeItem, false)]
	public class ReqMakeItem : ReqPacket
	{
		[Key(0)] public int resultItem;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldPlayerCharacter playerCharacter = playerSession.Character;
			if (!playerCharacter.CanAnyAction(ActionType.Craft) || playerCharacter.IsRest)
			{
				return;
			}

			ItemData itemData = GameDB.item.FindItemByCode(resultItem);
			if (itemData.code == 0)
			{
				throw new GameException(ErrorType.ItemNotFound);
			}

			Item material1 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial1, itemData.itemType,
				playerCharacter.Inventory, playerCharacter.Equipment);
			Item material2 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial2, itemData.itemType,
				playerCharacter.Inventory, playerCharacter.Equipment);
			if (material1 == null || material2 == null)
			{
				throw new GameException(ErrorType.NotEnoughItem);
			}

			Item item = gameService.Spawn.CreateItem(itemData, itemData.initialCount);
			Item createItem =
				SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeMakeItem(playerCharacter, item);
			if (createItem != null)
			{
				item = createItem;
			}

			bool isCombineable = GameDB.item.IsCombinable(item, playerCharacter.Inventory,
				playerCharacter.Equipment, ref material1, ref material2);
			if (!isCombineable)
			{
				throw new GameException(ErrorType.NotEnoughInventory);
			}

			CastingActionType actionCostType = ActionCostData.GetActionCostType(itemData);
			Action completeCallback = () =>
			{
				material1 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial1, itemData.itemType,
					playerCharacter.Inventory, playerCharacter.Equipment);
				material2 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial2, itemData.itemType,
					playerCharacter.Inventory, playerCharacter.Equipment);
				isCombineable = GameDB.item.IsCombinable(item, playerSession.Character.Inventory,
					playerSession.Character.Equipment, ref material1, ref material2);
				if (material1 == null || material2 == null)
				{
					throw new GameException(ErrorType.NotEnoughItem);
				}

				if (!isCombineable)
				{
					throw new GameException(ErrorType.NotEnoughInventory);
				}

				createItem =
					SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterMakeItem(playerSession.Character,
						item);
				if (createItem != null)
				{
					item = createItem;
				}

				material1.SubAmount(1);
				if (material1.IsEmpty())
				{
					if (playerSession.Character.FindInventoryItemById(material1.id, material1.madeType) != null)
					{
						playerSession.Character.FinishBulletCooldown(material1.id);
						playerSession.Character.RemoveInventoryItem(material1.id, material1.madeType);
					}
					else if (playerSession.Character.FindEquipById(material1.id) != null)
					{
						playerSession.Character.FinishBulletCooldown(material1.id);
						playerSession.Character.UnequipItem(material1);
					}
				}

				material2.SubAmount(1);
				if (material2.IsEmpty())
				{
					if (playerSession.Character.FindInventoryItemById(material2.id, material2.madeType) != null)
					{
						playerSession.Character.FinishBulletCooldown(material2.id);
						playerSession.Character.RemoveInventoryItem(material2.id, material2.madeType);
					}
					else if (playerSession.Character.FindEquipById(material2.id) != null)
					{
						playerSession.Character.FinishBulletCooldown(material2.id);
						playerSession.Character.UnequipItem(material2);
					}
				}

				Item obj = null;
				if (item.ItemData.itemType == ItemType.Weapon)
				{
					obj = playerSession.Character.GetWeapon();
				}
				else if (item.ItemData.itemType == ItemType.Armor)
				{
					obj = playerSession.Character.GetArmor(item.ItemData.GetSubTypeData<ItemArmorData>().armorType);
				}

				bool flag = false;
				if (playerSession.Character.IsEquipableItem(item.ItemData))
				{
					if (obj == null)
					{
						playerSession.Character.EquipItem(item);
						flag = true;
					}
					else if (obj.ItemData.code == item.ItemData.code &&
					         item.Amount <= obj.ItemData.stackable - obj.Amount)
					{
						obj.Merge(item);
						flag = true;
					}
				}

				if (!flag)
				{
					playerSession.Character.AddInventoryItem(item, out int _);
				}

				playerSession.Character.AddMasteryConditionExp(new UpdateMasteryInfo
				{
					conditionType = item.ItemData.GetMasteryConditionType(),
					takeMasteryValue = 1,
					itemGrade = item.ItemData.itemGrade,
					masteryType = item.ItemData.GetMasteryType()
				});
				playerSession.Character.CharacterSkill.SkillEvolution.OnCraftingItem(
					playerSession.Character.CharacterCode, itemData,
					playerSession.Character
						.UpdateSkillEvolutionPoint);
				playerSession.Character.CompleteMakeItem();
				playerSession.Character.SendInventoryUpdate(UpdateInventoryType.MakeItem);
				playerSession.Character.SendEquipmentUpdate();
				playerSession.Character.Status.AddItemCraftCount(item.ItemData.itemGrade);
				gameService.Announce.MakeNoise(playerSession.Character, null,
					NoiseType.Crafting);
			};
			playerSession.Character.StartActionCasting(actionCostType, true, null, completeCallback,
				extraParam: itemData.code);

			// co: dotPeek
			// WorldPlayerCharacter playerCharacter = playerSession.Character;
			// if (!playerCharacter.CanAnyAction(ActionType.Craft))
			// {
			// 	return;
			// }
			// if (playerCharacter.IsRest)
			// {
			// 	return;
			// }
			// ItemData itemData = GameDB.item.FindItemByCode(this.resultItem);
			// if (itemData.code == 0)
			// {
			// 	throw new GameException(ErrorType.ItemNotFound);
			// }
			// Item material1 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial1, itemData.itemType, playerCharacter.Inventory, playerCharacter.Equipment);
			// Item material2 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial2, itemData.itemType, playerCharacter.Inventory, playerCharacter.Equipment);
			// if (material1 == null || material2 == null)
			// {
			// 	throw new GameException(ErrorType.NotEnoughItem);
			// }
			// Item item = gameService.Spawn.CreateItem(itemData, itemData.initialCount);
			// Item createItem = SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforeMakeItem(playerCharacter, item);
			// if (createItem != null)
			// {
			// 	item = createItem;
			// }
			// bool isCombineable = GameDB.item.IsCombinable(item, playerCharacter.Inventory, playerCharacter.Equipment, ref material1, ref material2);
			// if (!isCombineable)
			// {
			// 	throw new GameException(ErrorType.NotEnoughInventory);
			// }
			// CastingActionType actionCostType = ActionCostData.GetActionCostType(itemData);
			// Action completeCallback = delegate()
			// {
			// 	material1 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial1, itemData.itemType, playerCharacter.Inventory, playerCharacter.Equipment);
			// 	material2 = GameDB.item.GetItemFromCharacter(itemData.makeMaterial2, itemData.itemType, playerCharacter.Inventory, playerCharacter.Equipment);
			// 	Item item;
			// 	isCombineable = GameDB.item.IsCombinable(item, playerSession.Character.Inventory, playerSession.Character.Equipment, ref material1, ref material2);
			// 	if (material1 == null || material2 == null)
			// 	{
			// 		throw new GameException(ErrorType.NotEnoughItem);
			// 	}
			// 	if (!isCombineable)
			// 	{
			// 		throw new GameException(ErrorType.NotEnoughInventory);
			// 	}
			// 	createItem = SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterMakeItem(playerSession.Character, item);
			// 	if (createItem != null)
			// 	{
			// 		item = createItem;
			// 	}
			// 	material1.SubAmount(1);
			// 	if (material1.IsEmpty())
			// 	{
			// 		if (playerSession.Character.FindInventoryItemById(material1.id, material1.madeType) != null)
			// 		{
			// 			playerSession.Character.FinishBulletCooldown(material1.id);
			// 			playerSession.Character.RemoveInventoryItem(material1.id, material1.madeType);
			// 		}
			// 		else if (playerSession.Character.FindEquipById(material1.id) != null)
			// 		{
			// 			playerSession.Character.FinishBulletCooldown(material1.id);
			// 			playerSession.Character.UnequipItem(material1);
			// 		}
			// 	}
			// 	material2.SubAmount(1);
			// 	if (material2.IsEmpty())
			// 	{
			// 		if (playerSession.Character.FindInventoryItemById(material2.id, material2.madeType) != null)
			// 		{
			// 			playerSession.Character.FinishBulletCooldown(material2.id);
			// 			playerSession.Character.RemoveInventoryItem(material2.id, material2.madeType);
			// 		}
			// 		else if (playerSession.Character.FindEquipById(material2.id) != null)
			// 		{
			// 			playerSession.Character.FinishBulletCooldown(material2.id);
			// 			playerSession.Character.UnequipItem(material2);
			// 		}
			// 	}
			// 	item = null;
			// 	if (item.ItemData.itemType == ItemType.Weapon)
			// 	{
			// 		item = playerSession.Character.GetWeapon();
			// 	}
			// 	else if (item.ItemData.itemType == ItemType.Armor)
			// 	{
			// 		item = playerSession.Character.GetArmor(item.ItemData.GetSubTypeData<ItemArmorData>().armorType);
			// 	}
			// 	bool flag = false;
			// 	if (playerSession.Character.IsEquipableItem(item.ItemData))
			// 	{
			// 		if (item == null)
			// 		{
			// 			playerSession.Character.EquipItem(item);
			// 			flag = true;
			// 		}
			// 		else if (item.ItemData.code == item.ItemData.code && item.Amount <= item.ItemData.stackable - item.Amount)
			// 		{
			// 			item.Merge(item);
			// 			flag = true;
			// 		}
			// 	}
			// 	if (!flag)
			// 	{
			// 		int num;
			// 		playerSession.Character.AddInventoryItem(item, out num);
			// 	}
			// 	playerSession.Character.AddMasteryConditionExp(new UpdateMasteryInfo
			// 	{
			// 		conditionType = item.ItemData.GetMasteryConditionType(),
			// 		takeMasteryValue = 1,
			// 		itemGrade = item.ItemData.itemGrade,
			// 		masteryType = item.ItemData.GetMasteryType()
			// 	});
			// 	playerSession.Character.CharacterSkill.SkillEvolution.OnCraftingItem(playerSession.Character.CharacterCode, itemData, new Action<SkillEvolutionPointType, int>(playerSession.Character.UpdateSkillEvolutionPoint));
			// 	playerSession.Character.CompleteMakeItem();
			// 	playerSession.Character.SendInventoryUpdate(UpdateInventoryType.MakeItem);
			// 	playerSession.Character.SendEquipmentUpdate();
			// 	playerSession.Character.Status.AddItemCraftCount(item.ItemData.itemGrade);
			// 	gameService.Announce.MakeNoise(playerSession.Character, null, NoiseType.Crafting);
			// };
			// playerSession.Character.StartActionCasting(actionCostType, true, null, completeCallback, null, itemData.code);
		}
	}
}