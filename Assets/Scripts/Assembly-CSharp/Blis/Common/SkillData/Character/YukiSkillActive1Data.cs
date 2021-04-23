using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillActive1Data : Singleton<YukiSkillActive1Data>
	{
		
		public readonly Dictionary<int, int> AttackDamage = new Dictionary<int, int>();

		
		public readonly int BuffState = 1011201;

		
		public readonly int DebuffState = 1011211;

		
		public readonly float DualSworldNormalAttackApCoef = 2f;

		
		public readonly int EffectAndSoundWeaponType = 1011201;

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly float NormalAttackDelay = 0.23f;

		
		public readonly int PassiveEffectAndSoundWeaponType = 1011202;

		
		public readonly float SlowDuration = 1f;

		
		public readonly float SlowMoveSpeedRatio = -50f;

		
		public readonly float StunDuration = 0.5f;

		
		public readonly Dictionary<int, int> YukiActive1Attack = new Dictionary<int, int>();

		
		public YukiSkillActive1Data()
		{
			YukiActive1Attack.Add(1, 1011211);
			YukiActive1Attack.Add(2, 1011212);
			YukiActive1Attack.Add(3, 1011213);
			YukiActive1Attack.Add(4, 1011214);
			YukiActive1Attack.Add(5, 1011215);
			AttackDamage.Add(1, 20);
			AttackDamage.Add(2, 45);
			AttackDamage.Add(3, 70);
			AttackDamage.Add(4, 95);
			AttackDamage.Add(5, 120);
		}
	}
}