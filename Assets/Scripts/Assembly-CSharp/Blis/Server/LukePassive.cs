using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukePassive)]
	public class LukePassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterAirSupplyItemBoxTakeEvent = (Action<WorldPlayerCharacter, WorldItemBox>) Delegate.Combine(
				inst.OnAfterAirSupplyItemBoxTakeEvent,
				new Action<WorldPlayerCharacter, WorldItemBox>(OnGetAirSupplyItemBoxEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterAssistEvent = (Action<WorldPlayerCharacter, int, List<int>>) Delegate.Combine(
				inst2.OnAfterAssistEvent, new Action<WorldPlayerCharacter, int, List<int>>(OnAssistEvent));
			BattleEventCollector inst3 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst3.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst3.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterAirSupplyItemBoxTakeEvent = (Action<WorldPlayerCharacter, WorldItemBox>) Delegate.Remove(
				inst.OnAfterAirSupplyItemBoxTakeEvent,
				new Action<WorldPlayerCharacter, WorldItemBox>(OnGetAirSupplyItemBoxEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterAssistEvent = (Action<WorldPlayerCharacter, int, List<int>>) Delegate.Remove(
				inst2.OnAfterAssistEvent, new Action<WorldPlayerCharacter, int, List<int>>(OnAssistEvent));
			BattleEventCollector instance = SingletonMonoBehaviour<BattleEventCollector>.Instance;
			instance.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(instance.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		private void OnAssistEvent(WorldCharacter victim, int finishingAttacker, List<int> assistants)
		{
			if (finishingAttacker == Caster.ObjectId)
			{
				return;
			}

			if (assistants.Contains(Caster.ObjectId))
			{
				ApplyPassive(victim);
			}
		}

		
		private void OnGetAirSupplyItemBoxEvent(WorldPlayerCharacter getter, WorldItemBox itemBox)
		{
			if (!getter.IsAlive)
			{
				return;
			}

			if (getter.ObjectId != Caster.ObjectId)
			{
				return;
			}

			AddStack(itemBox);
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

			ApplyPassive(victim);
		}

		
		private void ApplyPassive(WorldCharacter victim)
		{
			if (victim == Caster.Character)
			{
				return;
			}

			if (victim.ObjectType == ObjectType.BotPlayerCharacter || victim.ObjectType == ObjectType.PlayerCharacter ||
			    victim.ObjectType == ObjectType.Monster)
			{
				AddStack(victim);
			}

			bool flag = false;
			bool isWicklineKill = false;
			ObjectType objectType = victim.ObjectType;
			if (objectType != ObjectType.PlayerCharacter)
			{
				if (objectType != ObjectType.Monster)
				{
					if (objectType != ObjectType.BotPlayerCharacter)
					{
						goto IL_75;
					}
				}
				else
				{
					WorldMonster worldMonster = victim as WorldMonster;
					if (worldMonster != null && worldMonster.MonsterData.monster == MonsterType.Wickline)
					{
						flag = true;
						isWicklineKill = true;
					}

					goto IL_75;
				}
			}

			flag = true;
			IL_75:
			if (flag)
			{
				RecoveryLostHP(isWicklineKill);
			}
		}

		
		private void AddStack(WorldObject garbage)
		{
			int stack = 0;
			ObjectType objectType = garbage.ObjectType;
			if (objectType <= ObjectType.Monster)
			{
				if (objectType != ObjectType.PlayerCharacter)
				{
					if (objectType != ObjectType.Monster)
					{
						goto IL_82;
					}

					WorldMonster worldMonster = garbage as WorldMonster;
					if (worldMonster != null)
					{
						stack = Singleton<LukeSkillPassiveData>.inst.MonsterStack[worldMonster.MonsterData.monster];
					}

					goto IL_82;
				}
			}
			else if (objectType != ObjectType.AirSupplyItemBox)
			{
				if (objectType != ObjectType.BotPlayerCharacter)
				{
					goto IL_82;
				}
			}
			else
			{
				WorldAirSupplyItemBox worldAirSupplyItemBox = garbage as WorldAirSupplyItemBox;
				if (worldAirSupplyItemBox != null)
				{
					stack = Singleton<LukeSkillPassiveData>.inst.AirSurpplyStack[worldAirSupplyItemBox.ItemGrade];
				}

				goto IL_82;
			}

			stack = Singleton<LukeSkillPassiveData>.inst.PlayerKillStack;
			IL_82:
			AddState(Caster, Singleton<LukeSkillPassiveData>.inst.BuffState, stack);
		}

		
		private void RecoveryLostHP(bool isWicklineKill)
		{
			float num = isWicklineKill
				? Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioWicklineKill[SkillLevel]
				: Singleton<LukeSkillPassiveData>.inst.ReocveryLostHPRatioPlayerKill[SkillLevel];
			CharacterStateData data = GameDB.characterState.GetData(Singleton<LukeSkillPassiveData>.inst.BuffState);
			CharacterState characterState = Caster.FindStateByGroup(data.group, Caster.ObjectId);
			float num2 = Singleton<LukeSkillPassiveData>.inst.GetAddRecoveryHpAmount(characterState.StackCount) * 0.01f;
			LostHpHealTo(Caster, num + num2, 0, true,
				Singleton<LukeSkillPassiveData>.inst.RecoveryHpEffectAndSoundCode);
		}
	}
}