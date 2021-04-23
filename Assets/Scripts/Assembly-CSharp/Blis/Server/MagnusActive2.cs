using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusActive2)]
	public class MagnusActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionCircle3D(Caster.Position, SkillRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float skillConcentrationTime = SkillConcentrationTime;
			int attackCount = Singleton<MagnusSkillActive2Data>.inst.AttackCountByLevel[SkillLevel];
			float skillApCoef = Singleton<MagnusSkillActive2Data>.inst.SkillApCoef[SkillLevel];
			float skillDefCoef = Singleton<MagnusSkillActive2Data>.inst.SkillDefCoef[SkillLevel];
			int skillDamage = Singleton<MagnusSkillActive2Data>.inst.DamageByLevel[SkillLevel];
			float tick = 1f;
			if (0 < attackCount - 1)
			{
				tick = skillConcentrationTime / (attackCount - 1);
			}

			AddState(Caster, Singleton<MagnusSkillActive2Data>.inst.MoveSpeedDown);
			int equipWeaponMasteryType = (int) GetEquipWeaponMasteryType(Caster);
			int effectAndSoundCode =
				Singleton<MagnusSkillActive2Data>.inst.EffectAndSoundWeaponType[equipWeaponMasteryType];
			int num;
			for (int index = 0; index < attackCount; index = num)
			{
				sector.UpdatePosition(Caster.Position);
				sector.UpdateRadius(SkillRange);
				foreach (SkillAgent skillAgent in GetEnemyCharacters(sector))
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef, skillApCoef);
					parameterCollection.Add(SkillScriptParameterType.DamageDefCoef, skillDefCoef);
					parameterCollection.Add(SkillScriptParameterType.Damage, skillDamage);
					if (skillAgent.AnyHaveNegativelyAffectsMovementState())
					{
						parameterCollection.Add(SkillScriptParameterType.FinalMoreDamage, 0.5f);
					}

					DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
						effectAndSoundCode);
				}

				if (index < attackCount - 1)
				{
					yield return WaitForSeconds(tick);
				}

				num = index + 1;
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
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<MagnusSkillActive2Data>.inst.MoveSpeedDown);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			}
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			Finish();
		}
	}
}