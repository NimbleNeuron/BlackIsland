using System.Collections.Generic;

namespace Blis.Common
{
	
	public class MagnusSkillActive2Data : Singleton<MagnusSkillActive2Data>
	{
		
		public readonly Dictionary<int, int> AttackCountByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly int MoveSpeedDown = 1004301;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> SkillDefCoef = new Dictionary<int, float>();

		
		public MagnusSkillActive2Data()
		{
			AttackCountByLevel.Add(1, 6);
			AttackCountByLevel.Add(2, 6);
			AttackCountByLevel.Add(3, 7);
			AttackCountByLevel.Add(4, 7);
			AttackCountByLevel.Add(5, 8);
			SkillApCoef.Add(1, 0.3f);
			SkillApCoef.Add(2, 0.3f);
			SkillApCoef.Add(3, 0.3f);
			SkillApCoef.Add(4, 0.3f);
			SkillApCoef.Add(5, 0.3f);
			SkillDefCoef.Add(1, 0.3f);
			SkillDefCoef.Add(2, 0.3f);
			SkillDefCoef.Add(3, 0.3f);
			SkillDefCoef.Add(4, 0.3f);
			SkillDefCoef.Add(5, 0.3f);
			DamageByLevel.Add(1, 10);
			DamageByLevel.Add(2, 20);
			DamageByLevel.Add(3, 20);
			DamageByLevel.Add(4, 30);
			DamageByLevel.Add(5, 30);
			EffectAndSoundWeaponType.Add(0, 1004006);
			EffectAndSoundWeaponType.Add(3, 1004006);
			EffectAndSoundWeaponType.Add(13, 1004006);
		}
	}
}