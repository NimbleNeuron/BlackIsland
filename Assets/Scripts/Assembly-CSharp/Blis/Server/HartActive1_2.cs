using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive1_2)]
	public class HartActive1_2 : HartSkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			CharacterStateData data = GameDB.characterState.GetData(Singleton<HartSkillActive1Data>.inst.BuffState);
			CharacterState characterState = Caster.FindStateByGroup(data.group, Caster.ObjectId);
			if (characterState != null)
			{
				bool isEvolution = IsEvolution();
				float num = Mathf.Min(1f,
					characterState.ElapsedTime() * Singleton<HartSkillActive1Data>.inst.ChargeDuration);
				bool flag = 1f <= num;
				int dataCode;
				if (isEvolution)
				{
					dataCode = IsEnchanted() ? flag
							?
							Singleton<HartSkillActive1Data>.inst.ChargeEnchantedEvolutionProjectileCode
							: Singleton<HartSkillActive1Data>.inst.EnchantedEvolutionProjectileCode :
						flag ? Singleton<HartSkillActive1Data>.inst.ChargeEvolutionProjectileCode :
						Singleton<HartSkillActive1Data>.inst.EvolutionProjectileCode;
				}
				else
				{
					dataCode = IsEnchanted() ? flag
							?
							Singleton<HartSkillActive1Data>.inst.ChargeEnchantedProjectileCode
							: Singleton<HartSkillActive1Data>.inst.EnchantedProjectileCode :
						flag ? Singleton<HartSkillActive1Data>.inst.ChargeProjectileCode :
						Singleton<HartSkillActive1Data>.inst.ProjectileCode;
				}

				int damage = (int) Mathf.Lerp(Singleton<HartSkillActive1Data>.inst.MinSkillDamage[SkillLevel],
					Singleton<HartSkillActive1Data>.inst.MaxSkillDamage[SkillLevel], num);
				float skillApCoef = Mathf.Lerp(Singleton<HartSkillActive1Data>.inst.MinSkillApCoef,
					Singleton<HartSkillActive1Data>.inst.MaxSkillApCoef, num);
				Vector3 targetDirection = GameUtil.Direction(Caster.Position, GetSkillPoint());
				ProjectileProperty projectile = PopProjectileProperty(Caster, dataCode);
				projectile.SetTargetDirection(targetDirection);
				projectile.SetActionOnCollisionCharacter(
					delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
						Vector3 damageDirection)
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage, damage);
						parameterCollection.Add(SkillScriptParameterType.DamageApCoef, skillApCoef);
						DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
							projectile.ProjectileData.damageSubType, 0, parameterCollection,
							projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
						if (isEvolution)
						{
							CharacterState state = CreateState(targetAgent,
								Singleton<HartSkillActive1Data>.inst.DebuffState[SkillEvolutionLevel]);
							AddState(targetAgent, state);
						}
					});
				base.LaunchProjectile(projectile);
				Caster.RemoveStateByGroup(characterState.Group, Caster.ObjectId);
				yield return WaitForSeconds(0.13f);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}