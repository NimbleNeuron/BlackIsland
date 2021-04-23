namespace Blis.Server
{
	
	public class SightRangeLink
	{
		
		public SightRangeLink(WorldObject link_1, WorldObject link_2)
		{
			if (link_1 == null || link_2 == null)
			{
				return;
			}
			this.link_1 = link_1;
			this.link_2 = link_2;
			link_1.AddSightRangeLink(this);
			link_2.AddSightRangeLink(this);
		}

		
		public void RemoveSightRangeLink()
		{
			if (this.link_1 != null)
			{
				this.link_1.RemoveSightRangeLink(this);
			}
			if (this.link_2 != null)
			{
				this.link_2.RemoveSightRangeLink(this);
			}
		}

		
		public WorldObject GetLinkedObject(WorldObject own)
		{
			if (this.link_1.ObjectId != own.ObjectId)
			{
				return this.link_1;
			}
			return this.link_2;
		}

		
		private readonly WorldObject link_1;

		
		private readonly WorldObject link_2;
	}
}
