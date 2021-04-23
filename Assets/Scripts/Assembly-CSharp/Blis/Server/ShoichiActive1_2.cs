using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive1_2)]
	public class ShoichiActive1_2 : ShoichiSkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionSector3D sector = new CollisionSector3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		protected override void Start()
		{
			base.Start();
			Caster.RemoveStateByGroup(Singleton<ShoichiSkillActive1Data>.inst.BuffStateGroup, Caster.ObjectId);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			CasterLockRotation(true);
			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			sector.UpdateNormalized(direction);
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillRange);
			sector.UpdateAngle(SkillAngle);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			if (enemyCharacters.Count > 0)
			{
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<ShoichiSkillActive1Data>.inst.SkillApCoef);
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<ShoichiSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<ShoichiSkillActive1Data>.inst.EffectAndSoundCode2);
				Caster.Owner.SetReservationCooldown(SkillSlotSet.Active1_1,
					Singleton<ShoichiSkillActive1Data>.inst.CooldownReduce);
				AddState(enemyCharacters, Singleton<ShoichiSkillActive1Data>.inst.DeBuffStateCode);
			}

			yield return WaitForSeconds(Singleton<ShoichiSkillActive1Data>.inst.DaggerInstallTime);
			CreatePassiveDagger(Caster.Position, direction, Singleton<ShoichiSkillActive1Data>.inst.PassiveDaggerRange);
			if (SkillFinishDelayTime > 0f)
			{
				yield return WaitForSeconds(SkillFinishDelayTime -
				                            Singleton<ShoichiSkillActive1Data>.inst.DaggerInstallTime);
			}

			Finish();
		}
	}
}