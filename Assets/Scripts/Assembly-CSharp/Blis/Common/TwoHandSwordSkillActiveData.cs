using System.Collections.Generic;

namespace Blis.Common
{
	
	public class TwoHandSwordSkillActiveData : Singleton<TwoHandSwordSkillActiveData>
	{
		
		public readonly int BlockCount = int.MaxValue;

		
		public readonly DamageSubType[] blockDamageSubType =
		{
			DamageSubType.Normal,
			DamageSubType.Area
		};

		
		public readonly float BlockingAngle = 360f;

		
		public readonly float BlockingDurtaion = 0.75f;

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> DamageApCoef = new Dictionary<int, float>();

		
		public readonly float DamageBoxDepth = 3f;

		
		public readonly float DamageBoxWidth = 1f;

		
		public readonly int DamageEffectAndSoundCode = 1000000;

		
		public readonly float DashDistance = 3f;

		
		public readonly float DashDuration = 0.15f;

		
		public TwoHandSwordSkillActiveData()
		{
			BuffState.Add(1, 3016001);
			BuffState.Add(2, 3016002);
			DamageApCoef.Add(1, 2f);
			DamageApCoef.Add(2, 2.5f);
		}
	}
}