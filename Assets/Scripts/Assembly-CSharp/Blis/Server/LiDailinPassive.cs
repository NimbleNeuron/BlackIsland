using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinPassive)]
	public class LiDailinPassive : SkillScript
	{
		
		private WorldPlayerCharacter wpcSelf;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterConsumeItemConsumableEvent =
				(Action<WorldPlayerCharacter, ItemConsumableData>) Delegate.Combine(
					inst.OnAfterConsumeItemConsumableEvent,
					new Action<WorldPlayerCharacter, ItemConsumableData>(OnAfterConsumeItemConsumableEvent));
			if (wpcSelf == null)
			{
				wpcSelf = Caster.Character as WorldPlayerCharacter;
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterConsumeItemConsumableEvent = (Action<WorldPlayerCharacter, ItemConsumableData>) Delegate.Remove(
				inst.OnAfterConsumeItemConsumableEvent,
				new Action<WorldPlayerCharacter, ItemConsumableData>(OnAfterConsumeItemConsumableEvent));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			float timeStack = 0f;
			for (;;)
			{
				yield return WaitForFrame();
				if (Caster.Status.ExtraPoint == Caster.Stat.MaxExtraPoint)
				{
					if (!Caster.AnyHaveStateByGroup(GameDB.characterState
						    .GetData(Singleton<LiDailinSkillData>.inst.PassiveDrunkennessStateCode).group) &&
					    !wpcSelf.SkillController.IsPlaying(SkillSlotIndex.Active2))
					{
						AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDrunkennessStateCode);
					}

					timeStack = 0f;
				}
				else if (Caster.Status.ExtraPoint <= 0)
				{
					timeStack = 0f;
				}
				else if (wpcSelf.SkillController.IsPlaying(SkillSlotIndex.Active2))
				{
					timeStack = Singleton<LiDailinSkillData>.inst.PassiveDecompositionTermTime -
					            Singleton<LiDailinSkillData>.inst.DecompositionPreventTime;
				}
				else if (Caster.AnyHaveStateByGroup(GameDB.characterState
					.GetData(Singleton<LiDailinSkillData>.inst.PassiveDrunkennessStateCode).group))
				{
					timeStack = 0f;
				}
				else
				{
					timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
					if (timeStack >= Singleton<LiDailinSkillData>.inst.PassiveDecompositionTermTime)
					{
						timeStack -= Singleton<LiDailinSkillData>.inst.PassiveDecompositionTermTime;
						Caster.ExtraPointModifyTo(Caster, Singleton<LiDailinSkillData>.inst.PassiveDecompositionAmount);
					}
				}
			}
		}

		
		private void OnAfterConsumeItemConsumableEvent(WorldPlayerCharacter consumer, ItemConsumableData itemData)
		{
			if (Caster.Character != consumer)
			{
				return;
			}

			if (!itemData.consumableTagFlag.HasFlag(ItemConsumableTag.Alcohol))
			{
				return;
			}

			AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveAlcoholItemConsumeBuff[SkillLevel]);
		}
	}
}