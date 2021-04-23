namespace Blis.Common
{
	
	public static class SkillCastWaysTypeExtensions
	{
		
		public static bool IsTargeting(this SkillCastWaysType castWaysType)
		{
			switch (castWaysType)
			{
				case SkillCastWaysType.Instant:
				case SkillCastWaysType.Directional:
				case SkillCastWaysType.PickPoint:
				case SkillCastWaysType.PickPointInArea:
				case SkillCastWaysType.PickPointThenDirection:
					return false;
				case SkillCastWaysType.PickTargetEdge:
				case SkillCastWaysType.PickTargetCenter:
					return true;
				default:
					return false;
			}
		}

		
		public static bool CanInputReleaseKey(this SkillCastWaysType skillCastWaysType)
		{
			return skillCastWaysType == SkillCastWaysType.PickPointThenDirection;
		}
	}
}