namespace Blis.Common
{
	
	public class EmotionslotTypeComparer : SingletonComparerEnum<EmotionslotTypeComparer, EmotionPlateType>
	{
		
		public override bool Equals(EmotionPlateType x, EmotionPlateType y)
		{
			return x == y;
		}

		
		public override int GetHashCode(EmotionPlateType obj)
		{
			return (int) obj;
		}
	}
}