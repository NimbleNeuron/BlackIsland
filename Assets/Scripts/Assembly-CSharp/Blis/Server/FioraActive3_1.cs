using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive3_1)]
	public class FioraActive3_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionBox3D collision;

		
		private SkillAgent target;

		
		protected override void Start()
		{
			base.Start();
			target = null;
			if (collision == null)
			{
				collision = new CollisionBox3D(Vector3.left, SkillWidth,
					Singleton<FioraSkillActive3Data>.inst.CollisionBoxDepth, Vector3.zero);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float dashDistance = Singleton<FioraSkillActive3Data>.inst.DashDistance;
			float dashDuration = Singleton<FioraSkillActive3Data>.inst.DashDuration;
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			target = CheckCollision(direction);
			if (target != null)
			{
				LookAtDirection(Caster, GameUtil.Direction(Caster.Position, target.Position));
				PlaySkillAction(Caster, 1);
				yield return WaitForSeconds(0.09f);
				HitAction(target);
			}
			else
			{
				LookAtDirection(Caster, direction);
				float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				Vector3 vector;
				bool flag;
				float finalDuration;
				Caster.MoveToDirectionForTime(direction, dashDistance, dashDuration, EasingFunction.Ease.Linear, false,
					out vector, out flag, out finalDuration);
				while (Caster.IsMoving() && target == null &&
				       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
				{
					yield return WaitForFrame();
					target = CheckCollision(direction);
					if (target != null)
					{
						PlaySkillAction(Caster, 1);
						Caster.StopMove();
						yield return WaitForSeconds(0.09f);
						HitAction(target);
					}
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private SkillAgent CheckCollision(Vector3 direction)
		{
			collision.UpdatePosition(Caster.Position +
			                         direction * (Singleton<FioraSkillActive3Data>.inst.CollisionBoxDepth * 0.5f));
			collision.UpdateWidth(SkillWidth);
			collision.UpdateDepth(Singleton<FioraSkillActive3Data>.inst.CollisionBoxDepth);
			collision.UpdateNormalized(direction);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			if (enemyCharacters.Any<SkillAgent>())
			{
				return enemyCharacters.NearestOne(Caster.Position);
			}

			return null;
		}

		
		private void HitAction(SkillAgent target)
		{
			SkillData skillData = Caster.GetSkillData(SkillSlotIndex.Passive);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[skillData.level]);
			CharacterState characterState = target.FindStateByGroup(data.group, Caster.ObjectId);
			parameterCollection.Clear();
			if (characterState != null && characterState.StateData.maxStack <= characterState.StackCount)
			{
				float num = 1.2f + Caster.Stat.CriticalStrikeDamage;
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<FioraSkillActive3Data>.inst.DamageByLevel[SkillLevel] * num);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<FioraSkillActive3Data>.inst.SkillApCoef * num);
				target.RemoveStateByGroup(data.group, Caster.ObjectId);
				PlaySkillAction(Caster, 2, target);
				ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
					Singleton<FioraSkillActive3Data>.inst.CooldownReduce);
				ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
					Singleton<FioraSkillActive3Data>.inst.CooldownReduce2);
			}
			else
			{
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<FioraSkillActive3Data>.inst.DamageByLevel[SkillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<FioraSkillActive3Data>.inst.SkillApCoef);
			}

			DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<FioraSkillActive3Data>.inst.EffectAndSound);
		}

		
		protected override bool IsChangedSkillSequence()
		{
			return target != null;
		}
	}
}