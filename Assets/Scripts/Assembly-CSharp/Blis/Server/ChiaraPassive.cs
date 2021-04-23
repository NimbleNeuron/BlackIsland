using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraPassive)]
	public class ChiaraPassive : SkillScript
	{
		
		private float passiveBuffAddedLastTime;

		
		private int passiveBuffStateGroupCode;

		
		private int passiveDebuffMaxStack;

		
		private int passiveDebuffStateGroupCode;

		
		protected override void Start()
		{
			base.Start();
			if (passiveDebuffStateGroupCode == 0)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[1]);
				passiveDebuffStateGroupCode = data.group;
				passiveDebuffMaxStack = data.maxStack;
				passiveBuffStateGroupCode = GameDB.characterState
					.GetData(Singleton<ChiaraSkillData>.inst.PassiveBuffStateCode[1]).group;
			}

			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.RemoveStateByGroup(passiveBuffStateGroupCode, Caster.ObjectId);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (!victim.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.Attacker != null && Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			if (damageInfo.DamageType != DamageType.Skill)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			AddState(victim.SkillAgent, Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[SkillLevel]);
			if (passiveBuffAddedLastTime == MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				return;
			}

			if (passiveDebuffMaxStack <=
			    victim.StateEffector.GetStackByGroup(passiveDebuffStateGroupCode, Caster.ObjectId))
			{
				passiveBuffAddedLastTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				AddState(Caster, Singleton<ChiaraSkillData>.inst.PassiveBuffStateCode[SkillLevel]);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}