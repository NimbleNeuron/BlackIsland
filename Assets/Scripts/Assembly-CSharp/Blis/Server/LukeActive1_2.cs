using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive1_2)]
	public class LukeActive1_2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private SkillAgent attachedTarget;

		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (attachedTarget == null)
			{
				yield break;
			}

			LookAtPosition(Caster, attachedTarget.Position);
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			Caster.RemoveStateByGroup(Singleton<LukeSkillActive1_1Data>.inst.BuffGroupCode, Caster.ObjectId);
			bool startMoveToTarget = false;
			if (Singleton<LukeSkillActive1_2Data>.inst.WarpDistance <
			    GameUtil.DistanceOnPlane(Caster.Position, attachedTarget.Position))
			{
				Caster.MoveToTargetWithoutNavSpeed(Caster.Position, attachedTarget.Character,
					Singleton<LukeSkillActive1_2Data>.inst.MoveToTagetSpeed,
					Singleton<LukeSkillActive1_2Data>.inst.WarpDistance);
				startMoveToTarget = true;
			}

			while (startMoveToTarget && Caster.IsMoving())
			{
				yield return WaitForFrame();
				if (Singleton<LukeSkillActive1_2Data>.inst.MaxMoveDistance <=
				    GameUtil.DistanceOnPlane(Caster.Position, attachedTarget.Position))
				{
					Caster.StopMove();
					break;
				}
			}

			PlaySkillAction(Caster, 2);
			if (attachedTarget != null)
			{
				Vector3 vector;
				if (!MoveAgent.SamplePosition(Caster.Position, 2147483640, out vector))
				{
					Caster.WarpTo(attachedTarget.Position);
				}

				if (attachedTarget.IsAlive && GameUtil.DistanceOnPlane(Caster.Position, attachedTarget.Position) <=
					Singleton<LukeSkillActive1_2Data>.inst.AttackPossibleRange)
				{
					HitAction(attachedTarget);
					PlaySkillAction(Caster, 1);
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void HitAction(SkillAgent target)
		{
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<LukeSkillActive1_2Data>.inst.BaseDamage[SkillLevel]);
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LukeSkillActive1_2Data>.inst.DamageApCoef);
			if (target.AnyHaveStateByType(StateType.Shield))
			{
				target.RemoveStateByType(StateType.Shield);
			}

			target.RemoveStateByGroup(Singleton<LukeSkillActive1_1Data>.inst.DebuffGroupCode, Caster.ObjectId);
			DamageInfo damageInfo = DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<LukeSkillActive1_2Data>.inst.EffectSoundCode);
			if (IsEvolution())
			{
				int num = damageInfo != null ? damageInfo.Damage : 0;
				HpHealTo(Caster, 0, 0f,
					Mathf.RoundToInt(num * Singleton<LukeSkillActive1_2Data>.inst.EvolutionBuffRatio), false,
					Singleton<LukeSkillActive1_2Data>.inst.EvolutionBuffEffectSoundCode);
			}
		}

		
		public override UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition,
			WorldMovableCharacter caster)
		{
			SkillData skillData = caster.GetSkillData(SkillSlotSet.Active1_2);
			attachedTarget = null;
			List<WorldCharacter> enemiesWithinRange = caster.GetEnemiesWithinRange(caster.GetPosition(),
				skillData.range,
				worldCharacter => worldCharacter.SkillAgent != null &&
				                  worldCharacter.SkillAgent.IsHaveStateByGroup(
					                  Singleton<LukeSkillActive1_1Data>.inst.DebuffGroupCode, caster.ObjectId));
			if (enemiesWithinRange == null)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}

			if (enemiesWithinRange.Count <= 0)
			{
				return UseSkillErrorCode.NotAvailableNow;
			}

			foreach (WorldCharacter worldCharacter2 in enemiesWithinRange)
			{
				if (worldCharacter2.SkillAgent != null &&
				    GameUtil.DistanceOnPlane(caster.GetPosition(), worldCharacter2.GetPosition()) <= skillData.range)
				{
					attachedTarget = worldCharacter2.SkillAgent;
					return UseSkillErrorCode.None;
				}
			}

			return UseSkillErrorCode.NotAvailableNow;
		}
	}
}