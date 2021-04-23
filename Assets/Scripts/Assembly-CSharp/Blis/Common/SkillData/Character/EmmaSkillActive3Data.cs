using System.Collections.Generic;

namespace Blis.Common
{
	
	public class EmmaSkillActive3Data : Singleton<EmmaSkillActive3Data>
	{
		
		public readonly Dictionary<int, float> HealRatioPerConsumeSPBySkillLevel = new Dictionary<int, float>();

		
		public readonly int MagicRabbitBeamProjectileCode = 101941;

		
		public readonly int MagicRabbitFetterStateCode = 1019411;

		
		public readonly int MagicRabbitHealEffectAndSoundCode = 1019402;

		
		public readonly Dictionary<int, int> MagicRabbitStateCode = new Dictionary<int, int>();

		
		public readonly int MagicRabbitStateGroupCode = 1019400;

		
		public EmmaSkillActive3Data()
		{
			MagicRabbitStateCode.Add(1, 1019401);
			MagicRabbitStateCode.Add(2, 1019402);
			MagicRabbitStateCode.Add(3, 1019403);
			MagicRabbitStateCode.Add(4, 1019404);
			MagicRabbitStateCode.Add(5, 1019405);
			HealRatioPerConsumeSPBySkillLevel.Add(1, 0.12f);
			HealRatioPerConsumeSPBySkillLevel.Add(2, 0.14f);
			HealRatioPerConsumeSPBySkillLevel.Add(3, 0.16f);
			HealRatioPerConsumeSPBySkillLevel.Add(4, 0.18f);
			HealRatioPerConsumeSPBySkillLevel.Add(5, 0.2f);
		}
	}
}