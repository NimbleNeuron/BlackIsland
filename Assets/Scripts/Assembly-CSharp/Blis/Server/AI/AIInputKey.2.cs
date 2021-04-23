using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("지정한 키를 입력한 것과 동일하게 행동")]
	public class AiInputKey : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsAlive)
			{
				base.EndAction(false);
				return;
			}
			try
			{
				bool success = false;
				switch (this.inputKey)
				{
				case AIInputKey.KeyQ:
				case AIInputKey.KeyW:
				case AIInputKey.KeyE:
				case AIInputKey.KeyR:
				case AIInputKey.KeyD:
					success = this.InputSkillKey(this.inputKey);
					break;
				case AIInputKey.KeyF:
					success = this.Reload();
					break;
				case AIInputKey.Key1:
				case AIInputKey.Key2:
				case AIInputKey.Key3:
				case AIInputKey.Key4:
				case AIInputKey.Key5:
				case AIInputKey.Key6:
				case AIInputKey.Key7:
				case AIInputKey.Key8:
				case AIInputKey.Key9:
				case AIInputKey.Key0:
					success = this.InputItemSlot(this.inputKey);
					break;
				}
				base.EndAction(success);
			}
			finally
			{
				base.EndAction(false);
			}
		}

		
		private bool InputSkillKey(AIInputKey inputKey)
		{
			switch (inputKey)
			{
			case AIInputKey.KeyQ:
				return this.UseSkill(SkillSlotIndex.Active1);
			case AIInputKey.KeyW:
				return this.UseSkill(SkillSlotIndex.Active2);
			case AIInputKey.KeyE:
				return this.UseSkill(SkillSlotIndex.Active3);
			case AIInputKey.KeyR:
				return this.UseSkill(SkillSlotIndex.Active4);
			case AIInputKey.KeyD:
				return this.UseSkill(SkillSlotIndex.WeaponSkill);
			default:
				return false;
			}
		}

		
		private bool UseSkill(SkillSlotIndex skillSlotIndex)
		{
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			if (worldCharacter == null)
			{
				return true;
			}
			SkillSlotSet? skillSlotSet = base.agent.CharacterSkill.GetSkillSlotSet(skillSlotIndex);
			if (skillSlotSet == null)
			{
				return true;
			}
			SkillData skillData = base.agent.GetSkillData(skillSlotSet.Value, -1);
			if (skillData == null)
			{
				return true;
			}
			switch (skillData.CastWaysType)
			{
			case SkillCastWaysType.Instant:
			case SkillCastWaysType.PickPoint:
			case SkillCastWaysType.PickPointInArea:
			case SkillCastWaysType.PickPointThenDirection:
				base.agent.UseSkill(skillSlotSet.Value, worldCharacter.GetPosition(), worldCharacter.GetPosition());
				break;
			case SkillCastWaysType.Directional:
				base.agent.UseSkill(skillSlotSet.Value, GameUtil.DirectionOnPlane(base.agent.GetPosition(), worldCharacter.GetPosition()), GameUtil.DirectionOnPlane(base.agent.GetPosition(), worldCharacter.GetPosition()));
				break;
			case SkillCastWaysType.PickTargetEdge:
			case SkillCastWaysType.PickTargetCenter:
				base.agent.UseSkill(skillSlotSet.Value, worldCharacter);
				break;
			}
			return true;
		}

		
		private bool Reload()
		{
			if (!base.agent.IsTypeOf<WorldPlayerCharacter>())
			{
				return false;
			}
			WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter)base.agent;
			if (!worldPlayerCharacter.GetWeapon().ItemData.IsGunType())
			{
				return false;
			}
			worldPlayerCharacter.GunReload(true);
			return true;
		}

		
		private bool InputItemSlot(AIInputKey inputKey)
		{
			if (!base.agent.IsTypeOf<WorldPlayerCharacter>())
			{
				return false;
			}
			int itemSlot = inputKey.GetItemSlot();
			if (itemSlot < 0)
			{
				return false;
			}
			WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter)base.agent;
			Item item = worldPlayerCharacter.Inventory.FindByIndex(itemSlot);
			if (item == null)
			{
				return false;
			}
			if (item.ItemData.itemType == ItemType.Weapon || item.ItemData.itemType == ItemType.Armor)
			{
				if (!worldPlayerCharacter.CanAnyAction(ActionType.ItemEquipOrUnequip))
				{
					return false;
				}
				Item item2 = worldPlayerCharacter.RemoveInventoryItem(item.id, item.madeType);
				if (worldPlayerCharacter.EquipItem(item2))
				{
					worldPlayerCharacter.SendInventoryUpdate(UpdateInventoryType.Invalid);
					worldPlayerCharacter.SendEquipmentUpdate();
				}
				else
				{
					worldPlayerCharacter.ForceAddInventoryItem(item2);
					worldPlayerCharacter.SendInventoryUpdate(UpdateInventoryType.Invalid);
				}
			}
			else
			{
				if (worldPlayerCharacter.IsCooldown(item.ItemData))
				{
					return false;
				}
				if (item.ItemData.itemType == ItemType.Consume)
				{
					worldPlayerCharacter.UseItem(item.id, item.madeType);
				}
				else if (item.ItemData.itemType == ItemType.Special && item.GetItemData<ItemSpecialData>().GetSubType() == 2)
				{
					worldPlayerCharacter.Controller.InstallSummon(item, worldPlayerCharacter.GetPosition());
				}
			}
			return true;
		}

		
		public AIInputKey inputKey;
	}
}
