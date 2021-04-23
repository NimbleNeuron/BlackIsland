using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive4)]
	public class LukeActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			int dataCode = IsEvolution()
				? Singleton<LukeSkillActive4Data>.inst.AfterServiceEvolutionProjectileCode
				: Singleton<LukeSkillActive4Data>.inst.AfterServiceProjectileCode;
			ProjectileProperty afterServiceProjectile = PopProjectileProperty(Caster, dataCode);
			afterServiceProjectile.SetTargetDirection(GameUtil.Direction(Caster.Position, info.cursorPosition));
			afterServiceProjectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackInfo, Vector3 damagePoint, Vector3 damageDirection)
				{
					damageParameter.Clear();
					damageParameter.Add(SkillScriptParameterType.Damage,
						Singleton<LukeSkillActive4Data>.inst.DamageBySkillLevel[SkillLevel]);
					damageParameter.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<LukeSkillActive4Data>.inst.DamageApCoef);
					damageParameter.Add(SkillScriptParameterType.DamageTargetLossHpCoef,
						Singleton<LukeSkillActive4Data>.inst.DamageTargetLossHpCoef);
					float num = 100f * (1f - targetAgent.Status.Hp / (float) targetAgent.Stat.MaxHp);
					float value = Singleton<LukeSkillActive4Data>.inst.DamageTargetLossHpCoef * num;
					damageParameter.Add(SkillScriptParameterType.FinalMoreDamage, value);
					DamageTo(targetAgent, afterServiceProjectile.ProjectileData.damageType,
						afterServiceProjectile.ProjectileData.damageSubType, 0, damageParameter,
						Singleton<LukeSkillActive4Data>.inst.AfterServiceDamageEffectAndSoundCode);
					Caster.AttachSight(targetAgent.WorldObject, Singleton<LukeSkillActive4Data>.inst.AttachSightRange,
						Singleton<LukeSkillActive4Data>.inst.AttachSightDuration, false);
					AddState(targetAgent, Singleton<LukeSkillActive4Data>.inst.AfterServiceStateCode);
					if (IsEvolution())
					{
						ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
							-Caster.GetSkillCooldown(SkillSlotSet.Active1_1));
					}
				});
			LaunchProjectile(afterServiceProjectile);
			Vector3 vector;
			bool flag;
			float seconds;
			Caster.MoveToDirectionForTime(-Caster.Forward, Singleton<LukeSkillActive4Data>.inst.DistanceBackAction,
				Singleton<LukeSkillActive4Data>.inst.DurationBackAction, EasingFunction.Ease.EaseOutQuad, false,
				out vector, out flag, out seconds, true);
			Caster.LookAt(Caster.Forward);
			yield return WaitForSeconds(seconds);
			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}
	}
}