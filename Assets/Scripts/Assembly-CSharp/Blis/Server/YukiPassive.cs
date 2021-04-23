using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiPassive)]
	public class YukiPassive : SkillScript
	{
		
		public enum YukiSpecialSkillId
		{
			
			None,

			
			Active4Bomb = 40,

			
			Active4BombPassive,

			
			Active4
		}

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float waitCountTime = 4f;

		
		private float waitTime;

		
		public override SkillScriptParameterCollection GetParameters(SkillAgent target, DamageType type,
			DamageSubType subType, int damageId)
		{
			parameterCollection.Clear();
			if (Caster.ObjectId != target.ObjectId && IsDamageAffectedPassive(type, subType, damageId))
			{
				parameterCollection.Add(SkillScriptParameterType.FinalAddDamage,
					Singleton<YukiSkillPassiveData>.inst.Damage[SkillLevel]);
			}

			return parameterCollection;
		}

		
		private bool IsDamageAffectedPassive(DamageType type, DamageSubType subType, int damageId)
		{
			return damageId == 41 || damageId != 40 && Caster.Status.ExtraPoint > 0 && subType != DamageSubType.Dot &&
				subType != DamageSubType.Area && subType != DamageSubType.Trap && subType != DamageSubType.Summon;
		}

		
		protected override void Start()
		{
			base.Start();
			waitCountTime = Singleton<YukiSkillPassiveData>.inst.RecoverySeconds;
			waitTime = 0f;
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			StartCoroutine(WaitForInCombat());
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		private IEnumerator WaitForInCombat()
		{
			for (;;)
			{
				if (Caster.Character.IsInCombat)
				{
					waitTime = 0f;
				}
				else if (waitTime >= waitCountTime)
				{
					waitTime = 0f;
					if (Caster.Status.ExtraPoint < Caster.Stat.MaxExtraPoint)
					{
						if (Caster.Status.ExtraPoint == 0)
						{
							PlaySkillAction(Caster, info.skillData.PassiveSkillId, 1);
						}

						Caster.ExtraPointModifyTo(Caster, 1);
					}
				}
				else
				{
					waitTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				}

				yield return WaitForFrame();
			}
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
			if (!Caster.IsAlive)
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

			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (Caster.Status.ExtraPoint <= 0)
			{
				return;
			}

			if (damageInfo.DamageSubType == DamageSubType.Dot || damageInfo.DamageSubType == DamageSubType.Area ||
			    damageInfo.DamageSubType == DamageSubType.Trap || damageInfo.DamageSubType == DamageSubType.Summon)
			{
				return;
			}

			if (damageInfo.DamageId == 40 || damageInfo.DamageId == 41 || damageInfo.DamageId == 42)
			{
				return;
			}

			Caster.ExtraPointModifyTo(Caster, -1);
			if (Caster.Status.ExtraPoint == 0)
			{
				PlaySkillAction(Caster, info.skillData.PassiveSkillId, 3);
			}
		}
	}
}