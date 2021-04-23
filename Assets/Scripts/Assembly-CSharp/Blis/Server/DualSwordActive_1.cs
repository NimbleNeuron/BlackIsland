using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DualSwordActive_1)]
	public class DualSwordActive_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D circle;

		
		private bool isHit;

		
		protected override void Start()
		{
			base.Start();
			isHit = false;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			yield return PlaySkill(direction);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		protected override bool IsChangedSkillSequence()
		{
			return isHit;
		}

		
		private IEnumerator PlaySkill(Vector3 direction)
		{
			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDirectionForTime(direction, Singleton<DualSwordSkillActiveData>.inst.DashDistance,
				Singleton<DualSwordSkillActiveData>.inst.DashDuration, EasingFunction.Ease.Linear, false, out vector,
				out flag, out num);
			yield return WaitForSeconds(0.25f);
			Caster.MoveToDirectionForTime(direction, Singleton<DualSwordSkillActiveData>.inst.AttackDistance,
				Singleton<DualSwordSkillActiveData>.inst.AttackDuration, EasingFunction.Ease.Linear, false, out vector,
				out flag, out num);
			PlaySkillAction(Caster, 1);
			yield return WaitForSeconds(0.08f);
			Damage();
			yield return WaitForSeconds(0.085f);
			Damage();
			yield return WaitForSeconds(0.08f);
			Damage();
			yield return WaitForSeconds(0.085f);
			Damage();
			yield return WaitForSeconds(0.08f);
			Damage();
			yield return WaitForSeconds(0.085f);
			Damage();
		}

		
		private void Damage()
		{
			if (circle == null)
			{
				circle = new CollisionCircle3D(Caster.Position, Singleton<DualSwordSkillActiveData>.inst.AttackRange);
			}
			else
			{
				circle.UpdatePosition(Caster.Position);
			}

			List<SkillAgent> enemyCharacters = GetEnemyCharacters(circle);
			if (enemyCharacters.Any<SkillAgent>())
			{
				isHit = true;
				damage.Clear();
				damage.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<DualSwordSkillActiveData>.inst.DamageApCoef[SkillLevel]);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, damage,
					Singleton<DualSwordSkillActiveData>.inst.EffectAndSoundCode);
			}
		}
	}
}