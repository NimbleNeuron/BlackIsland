using System.Collections.Generic;

namespace Blis.Common
{
	public class ChiaraSkillData : Singleton<ChiaraSkillData>
	{
		public readonly float A1ApDamage_1 = 0.6f;


		public readonly float A1ApDamage_2 = 0.6f;


		public readonly Dictionary<int, int> A1BaseDamage_1 = new Dictionary<int, int>
		{
			{
				1,
				60
			},
			{
				2,
				100
			},
			{
				3,
				140
			},
			{
				4,
				180
			},
			{
				5,
				220
			}
		};


		public readonly Dictionary<int, int> A1BaseDamage_2 = new Dictionary<int, int>
		{
			{
				1,
				60
			},
			{
				2,
				100
			},
			{
				3,
				140
			},
			{
				4,
				180
			},
			{
				5,
				220
			}
		};


		public readonly float A1DebuffGroundProjectileRefreshPeriod = 0.1f;


		public readonly int A1EffectSoundCode_1 = 1014201;


		public readonly int A1EffectSoundCode_2 = 1014202;


		public readonly int A1GroundDebuffStateCode = 1014201;


		public readonly int A1GroundProjectileCode_1 = 101401;


		public readonly int A1GroundProjectileCode_2 = 101403;


		public readonly float A1GroundProjectileDuration_2 = 1.2f;


		public readonly float A1GroundProjectileWidth_2 = 1f;


		public readonly float A1Length_2 = 12f;


		public readonly float A1PlayAgainCastTime_1 = 0.33f;


		public readonly float A1PlayAgainCastTime_2 = 0.26f;


		public readonly int A1ProjectileCode_2 = 101402;


		public readonly float A1Range_2 = 12f;


		public readonly float A1SkillActiveCooldownModify_1 = -0.2f;


		public readonly float A1SkillChangeTime = 0.49f;


		public readonly float A1SkillChangeTimeClient = 0.42f;


		public readonly float A1TimeOverCooldownModify = -0.5f;


		public readonly float A1TimeOverRecoverySpRatio = 0.5f;


		public readonly float A1Width_1 = 1.5f;


		public readonly float A2ApDamage = 0.6f;


		public readonly float A2ApShield = 0.75f;


		public readonly Dictionary<int, int> A2BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				80
			},
			{
				2,
				120
			},
			{
				3,
				160
			},
			{
				4,
				200
			},
			{
				5,
				240
			}
		};


		public readonly Dictionary<int, int> A2BaseShield = new Dictionary<int, int>
		{
			{
				1,
				90
			},
			{
				2,
				125
			},
			{
				3,
				160
			},
			{
				4,
				195
			},
			{
				5,
				230
			}
		};


		public readonly int A2EffectSoundCode = 1014401;


		public readonly int A2ShieldState = 1014401;


		public readonly float A3CollisionApDamage = 0.3f;


		public readonly Dictionary<int, int> A3CollisionBaseDamage = new Dictionary<int, int>
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


		public readonly int A3CollisionEffectCode;


		public readonly float A3ConnectionCompleteApDamage = 0.7f;


		public readonly Dictionary<int, int> A3ConnectionCompleteBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				70
			},
			{
				2,
				110
			},
			{
				3,
				150
			},
			{
				4,
				190
			},
			{
				5,
				230
			}
		};


		public readonly int A3ConnectionCompleteEffectCode = 1014302;


		public readonly int A3FetterStateCode = 1014301;


		public readonly int A3HookLineCode = 101411;


		public readonly float A3KnockBackMoveDuration = 0.5f;


		public readonly int A3ProjectileCode = 101411;


		public readonly float A4JudgmentApDamage = 1.2f;


		public readonly Dictionary<int, int> A4JudgmentBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				200
			},
			{
				2,
				300
			},
			{
				3,
				400
			}
		};


		public readonly int A4JudgmentDamageEffectAndSound = 1014503;


		public readonly float A4JudgmentMoveDelayTime = 0.2f;


		public readonly float A4KillCooldownModify = -0.5f;


		public readonly float A4NormalAttackApCoef = 1f;


		public readonly float A4NormalAttackDelay = 0.29f;


		public readonly int A4NormalAttackProjectileCode = 101451;


		public readonly float A4SightShareRange = 0.75f;


		public readonly float A4SkillActiveCooldownModify;


		public readonly float A4TransformAroundApDamage = 0.15f;


		public readonly Dictionary<int, int> A4TransformAroundBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				20
			},
			{
				2,
				27
			},
			{
				3,
				34
			}
		};


		public readonly int A4TransformAroundDamageEffectAndSound = 1014502;


		public readonly float A4TransformAroundDamagePeriod = 1f;


		public readonly Dictionary<int, int> A4TransformAroundLifeStealRate = new Dictionary<int, int>
		{
			{
				1,
				20
			},
			{
				2,
				20
			},
			{
				3,
				20
			}
		};


		public readonly Dictionary<int, int> A4TransformMaxHpBonus = new Dictionary<int, int>
		{
			{
				1,
				100
			},
			{
				2,
				200
			},
			{
				3,
				300
			}
		};


		public readonly int A4TransformStateCode = 1014501;

		public readonly int ChiaraCharacterCode = 14;


		public readonly float NormalAttackApCoef = 1f;


		public readonly Dictionary<MasteryType, float> NormalAttackDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Rapier,
					0.19f
				},
				{
					MasteryType.OneHandSword,
					0.19f
				}
			};


		public readonly Dictionary<MasteryType, int> NormalAttackEffectAndSoundWeaponType =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Rapier,
					1014101
				},
				{
					MasteryType.OneHandSword,
					0
				}
			};


		public readonly Dictionary<int, int> PassiveBuffStateCode = new Dictionary<int, int>
		{
			{
				1,
				1014111
			},
			{
				2,
				1014112
			},
			{
				3,
				1014113
			}
		};


		public readonly Dictionary<int, int> PassiveDebuffStateCode = new Dictionary<int, int>
		{
			{
				1,
				1014101
			},
			{
				2,
				1014102
			},
			{
				3,
				1014103
			}
		};
	}
}