using System;
using Blis.Common;

namespace Blis.Client
{
	public class LocalBattleEventCollector : SingletonMonoBehaviour<LocalBattleEventCollector>
	{
		
		
		public event Action<CharacterStateValue, LocalCharacter> OnAddEffectStateEvent;


		
		
		public event Action<CharacterStateValue, LocalCharacter> OnRemoveEffectStateEvent;


		
		
		public event Action<LocalCharacter> OnDeadAction;


		
		
		public event Action<LocalCharacter> OnDyingConditionAction;


		
		
		public event Action<LocalCharacter> OnRevivalAction;


		
		
		public event Func<LocalPlayerCharacter, Item, Item> OnBeforeMakeItemProcessEvent;


		
		
		public event Action<LocalCharacter, LocalCharacter.SkillDamageInfo> OnAfterSkillDamageProcess;


		
		
		public event Action<LocalPlayerCharacter, SkillId> OnSkillFinishAction;


		public void OnAddEffectState(CharacterStateValue characterStateValue, LocalCharacter target)
		{
			Action<CharacterStateValue, LocalCharacter> onAddEffectStateEvent = OnAddEffectStateEvent;
			if (onAddEffectStateEvent == null)
			{
				return;
			}

			onAddEffectStateEvent(characterStateValue, target);
		}


		public void OnRemoveEffectState(CharacterStateValue characterStateValue, LocalCharacter target)
		{
			Action<CharacterStateValue, LocalCharacter> onRemoveEffectStateEvent = OnRemoveEffectStateEvent;
			if (onRemoveEffectStateEvent == null)
			{
				return;
			}

			onRemoveEffectStateEvent(characterStateValue, target);
		}


		public void OnDead(LocalCharacter target)
		{
			Action<LocalCharacter> onDeadAction = OnDeadAction;
			if (onDeadAction == null)
			{
				return;
			}

			onDeadAction(target);
		}


		public void OnDyingCondition(LocalCharacter target)
		{
			Action<LocalCharacter> onDyingConditionAction = OnDyingConditionAction;
			if (onDyingConditionAction == null)
			{
				return;
			}

			onDyingConditionAction(target);
		}


		public void OnRevival(LocalCharacter target)
		{
			Action<LocalCharacter> onRevivalAction = OnRevivalAction;
			if (onRevivalAction == null)
			{
				return;
			}

			onRevivalAction(target);
		}


		public Item OnBeforeMakeItemProcess(LocalPlayerCharacter playerCharacter, Item item)
		{
			Func<LocalPlayerCharacter, Item, Item> onBeforeMakeItemProcessEvent = OnBeforeMakeItemProcessEvent;
			if (onBeforeMakeItemProcessEvent == null)
			{
				return null;
			}

			return onBeforeMakeItemProcessEvent(playerCharacter, item);
		}


		public void OnAfterSkillDamage(LocalCharacter target, LocalCharacter.SkillDamageInfo info)
		{
			Action<LocalCharacter, LocalCharacter.SkillDamageInfo>
				onAfterSkillDamageProcess = OnAfterSkillDamageProcess;
			if (onAfterSkillDamageProcess == null)
			{
				return;
			}

			onAfterSkillDamageProcess(target, info);
		}


		public void OnSkillFinish(LocalPlayerCharacter playerCharacter, SkillId skillId)
		{
			Action<LocalPlayerCharacter, SkillId> onSkillFinishAction = OnSkillFinishAction;
			if (onSkillFinishAction == null)
			{
				return;
			}

			onSkillFinishAction(playerCharacter, skillId);
		}
	}
}