using System.Collections.Generic;

namespace Blis.Common
{
	public class Area
	{
		public readonly int AreaCode;


		public readonly List<int> nearByAreaCodes;


		private AreaRestrictionState areaRestrictionState;


		public Area(int areaCode, List<int> nearByAreaCodes)
		{
			AreaCode = areaCode;
			areaRestrictionState = AreaRestrictionState.Normal;
			this.nearByAreaCodes = nearByAreaCodes;
		}


		public AreaRestrictionState AreaRestrictionState => areaRestrictionState;


		public void UpdateAreaState(AreaRestrictionState areaRestrictionState)
		{
			this.areaRestrictionState = areaRestrictionState;
		}


		public bool IsNearArea(int areaCode)
		{
			return nearByAreaCodes.Contains(areaCode);
		}
	}
}