using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillActive4Data : Singleton<YukiSkillActive4Data>
	{
		
		public readonly float ConcentrationTime = 1f;

		
		public readonly float DebuffMoveSpeedRatio = -70f;

		
		public readonly Dictionary<int, int> DebuffState_1 = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffState_2 = new Dictionary<int, int>();

		
		public readonly int EffectAndSound = 1011501;

		
		public readonly int EffectAndSoundPassiveSignal = 1011504;

		
		public readonly int EffectAndSoundSignal = 1011503;

		
		public readonly float FinalMotionTime = 1.29f;

		
		public readonly Dictionary<int, float> HpRateByLevel = new Dictionary<int, float>();

		
		public readonly int PassiveEffectAndSound = 1011502;

		
		public readonly float SkillApCoef = 1.5f;

		
		public readonly float SkillApCoefPassiveSignal = 1.5f;

		
		public readonly Dictionary<int, int> SkillDamage = new Dictionary<int, int>();

		
		public readonly float WaitTime = 1f;

		
		public YukiSkillActive4Data()
		{
			SkillDamage.Add(1, 250);
			SkillDamage.Add(2, 375);
			SkillDamage.Add(3, 500);
			DebuffState_1.Add(1, 1011501);
			DebuffState_1.Add(2, 1011502);
			DebuffState_1.Add(3, 1011503);
			DebuffState_2.Add(1, 1011511);
			DebuffState_2.Add(2, 1011512);
			DebuffState_2.Add(3, 1011513);
			HpRateByLevel.Add(1, 0.15f);
			HpRateByLevel.Add(2, 0.2f);
			HpRateByLevel.Add(3, 0.25f);
		}
	}
}