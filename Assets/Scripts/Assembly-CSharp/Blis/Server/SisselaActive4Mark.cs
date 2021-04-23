using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive4Mark)]
	public class SisselaActive4Mark : SisselaSkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (cancel)
			{
				return;
			}

			damageParam.Clear();
			damageParam.Add(SkillScriptParameterType.Damage, Singleton<SisselaSkillData>.inst.A4BaseDamage[SkillLevel]);
			damageParam.Add(SkillScriptParameterType.DamageApCoef, Singleton<SisselaSkillData>.inst.A4ApDamage);
			damageParam.Add(SkillScriptParameterType.DamageCasterLossHpCoef,
				Singleton<SisselaSkillData>.inst.A4LostHpRateDamage);
			float num = GameUtil.DistanceOnPlane(Target.Position, Caster.Position);
			if (Singleton<SisselaSkillData>.inst.A4FullDamageDistance < num)
			{
				int code = ((WorldMovableCharacter) Target.Character)
					.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel).code;
				int code2 = ((WorldMovableCharacter) Caster.Character)
					.GetCurrentAreaData(MonoBehaviourInstance<GameService>.inst.CurrentLevel).code;
				if (code == code2)
				{
					if (Singleton<SisselaSkillData>.inst.A4SameAreaDamageModify != 0f)
					{
						damageParam.Add(SkillScriptParameterType.FinalMoreDamage,
							Singleton<SisselaSkillData>.inst.A4SameAreaDamageModify);
					}
				}
				else if (Singleton<SisselaSkillData>.inst.A4OtherAreaDamageModify != 0f)
				{
					damageParam.Add(SkillScriptParameterType.FinalMoreDamage,
						Singleton<SisselaSkillData>.inst.A4OtherAreaDamageModify);
				}
			}

			if (Caster.ObjectId == Target.ObjectId)
			{
				DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<SisselaSkillData>.inst.A4SelfEffectSoundCode, false,
					Singleton<SisselaSkillData>.inst.A4SelfDamageMinHp,
					Singleton<SisselaSkillData>.inst.A4DamageMasteryModifier, false);
			}
			else
			{
				DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<SisselaSkillData>.inst.A4EnemyEffectSoundCode, true, 0,
					Singleton<SisselaSkillData>.inst.A4DamageMasteryModifier, false);
			}

			if (Caster.ObjectId == Target.ObjectId)
			{
				AddState(Caster, Singleton<SisselaSkillData>.inst.A4BuffStateCode);
				PlaySkillAction(Target, SkillId.SisselaActive4Mark, 1);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}
	}
}