using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirActive2Passive)]
	public class ZahirActive2Passive : SkillScript
	{
		
		private const int ChangedStackActionNo = 1;

		
		private const int RemovedStackActionNo = 2;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst2.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteChangedStateEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnCompleteChangedStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst3.OnCompleteChangedStateEvent,
				new Action<WorldCharacter, CharacterState>(OnCompleteChangedStateEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnCompleteRemoveStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst4.OnCompleteRemoveStateEvent,
				new Action<WorldCharacter, CharacterState>(OnCompleteRemoveStateEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst2.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteChangedStateEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnCompleteChangedStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst3.OnCompleteChangedStateEvent,
				new Action<WorldCharacter, CharacterState>(OnCompleteChangedStateEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnCompleteRemoveStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst4.OnCompleteRemoveStateEvent,
				new Action<WorldCharacter, CharacterState>(OnCompleteRemoveStateEvent));
		}

		
		private void OnAfterSkillDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
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

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			if (damageInfo.SkillSlotSet == null)
			{
				return;
			}

			if (damageInfo.SkillSlotSet.Value != SkillSlotSet)
			{
				SkillSlotSet? skillSlotSet = damageInfo.SkillSlotSet;
				SkillSlotSet skillSlotSet2 = SkillSlotSet.Passive_1;
				if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
				{
					skillSlotSet = damageInfo.SkillSlotSet;
					skillSlotSet2 = SkillSlotSet.WeaponSkill;
					if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
					{
						AddState(Caster, Singleton<ZahirSkillActive2Data>.inst.BuffState,
							Singleton<ZahirSkillActive2Data>.inst.AddStackCount);
					}
				}
			}
		}

		
		private void OnCompleteChangedStateEvent(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState).group !=
			    characterState.StateData.group)
			{
				return;
			}

			PlaySkillAction(Caster, PassiveSkillId, 1);
		}

		
		private void OnCompleteRemoveStateEvent(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState).group !=
			    characterState.StateData.group)
			{
				return;
			}

			PlaySkillAction(Caster, PassiveSkillId, characterState.StackCount == 0 ? 1 : 2);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}