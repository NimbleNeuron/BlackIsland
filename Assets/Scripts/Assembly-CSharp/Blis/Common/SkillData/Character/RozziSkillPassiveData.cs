using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillPassiveData : Singleton<RozziSkillPassiveData>
	{
		
		public readonly float ChocolateHpRatio = 0.3f;

		
		public readonly float ChocolateSpRatio = 0.5f;

		
		public readonly Dictionary<int, int> DoubleShotSkillCodeByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DoubleShotStateCodeByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> HpRecoveryStateCodeByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> SpRecoveryStateCodeByLevel = new Dictionary<int, int>();

		
		public RozziSkillPassiveData()
		{
			HpRecoveryStateCodeByLevel.Add(1, 1021111);
			HpRecoveryStateCodeByLevel.Add(2, 1021112);
			HpRecoveryStateCodeByLevel.Add(3, 1021113);
			SpRecoveryStateCodeByLevel.Add(1, 1021121);
			SpRecoveryStateCodeByLevel.Add(2, 1021122);
			SpRecoveryStateCodeByLevel.Add(3, 1021123);
			DoubleShotStateCodeByLevel.Add(1, 1021101);
			DoubleShotStateCodeByLevel.Add(2, 1021102);
			DoubleShotStateCodeByLevel.Add(3, 1021103);
			DoubleShotSkillCodeByLevel.Add(1, 1021101);
			DoubleShotSkillCodeByLevel.Add(2, 1021102);
			DoubleShotSkillCodeByLevel.Add(3, 1021103);
		}
	}
}