using Blis.Common;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SummonCamera)]
	public class WorldSummonCamera : WorldSummonBase
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonCamera;
		}

		
		protected override int GetCharacterCode()
		{
			SummonData summonData = base.SummonData;
			if (summonData == null)
			{
				return 0;
			}
			return summonData.code;
		}
	}
}
