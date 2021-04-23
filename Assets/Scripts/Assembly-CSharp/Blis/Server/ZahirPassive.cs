using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirPassive)]
	public class ZahirPassive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly WaitForFrameUpdate waitFrame_2 = new WaitForFrameUpdate();

		
		private readonly HashSet<int> alwaysInsightObjectIds = new HashSet<int>();

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst2.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterSkillDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterSkillDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterSkillDamageProcess));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst2.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
			ClearAlwaysInSightObjectIds();
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

			SkillSlotSet? skillSlotSet = damageInfo.SkillSlotSet;
			SkillSlotSet skillSlotSet2 = SkillSlotSet.Attack_1;
			if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
			{
				skillSlotSet = damageInfo.SkillSlotSet;
				skillSlotSet2 = SkillSlotSet.Passive_1;
				if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
				{
					skillSlotSet = damageInfo.SkillSlotSet;
					skillSlotSet2 = SkillSlotSet.WeaponSkill;
					if (!((skillSlotSet.GetValueOrDefault() == skillSlotSet2) & (skillSlotSet != null)))
					{
						AddState(Caster, Singleton<ZahirSkillPassiveData>.inst.BuffState[SkillLevel]);
						CharacterStateData data =
							GameDB.characterState.GetData(Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateCode);
						if (victim.SkillAgent.IsHaveStateByGroup(data.group, Caster.ObjectId))
						{
							victim.SkillAgent.RemoveStateByGroup(data.group, Caster.ObjectId);
							parameterCollection.Clear();
							parameterCollection.Add(SkillScriptParameterType.Damage,
								Singleton<ZahirSkillPassiveData>.inst.DamageByLevel[data.level]);
							parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
								Singleton<ZahirSkillPassiveData>.inst.SkillApCoef);
							DamageTo(victim.SkillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
								0);
							return;
						}

						AddState(victim.SkillAgent, Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateCode);
					}
				}
			}
		}

		
		private void OnKillEvent(WorldCharacter victim, DamageInfo damageInfo)
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

			if (victim.ObjectType != ObjectType.PlayerCharacter && victim.ObjectType != ObjectType.BotPlayerCharacter)
			{
				return;
			}

			if (!IsReadySkill(Caster, SkillSlotSet.Passive_1))
			{
				return;
			}

			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			foreach (PlayerSession playerSession in MonoBehaviourInstance<GameService>.inst.Player.PlayerSessions)
			{
				if (playerSession.Character.ObjectId != Caster.ObjectId && !playerSession.Character.IsAlive)
				{
					alwaysInsightObjectIds.Add(playerSession.Character.ObjectId);
					worldPlayerCharacter.AddAlwaysInsightObjectId(playerSession.Character.ObjectId);
				}
			}

			foreach (WorldPlayerCharacter worldPlayerCharacter2 in MonoBehaviourInstance<GameService>.inst.Bot
				.Characters)
			{
				if (worldPlayerCharacter2.ObjectId != Caster.ObjectId && !worldPlayerCharacter2.IsAlive)
				{
					alwaysInsightObjectIds.Add(worldPlayerCharacter2.ObjectId);
					worldPlayerCharacter.AddAlwaysInsightObjectId(worldPlayerCharacter2.ObjectId);
				}
			}

			PlayPassiveSkill(info, 1, 0);
			StartCoroutine(ClearAlwaysInSightObjectIdsDelay());
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		private IEnumerator ClearAlwaysInSightObjectIdsDelay()
		{
			yield return waitFrame_2.Seconds(Singleton<ZahirSkillPassiveData>.inst.PassiveDuration + 1f);
			ClearAlwaysInSightObjectIds();
		}

		
		private void ClearAlwaysInSightObjectIds()
		{
			WorldPlayerCharacter worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			foreach (int pObjectId in alwaysInsightObjectIds)
			{
				worldPlayerCharacter.RemoveAlwaysInSightObjectId(pObjectId);
			}

			alwaysInsightObjectIds.Clear();
		}
	}
}