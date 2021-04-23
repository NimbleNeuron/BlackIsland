using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.XiukaiActive4)]
	public class XiukaiActive4 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, info.cursorPosition);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (sector == null)
			{
				sector = new CollisionSector3D(Caster.Position, SkillRange, SkillAngle, Caster.Forward);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartCoroutine(FireProc());
		}

		
		private IEnumerator FireProc()
		{
			int count = 0;
			WaitForSeconds waitForSeconds = new WaitForSeconds(Singleton<XiukaiSkillActive4Data>.inst.AttackDelay);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<XiukaiSkillActive4Data>.inst.PassiveBuffState);
			int stackByGroup = Caster.GetStackByGroup(data.group, Caster.ObjectId);
			int damage = (int) (Singleton<XiukaiSkillActive4Data>.inst.BaseDamage[SkillLevel] +
			                    stackByGroup * Singleton<XiukaiSkillActive4Data>.inst.AddStackDamage);
			while (count < Singleton<XiukaiSkillActive4Data>.inst.AttackCount)
			{
				yield return waitForSeconds;
				sector.UpdatePosition(Caster.Position);
				sector.UpdateRadius(SkillRange);
				sector.UpdateAngle(SkillAngle);
				sector.UpdateNormalized(Caster.Forward);
				List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
				PlaySkillAction(Caster, 1, null, Caster.Forward);
				parameters.Clear();
				parameters.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<XiukaiSkillActive4Data>.inst.SkillApCoef);
				parameters.Add(SkillScriptParameterType.Damage, damage);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameters, 1013501);
				for (int i = 0; i < enemyCharacters.Count; i++)
				{
					if (enemyCharacters[i].AnyHaveNegativelyAffectsMovementState())
					{
						AddState(enemyCharacters[i], Singleton<XiukaiSkillActive4Data>.inst.DebuffState[SkillLevel]);
					}
				}

				int num = count;
				count = num + 1;
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			Finish();
		}
	}
}