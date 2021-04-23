using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeActive1)]
	public class SilviaBikeActive1 : SkillScript
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
		}
		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			collision.UpdatePosition(Caster.Position);
			collision.UpdateRadius(SkillRange);
			damage.Clear();
			damage.Add(SkillScriptParameterType.Damage, Singleton<SilviaSkillBikeData>.inst.A1BaseDamage[SkillLevel]);
			damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<SilviaSkillBikeData>.inst.A1ApDamage);
			List<SkillAgent> enemies = GetEnemies(collision);
			DamageTo(enemies, DamageType.Skill, DamageSubType.Normal, 0, damage,
				Singleton<SilviaSkillBikeData>.inst.A1EffectSoundCode);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}