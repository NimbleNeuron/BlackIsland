using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive4Attack)]
	public class JackieActive4Attack : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionCircle3D(Caster.Position, SkillRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillRange);
			List<SkillAgent> enemies = GetEnemyCharacters(sector);
			damage.Clear();
			damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<JackieSkillActive4Data>.inst.FinishSkillApCoef);
			damage.Add(SkillScriptParameterType.Damage,
				Singleton<JackieSkillActive4Data>.inst.FinishDamageByLevel[SkillLevel]);
			yield return WaitForSeconds(0.03f);
			DamageTo(enemies, DamageType.Skill, DamageSubType.Normal, 0, damage, 1001502);
			yield return WaitForSeconds(0.47f);
			Finish();
		}
	}
}