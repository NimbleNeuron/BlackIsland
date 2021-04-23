using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class WorldCharacterSkillAgent : SkillAgent
	{
		
		public WorldCharacterSkillAgent(WorldObject worldObject) : base(worldObject)
		{
			this.worldCharacter = (worldObject as WorldCharacter);
		}

		
		protected override WorldObject GetWorldObject()
		{
			return this.worldCharacter;
		}

		
		protected override WorldCharacter GetWorldCharacter()
		{
			return this.worldCharacter;
		}

		
		protected override WorldMovableCharacter GetOwner()
		{
			return null;
		}

		
		protected override bool GetIsAlive()
		{
			return this.worldCharacter.IsAlive;
		}

		
		protected override CharacterStat GetStat()
		{
			return this.worldCharacter.Stat;
		}

		
		protected override CharacterStatus GetStatus()
		{
			return this.worldCharacter.Status;
		}

		
		protected override CollisionObject3D GetCollisionObject()
		{
			return this.worldCharacter.GetCollisionObject();
		}

		
		public override HostileType GetHostileType(WorldCharacter target)
		{
			return this.worldCharacter.GetHostileType(target);
		}

		
		public override WorldObject GetTarget()
		{
			return null;
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, int targetId, BlisVector targetPosition)
		{
		}

		
		public override void PlaySkillAction(SkillId skillId, int actionNo, List<SkillActionTarget> targets)
		{
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
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DirectDamageCalculator calculator = new DirectDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
			return Singleton<DamageService>.inst.DamageTo(target.Character, attackerInfo, calculator, new Vector3?(target.Position), skillSlotSet, isCheckAlly, effectAndSoundCode, targetInCombat);
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
			int baseDamage = (int)parameters.Get(SkillScriptParameterType.Damage);
			DamageCalculator calculator = new BasicDamageCalculator(WeaponType.None, type, subType, damageDataCode, damageId, baseDamage, minRemain, damageMasteryModifier);
			calculator.SetAttackerSkillScriptParameter(parameters);
			target.Character.IfTypeOf<WorldMovableCharacter>(delegate(WorldMovableCharacter targetCharacter)
			{
				SkillScriptParameterCollection playingScriptParameterCollection = targetCharacter.SkillController.GetPlayingScriptParameterCollection(this.GetOwner(), type, subType, damageId);
				calculator.SetVictimSkillScriptParameter(playingScriptParameterCollection);
			});
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
			Singleton<HealService>.inst.HealTo(target.Character, this.worldCharacter, calculator, showUI, effectAndSoundCode);
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
			Singleton<HealService>.inst.HealTo(target.Character, this.worldCharacter, calculator, showUI, effectAndSoundCode);
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
			Singleton<HealService>.inst.LostHpHealTo(target.Character, this.worldCharacter, calculator, showUI, effectAndSoundCode);
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
			Singleton<HealService>.inst.HealTo(target.Character, this.worldCharacter, calculator, showUI, effectAndSoundCode);
		}

		
		public override void ExtraPointModifyTo(SkillAgent target, int deltaAmount)
		{
			if (target.Character == null)
			{
				return;
			}
			if (!target.IsAlive)
			{
				return;
			}
			target.Character.ModifyExtraPoint(deltaAmount);
		}

		
		public override void ModifySkillCooldown(SkillSlotSet skillSlotSetFlag, float time)
		{
		}

		
		public override void Airborne(float airborneDuration, float? power = null)
		{
			float airbornePower;
			if (power != null)
			{
				airbornePower = power.Value;
			}
			else if (airborneDuration < 0.5f)
			{
				airbornePower = 0f;
			}
			else if (airborneDuration < 1.5f)
			{
				airbornePower = 1f;
			}
			else
			{
				airbornePower = airborneDuration - 0.5f;
			}
			this.worldCharacter.Airborne(airborneDuration, airbornePower);
		}

		
		public override bool IsInAllySight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this.worldCharacter.SightAgent.IsInAllySight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}

		
		public override bool IsInSight(SightAgent targetSightAgent, Vector3 targetPosition, float targetRadius, bool targetIsInvisible)
		{
			return this.worldCharacter.SightAgent.IsInSight(targetSightAgent, targetPosition, targetRadius, targetIsInvisible);
		}

		
		private readonly WorldCharacter worldCharacter;

		
		private const float MIN_AIRBORNE_DURATION = 0.5f;

		
		private const float MAX_AIRBORNE_DURATION = 1.5f;
	}
}
