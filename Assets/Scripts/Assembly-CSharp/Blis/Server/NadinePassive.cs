using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadinePassive)]
	public class NadinePassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
			Caster.Character.SightAgent.UpdateTargetTypeInSightRange(typeof(WorldMonster),
				Singleton<NadineSkillPassiveData>.inst.PassiveSightRange);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector instance = SingletonMonoBehaviour<BattleEventCollector>.Instance;
			instance.OnKillEvent = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(instance.OnKillEvent,
				new Action<WorldCharacter, DamageInfo>(OnKillEvent));
			Caster.Character.SightAgent.RemoveTargetTypeInSightRange(typeof(WorldMonster));
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

			if (victim.ObjectType != ObjectType.Monster)
			{
				return;
			}

			int stack = 1;
			switch ((victim as WorldMonster).MonsterData.monster)
			{
				case MonsterType.Chicken:
					stack = Singleton<NadineSkillPassiveData>.inst.StackCountOnKillCommon[SkillLevel];
					break;
				case MonsterType.Bat:
				case MonsterType.Boar:
					stack = Singleton<NadineSkillPassiveData>.inst.StackCountOnKillUncommon[SkillLevel];
					break;
				case MonsterType.WildDog:
				case MonsterType.Wolf:
					stack = Singleton<NadineSkillPassiveData>.inst.StackCountOnKillRare[SkillLevel];
					break;
				case MonsterType.Bear:
					stack = Singleton<NadineSkillPassiveData>.inst.StackCountOnKillEpic[SkillLevel];
					break;
			}

			AddState(Caster, Singleton<NadineSkillPassiveData>.inst.BuffState, stack);
		}
	}
}