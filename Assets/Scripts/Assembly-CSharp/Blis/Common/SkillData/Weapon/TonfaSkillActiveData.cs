using System.Collections.Generic;

namespace Blis.Common
{
	
	public class TonfaSkillActiveData : Singleton<TonfaSkillActiveData>
	{
		
		public readonly float BlockAngle = 360f;

		
		public readonly DamageSubType[] blockDamageSubType =
		{
			DamageSubType.Normal,
			DamageSubType.Area,
			DamageSubType.Trap
		};

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSoundCode = 3002001;

		
		public readonly Dictionary<int, float> ReturnApCoef = new Dictionary<int, float>();

		
		public TonfaSkillActiveData()
		{
			BuffState.Add(1, 3002001);
			BuffState.Add(2, 3002002);
			ReturnApCoef.Add(1, -0.5f);
			ReturnApCoef.Add(2, -0.3f);
		}
	}
}