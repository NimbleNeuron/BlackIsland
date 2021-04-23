using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillPassiveData : Singleton<EmmaSkillPassiveData>
	{
		
		public readonly Dictionary<int, float> CheerUpDamageMaxSpRatioByLevel = new Dictionary<int, float>();

		
		public readonly int CheerUpNormalAttackBuffStateCode = 1019111;

		
		public readonly int CheerUpNormalAttackBuffStateGroupCode = 1019110;

		
		public readonly Dictionary<int, float> CheerUpShieldAdditionalMaxSpRatio = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> CheerUpShieldByLevel = new Dictionary<int, int>();

		
		public readonly int CheerUpShieldStateCode = 1019101;

		
		public readonly int CheerUpShieldStateGroupCode = 1019100;

		
		public readonly float PassiveUpdateTime = 0.09f;

		
		public EmmaSkillPassiveData()
		{
			CheerUpDamageMaxSpRatioByLevel.Add(1, 0.03f);
			CheerUpDamageMaxSpRatioByLevel.Add(2, 0.035f);
			CheerUpDamageMaxSpRatioByLevel.Add(3, 0.04f);
			CheerUpShieldByLevel.Add(1, 100);
			CheerUpShieldByLevel.Add(2, 125);
			CheerUpShieldByLevel.Add(3, 150);
			CheerUpShieldAdditionalMaxSpRatio.Add(1, 0.03f);
			CheerUpShieldAdditionalMaxSpRatio.Add(2, 0.06f);
			CheerUpShieldAdditionalMaxSpRatio.Add(3, 0.09f);
		}
	}
}