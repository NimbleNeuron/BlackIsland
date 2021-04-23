using Blis.Common;

namespace Blis.Server
{
	
	public static class ServerUtil
	{
		
		public static float GetSkillRangeWithRadius(SkillData skillData, WorldCharacter caster, WorldCharacter target)
		{
			float num = skillData.UseWeaponRange ? caster.Stat.AttackRange : 0f;
			return target.Stat.Radius + skillData.range + num;
		}

		
		public static float GetSkillRange(SkillData skillData, WorldCharacter caster)
		{
			float num = skillData.UseWeaponRange ? caster.Stat.AttackRange : 0f;
			return skillData.range + num;
		}
	}
}
