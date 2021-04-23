using System.Collections.Generic;

namespace Blis.Common
{
	public class SisselaSkillData : Singleton<SisselaSkillData>
	{
		public readonly int A1MoveEffectCode = 1015101;


		public readonly float A1PassApDamage = 0.3f;


		public readonly Dictionary<int, int> A1PassBaseDamage = new Dictionary<int, int>
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


		public readonly float A1SeparateStartDistance = 0.5f;


		public readonly float A1StopApDamage = 0.5f;


		public readonly Dictionary<int, int> A1StopBaseDamage = new Dictionary<int, int>
		{
			{
				1,
				60
			},
			{
				2,
				90
			},
			{
				3,
				120
			},
			{
				4,
				150
			},
			{
				5,
				180
			}
		};


		public readonly float A1StopDamageDelay;


		public readonly int A1StopEffectCode = 1015102;


		public readonly float A1WilsonSpeed = 11.5f;


		public readonly float A2ApDamage = 0.7f;


		public readonly Dictionary<int, int> A2BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				30
			},
			{
				2,
				90
			},
			{
				3,
				150
			},
			{
				4,
				210
			},
			{
				5,
				270
			}
		};


		public readonly float A2DamageDelay = 0.2f;


		public readonly int A2EffectCode = 1015201;


		public readonly float A2KnockBackDistance = 2f;


		public readonly float A2KnockBackMoveDuration = 0.3f;


		public readonly int A2KnockBackStateCode = 1015311;


		public readonly int A2UntargetableStateCode = 1015301;


		public readonly float A3ApDamage = 0.6f;


		public readonly float A3ApShield = 0.5f;


		public readonly Dictionary<int, int> A3BaseDamage = new Dictionary<int, int>
		{
			{
				1,
				40
			},
			{
				2,
				90
			},
			{
				3,
				140
			},
			{
				4,
				190
			},
			{
				5,
				240
			}
		};


		public readonly Dictionary<int, int> A3BaseShield = new Dictionary<int, int>
		{
			{
				1,
				60
			},
			{
				2,
				110
			},
			{
				3,
				160
			},
			{
				4,
				210
			},
			{
				5,
				260
			}
		};


		public readonly int A3EffectSoundCode = 1015303;


		public readonly float A3GrabSpeed = 8f;


		public readonly int A3GrabState = 1015401;


		public readonly float A3KnockBackSpeed = 5f;


		public readonly int A3ProjectileCode = 101501;


		public readonly float A3ProjectileStartPointAdd = 1f;


		public readonly float A3SeparateStartDistance = 0.3f;


		public readonly float A3ShieldCooldownReduce = -1.5f;


		public readonly int A3ShieldState = 1015421;


		public readonly int A3StunState = 1015431;


		public readonly float A4ApDamage = 1f;


		public readonly Dictionary<int, int> A4BaseDamage = new Dictionary<int, int>
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


		public readonly float A4BuffIncreaseRate = 1f;


		public readonly int A4BuffStateCode = 1015501;


		public readonly float A4DamageMasteryModifier = 0.2f;


		public readonly int A4EnemyEffectSoundCode = 1015402;


		public readonly float A4FullDamageDistance = 10f;


		public readonly int A4LostHpRateDamage = 2;


		public readonly Dictionary<int, int> A4MarkStateCode = new Dictionary<int, int>
		{
			{
				1,
				1015511
			},
			{
				2,
				1015512
			},
			{
				3,
				1015513
			}
		};


		public readonly float A4OtherAreaDamageModify = -0.5f;


		public readonly float A4SameAreaDamageModify;


		public readonly int A4SelfDamageMinHp = 100;


		public readonly int A4SelfEffectSoundCode = 1015401;


		public readonly float NormalAttackApCoef = 1f;


		public readonly Dictionary<MasteryType, float> NormalAttackDelay =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.DirectFire,
					0.29f
				},
				{
					MasteryType.HighAngleFire,
					0.29f
				}
			};


		public readonly Dictionary<MasteryType, int> NormalAttackEffectAndSound =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.DirectFire,
					0
				},
				{
					MasteryType.HighAngleFire,
					0
				}
			};


		public readonly Dictionary<int, int> PassiveHpRegenBase = new Dictionary<int, int>
		{
			{
				1,
				2
			},
			{
				2,
				4
			},
			{
				3,
				6
			}
		};


		public readonly Dictionary<int, int> PassiveHpRegenPerLostHp = new Dictionary<int, int>
		{
			{
				1,
				3
			},
			{
				2,
				4
			},
			{
				3,
				5
			}
		};


		public readonly float PassiveNormalAttackApDamageNormalType = 1f;


		public readonly float PassiveNormalAttackApDamageSkillType = 0.2f;


		public readonly int PassiveNormalAttackBaseDamage = 18;


		public readonly int PassiveNormalAttackCharacterLvPerDamage = 10;


		public readonly Dictionary<int, int> PassiveNormalAttackDebuff = new Dictionary<int, int>
		{
			{
				1,
				1015141
			},
			{
				2,
				1015142
			},
			{
				3,
				1015143
			}
		};


		public readonly Dictionary<MasteryType, int> PassiveNormalAttackDebuffEffectAndSound =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.DirectFire,
					0
				},
				{
					MasteryType.HighAngleFire,
					0
				}
			};


		public readonly int PassiveNormalAttackMountSkillCode = 1015111;


		public readonly int PassiveNormalAttackMountState = 1015131;


		public readonly Dictionary<int, int> PassiveSkillIncreaseBase = new Dictionary<int, int>
		{
			{
				1,
				2
			},
			{
				2,
				5
			},
			{
				3,
				8
			}
		};


		public readonly Dictionary<int, float> PassiveSkillIncreasePerLostHp = new Dictionary<int, float>
		{
			{
				1,
				1f
			},
			{
				2,
				2.5f
			},
			{
				3,
				4f
			}
		};


		public readonly int PassiveSkillIncreaseStateCode = 1015121;


		public readonly Dictionary<MasteryType, int> ProjectileCode =
			new Dictionary<MasteryType, int>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.DirectFire,
					100002
				},
				{
					MasteryType.HighAngleFire,
					101502
				}
			};


		public readonly float WilsonDistanceColorGreenYellow = 5f;


		public readonly float WilsonDistanceColorYellowRed = 9f;


		public readonly float WilsonMaxDistance = 10.25f;


		public readonly float WilsonMinDistance = 1f;


		public readonly int WilsonSeparateState = 1015111;


		public readonly int WilsonSeparateStateGroup = 1015110;


		public readonly int WilsonSummonCode = 1030;


		public readonly float WilsonTongueBaseLength = 2f;


		public readonly int WilsonUnionState = 1015101;


		public readonly int WilsonUnionStateGroup = 1015100;

		public static int GetLostHpSection(int hp, int maxHp)
		{
			int num = hp * 100 / maxHp;
			if (num > 90)
			{
				return -1;
			}

			if (num > 80)
			{
				return 0;
			}

			if (num > 70)
			{
				return 1;
			}

			if (num > 60)
			{
				return 2;
			}

			if (num > 50)
			{
				return 3;
			}

			if (num > 40)
			{
				return 4;
			}

			if (num > 30)
			{
				return 5;
			}

			if (num > 20)
			{
				return 6;
			}

			if (num > 10)
			{
				return 7;
			}

			return 8;
		}
	}
}