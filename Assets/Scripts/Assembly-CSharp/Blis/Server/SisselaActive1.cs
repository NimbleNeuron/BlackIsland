using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive1)]
	public class SisselaActive1 : SisselaSkillScript
	{
		
		private const SkillSlotSet skillSlotSetFlag =
			SkillSlotSet.Active1_1 | SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1;

		
		private readonly SkillScriptParameterCollection passDamage = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection stopDamage = SkillScriptParameterCollection.Create();

		
		private readonly HashSet<int> strickenObjectIds = new HashSet<int>();

		
		private readonly WaitForFrameUpdate WilsonMoveCollisionCheckWaitFrame = new WaitForFrameUpdate();

		
		private CollisionCircle3D collision;

		
		private WorldSummonServant wilson;

		
		private bool wilsonMoving;

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
			LockSkillSlotWithPacket(SkillSlotSet.Active1_1 | SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, true);
			wilsonMoving = false;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (!wilsonMoving)
			{
				LockSkillSlotWithPacket(SkillSlotSet.Active1_1 | SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1,
					false);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			if (wilson == null)
			{
				WorldPlayerCharacter owner = (WorldPlayerCharacter) Caster.Character;
				wilson = GetWilson(owner);
			}

			Vector3 vector = wilson.GetPosition();
			if (IsWilsonUnion())
			{
				vector += Caster.Forward * Singleton<SisselaSkillData>.inst.A1SeparateStartDistance;
				SetWilsonState(false, wilson);
			}

			wilson.MoveStraightWithoutNavSpeed(vector, GetSkillPoint(), Singleton<SisselaSkillData>.inst.A1WilsonSpeed);
			wilson.StartThrowingCoroutine(WilsonMoveCollisionCheck(wilson),
				delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", SkillId,
						exception.Message, exception.StackTrace));
				});
			wilsonMoving = true;
			PlaySkillAction(Caster, 1);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private IEnumerator WilsonMoveCollisionCheck(WorldSummonServant wilson)
		{
			float radius = wilson.Stat.Radius;
			strickenObjectIds.Clear();
			if (collision == null)
			{
				collision = new CollisionCircle3D(wilson.GetPosition(), SkillWidth * 0.5f);
			}
			else
			{
				collision.UpdateRadius(SkillWidth * 0.5f);
			}

			int skillLv = SkillLevel;
			while (!wilson.IsStopped())
			{
				collision.UpdatePosition(wilson.GetPosition());
				foreach (SkillAgent skillAgent in GetEnemyCharacters(collision))
				{
					if (!strickenObjectIds.Contains(skillAgent.ObjectId))
					{
						passDamage.Clear();
						passDamage.Add(SkillScriptParameterType.Damage,
							Singleton<SisselaSkillData>.inst.A1PassBaseDamage[skillLv]);
						passDamage.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<SisselaSkillData>.inst.A1PassApDamage);
						DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, passDamage,
							Singleton<SisselaSkillData>.inst.A1MoveEffectCode);
						strickenObjectIds.Add(skillAgent.ObjectId);
					}
				}

				yield return WilsonMoveCollisionCheckWaitFrame.Frame(1);
			}

			if (Singleton<SisselaSkillData>.inst.A1StopDamageDelay > 0f)
			{
				yield return WilsonMoveCollisionCheckWaitFrame.Seconds(Singleton<SisselaSkillData>.inst
					.A1StopDamageDelay);
			}

			collision.UpdatePosition(wilson.GetPosition());
			collision.UpdateRadius(SkillInnerRange);
			foreach (SkillAgent target in GetEnemyCharacters(collision))
			{
				stopDamage.Clear();
				stopDamage.Add(SkillScriptParameterType.Damage,
					Singleton<SisselaSkillData>.inst.A1StopBaseDamage[skillLv]);
				stopDamage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SisselaSkillData>.inst.A1StopApDamage);
				DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, stopDamage,
					Singleton<SisselaSkillData>.inst.A1StopEffectCode);
			}

			LockSkillSlotWithPacket(SkillSlotSet.Active1_1 | SkillSlotSet.Active2_1 | SkillSlotSet.Active3_1, false);
			PlaySkillAction(Caster, 2);
		}
	}
}