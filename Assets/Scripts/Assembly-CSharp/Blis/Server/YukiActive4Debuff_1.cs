using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive4Debuff_1)]
	public class YukiActive4Debuff_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Caster.AttachSight(Target.WorldObject, 1f, Singleton<YukiSkillActive4Data>.inst.WaitTime + 1f, true);
			yield return WaitForSeconds(Singleton<YukiSkillActive4Data>.inst.WaitTime);
			int num = (int) (Target.Stat.MaxHp * Singleton<YukiSkillActive4Data>.inst.HpRateByLevel[SkillLevel]);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.Damage, num);
			DirectDamageTo(Target, DamageType.Skill, DamageSubType.Normal, 40, parameterCollection,
				Singleton<YukiSkillActive4Data>.inst.EffectAndSoundSignal, true, 0, 1f, false);
			Finish();
		}
	}
}