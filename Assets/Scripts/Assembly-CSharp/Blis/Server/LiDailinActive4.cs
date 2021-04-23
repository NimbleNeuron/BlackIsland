using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive4)]
	public class LiDailinActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		private bool isReinforce;

		
		private SkillAgent targetEnemy;

		
		protected override void Start()
		{
			base.Start();
			isReinforce = Caster.Status.ExtraPoint >= Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint;
			if (isReinforce)
			{
				Caster.ExtraPointModifyTo(Caster, Singleton<LiDailinSkillData>.inst.A4ReinforceConsumeExtraPoint);
			}

			targetEnemy = null;
			if (collision == null)
			{
				collision = new CollisionCircle3D(Caster.Position, SkillWidth * 0.5f);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (targetEnemy != null)
			{
				targetEnemy.RemoveStateByGroup(
					GameDB.characterState
						.GetData(isReinforce
							? Singleton<LiDailinSkillData>.inst.A4TargetSuppressedDebuffReinforce
							: Singleton<LiDailinSkillData>.inst.A4TargetSuppressedDebuffBase).group, Caster.ObjectId);
				ModifySkillCooldown(Caster, SkillSlotSet.Active4_1,
					Caster.GetSkillCooldown(SkillSlotSet.Active4_1) *
					Singleton<LiDailinSkillData>.inst.A4HitCooldownModify);
			}

			if (isReinforce)
			{
				AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			PlaySkillAction(Caster, isReinforce ? 2 : 1);
			AddState(Caster, Singleton<LiDailinSkillData>.inst.A4UnstoppableStateCode);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			CasterLockRotation(true);
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			float dashStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			Vector3 vector;
			bool flag;
			float finalDashDuration;
			Caster.MoveToDirectionForTime(direction, Singleton<LiDailinSkillData>.inst.A4DashDistance,
				Singleton<LiDailinSkillData>.inst.A4DashDuration, EasingFunction.Ease.EaseOutQuad, true, out vector,
				out flag, out finalDashDuration);
			collision.UpdateRadius(SkillWidth * 0.5f);
			CheckCollision();
			while (Caster.IsMoving() && targetEnemy == null && dashStartTime + finalDashDuration >=
				MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				yield return WaitForFrame();
				CheckCollision();
			}

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<LiDailinSkillData>.inst.A4UnstoppableStateCode).group,
				Caster.ObjectId);
			StartConcentration();
			if (targetEnemy != null)
			{
				PlaySkillAction(Caster, isReinforce ? 12 : 11);
				Caster.StopMove();
				AddState(targetEnemy,
					isReinforce
						? Singleton<LiDailinSkillData>.inst.A4TargetSuppressedDebuffReinforce
						: Singleton<LiDailinSkillData>.inst.A4TargetSuppressedDebuffBase);
				float hitEndTime = isReinforce
					? Singleton<LiDailinSkillData>.inst.A4HitDurationReinforce
					: Singleton<LiDailinSkillData>.inst.A4HitDurationBase;
				hitEndTime += MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				float hitTerm = isReinforce
					? Singleton<LiDailinSkillData>.inst.A4HitTermReinforce
					: Singleton<LiDailinSkillData>.inst.A4HitTermBase;
				int hitCount = isReinforce
					? Singleton<LiDailinSkillData>.inst.A4HitCountReinforce
					: Singleton<LiDailinSkillData>.inst.A4HitCountBase;
				float num = 100f * (1f - targetEnemy.Status.Hp / (float) targetEnemy.Stat.MaxHp);
				float damageIncreaseAmount = Singleton<LiDailinSkillData>.inst.A4DamageIncreasePerTargetLossHp * num;
				damageIncreaseAmount = Mathf.Min(Singleton<LiDailinSkillData>.inst.A4DamageIncreaseMax,
					damageIncreaseAmount);
				int skillLv = SkillLevel;
				int effectCode = isReinforce
					? Singleton<LiDailinSkillData>.inst.A4EffectCodeReinforce
					: Singleton<LiDailinSkillData>.inst.A4EffectCodeBase;
				float timeStack = hitTerm;
				while (hitCount > 0 && targetEnemy.IsAlive)
				{
					if (timeStack >= hitTerm)
					{
						parameters.Clear();
						parameters.Add(SkillScriptParameterType.Damage,
							Singleton<LiDailinSkillData>.inst.A4DamageBase[skillLv]);
						parameters.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<LiDailinSkillData>.inst.A4ApDamage);
						parameters.Add(SkillScriptParameterType.FinalMoreDamage, damageIncreaseAmount);
						DamageTo(targetEnemy, DamageType.Skill, DamageSubType.Normal, 0, parameters, effectCode);
						int num2 = hitCount - 1;
						hitCount = num2;
						timeStack -= hitTerm;
					}

					yield return WaitForFrame();
					timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				}

				if (hitCount == 0)
				{
					yield return WaitForSeconds(hitEndTime -
					                            MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime);
				}
			}

			PlaySkillAction(Caster, 3);
			FinishConcentration(false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void CheckCollision()
		{
			collision.UpdatePosition(Caster.Position);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Count != 0)
			{
				targetEnemy = enemyCharacters.NearestOne(Caster.Position);
			}
		}
	}
}