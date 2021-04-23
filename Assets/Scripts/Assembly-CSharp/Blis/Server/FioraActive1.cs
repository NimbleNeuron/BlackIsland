using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive1)]
	public class FioraActive1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly CollisionBox3D sector = new CollisionBox3D(Vector3.zero, 0f, 0f, Vector3.zero);

		
		protected override void Start()
		{
			base.Start();
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

			sector.UpdatePosition(Caster.Position + SkillRange * 0.5f * direction);
			sector.UpdateWidth(SkillWidth);
			sector.UpdateDepth(SkillRange);
			sector.UpdateNormalized(direction);
			foreach (SkillAgent skillAgent in GetEnemyCharacters(sector))
			{
				parameterCollection.Clear();
				SkillData skillData = Caster.GetSkillData(SkillSlotIndex.Passive);
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[skillData.level]);
				CharacterState characterState = skillAgent.FindStateByGroup(data.group, Caster.ObjectId);
				if (characterState != null && characterState.StateData.maxStack <= characterState.StackCount)
				{
					skillAgent.RemoveStateByGroup(data.group, Caster.ObjectId);
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<FioraSkillActive1Data>.inst.DamageByLevel[SkillLevel] *
						(1.2f + Caster.Stat.CriticalStrikeDamage));
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<FioraSkillActive1Data>.inst.SkillApCoef * (1.2f + Caster.Stat.CriticalStrikeDamage));
					ModifySkillCooldown(Caster, SkillSlotSet.Active2_1,
						Singleton<FioraSkillActive1Data>.inst.CooldownReduce);
					PlaySkillAction(Caster, 1, skillAgent);
					AddState(skillAgent, Singleton<FioraSkillActive1Data>.inst.MarkingSlowState[SkillLevel]);
				}
				else
				{
					parameterCollection.Add(SkillScriptParameterType.Damage,
						Singleton<FioraSkillActive1Data>.inst.DamageByLevel[SkillLevel]);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<FioraSkillActive1Data>.inst.SkillApCoef);
					AddState(skillAgent, Singleton<FioraSkillActive1Data>.inst.DebuffState[SkillLevel]);
				}

				DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<FioraSkillActive1Data>.inst.EffectAndSound);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}