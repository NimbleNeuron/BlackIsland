using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	
	public class LenoxSkillActive4Data : Singleton<LenoxSkillActive4Data>
	{
		
		public readonly float Active4AttackSightDuration = 1.5f;

		
		public readonly float Active4AttackSightRange = 2f;

		
		public readonly Dictionary<int, Quaternion> Active4ColisionBoxAngleY = new Dictionary<int, Quaternion>
		{
			{
				1,
				Quaternion.Euler(0f, 135f, 0f)
			},
			{
				2,
				Quaternion.Euler(0f, 225f, 0f)
			}
		};

		
		public readonly Dictionary<int, float> Active4NormalDamageByLevel = new Dictionary<int, float>
		{
			{
				1,
				10f
			},
			{
				2,
				15f
			},
			{
				3,
				20f
			}
		};

		
		public readonly Dictionary<int, int> Active4NormalDeBuffCode = new Dictionary<int, int>
		{
			{
				1,
				1020501
			},
			{
				2,
				1020502
			},
			{
				3,
				1020503
			}
		};

		
		public readonly int Active4NormalDeBuffGroup = 1020500;

		
		public readonly int Active4SkillPointDisplayProjectileCode = 102050;

		
		public readonly Dictionary<int, float> Active4UpgradeDamageByLevel = new Dictionary<int, float>
		{
			{
				1,
				20f
			},
			{
				2,
				30f
			},
			{
				3,
				40f
			}
		};

		
		public readonly Dictionary<int, int> Active4UpgradeDeBuffCode = new Dictionary<int, int>
		{
			{
				1,
				1020511
			},
			{
				2,
				1020512
			},
			{
				3,
				1020513
			}
		};

		
		public readonly int Active4UpgradeDeBuffGroup = 1020510;

		
		public readonly float BlueSnakeActiveTime = 0.25f;

		
		public readonly int BlueSnakeEffectAndSoundCode = 1020503;

		
		public readonly float BlueSnakeMaxDistance = 10f;

		
		public readonly float BlueSnakeMinDistance = 0.75f;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				100
			},
			{
				3,
				150
			}
		};

		
		public readonly int EffectAndSoundCodeFirst = 1020501;

		
		public readonly int EffectAndSoundCodeSecond = 1020502;

		
		public readonly float SkillApCoef = 0.8f;
	}
}