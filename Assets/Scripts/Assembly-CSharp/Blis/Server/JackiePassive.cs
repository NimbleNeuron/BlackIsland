using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackiePassive)]
	public class JackiePassive : SkillScript
	{
		
		private int lessThanLargeSizeMonsterKill;

		
		private int playerOrOverLargeSizeMonsterKill;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector instance = SingletonMonoBehaviour<BattleEventCollector>.Instance;
			instance.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(instance.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector instance = SingletonMonoBehaviour<BattleEventCollector>.Instance;
			instance.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(instance.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
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

			ObjectType objectType = victim.ObjectType;
			if (objectType != ObjectType.PlayerCharacter)
			{
				if (objectType == ObjectType.Monster)
				{
					victim.IfTypeOf<WorldMonster>(delegate(WorldMonster monster)
					{
						if (monster.MonsterData.grade < MonsterGrade.Epic)
						{
							lessThanLargeSizeMonsterKill++;
							return;
						}

						playerOrOverLargeSizeMonsterKill++;
					});
					goto IL_83;
				}

				if (objectType != ObjectType.BotPlayerCharacter)
				{
					goto IL_83;
				}
			}

			playerOrOverLargeSizeMonsterKill++;
			IL_83:
			if (Singleton<JackieSkillPassiveData>.inst.PlayerOrOverLargeSizeMonsterKillCondition <=
			    playerOrOverLargeSizeMonsterKill)
			{
				playerOrOverLargeSizeMonsterKill = 0;
				AddState(Caster, Singleton<JackieSkillPassiveData>.inst.BuffState[SkillLevel]);
				AddState(Caster, Singleton<JackieSkillPassiveData>.inst.BuffState_2[SkillLevel]);
			}

			if (Singleton<JackieSkillPassiveData>.inst.LessThanLargeSizeMonsterKillCondition <=
			    lessThanLargeSizeMonsterKill)
			{
				lessThanLargeSizeMonsterKill = 0;
				AddState(Caster, Singleton<JackieSkillPassiveData>.inst.BuffState_2[SkillLevel]);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}