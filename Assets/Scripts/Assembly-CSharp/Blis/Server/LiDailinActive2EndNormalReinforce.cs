using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive2EndNormalReinforce)]
	public class LiDailinActive2EndNormalReinforce : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollectionEmpty =
			SkillScriptParameterCollection.Create();

		
		private readonly SkillScriptParameterCollection parameterCollectionReinforce =
			SkillScriptParameterCollection.Create();

		
		public override SkillScriptParameterCollection GetParameters(SkillAgent target, DamageType type,
			DamageSubType subType, int damageId)
		{
			if (type == DamageType.Normal && subType == DamageSubType.Normal)
			{
				return parameterCollectionReinforce;
			}

			return parameterCollectionEmpty;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			while (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <= StateDuration)
			{
				yield return WaitForFrame();
			}

			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			float value = 0f;
			CharacterState characterState = Caster.FindStateByGroup(StateGroup, Caster.ObjectId);
			if (characterState != null)
			{
				value = (characterState as ExtensionCommonState).ExtraValue;
			}

			parameterCollectionReinforce.Clear();
			parameterCollectionReinforce.Add(SkillScriptParameterType.FinalMoreDamage, value);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnHitNormalAttack));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnHitNormalAttack));
			Caster.RemoveStateByGroup(StateGroup, Caster.ObjectId);
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

			if (damageInfo.DamageId != 0)
			{
				return;
			}

			Finish();
		}
	}
}