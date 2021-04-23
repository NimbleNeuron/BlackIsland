using Blis.Common;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.StaticItemBox)]
	public class WorldStaticItemBox : WorldItemBox
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.StaticItemBox;
		}

		
		public void Init(int itemSpawnPointCode, ItemBoxSize itemBoxSize)
		{
			base.Init(itemSpawnPointCode, itemBoxSize.GetBoxCapacity());
		}
	}
}
