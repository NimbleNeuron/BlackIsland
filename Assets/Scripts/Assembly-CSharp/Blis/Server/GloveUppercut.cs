using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.GloveUppercut)]
	public class GloveUppercut : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private bool isHit;

		
		public override SkillScriptParameterCollection GetParameters(SkillAgent target, DamageType type,
			DamageSubType subType, int damageId)
		{
			parameterCollection.Clear();
			if (type == DamageType.Normal && subType == DamageSubType.Normal && damageId == 1)
			{
				parameterCollection.Add(SkillScriptParameterType.FinalMoreDamage,
					Singleton<GloveSkillActiveData>.inst.UppercutFinalMoreDamage[SkillLevel]);
			}

			return parameterCollection;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			while (!isHit && MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <=
				StateDuration)
			{
				yield return WaitForFrame();
			}

			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			isHit = false;
			Caster.MountNormalAttack(Singleton<GloveSkillActiveData>.inst.GloveUppercutAttackSkillCode[SkillLevel]);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnHitNormalAttack));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnHitNormalAttack));
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<GloveSkillActiveData>.inst.BuffState[SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
		}

		
		private void OnHitNormalAttack(WorldCharacter victim, DamageInfo damageInfo)
		{
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

			isHit = true;
		}
	}
}