using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolActive1Attach)]
	public class IsolActive1Attach : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			if (sector == null)
			{
				sector = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			CharacterState characterState = Target.FindStateByGroup(StateGroup, Caster.ObjectId);
			SummonData summonData =
				GameDB.character.GetSummonData(Singleton<IsolSkillActive1Data>.inst.SummonObjectCode);
			sector.UpdatePosition(Target.Position);
			sector.UpdateRadius(summonData.rangeRadius);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			if (enemyCharacters.Any<SkillAgent>())
			{
				int skillLevel = SkillLevel;
				int num = Singleton<IsolSkillActive1Data>.inst.BaseDamage[skillLevel];
				float num2 = Singleton<IsolSkillActive1Data>.inst.BaseSkillApCoef;
				num += Singleton<IsolSkillActive1Data>.inst.AdditionalDamagePerHit[skillLevel] *
				       characterState.StackCount;
				num2 += Singleton<IsolSkillActive1Data>.inst.AdditionalSkillApCoefPerHit * characterState.StackCount;
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage, num);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef, num2);
				DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
				float value = Mathf.Min(
					GameDB.characterState.GetData(Singleton<IsolSkillActive1Data>.inst.DebuffState).duration +
					characterState.StackCount *
					Singleton<IsolSkillActive1Data>.inst.DebuffStateDurationIncreasePerStack,
					Singleton<IsolSkillActive1Data>.inst.DebuffStateMaxDuration);
				foreach (SkillAgent target in enemyCharacters)
				{
					CharacterState state = CreateState(Caster, Singleton<IsolSkillActive1Data>.inst.DebuffState, 0,
						value);
					AddState(target, state);
				}

				int stateCode =
					Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[
						Caster.GetSkillLevel(SkillSlotIndex.Passive)];
				AddState(enemyCharacters, stateCode);
			}
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (victim == null)
			{
				return;
			}

			if (victim == Caster.Character)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.Attacker == null || damageInfo.Attacker != Caster.Character)
			{
				return;
			}

			PlaySkillAction(Target, SkillId.IsolActive1Attach, 1, victim.ObjectId);
			CharacterStateData data = GameDB.characterState.GetData(Singleton<IsolSkillActive1Data>.inst.AttachState);
			victim.SkillAgent.ModifyStateValue(data.group, Caster.ObjectId,
				-Singleton<IsolSkillActive1Data>.inst.DurationDecreasePerHit, 1, false);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}