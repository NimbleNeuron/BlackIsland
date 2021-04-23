using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillActive1Data : Singleton<FioraSkillActive1Data>
	{
		
		public readonly float CooldownReduce = -4f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly int EffectAndSound = 1003201;

		
		public readonly Dictionary<int, int> EffectAndSoundWeaponType = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> MarkingSlowState = new Dictionary<int, int>();

		
		public readonly float SkillApCoef = 0.25f;

		
		public FioraSkillActive1Data()
		{
			DamageByLevel.Add(1, 60);
			DamageByLevel.Add(2, 120);
			DamageByLevel.Add(3, 180);
			DamageByLevel.Add(4, 240);
			DamageByLevel.Add(5, 300);
			EffectAndSoundWeaponType.Add(0, 1003201);
			EffectAndSoundWeaponType.Add(21, 1003201);
			EffectAndSoundWeaponType.Add(16, 1003201);
			EffectAndSoundWeaponType.Add(19, 1003201);
			DebuffState.Add(1, 1003401);
			DebuffState.Add(2, 1003402);
			DebuffState.Add(3, 1003403);
			DebuffState.Add(4, 1003404);
			DebuffState.Add(5, 1003405);
			MarkingSlowState.Add(1, 1003411);
			MarkingSlowState.Add(2, 1003412);
			MarkingSlowState.Add(3, 1003413);
			MarkingSlowState.Add(4, 1003414);
			MarkingSlowState.Add(5, 1003415);
		}
	}
}