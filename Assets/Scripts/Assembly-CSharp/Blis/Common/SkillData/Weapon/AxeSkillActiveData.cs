using System.Collections.Generic;

namespace Blis.Common
{
	public class AxeSkillActiveData : Singleton<AxeSkillActiveData>
	{
		public readonly float BuffIncreaseValue = 3.5f;


		public readonly int BuffState = 3014001;


		public readonly Dictionary<int, float> Duration = new Dictionary<int, float>();


		public AxeSkillActiveData()
		{
			Duration.Add(1, 5f);
			Duration.Add(2, 7f);
		}
	}
}