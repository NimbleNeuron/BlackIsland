using System.Collections.Generic;

namespace Blis.Common
{
	public class LiDailinSkillData : Singleton<LiDailinSkillData>
	{
		public readonly Dictionary<int, float> A1AfterDelayTime = new Dictionary<int, float>
		{
			{
				0,
				0.06f
			},
			{
				1,
				0.06f
			},
			{
				2,
				0.13f
			}
		};


		public readonly float A1ApDamage = 0.5f;


		public readonly float A1AttackEndTime = 0.25f;


		public readonly float A1AttackStartTime = 0.1f;


		public readonly Dictionary<int, int> A1BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				20
			},
			{
				2,
				40
			},
			{
				3,
				60
			},
			{
				4,
				80
			},
			{
				5,
				100
			}
		};


		public readonly float A1DashDistance = 2.25f;


		public readonly float A1DashDuration = 0.27f;


		public readonly int A1EffectCode = 1010002;


		public readonly int A1EffectCodeReinforce = 1010003;


		public readonly float A1ReinforceApDamage = 0.7f;


		public readonly Dictionary<int, int> A1ReinforceBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				28
			},
			{
				2,
				56
			},
			{
				3,
				84
			},
			{
				4,
				112
			},
			{
				5,
				140
			}
		};


		public readonly int A1ReinforceConsumeExtraPoint = -40;


		public readonly float A1ReinforceDashDistance = 2.5f;


		public readonly int A1ReinforceStateCode = 1010201;


		public readonly float A1StopRadiusModifier = 0.5f;


		public readonly float A1ThirdAttackTime = 0.2f;


		public readonly int A2ChargeAmount = 9;


		public readonly int A2ChargeCount = 5;


		public readonly float A2ChargeTerm = 0.16f;


		public readonly float A2CooldownReduce = 1f;


		public readonly int A2SkillEndBuff = 1010301;


		public readonly float A2SkillEndBuffEffectAmountPerAlcohol = 0.002f;


		public readonly int A3Angle = 60;


		public readonly float A3ApDamage = 0.5f;


		public readonly Dictionary<int, int> A3BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				80
			},
			{
				2,
				135
			},
			{
				3,
				190
			},
			{
				4,
				245
			},
			{
				5,
				300
			}
		};


		public readonly Dictionary<int, int> A3BaseDebuff = new Dictionary<int, int>
		{
			{
				1,
				1010401
			},
			{
				2,
				1010402
			},
			{
				3,
				1010403
			},
			{
				4,
				1010404
			},
			{
				5,
				1010405
			}
		};


		public readonly float A3Duration = 0.2f;


		public readonly int A3EffectCodeBase = 1010004;


		public readonly int A3EffectCodeReinforce = 1010005;


		public readonly float A3Radius = 4f;


		public readonly int A3ReinforceConsumeExtraPoint = -40;


		public readonly Dictionary<int, int> A3ReinforceDebuff = new Dictionary<int, int>
		{
			{
				1,
				1010411
			},
			{
				2,
				1010412
			},
			{
				3,
				1010413
			},
			{
				4,
				1010414
			},
			{
				5,
				1010415
			}
		};


		public readonly float A4ApDamage = 0.2f;


		public readonly Dictionary<int, int> A4DamageBase = new Dictionary<int, int>
		{
			{
				1,
				40
			},
			{
				2,
				70
			},
			{
				3,
				100
			}
		};


		public readonly float A4DamageIncreaseMax = 2f;


		public readonly float A4DamageIncreasePerTargetLossHp = 0.0267f;


		public readonly float A4DashDistance = 8f;


		public readonly float A4DashDuration = 0.5f;


		public readonly int A4EffectCodeBase = 1010006;


		public readonly int A4EffectCodeReinforce = 1010007;


		public readonly float A4HitCooldownModify = -0.4f;


		public readonly int A4HitCountBase = 2;


		public readonly int A4HitCountReinforce = 4;


		public readonly float A4HitDurationBase = 0.7f;


		public readonly float A4HitDurationReinforce = 1.2f;


		public readonly float A4HitTermBase = 0.63f;


		public readonly float A4HitTermReinforce = 0.27f;


		public readonly int A4ReinforceConsumeExtraPoint = -40;


		public readonly int A4TargetSuppressedDebuffBase = 1010501;


		public readonly int A4TargetSuppressedDebuffReinforce = 1010502;


		public readonly int A4UnstoppableStateCode = 1010511;


		public readonly float DecompositionPreventTime = 5f;


		public readonly float NormalAttackApCoef = 1f;


		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>
		{
			{
				1,
				0.17f
			},
			{
				2,
				0.23f
			},
			{
				20,
				0.19f
			}
		};


		public readonly Dictionary<int, int> NormalAttackEffectAndSoundWeaponType = new Dictionary<int, int>
		{
			{
				1,
				1010001
			},
			{
				2,
				1010001
			},
			{
				20,
				1010011
			}
		};


		public readonly Dictionary<int, int> PassiveAlcoholItemConsumeBuff = new Dictionary<int, int>
		{
			{
				1,
				1010141
			},
			{
				2,
				1010142
			},
			{
				3,
				1010143
			}
		};


		public readonly int PassiveDecompositionAmount = -5;


		public readonly float PassiveDecompositionTermTime = 1f;


		public readonly Dictionary<int, float> PassiveDoubleAttackAp = new Dictionary<int, float>
		{
			{
				1,
				0.5f
			},
			{
				2,
				0.75f
			},
			{
				3,
				1f
			}
		};


		public readonly Dictionary<int, float> PassiveDoubleAttackDelayFirst = new Dictionary<int, float>
		{
			{
				1,
				0.11f
			},
			{
				2,
				0.1f
			},
			{
				20,
				0.11f
			}
		};


		public readonly Dictionary<int, float> PassiveDoubleAttackDelaySecond = new Dictionary<int, float>
		{
			{
				1,
				0.34f
			},
			{
				2,
				0.31f
			},
			{
				20,
				0.38f
			}
		};


		public readonly int PassiveDoubleAttackDummyStateCode = 1010131;


		public readonly Dictionary<int, int> PassiveDoubleAttackEffectAndSoundWeaponType = new Dictionary<int, int>
		{
			{
				1,
				1010013
			},
			{
				2,
				1010001
			},
			{
				20,
				1010012
			}
		};


		public readonly int PassiveDoubleAttackSkillCode = 1010111;


		public readonly int PassiveDoubleAttackStateCode = 1010121;


		public readonly int PassiveDrunkennessDecompositionAmount = -20;


		public readonly int PassiveDrunkennessStateCode = 1010111;


		public readonly int SkillOverExtraPoint = 100;

		public readonly int SkillReinforceExtraPoint = 40;
	}
}