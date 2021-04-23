using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class BattleEventCollector : SingletonMonoBehaviour<BattleEventCollector>
	{
		
		public void OnAfterTeamRevival(WorldPlayerCharacter caster, WorldPlayerCharacter revivePlayer)
		{
			Action<WorldPlayerCharacter, WorldPlayerCharacter> onAfterTeamRevivalEvent = this.OnAfterTeamRevivalEvent;
			if (onAfterTeamRevivalEvent == null)
			{
				return;
			}
			onAfterTeamRevivalEvent(caster, revivePlayer);
		}

		
		public void FinishNormalAttack(WorldCharacter victim, WorldCharacter attacker)
		{
			Action<WorldCharacter, WorldCharacter> onFinishNormalAttack = this.OnFinishNormalAttack;
			if (onFinishNormalAttack == null)
			{
				return;
			}
			onFinishNormalAttack(victim, attacker);
		}

		
		public void BeforeDamageCaculator(WorldCharacter victim, WorldCharacter attacker, DamageCalculator calcuator)
		{
			switch (calcuator.DamageType)
			{
			case DamageType.Normal:
			{
				Action<WorldCharacter, WorldCharacter, DamageCalculator> onBeforeNormalDamageCaculator = this.OnBeforeNormalDamageCaculator;
				if (onBeforeNormalDamageCaculator != null)
				{
					onBeforeNormalDamageCaculator(victim, attacker, calcuator);
				}
				break;
			}
			case DamageType.Skill:
			{
				Action<WorldCharacter, WorldCharacter, DamageCalculator> onBeforeSkillDamageCaculator = this.OnBeforeSkillDamageCaculator;
				if (onBeforeSkillDamageCaculator != null)
				{
					onBeforeSkillDamageCaculator(victim, attacker, calcuator);
				}
				break;
			}
			case DamageType.Sp:
			case DamageType.RedZone:
			case DamageType.DyingCondition:
				return;
			default:
				throw new GameException("BeforeDamageCaculator invalidate DamageType : " + calcuator.DamageType);
			}
			Action<WorldCharacter, WorldCharacter, DamageCalculator> onBeforeDamageCaculator = this.OnBeforeDamageCaculator;
			if (onBeforeDamageCaculator == null)
			{
				return;
			}
			onBeforeDamageCaculator(victim, attacker, calcuator);
		}

		
		public void BeforeDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			switch (damageInfo.DamageType)
			{
			case DamageType.Normal:
			{
				Action<WorldCharacter, DamageInfo> onBeforeNormalDamageProcess = this.OnBeforeNormalDamageProcess;
				if (onBeforeNormalDamageProcess != null)
				{
					onBeforeNormalDamageProcess(victim, damageInfo);
				}
				break;
			}
			case DamageType.Skill:
			{
				Action<WorldCharacter, DamageInfo> onBeforeSkillDamageProcess = this.OnBeforeSkillDamageProcess;
				if (onBeforeSkillDamageProcess != null)
				{
					onBeforeSkillDamageProcess(victim, damageInfo);
				}
				break;
			}
			case DamageType.Sp:
			case DamageType.RedZone:
			case DamageType.DyingCondition:
				return;
			default:
				throw new GameException("BeforeDamageProcess invalidate DamageType : " + damageInfo.DamageType);
			}
			Action<WorldCharacter, DamageInfo> onBeforeDamageProcess = this.OnBeforeDamageProcess;
			if (onBeforeDamageProcess == null)
			{
				return;
			}
			onBeforeDamageProcess(victim, damageInfo);
		}

		
		public void AfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			switch (damageInfo.DamageType)
			{
			case DamageType.Normal:
			{
				Action<WorldCharacter, DamageInfo> onAfterNormalDamageProcess = this.OnAfterNormalDamageProcess;
				if (onAfterNormalDamageProcess != null)
				{
					onAfterNormalDamageProcess(victim, damageInfo);
				}
				break;
			}
			case DamageType.Skill:
			{
				Action<WorldCharacter, DamageInfo> onAfterSkillDamageProcess = this.OnAfterSkillDamageProcess;
				if (onAfterSkillDamageProcess != null)
				{
					onAfterSkillDamageProcess(victim, damageInfo);
				}
				break;
			}
			case DamageType.Sp:
			case DamageType.RedZone:
			case DamageType.DyingCondition:
				return;
			default:
				throw new GameException("AfterDamageProcess invalidate DamageType : " + damageInfo.DamageType);
			}
			Action<WorldCharacter, DamageInfo> onAfterDamageProcess = this.OnAfterDamageProcess;
			if (onAfterDamageProcess == null)
			{
				return;
			}
			onAfterDamageProcess(victim, damageInfo);
		}

		
		public void OnKill(WorldCharacter victim, DamageInfo damageInfo)
		{
			Action<WorldCharacter, DamageInfo> onKillEvent = this.OnKillEvent;
			if (onKillEvent == null)
			{
				return;
			}
			onKillEvent(victim, damageInfo);
		}

		
		public void OnCompleteAddState(WorldCharacter target, CharacterState state)
		{
			Action<WorldCharacter, CharacterState> onCompleteAddStateEvent = this.OnCompleteAddStateEvent;
			if (onCompleteAddStateEvent == null)
			{
				return;
			}
			onCompleteAddStateEvent(target, state);
		}

		
		public void OnCompleteRemoveState(WorldCharacter target, CharacterState state)
		{
			Action<WorldCharacter, CharacterState> onCompleteRemoveStateEvent = this.OnCompleteRemoveStateEvent;
			if (onCompleteRemoveStateEvent == null)
			{
				return;
			}
			onCompleteRemoveStateEvent(target, state);
		}

		
		public void OnCompleteChangedState(WorldCharacter target, CharacterState state)
		{
			Action<WorldCharacter, CharacterState> onCompleteChangedStateEvent = this.OnCompleteChangedStateEvent;
			if (onCompleteChangedStateEvent == null)
			{
				return;
			}
			onCompleteChangedStateEvent(target, state);
		}

		
		public void OnBlockDamage(WorldCharacter target, int casterId, WorldCharacter attacker, int undefendedDamage, int blockDamage, Vector3? damagePoint, DamageSubType damageSubType)
		{
			Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType> onBlockDamageEvent = this.OnBlockDamageEvent;
			if (onBlockDamageEvent == null)
			{
				return;
			}
			onBlockDamageEvent(target, casterId, attacker, undefendedDamage, blockDamage, damagePoint, damageSubType);
		}

		
		public void OnInstalledTrap(WorldSummonBase summon, WorldCharacter owner, SummonData summonData)
		{
			Action<WorldSummonBase, WorldCharacter, SummonData> onAfterInstalledTrapEvent = this.OnAfterInstalledTrapEvent;
			if (onAfterInstalledTrapEvent == null)
			{
				return;
			}
			onAfterInstalledTrapEvent(summon, owner, summonData);
		}

		
		public void OnBeforeInstallSummon(WorldPlayerCharacter owner, int summonCode)
		{
			Action<WorldPlayerCharacter, int> onBeforeInstallSummonEvent = this.OnBeforeInstallSummonEvent;
			if (onBeforeInstallSummonEvent == null)
			{
				return;
			}
			onBeforeInstallSummonEvent(owner, summonCode);
		}

		
		public void OnBeforeActionCasting(WorldCharacter actionCaster, ActionCostData actionCostData)
		{
			Action<WorldCharacter, ActionCostData> onBeforeActionCastingEvent = this.OnBeforeActionCastingEvent;
			if (onBeforeActionCastingEvent == null)
			{
				return;
			}
			onBeforeActionCastingEvent(actionCaster, actionCostData);
		}

		
		public void OnAfterActionCasting(WorldCharacter actionCaster, ActionCostData actionCostData)
		{
			Action<WorldCharacter, ActionCostData> onAfterActionCastingEvent = this.OnAfterActionCastingEvent;
			if (onAfterActionCastingEvent == null)
			{
				return;
			}
			onAfterActionCastingEvent(actionCaster, actionCostData);
		}

		
		public void OnBeforeSkillActive(WorldCharacter skillCaster, SkillData skillData, SkillSlotSet skillSlotSet)
		{
			Action<WorldCharacter, SkillData, SkillSlotSet> onBeforeSkillActiveEvent = this.OnBeforeSkillActiveEvent;
			if (onBeforeSkillActiveEvent == null)
			{
				return;
			}
			onBeforeSkillActiveEvent(skillCaster, skillData, skillSlotSet);
		}

		
		public void OnBeforeActiveSkillFinish(WorldCharacter skillCaster, SkillData skillData, SkillSlotSet skillSlotSe, bool cancel)
		{
			Action<WorldCharacter, SkillData, SkillSlotSet, bool> onBeforeActiveSkillFinishEvent = this.OnBeforeActiveSkillFinishEvent;
			if (onBeforeActiveSkillFinishEvent == null)
			{
				return;
			}
			onBeforeActiveSkillFinishEvent(skillCaster, skillData, skillSlotSe, cancel);
		}

		
		public void OnAfterConsumeItemConsumable(WorldPlayerCharacter consumer, ItemConsumableData itemData)
		{
			Action<WorldPlayerCharacter, ItemConsumableData> onAfterConsumeItemConsumableEvent = this.OnAfterConsumeItemConsumableEvent;
			if (onAfterConsumeItemConsumableEvent == null)
			{
				return;
			}
			onAfterConsumeItemConsumableEvent(consumer, itemData);
		}

		
		public void OnBeforeOpenCorpse(WorldPlayerCharacter opener, WorldCharacter corpse)
		{
			Action<WorldPlayerCharacter, WorldCharacter> onBeforeOpenCorpseEvent = this.OnBeforeOpenCorpseEvent;
			if (onBeforeOpenCorpseEvent == null)
			{
				return;
			}
			onBeforeOpenCorpseEvent(opener, corpse);
		}

		
		public Item OnBeforeMakeItem(WorldPlayerCharacter maker, Item item)
		{
			Func<WorldPlayerCharacter, Item, Item> onBeforeMakeItemProcessEvent = this.OnBeforeMakeItemProcessEvent;
			if (onBeforeMakeItemProcessEvent == null)
			{
				return null;
			}
			return onBeforeMakeItemProcessEvent(maker, item);
		}

		
		public Item OnAfterMakeItem(WorldPlayerCharacter maker, Item item)
		{
			Func<WorldPlayerCharacter, Item, Item> onAfterMakeItemProcessEvent = this.OnAfterMakeItemProcessEvent;
			if (onAfterMakeItemProcessEvent == null)
			{
				return null;
			}
			return onAfterMakeItemProcessEvent(maker, item);
		}

		
		public void OnAfterProjectileDestory(int projectileCode, int projectileObjectId, int ownerObjectId)
		{
			Action<int, int, int> onAfterProjectileDestoryEvent = this.OnAfterProjectileDestoryEvent;
			if (onAfterProjectileDestoryEvent == null)
			{
				return;
			}
			onAfterProjectileDestoryEvent(projectileCode, projectileObjectId, ownerObjectId);
		}

		
		public void OnAfterVisitedAreaAdd(WorldPlayerCharacter playerCharacter, int areaMaskCode)
		{
			Action<WorldPlayerCharacter, int> onAfterVisitedAreaAddEvent = this.OnAfterVisitedAreaAddEvent;
			if (onAfterVisitedAreaAddEvent == null)
			{
				return;
			}
			onAfterVisitedAreaAddEvent(playerCharacter, areaMaskCode);
		}

		
		public void OnCurrentAreaCheck(WorldPlayerCharacter playerCharacter, int areaMaskCode)
		{
			Action<WorldPlayerCharacter, int> onCurrentAreaCheckEvent = this.OnCurrentAreaCheckEvent;
			if (onCurrentAreaCheckEvent == null)
			{
				return;
			}
			onCurrentAreaCheckEvent(playerCharacter, areaMaskCode);
		}

		
		public void OnAfterSkillLevelUp(WorldPlayerCharacter playerCharacter, SkillSlotIndex skillSlotIndex, int skillLv)
		{
			Action<WorldPlayerCharacter, SkillSlotIndex, int> onAfterSkillLevelUpEvent = this.OnAfterSkillLevelUpEvent;
			if (onAfterSkillLevelUpEvent == null)
			{
				return;
			}
			onAfterSkillLevelUpEvent(playerCharacter, skillSlotIndex, skillLv);
		}

		
		public void OnBeforePickupItem(WorldPlayerCharacter playerCharacter, Item item)
		{
			Action<WorldPlayerCharacter, Item> onBeforePickupItemEvent = this.OnBeforePickupItemEvent;
			if (onBeforePickupItemEvent == null)
			{
				return;
			}
			onBeforePickupItemEvent(playerCharacter, item);
		}

		
		public void OnAfterHyperLoopWarp(WorldPlayerCharacter playerCharacter, Vector3 warpPosition, int areaCode)
		{
			Action<WorldPlayerCharacter, Vector3, int> onAfterHyperLoopWarpEvent = this.OnAfterHyperLoopWarpEvent;
			if (onAfterHyperLoopWarpEvent == null)
			{
				return;
			}
			onAfterHyperLoopWarpEvent(playerCharacter, warpPosition, areaCode);
		}

		
		public void OnAfterOepnItemBox(WorldPlayerCharacter playerCharacter, WorldItemBox itemBox)
		{
			Action<WorldPlayerCharacter, WorldItemBox> onAfterOepnItemBoxEvent = this.OnAfterOepnItemBoxEvent;
			if (onAfterOepnItemBoxEvent == null)
			{
				return;
			}
			onAfterOepnItemBoxEvent(playerCharacter, itemBox);
		}

		
		public void OnAfterAirSupplyItemBoxTakeItem(WorldPlayerCharacter playerCharacter, WorldItemBox itemBox)
		{
			Action<WorldPlayerCharacter, WorldItemBox> onAfterAirSupplyItemBoxTakeEvent = this.OnAfterAirSupplyItemBoxTakeEvent;
			if (onAfterAirSupplyItemBoxTakeEvent == null)
			{
				return;
			}
			onAfterAirSupplyItemBoxTakeEvent(playerCharacter, itemBox);
		}

		
		public void OnAfterAssist(WorldPlayerCharacter victim, int finishingAttacker, List<int> assistants)
		{
			Action<WorldPlayerCharacter, int, List<int>> onAfterAssistEvent = this.OnAfterAssistEvent;
			if (onAfterAssistEvent == null)
			{
				return;
			}
			onAfterAssistEvent(victim, finishingAttacker, assistants);
		}

		
		public Action<WorldCharacter, WorldCharacter> OnFinishNormalAttack;

		
		public Action<WorldCharacter, DamageInfo> OnBeforeDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnAfterDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnBeforeNormalDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnBeforeSkillDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnAfterNormalDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnAfterSkillDamageProcess;

		
		public Action<WorldCharacter, DamageInfo> OnKillEvent;

		
		public Action<WorldCharacter, CharacterState> OnCompleteAddStateEvent;

		
		public Action<WorldCharacter, CharacterState> OnCompleteChangedStateEvent;

		
		public Action<WorldCharacter, CharacterState> OnCompleteRemoveStateEvent;

		
		public Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType> OnBlockDamageEvent;

		
		public Action<WorldSummonBase, WorldCharacter, SummonData> OnAfterInstalledTrapEvent;

		
		public Action<WorldPlayerCharacter, int> OnBeforeInstallSummonEvent;

		
		public Action<WorldCharacter, ActionCostData> OnBeforeActionCastingEvent;

		
		public Action<WorldCharacter, ActionCostData> OnAfterActionCastingEvent;

		
		public Action<WorldCharacter, SkillData, SkillSlotSet> OnBeforeSkillActiveEvent;

		
		public Action<WorldCharacter, SkillData, SkillSlotSet, bool> OnBeforeActiveSkillFinishEvent;

		
		public Action<WorldPlayerCharacter, ItemConsumableData> OnAfterConsumeItemConsumableEvent;

		
		public Action<WorldPlayerCharacter, WorldCharacter> OnBeforeOpenCorpseEvent;

		
		public Func<WorldPlayerCharacter, Item, Item> OnBeforeMakeItemProcessEvent;

		
		public Func<WorldPlayerCharacter, Item, Item> OnAfterMakeItemProcessEvent;

		
		public Action<int, int, int> OnAfterProjectileDestoryEvent;

		
		public Action<WorldPlayerCharacter, int> OnCurrentAreaCheckEvent;

		
		public Action<WorldPlayerCharacter, int> OnAfterVisitedAreaAddEvent;

		
		public Action<WorldPlayerCharacter, SkillSlotIndex, int> OnAfterSkillLevelUpEvent;

		
		public Action<WorldPlayerCharacter, Item> OnBeforePickupItemEvent;

		
		public Action<WorldPlayerCharacter, Vector3, int> OnAfterHyperLoopWarpEvent;

		
		public Action<WorldPlayerCharacter, WorldItemBox> OnAfterOepnItemBoxEvent;

		
		public Action<WorldPlayerCharacter, WorldItemBox> OnAfterAirSupplyItemBoxTakeEvent;

		
		public Action<WorldPlayerCharacter, int, List<int>> OnAfterAssistEvent;

		
		public Action<WorldCharacter, WorldCharacter, DamageCalculator> OnBeforeDamageCaculator;

		
		public Action<WorldCharacter, WorldCharacter, DamageCalculator> OnBeforeNormalDamageCaculator;

		
		public Action<WorldCharacter, WorldCharacter, DamageCalculator> OnBeforeSkillDamageCaculator;

		
		public Action<WorldPlayerCharacter, WorldPlayerCharacter> OnAfterTeamRevivalEvent;
	}
}
