using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive4BlueSnake)]
	public class LenoxActive4BlueSnake : LenoxBlueSnake
	{
		
		protected override float GetMeterPerDamage()
		{
			return Singleton<LenoxSkillActive4Data>.inst.Active4NormalDamageByLevel[SkillLevel];
		}
	}
}