using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillNormalAttackData : Singleton<RozziSkillNormalAttackData>
	{
		
		public readonly Dictionary<int, float> NormalAttackApCoef = new Dictionary<int, float>
		{
			{
				9,
				1f
			},
			{
				11,
				1f
			}
		};

		
		public readonly Dictionary<int, float> NormalAttackDelay = new Dictionary<int, float>
		{
			{
				9,
				0.16f
			},
			{
				11,
				0.23f
			}
		};

		
		public readonly int ProjectileCodeSniper = 100901;

		
		public bool pistolRightFlag;

		
		private readonly int ProjectileCodePistolLeft = 102102;

		
		private readonly int ProjectileCodePistolRight = 102101;

		
		public int GetPistolProjectileCode()
		{
			pistolRightFlag = !pistolRightFlag;
			if (!pistolRightFlag)
			{
				return ProjectileCodePistolLeft;
			}

			return ProjectileCodePistolRight;
		}

		
		public bool IsRightProjectileCodePistol(int projectileCode)
		{
			return projectileCode == ProjectileCodePistolRight;
		}
	}
}