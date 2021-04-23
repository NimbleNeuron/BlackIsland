using Blis.Common;

namespace Blis.Client
{
	public static class NavigationEvent
	{
		public delegate void NavigationCloseEvent();


		public delegate void NavMaterialItemClickEvent(ItemData itemData);


		public delegate void NavWayPointItemClickEvent(ItemData root, ItemData itemData);
	}
}