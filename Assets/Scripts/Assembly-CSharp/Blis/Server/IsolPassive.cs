using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolPassive)]
	public class IsolPassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterInstalledTrapEvent = (Action<WorldSummonBase, WorldCharacter, SummonData>) Delegate.Combine(
				inst.OnAfterInstalledTrapEvent,
				new Action<WorldSummonBase, WorldCharacter, SummonData>(OnAfterInstalledTrapEvent));
			int skillLevel = SkillLevel;
			int num = Singleton<IsolSkillPassiveData>.inst.BuffState[skillLevel];
			Caster.RemoveStateByGroup(GameDB.characterState.GetData(num).group, Caster.ObjectId);
			CharacterState characterState = CreateState(Caster, num, 0, 0f);
			characterState.AddExternalStat(StatType.InstallTrapCastingTimeReduce,
				Singleton<IsolSkillPassiveData>.inst.InstallTrapCastingTimeReduce[skillLevel], StatType.None, 0f);
			AddState(Caster, characterState);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterInstalledTrapEvent = (Action<WorldSummonBase, WorldCharacter, SummonData>) Delegate.Remove(
				inst.OnAfterInstalledTrapEvent,
				new Action<WorldSummonBase, WorldCharacter, SummonData>(OnAfterInstalledTrapEvent));
		}

		
		private void OnAfterInstalledTrapEvent(WorldSummonBase summonBase, WorldCharacter owner, SummonData summonData)
		{
			if (summonBase == null || owner == null || summonData == null)
			{
				return;
			}

			if (Caster.Character != owner)
			{
				return;
			}

			WorldSummonTrap worldSummonTrap = summonBase as WorldSummonTrap;
			if (worldSummonTrap == null)
			{
				return;
			}

			int skillLevel = SkillLevel;
			if (summonData.castingActionType == CastingActionType.InstallTrap ||
			    summonData.code == Singleton<IsolSkillActive1Data>.inst.SummonObjectCode ||
			    summonData.code == Singleton<IsolSkillActive4Data>.inst.SummonObjectCode)
			{
				worldSummonTrap.SetAdditionalStateEffectList(new List<int>
				{
					Singleton<IsolSkillPassiveData>.inst.InstallTrapAdditionalStateEffect[skillLevel]
				});
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			for (;;)
			{
				yield return WaitForFrame();
				List<WorldSummonBase> list = Caster.Character.SightAgent.FindAllInAllySights<WorldSummonBase>();
				for (int i = list.Count - 1; i >= 0; i--)
				{
					WorldSummonBase worldSummonBase = list[i];
					if (!worldSummonBase.IsAlive || !(worldSummonBase.Owner != Caster.Character) ||
					    Caster.Character.SightAgent.IsMemorizedTarget(worldSummonBase.ObjectId) ||
					    worldSummonBase.SummonData.createVisibleTime >= worldSummonBase.SummonData.duration)
					{
						list.RemoveAt(i);
					}
				}

				Caster.Character.MemorizedObjectAdd<WorldSummonBase>(list);
			}
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			Start();
		}
	}
}