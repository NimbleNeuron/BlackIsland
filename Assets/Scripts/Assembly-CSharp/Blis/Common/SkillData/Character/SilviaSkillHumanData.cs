using System.Collections.Generic;

namespace Blis.Common
{
	
	public class SilviaSkillHumanData : Singleton<SilviaSkillHumanData>
	{
		
		public readonly float A1ApDamage = 0.4f;

		
		public readonly float A1ApHeal = 0.5f;

		
		public readonly Dictionary<int, int> A1BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				30
			},
			{
				2,
				65
			},
			{
				3,
				100
			},
			{
				4,
				135
			},
			{
				5,
				170
			}
		};

		
		public readonly Dictionary<int, int> A1BaseHeal = new Dictionary<int, int>
		{
			{
				1,
				40
			},
			{
				2,
				60
			},
			{
				3,
				80
			},
			{
				4,
				100
			},
			{
				5,
				120
			}
		};

		
		public readonly float A1CooldownModifyMelee = -1f;

		
		public readonly float A1CooldownModifyRange = 0f;

		
		public readonly int A1DamageEffectSoundCode = 1016202;

		
		public readonly int A1EpGainPerHit = 5;

		
		public readonly int A1HealEffectSoundCode = 1016204;

		
		public readonly float A1HitRange = 7f;

		
		public readonly float A2ApDamage = 0.3f;

		
		public readonly Dictionary<int, int> A2BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				40
			},
			{
				2,
				60
			},
			{
				3,
				80
			},
			{
				4,
				100
			},
			{
				5,
				120
			}
		};

		
		public readonly Dictionary<int, int> A2BaseDebuffCodes = new Dictionary<int, int>
		{
			{
				1,
				1016301
			},
			{
				2,
				1016302
			},
			{
				3,
				1016303
			},
			{
				4,
				1016304
			},
			{
				5,
				1016305
			}
		};

		
		public readonly int A2DamageEffectSoundCode = 1016301;

		
		public readonly Dictionary<int, int> A2FinishLineProjectileCode = new Dictionary<int, int>
		{
			{
				1,
				101603
			},
			{
				2,
				101605
			},
			{
				3,
				101606
			},
			{
				4,
				101607
			},
			{
				5,
				101608
			}
		};

		
		public readonly float A2FinishLineSightRange = 4f;

		
		public readonly float A2NotLaunchProjectileAddTime = 1f;

		
		public readonly float A2SkillDepth = 0.3f;

		
		public readonly float A3ApDamage = 0.6f;

		
		public readonly Dictionary<int, int> A3BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				80
			},
			{
				2,
				100
			},
			{
				3,
				120
			},
			{
				4,
				140
			},
			{
				5,
				160
			}
		};

		
		public readonly int A3EpGainPerHit = 2;

		
		public readonly int A3FProjectileCode = 101601;

		
		public readonly float A3KnockbackDistance = 3f;

		
		public readonly float A3KnockbackDuration = 0.5f;

		
		public readonly float A3MeterPerDamageRate = 0.1f;

		
		public readonly float A3ProjectileIntersection = 3f;

		
		public readonly int A4CanUseConditionEp = 30;

		
		public readonly int FillEpAmount = 1;

		
		public readonly float FillEpPeriod = 3f;

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<MasteryType, float> NormalAttackDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Pistol,
					0.16f
				},
				{
					MasteryType.Glove,
					0.17f
				}
			};

		
		public readonly Dictionary<MasteryType, int> NormalAttackEffectAndSoundWeaponType =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Pistol,
					0
				},
				{
					MasteryType.Glove,
					0
				}
			};

		
		public readonly Dictionary<MasteryType, float> NormalAttackMountDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Pistol,
					0.16f
				},
				{
					MasteryType.Glove,
					0.17f
				}
			};

		
		public readonly Dictionary<int, int> NormalAttackMountSkillCode = new Dictionary<int, int>
		{
			{
				1,
				1016011
			},
			{
				2,
				1016012
			},
			{
				3,
				1016013
			}
		};

		
		public readonly Dictionary<int, float> NormalAttackReinforceApCoef = new Dictionary<int, float>
		{
			{
				1,
				1f
			},
			{
				2,
				1.5f
			},
			{
				3,
				2f
			}
		};

		
		public readonly int NormalAttackReinforceState = 1016001;

		
		public readonly Dictionary<MasteryType, int> ProjectileCode =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Pistol,
					101604
				}
			};
	}
}