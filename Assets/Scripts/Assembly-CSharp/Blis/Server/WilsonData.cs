using UnityEngine;

namespace Blis.Server
{
	
	internal class WilsonData : WorldSummonServant.WorldSummonServantData
	{
		

		
		public WilsonData(Transform originalWilsonParent)
		{
			OriginalWilsonParent = originalWilsonParent;
		}

		
		
		public Transform OriginalWilsonParent { get; }
	}
}