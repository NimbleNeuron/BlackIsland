using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	
	public class ShoichiSkillActive4Data : Singleton<ShoichiSkillActive4Data>
	{
		
		public readonly Dictionary<int, Quaternion> DaggerAngles = new Dictionary<int, Quaternion>
		{
			{
				0,
				Quaternion.Euler(0f, 0f, 0f)
			},
			{
				1,
				Quaternion.Euler(0f, 270f, 0f)
			},
			{
				2,
				Quaternion.Euler(0f, 180f, 0f)
			},
			{
				3,
				Quaternion.Euler(0f, 90f, 0f)
			}
		};

		
		public readonly Dictionary<int, int> DaggerDamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				25
			},
			{
				2,
				60
			},
			{
				3,
				95
			}
		};

		
		public readonly int DaggerEffectAndSoundCode = 1018403;

		
		public readonly float DaggerMinRange = 0.1f;

		
		public readonly int DaggerProjectile = 101850;

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				50
			},
			{
				2,
				150
			},
			{
				3,
				250
			}
		};

		
		public readonly int DebuffStateCode = 1018501;

		
		public readonly int EffectAndSoundCode = 1018401;

		
		public readonly float LaunchDelay = 0.09f;

		
		public readonly float SkillApCoef = 0.3f;
	}
}