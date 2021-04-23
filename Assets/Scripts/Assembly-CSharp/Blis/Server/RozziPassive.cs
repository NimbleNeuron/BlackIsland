using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziPassive)]
	public class RozziPassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterConsumeItemConsumableEvent =
				(Action<WorldPlayerCharacter, ItemConsumableData>) Delegate.Combine(
					inst.OnAfterConsumeItemConsumableEvent,
					new Action<WorldPlayerCharacter, ItemConsumableData>(OnAfterConsumeItemConsumable));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeActiveSkillFinishEvent =
				(Action<WorldCharacter, SkillData, SkillSlotSet, bool>) Delegate.Combine(
					inst2.OnBeforeActiveSkillFinishEvent,
					new Action<WorldCharacter, SkillData, SkillSlotSet, bool>(OnBeforeActiveSkillFinish));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterConsumeItemConsumableEvent = (Action<WorldPlayerCharacter, ItemConsumableData>) Delegate.Remove(
				inst.OnAfterConsumeItemConsumableEvent,
				new Action<WorldPlayerCharacter, ItemConsumableData>(OnAfterConsumeItemConsumable));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnBeforeActiveSkillFinishEvent =
				(Action<WorldCharacter, SkillData, SkillSlotSet, bool>) Delegate.Remove(
					inst2.OnBeforeActiveSkillFinishEvent,
					new Action<WorldCharacter, SkillData, SkillSlotSet, bool>(OnBeforeActiveSkillFinish));
		}

		
		public void OnAfterConsumeItemConsumable(WorldPlayerCharacter consumer, ItemConsumableData itemData)
		{
			if (consumer.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (!consumer.IsAlive || consumer.IsDyingCondition)
			{
				return;
			}

			if (!itemData.consumableTagFlag.HasFlag(ItemConsumableTag.Chocolate))
			{
				return;
			}

			if (itemData.hpRecover > 0)
			{
				int spRecoverAmount =
					(int) (itemData.hpRecover * Singleton<RozziSkillPassiveData>.inst.ChocolateSpRatio);
				consumer.SpRecover(Singleton<RozziSkillPassiveData>.inst.SpRecoveryStateCodeByLevel[SkillLevel],
					spRecoverAmount);
				return;
			}

			if (itemData.spRecover > 0)
			{
				int hpRecoverAmount =
					(int) (itemData.spRecover * Singleton<RozziSkillPassiveData>.inst.ChocolateHpRatio);
				consumer.HpRecover(Singleton<RozziSkillPassiveData>.inst.HpRecoveryStateCodeByLevel[SkillLevel],
					hpRecoverAmount);
			}
		}

		
		public void OnBeforeActiveSkillFinish(WorldCharacter caster, SkillData skillData, SkillSlotSet skillSlotSet,
			bool cancel)
		{
			if (caster.ObjectId != Caster.ObjectId)
			{
				return;
			}

			if (!caster.IsAlive || caster.IsDyingCondition)
			{
				return;
			}

			if (!IsPossibleDoubleShot(skillData.SkillId))
			{
				return;
			}

			if (cancel)
			{
				return;
			}

			AddState(Caster, Singleton<RozziSkillPassiveData>.inst.DoubleShotStateCodeByLevel[SkillLevel]);
		}

		
		private bool IsPossibleDoubleShot(SkillId skillId)
		{
			return skillId == SkillId.RozziActive1 || skillId == SkillId.RozziActive2 ||
			       skillId == SkillId.RozziActive3_1 || skillId == SkillId.RozziActive3_2 ||
			       skillId == SkillId.RozziActive4;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}