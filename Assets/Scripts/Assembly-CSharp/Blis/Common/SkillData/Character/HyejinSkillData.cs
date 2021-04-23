using System.Collections.Generic;

namespace Blis.Common
{
	public class HyejinSkillData : Singleton<HyejinSkillData>
	{
		public readonly float A1ApDamage = 0.4f;


		public readonly Dictionary<int, int> A1BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				100
			},
			{
				2,
				125
			},
			{
				3,
				150
			},
			{
				4,
				175
			},
			{
				5,
				200
			}
		};


		public readonly int A1HitDebuff = 1012201;


		public readonly int A1ProjectileCode = 101201;


		public readonly float A2ApDamage = 0.5f;


		public readonly Dictionary<int, int> A2BaseMaxDamage = new Dictionary<int, int>
		{
			{
				1,
				140
			},
			{
				2,
				205
			},
			{
				3,
				270
			},
			{
				4,
				335
			},
			{
				5,
				400
			}
		};


		public readonly Dictionary<int, int> A2BaseMinDamage = new Dictionary<int, int>
		{
			{
				1,
				15
			},
			{
				2,
				20
			},
			{
				3,
				25
			},
			{
				4,
				30
			},
			{
				5,
				35
			}
		};


		public readonly int A2DamageEffectSound = 1012201;


		public readonly int A2DebuffCode = 1012301;


		public readonly float A2MinActiveTime = 0.5f;


		public readonly int A2SummonCodeBow = 1021;


		public readonly int A2SummonCodeShuriken = 1022;


		public readonly float A3_1ApDamage = 0.3f;


		public readonly Dictionary<int, int> A3_1BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				45
			},
			{
				2,
				70
			},
			{
				3,
				95
			},
			{
				4,
				120
			},
			{
				5,
				145
			}
		};


		public readonly float A3_2ApDamage = 0.5f;


		public readonly float A3_2ApRange = 2.5f;


		public readonly Dictionary<int, int> A3_2BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				75
			},
			{
				3,
				100
			},
			{
				4,
				125
			},
			{
				5,
				150
			}
		};


		public readonly int A3EndPositionDisplayProjectileCode = 101206;


		public readonly int A3ProjectileCode = 101204;


		public readonly int A4ConcentrationDebuffCode = 1012501;


		public readonly float A4ConcentrationEndApDamage = 0.7f;


		public readonly Dictionary<int, int> A4ConcentrationEndBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				150
			},
			{
				2,
				275
			},
			{
				3,
				400
			}
		};


		public readonly int A4ConcentrationEndDamageEffect = 1012401;


		public readonly float A4ProjectileApDamage = 0.5f;


		public readonly Dictionary<int, int> A4ProjectileBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				80
			},
			{
				2,
				130
			},
			{
				3,
				180
			}
		};


		public readonly int A4ProjectileCode = 101205;


		public readonly int A4ProjectileCount = 5;


		public readonly int A4ProjectileState = 1012511;


		public readonly float NormalAttackApCoef = 1f;


		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>
		{
			{
				7,
				0.23f
			},
			{
				6,
				0.29f
			}
		};


		public readonly Dictionary<int, int> NormalAttackEffectAndSoundWeaponType = new Dictionary<int, int>
		{
			{
				7,
				1010001
			},
			{
				6,
				1010001
			}
		};


		public readonly float PassiveFearMoveSpeedRatio = -30f;


		public readonly Dictionary<int, int> PassiveFearState = new Dictionary<int, int>
		{
			{
				1,
				1012111
			},
			{
				2,
				1012112
			},
			{
				3,
				1012113
			}
		};


		public readonly int PassiveSamjeaImmuneState = 1012121;

		public readonly int PassiveStackState = 1012101;


		public readonly int PassiveTriggerStack = 3;


		public readonly Dictionary<int, int> ProjectileCode = new Dictionary<int, int>
		{
			{
				7,
				100001
			},
			{
				6,
				100002
			}
		};
	}
}