namespace BIFog
{
	
	public struct EdgeInfo
	{
		
		public EdgeInfo(bool castedA, bool castedB, ViewCastInfo viewCastInfoA, ViewCastInfo viewCastInfoB)
		{
			this.castedA = castedA;
			this.castedB = castedB;
			this.viewCastInfoA = viewCastInfoA;
			this.viewCastInfoB = viewCastInfoB;
		}

		
		public bool castedA;

		
		public bool castedB;

		
		public ViewCastInfo viewCastInfoA;

		
		public ViewCastInfo viewCastInfoB;
	}
}