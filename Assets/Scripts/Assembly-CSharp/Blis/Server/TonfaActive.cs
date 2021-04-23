using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.TonfaActive)]
	public class TonfaActive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			BlockingState blockingState =
				CreateState<BlockingState>(Caster, Singleton<TonfaSkillActiveData>.inst.BuffState[SkillLevel]);
			blockingState.Init(Singleton<TonfaSkillActiveData>.inst.BlockAngle, int.MaxValue,
				Singleton<TonfaSkillActiveData>.inst.blockDamageSubType);
			AddState(Caster, blockingState);
			CharacterStateData state =
				GameDB.characterState.GetData(Singleton<TonfaSkillActiveData>.inst.BuffState[SkillLevel]);
			while (Caster.IsHaveStateByGroup(state.group, Caster.ObjectId))
			{
				yield return WaitForFrame();
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBlockDamageEvent =
				(Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType>) Delegate.Combine(
					inst.OnBlockDamageEvent,
					new Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType>(
						OnBlockingDamageEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBlockDamageEvent =
				(Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType>) Delegate.Remove(
					inst.OnBlockDamageEvent,
					new Action<WorldCharacter, int, WorldCharacter, int, int, Vector3?, DamageSubType>(
						OnBlockingDamageEvent));
		}

		
		private void OnBlockingDamageEvent(WorldCharacter target, int casterId, WorldCharacter attacker,
			int undefendedDamage, int blockDamage, Vector3? damagePoint, DamageSubType damageSubType)
		{
			if (damageSubType == DamageSubType.Normal || damageSubType == DamageSubType.Area)
			{
				if (Vector3.Distance(Caster.Position, damagePoint.Value) > 10f)
				{
					return;
				}

				undefendedDamage = (int) (undefendedDamage +
				                          undefendedDamage *
				                          Singleton<TonfaSkillActiveData>.inst.ReturnApCoef[SkillLevel]);
				parameters.Clear();
				parameters.Add(SkillScriptParameterType.Damage, undefendedDamage);
				DirectDamageTo(attacker.SkillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameters,
					Singleton<TonfaSkillActiveData>.inst.EffectAndSoundCode);
			}
		}
	}
}