using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineKillBuff)]
	public class WicklineKillBuff : SkillScript
	{
		
		private float cooltimeDuration;

		
		private float lastActiveDfsTime;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			cooltimeDuration = GameDB.characterState
				.GetData(Singleton<WicklineKillSkillBuffData>.inst.DFS_EffectCooltimeState).duration;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (victim == null || !victim.IsAlive)
			{
				return;
			}

			if (victim.ObjectId == Target.Character.ObjectId)
			{
				return;
			}

			if (damageInfo == null)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			if (damageInfo.Attacker == null || damageInfo.Attacker.ObjectId != Target.Character.ObjectId)
			{
				return;
			}

			if (lastActiveDfsTime != MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
			{
				if (lastActiveDfsTime + cooltimeDuration >
				    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime)
				{
					return;
				}

				lastActiveDfsTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				AddState(damageInfo.Attacker.SkillAgent,
					Singleton<WicklineKillSkillBuffData>.inst.DFS_EffectCooltimeState);
			}

			AddState(victim.SkillAgent,
				Singleton<WicklineKillSkillBuffData>.inst.DFS_Code[GameDB.characterState.GetData(StateCode).level]);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}