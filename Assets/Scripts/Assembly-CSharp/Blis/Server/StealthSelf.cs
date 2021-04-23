using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.StealthSelf)]
	public class StealthSelf : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Combine(
				inst.OnBeforeActionCastingEvent,
				new Action<WorldCharacter, ActionCostData>(OnBeforeActionCastingEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Combine(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnBeforeInstallSummonEvent = (Action<WorldPlayerCharacter, int>) Delegate.Combine(
				inst3.OnBeforeInstallSummonEvent, new Action<WorldPlayerCharacter, int>(OnBeforeInstallSummonEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeOpenCorpseEvent = (Action<WorldPlayerCharacter, WorldCharacter>) Delegate.Combine(
				inst4.OnBeforeOpenCorpseEvent, new Action<WorldPlayerCharacter, WorldCharacter>(OnBeforeOpenCorpse));
			BattleEventCollector inst5 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst5.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst5.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
			Target.Invisible(true);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeActionCastingEvent = (Action<WorldCharacter, ActionCostData>) Delegate.Remove(
				inst.OnBeforeActionCastingEvent,
				new Action<WorldCharacter, ActionCostData>(OnBeforeActionCastingEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeSkillActiveEvent = (Action<WorldCharacter, SkillData, SkillSlotSet>) Delegate.Remove(
				inst2.OnBeforeSkillActiveEvent,
				new Action<WorldCharacter, SkillData, SkillSlotSet>(OnBeforeSkillActiveEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnBeforeInstallSummonEvent = (Action<WorldPlayerCharacter, int>) Delegate.Remove(
				inst3.OnBeforeInstallSummonEvent, new Action<WorldPlayerCharacter, int>(OnBeforeInstallSummonEvent));
			BattleEventCollector inst4 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst4.OnBeforeOpenCorpseEvent = (Action<WorldPlayerCharacter, WorldCharacter>) Delegate.Remove(
				inst4.OnBeforeOpenCorpseEvent, new Action<WorldPlayerCharacter, WorldCharacter>(OnBeforeOpenCorpse));
			BattleEventCollector inst5 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst5.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst5.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
			Target.Invisible(false);
			if (StateGroup != 0)
			{
				Target.RemoveStateByGroup(StateGroup, Caster.ObjectId);
			}
		}

		
		private void OnFinishNormalAttack(WorldCharacter victim, WorldCharacter attacker)
		{
			if (attacker.ObjectId != Target.ObjectId)
			{
				return;
			}

			Finish();
		}

		
		private void OnBeforeActionCastingEvent(WorldCharacter actionCaster, ActionCostData actionCostData)
		{
			if (actionCaster == null)
			{
				return;
			}

			if (actionCaster.ObjectId != Target.ObjectId)
			{
				return;
			}

			Finish();
		}

		
		private void OnBeforeSkillActiveEvent(WorldCharacter skillCaster, SkillData skillData,
			SkillSlotSet skillSlotSet)
		{
			if (skillCaster == null)
			{
				return;
			}

			if (skillCaster.ObjectId != Target.ObjectId)
			{
				return;
			}

			if (skillSlotSet.IsNormalAttack())
			{
				return;
			}

			Finish();
		}

		
		private void OnBeforeInstallSummonEvent(WorldCharacter owner, int summonCode)
		{
			if (owner == null)
			{
				return;
			}

			if (Target.Character.ObjectId != owner.ObjectId)
			{
				return;
			}

			Finish();
		}

		
		private void OnBeforeOpenCorpse(WorldPlayerCharacter opener, WorldCharacter corpse)
		{
			if (opener == null)
			{
				return;
			}

			if (opener.ObjectId != Target.ObjectId)
			{
				return;
			}

			Finish();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}