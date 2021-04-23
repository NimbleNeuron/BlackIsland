using System.Collections.Generic;

namespace Blis.Common
{
	
	public class SilviaSkillBikeData : Singleton<SilviaSkillBikeData>
	{
		
		public readonly float A1ApDamage = 0.7f;

		
		public readonly Dictionary<int, int> A1BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				60
			},
			{
				2,
				102
			},
			{
				3,
				144
			},
			{
				4,
				186
			},
			{
				5,
				228
			}
		};

		
		public readonly int A1EffectSoundCode = 1016601;

		
		public readonly float A2ApDamage = 0.6f;

		
		public readonly Dictionary<int, int> A2BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				90
			},
			{
				2,
				130
			},
			{
				3,
				170
			},
			{
				4,
				210
			},
			{
				5,
				250
			}
		};

		
		public readonly int A2DamageEffectSoundCode = 1016701;

		
		public readonly float A2SKillAirBoneDuration = 0.5f;

		
		public readonly float A2SkillBikeWheelDistance = 1f;

		
		public readonly float A2SkillDuration = 0.5f;

		
		public readonly float A3Active3MoveSpeed = 12f;

		
		public readonly int A3AfterStateCode = 1016811;

		
		public readonly float A3ApDamage = 0.6f;

		
		public readonly Dictionary<int, int> A3BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				45
			},
			{
				2,
				65
			},
			{
				3,
				90
			},
			{
				4,
				115
			},
			{
				5,
				140
			}
		};

		
		public readonly int A3CollisionEffectCode = 1016801;

		
		public readonly float A3CollisionRadius = 3f;

		
		public readonly int A3KnockBackDistance = 1;

		
		public readonly float A3knockBackDuration = 0.5f;

		
		public readonly Dictionary<int, int> A3SpeedCoefDamage = new Dictionary<int, int>
		{
			{
				1,
				6
			},
			{
				2,
				10
			},
			{
				3,
				14
			},
			{
				4,
				18
			},
			{
				5,
				22
			}
		};

		
		public readonly float A4ComboApDamage = 0.4f;

		
		public readonly Dictionary<int, int> A4ComboBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				30
			},
			{
				2,
				51
			},
			{
				3,
				72
			},
			{
				4,
				93
			},
			{
				5,
				114
			}
		};

		
		public readonly int A4ComboEffectSoundCode = 1016601;

		
		public readonly float BikeSpeedCalculateAnglePerDecrease = 0.5f;

		
		public readonly float BikeSpeedCalculateIgnoreAngle = 10f;

		
		public readonly float BikeSpeedCalculateIncreaseAmount = 1.8f;

		
		public readonly int BikeSpeedCalculateIncreasePriod = 4;

		
		public readonly float BikeSpeedCalculateMax = 70f;

		
		public readonly float BikeSpeedCalculateMin = -80f;

		
		public readonly float BikeSpeedCalculateRestoreAmount = 5f;

		
		public readonly int BikeSpeedCalculateStateCode = 1016061;

		
		public readonly int BikeSpeedCalculateStateGroup = 1016060;

		
		public readonly int BikeSpeedCalculateStopCCPeriod = 4;

		
		public readonly float BikeSpeedCalculateStopCCRestoreAmount = 5f;

		
		public readonly int BikeSpeedDownStateCode = 1016071;

		
		public readonly Dictionary<int, int> BikeSpeedUpState = new Dictionary<int, int>
		{
			{
				1,
				1016051
			},
			{
				2,
				1016052
			},
			{
				3,
				1016053
			}
		};

		
		public readonly int ConsumeEpAmount = 5;

		
		public readonly float ConsumeEpPeriod = 1f;

		
		public float A4ComboDamageDelayTime_1 = 0.5f;

		
		public int A4ComboProjectileCode = 101602;
	}
}