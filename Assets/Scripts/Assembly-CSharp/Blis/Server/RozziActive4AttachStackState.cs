using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive4AttachStackState)]
	public class RozziActive4AttachStackState : SkillScript
	{
		
		private int attachAfterDamageStack;

		
		private int maxStack;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionFullStack =
			SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode);
			maxStack = data.maxStack;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
		}

		
		public void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null || damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			if (damageInfo.DamageType == DamageType.Normal)
			{
				victim.SkillAgent.ModifyStateValue(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateGroupId,
					Caster.ObjectId, 0f, 1, false);
			}
			else if (damageInfo.DamageType == DamageType.Skill)
			{
				SkillSlotSet? skillSlotSet = damageInfo.SkillSlotSet;
				SkillSlotSet skillSlotSet2 = SkillSlotSet.Active1_1;
				if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
				{
					skillSlotSet = damageInfo.SkillSlotSet;
					skillSlotSet2 = SkillSlotSet.Active2_1;
					if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
					{
						skillSlotSet = damageInfo.SkillSlotSet;
						skillSlotSet2 = SkillSlotSet.Active3_1;
						if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
						{
							goto IL_118;
						}
					}
				}

				victim.SkillAgent.ModifyStateValue(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateGroupId,
					Caster.ObjectId, 0f, 2, false);
			}

			IL_118:
			PlaySkillAction(victim.SkillAgent, SkillId.RozziActive4AttachStackState, 1);
			attachAfterDamageStack =
				victim.SkillAgent.GetStackByGroup(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateGroupId,
					Caster.ObjectId);
			if (attachAfterDamageStack >= maxStack)
			{
				AddState(Caster, Singleton<RozziSkillActive4Data>.inst.AttachFullStackBuffStateCode);
				victim.RemoveStateByGroup(
					GameDB.characterState
						.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[SkillLevel]).group,
					Caster.ObjectId);
				victim.RemoveStateByGroup(
					GameDB.characterState.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStackStateCode)
						.group, Caster.ObjectId);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			SummonData summonData =
				GameDB.character.GetSummonData(Singleton<RozziSkillActive4Data>.inst.SummonObjectCode);
			CollisionCircle3D collisionObject = new CollisionCircle3D(Target.Position, summonData.rangeRadius);
			foreach (SkillAgent skillAgent in GetEnemyCharacters(collisionObject))
			{
				int skillLevel = SkillLevel;
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<RozziSkillActive4Data>.inst.DamageActive4ByLevel[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<RozziSkillActive4Data>.inst.DamageActive4ApCoef);
				DamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
				parameterCollectionFullStack.Clear();
				if (attachAfterDamageStack >= maxStack && Target.ObjectId == skillAgent.ObjectId)
				{
					parameterCollectionFullStack.Add(SkillScriptParameterType.DamageTargetMaxHpCoef,
						Singleton<RozziSkillActive4Data>.inst.AdditionalDamageRatioByLevel[SkillLevel] * 0.01f);
					DirectDamageTo(skillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollectionFullStack,
						0);
				}
			}

			MonoBehaviourInstance<GameService>.inst.Announce.MakeNoise(Caster.WorldObject, Target.Position, null,
				NoiseType.TrapHit);
		}
	}
}