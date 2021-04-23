namespace Blis.Common
{
	
	public class SkillSlotIndexComparer : SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
	{
		
		public override bool Equals(SkillSlotIndex x, SkillSlotIndex y)
		{
			return x == y;
		}

		
		public override int GetHashCode(SkillSlotIndex obj)
		{
			return (int) obj;
		}
	}
}