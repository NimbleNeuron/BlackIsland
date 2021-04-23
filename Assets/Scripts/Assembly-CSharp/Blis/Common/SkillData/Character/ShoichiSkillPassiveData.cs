using System.Collections.Generic;

namespace Blis.Common
{
	
	public class ShoichiSkillPassiveData : Singleton<ShoichiSkillPassiveData>
	{
		
		public readonly float Active2ChangeCoolTime = 0.09f;

		
		public readonly Dictionary<int, float> BuffMaxDamageByLevel = new Dictionary<int, float>
		{
			{
				1,
				0.1f
			},
			{
				2,
				0.15f
			},
			{
				3,
				0.2f
			}
		};

		
		public readonly int BuffMaxStackCount = 5;

		
		public readonly int BuffMaxStateGroup = 1018110;

		
		public readonly Dictionary<int, int> BuffMaxStateLevel = new Dictionary<int, int>
		{
			{
				1,
				1018111
			},
			{
				2,
				1018112
			},
			{
				3,
				1018113
			}
		};

		
		public readonly int BuffStateGroup = 1018100;

		
		public readonly Dictionary<int, int> BuffStateLevel = new Dictionary<int, int>
		{
			{
				1,
				1018101
			},
			{
				2,
				1018102
			},
			{
				3,
				1018103
			}
		};

		
		public readonly Dictionary<int, int> PassiveDaggerDamageByLevel = new Dictionary<int, int>
		{
			{
				1,
				10
			},
			{
				2,
				45
			},
			{
				3,
				80
			}
		};

		
		public readonly float PassiveDaggerLaunchDelay = 0.15f;

		
		public readonly float PassiveDaggerLaunchDelay2 = 0.15f;

		
		public readonly float PassiveDaggerMaxCount = 6f;

		
		public readonly int PassiveDaggerRemoveStateCode = 1018121;

		
		public readonly int PassiveDaggerRemoveStateGroup = 1018120;

		
		public readonly float PassiveDaggerSkillActiveRange = 1f;

		
		public readonly float PassiveDaggerSkillRange = 5f;

		
		public readonly int PassiveDaggerStateCode = 1018131;

		
		public readonly int PassiveDaggerStateGroup = 1018130;

		
		public readonly int PassiveProjectileId = 101802;

		
		public readonly int PassiveSummonObjectId = 1040;

		
		public readonly float SkillApCoef = 0.3f;
	}
}