using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolActive2)]
	public class IsolActive2 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollectionNormal =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionSummon =
			SkillScriptParameterCollection.Create();

		
		private CollisionSector3D sector;

		
		protected override void Start()
		{
			base.Start();
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			if (sector == null)
			{
				sector = new CollisionSector3D(Vector3.zero, SkillRange, SkillAngle, Vector3.zero);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Vector3 dirction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			LookAtDirection(Caster, dirction);
			CasterLockRotation(true);
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			int skillLevel = SkillLevel;
			int damage = Singleton<IsolSkillActive2Data>.inst.Damage[skillLevel];
			float skillApCoef = Singleton<IsolSkillActive2Data>.inst.SkillApCoef;
			int debuffState = Singleton<IsolSkillActive2Data>.inst.DebuffState[skillLevel];
			int damageCount = Singleton<IsolSkillActive2Data>.inst.DamageCount;
			float damageTermTime = Singleton<IsolSkillActive2Data>.inst.DamageTermTime;
			int effectCode = Singleton<IsolSkillActive2Data>.inst.effectCode;
			sector.UpdateNormalized(dirction);
			sector.UpdateRadius(SkillRange);
			sector.UpdateAngle(SkillAngle);
			for (;;)
			{
				int num = damageCount - 1;
				damageCount = num;
				sector.UpdatePosition(Caster.Position);
				List<SkillAgent> enemies = GetEnemies(sector);
				foreach (SkillAgent skillAgent in enemies)
				{
					WorldSummonBase worldSummonBase = skillAgent.Character as WorldSummonBase;
					if (worldSummonBase != null)
					{
						parameterCollectionSummon.Clear();
						parameterCollectionSummon.Add(SkillScriptParameterType.Damage, 1f);
						DamageToSummon(worldSummonBase, DamageType.Skill, DamageSubType.Normal, 0,
							parameterCollectionSummon, SkillSlotSet.Active1_1, skillAgent.Position,
							GameUtil.Direction(Caster.Position, skillAgent.Position), 0, true, 0, 1f);
					}
					else
					{
						parameterCollectionNormal.Clear();
						parameterCollectionNormal.Add(SkillScriptParameterType.Damage, damage);
						parameterCollectionNormal.Add(SkillScriptParameterType.DamageApCoef, skillApCoef);
						DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollectionNormal,
							effectCode);
					}
				}

				AddState(enemies, debuffState);
				if (damageCount <= 0)
				{
					break;
				}

				yield return WaitForSeconds(damageTermTime);
			}

			FinishConcentration(false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void OnPlayAgain(Vector3 hitPoint)
		{
			base.OnPlayAgain(hitPoint);
			Finish();
		}
	}
}