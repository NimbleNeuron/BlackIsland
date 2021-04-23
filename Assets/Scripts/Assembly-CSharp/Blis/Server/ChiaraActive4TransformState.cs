using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive4TransformState)]
	public class ChiaraActive4TransformState : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionCircle3D(Caster.Position, SkillRange);
			}

			Caster.SwitchSkillSet(SkillSlotIndex.Attack, SkillSlotSet.Attack_2);
			Caster.CancelNormalAttack();
			Caster.ReadyNormalAttack();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.SwitchSkillSet(SkillSlotIndex.Attack, SkillSlotSet.Attack_1);
			Caster.CancelNormalAttack();
			Caster.ReadyNormalAttack();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			int skillLv = Caster.GetSkillLevel(SkillSlotIndex.Active4);
			int baseDamage = Singleton<ChiaraSkillData>.inst.A4TransformAroundBaseDamage[skillLv];
			float timeStack = Singleton<ChiaraSkillData>.inst.A4TransformAroundDamagePeriod;
			for (;;)
			{
				if (timeStack >= Singleton<ChiaraSkillData>.inst.A4TransformAroundDamagePeriod)
				{
					timeStack -= Singleton<ChiaraSkillData>.inst.A4TransformAroundDamagePeriod;
					collision.UpdatePosition(Caster.Position);
					collision.UpdateRadius(SkillRange);
					List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
					float num = 0f;
					foreach (SkillAgent target in enemyCharacters)
					{
						damage.Clear();
						damage.Add(SkillScriptParameterType.Damage, baseDamage);
						damage.Add(SkillScriptParameterType.DamageApCoef,
							Singleton<ChiaraSkillData>.inst.A4TransformAroundApDamage);
						DamageInfo damageInfo = DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, damage,
							Singleton<ChiaraSkillData>.inst.A4TransformAroundDamageEffectAndSound);
						if (damageInfo != null)
						{
							num += damageInfo.Damage;
						}
					}

					if (num > 0f)
					{
						int fixAmount = (int) (num *
						                       Singleton<ChiaraSkillData>.inst.A4TransformAroundLifeStealRate[skillLv] *
						                       0.01f);
						HpHealTo(Caster, 0, 0f, fixAmount, true, 0);
					}
				}

				yield return WaitForFrame();
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			}
		}
	}
}