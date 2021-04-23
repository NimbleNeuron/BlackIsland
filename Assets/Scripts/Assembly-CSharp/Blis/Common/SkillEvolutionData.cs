using Newtonsoft.Json;

namespace Blis.Common
{
	public class SkillEvolutionData
	{
		public readonly int code;


		public readonly string Icon;


		public readonly int maxEvolutionLevel;


		public readonly int skillGroup;

		[JsonConstructor]
		public SkillEvolutionData(int code, int skillGroup, int maxEvolutionLevel, string icon)
		{
			this.code = code;
			this.skillGroup = skillGroup;
			this.maxEvolutionLevel = maxEvolutionLevel;
			Icon = icon;
		}
	}
}