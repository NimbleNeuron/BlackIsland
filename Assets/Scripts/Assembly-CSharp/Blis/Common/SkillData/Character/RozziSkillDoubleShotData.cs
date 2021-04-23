using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RozziSkillDoubleShotData : Singleton<RozziSkillDoubleShotData>
	{
		
		public readonly Dictionary<int, float> DoubleShotPistolApCoefByLevel_1 = new Dictionary<int, float>
		{
			{
				1,
				0.6f
			},
			{
				2,
				0.6f
			},
			{
				3,
				0.6f
			}
		};

		
		public readonly Dictionary<int, float> DoubleShotPistolApCoefByLevel_2 = new Dictionary<int, float>
		{
			{
				1,
				0.5f
			},
			{
				2,
				0.6f
			},
			{
				3,
				0.7f
			}
		};

		
		public readonly List<float> DoubleShotPistolDelay = new List<float>
		{
			0.12f,
			0.18f
		};

		
		public readonly Dictionary<int, float> DoubleShotSniperApCoefByLevel = new Dictionary<int, float>
		{
			{
				1,
				1.2f
			},
			{
				2,
				1.3f
			},
			{
				3,
				1.4f
			}
		};

		
		public readonly float DoubleShotSniperDelay = 0.27f;

		
		public readonly Dictionary<MasteryType, float> FinishDelayTime =
			new Dictionary<MasteryType, float>(SingletonComparerEnum<MasteryTypeComparer, MasteryType>.Instance)
			{
				{
					MasteryType.Pistol,
					0f
				},
				{
					MasteryType.SniperRifle,
					0f
				}
			};

		
		public readonly int ProjectileCodePistolDoubleShotLeft = 102104;

		
		public readonly int ProjectileCodePistolDoubleShotRight = 102103;

		
		public readonly int ProjectileCodeSniper = 100901;
	}
}