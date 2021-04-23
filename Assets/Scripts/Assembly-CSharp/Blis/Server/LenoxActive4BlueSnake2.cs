using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive4BlueSnake2)]
	public class LenoxActive4BlueSnake2 : LenoxBlueSnake
	{
		
		protected override float GetMeterPerDamage()
		{
			return Singleton<LenoxSkillActive4Data>.inst.Active4UpgradeDamageByLevel[SkillLevel];
		}
	}
}