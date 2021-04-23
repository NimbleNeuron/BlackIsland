using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class WorldCreatedObjectByMovableCharacter : SkillAgent
	{
		
		protected WorldCreatedObjectByMovableCharacter(WorldObject worldObject) : base(worldObject)
		{
		}

		
		protected override CharacterStat GetStat()
		{
			return this.GetOwner().Stat;
		}

		
		protected override CharacterStatus GetStatus()
		{
			return this.GetOwner().Status;
		}

		
		public override HostileType GetHostileType(WorldCharacter target)
		{
			return this.GetOwner().GetHostileType(target);
		}

		
		public override DamageInfo DirectDamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool isCheckAlly, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.GetOwner().SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DirectDamageCalculator calculator = new DirectDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(base.Position), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageDataCode, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, bool isCheckAlly, int minRemain, float damageMasteryModifier, int effectAndSoundCode, bool targetInCombat)
		{
			if (target.Character == null)
			{
				return null;
			}
			if (!target.IsAlive)
			{
				return null;
			}
			parameters.Merge(this.GetOwner().SkillController.GetPlayingScriptParameterCollection(target.Character, type, subType, damageId));
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DamageCalculator calculator = new BasicDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			if (this.GetOwner() != null && isCheckAlly && target.GetHostileType(this.GetOwner()) == HostileType.Ally)
			{
				return null;
			}
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(damagePoint), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
		}

		
		public override void HealTo(SkillAgent target, int hpBaseAmount, float hpCoefficient, int hpFixAmount, int spBaseAmount, float spCoefficient, int spFixAmount, bool showUI, int effectAndSoundCode)
		{
			if (target.Character == null)
			{
				return;
			}
			if (!target.IsAlive)
			{
				return;
			}
			BasicHealCalculator calculator = new BasicHealCalculator(hpBaseAmount, hpCoefficient, hpFixAmount, spBaseAmount, spCoefficient, spFixAmount);
			Singleton<HealService>.inst.HealTo(target.Character, this.GetOwner(), calculator, showUI, effectAndSoundCode);
		}

		
		public override void HpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			if (target.Character == null)
			{
				return;
			}
			if (!target.IsAlive)
			{
				return;
			}
			BasicHealCalculator calculator = new BasicHealCalculator(baseAmount, coefficient, fixAmount, 0, 0f, 0);
			Singleton<HealService>.inst.HealTo(target.Character, this.GetOwner(), calculator, showUI, effectAndSoundCode);
		}

		
		public override void LostHpHealTo(SkillAgent target, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			if (target.Character == null)
			{
				return;
			}
			if (!target.IsAlive)
			{
				return;
			}
			LostHpHealCalculator calculator = new LostHpHealCalculator(0, coefficient, fixAmount, 0, 0f, 0);
			Singleton<HealService>.inst.LostHpHealTo(target.Character, this.GetOwner(), calculator, showUI, effectAndSoundCode);
		}

		
		public override void SpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			if (target.Character == null)
			{
				return;
			}
			if (!target.IsAlive)
			{
				return;
			}
			BasicHealCalculator calculator = new BasicHealCalculator(0, 0f, 0, baseAmount, coefficient, fixAmount);
			Singleton<HealService>.inst.HealTo(target.Character, this.GetOwner(), calculator, showUI, effectAndSoundCode);
		}

		
		public override void ExtraPointModifyTo(SkillAgent target, int deltaAmount)
		{
		}

		
		public override void ModifySkillCooldown(SkillSlotSet skillSlotSetFlag, float time)
		{
			if (!base.IsAlive)
			{
				return;
			}
			WorldMovableCharacter owner = this.GetOwner();
			if (owner == null)
			{
				return;
			}
			if (!owner.IsAlive)
			{
				return;
			}
			owner.ModifyCooldown(skillSlotSetFlag, time);
		}

		
		public override SkillData GetSkillData(SkillSlotIndex skillSlotIndex)
		{
			return this.GetOwner().GetSkillData(skillSlotIndex, -1);
		}

		
		public override SkillData GetSkillData(SkillSlotSet skillSlotSet)
		{
			return this.GetOwner().GetSkillData(skillSlotSet, -1);
		}

		
		public override int GetSkillLevel(SkillSlotIndex skillSlotIndex)
		{
			return this.GetOwner().CharacterSkill.GetSkillLevel(skillSlotIndex);
		}

		
		public override int GetSkillLevel(MasteryType masteryType)
		{
			return this.GetOwner().CharacterSkill.GetSkillLevel(masteryType);
		}

		
		public override MasteryType GetEquipWeaponMasteryType()
		{
			WorldPlayerCharacter worldPlayerCharacter = this.GetOwner() as WorldPlayerCharacter;
			if (worldPlayerCharacter == null)
			{
				return base.GetEquipWeaponMasteryType();
			}
			return worldPlayerCharacter.GetEquipWeaponMasteryType();
		}

		
		public override float GetSkillCooldown(SkillSlotSet skillSlotSet)
		{
			return this.GetOwner().CharacterSkill.GetCooldown(skillSlotSet, MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
		}

		
		public override ServerSightAgent AttachSight(WorldObject target, float range, float duration, bool isRemoveWhenInvisibleStart)
		{
			if (target != null)
			{
				return this.GetOwner().AttachSight(target, range, duration, false, isRemoveWhenInvisibleStart);
			}
			return null;
		}

		
		public override void RemoveSight(ServerSightAgent target, int targetId)
		{
			this.GetOwner().RemoveSight(target, targetId);
		}

		
		public override void ResetSightDestroyTime(ServerSightAgent target, float duration)
		{
			this.GetOwner().ResetSightDestroyTime(target, duration);
		}

		
		public override bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this.GetOwner().SightAgent.IsInAllySight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}

		
		public override bool IsInSight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this.GetOwner().SightAgent.IsInSight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}
	}
}
