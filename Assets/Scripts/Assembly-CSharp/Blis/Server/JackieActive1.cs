using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive1)]
	public class JackieActive1 : SkillScript
	{
		
		private readonly List<int> hitTargets = new List<int>();

		
		private readonly SkillScriptParameterCollection parameterCollection_1 = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollection_2 = SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			hitTargets.Clear();
			if (sector == null)
			{
				sector = new CollisionSector3D(Caster.Position, SkillRange, SkillAngle, Vector3.forward);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			
			Vector3 direction = GameUtil.DirectionOnPlane(Caster.Position, info.cursorPosition);
			
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
			
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int masteryType = (int) GetEquipWeaponMasteryType(Caster);
			int skillLv = SkillLevel;
			
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillRange);
			sector.UpdateAngle(SkillAngle);
			sector.UpdateNormalized(direction);
			
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			
			parameterCollection_1.Clear();
			parameterCollection_1.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<JackieSkillActive1Data>.inst.SkillApCoef);
			
			parameterCollection_1.Add(SkillScriptParameterType.Damage,
				Singleton<JackieSkillActive1Data>.inst.DamageByLevel[skillLv]);
			
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection_1,
				Singleton<JackieSkillActive1Data>.inst.EffectAndSoundWeaponType[masteryType]);
			
			AddState(enemyCharacters, Singleton<JackieSkillActive1Data>.inst.DebuffState[skillLv]);
			
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				hitTargets.Add(skillAgent.ObjectId);
			}

			yield return WaitForSeconds(Singleton<JackieSkillActive1Data>.inst.SkillAttackDelay_2);
			
			sector.UpdatePosition(Caster.Position);
			sector.UpdateNormalized(direction);
			
			List<SkillAgent> enemyCharacters2 = GetEnemyCharacters(sector);
			
			parameterCollection_2.Clear();
			parameterCollection_2.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<JackieSkillActive1Data>.inst.SkillApCoef_2);
			
			parameterCollection_2.Add(SkillScriptParameterType.Damage,
				Singleton<JackieSkillActive1Data>.inst.DamageByLevel_2[skillLv]);
			
			DamageTo(enemyCharacters2, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection_2,
				Singleton<JackieSkillActive1Data>.inst.EffectAndSoundWeaponType[masteryType]);
			
			foreach (SkillAgent skillAgent2 in enemyCharacters2)
			{
				if (!hitTargets.Contains(skillAgent2.ObjectId))
				{
					AddState(skillAgent2, Singleton<JackieSkillActive1Data>.inst.DebuffState[skillLv]);
				}
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
		}
	}
}