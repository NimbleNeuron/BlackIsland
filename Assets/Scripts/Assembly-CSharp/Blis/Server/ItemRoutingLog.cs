using System;
using System.Collections.Generic;

namespace Blis.Server
{
	
	public class ItemRoutingLog
	{
		
		public int boxId;

		
		public DateTime closeTime = DateTime.MinValue;

		
		public List<int> itemIds = new List<int>();

		
		public DateTime openTime = DateTime.MinValue;

		
		public float stayedTime;

		
		public ItemRoutingLog(int boxId, DateTime openTime, DateTime closeTime, int itemId)
		{
			this.boxId = boxId;
			this.openTime = openTime;
			CloseTime = closeTime;
			if (itemId != 0)
			{
				itemIds.Add(itemId);
			}
		}

		
		
		
		public DateTime CloseTime {
			get => closeTime;
			set
			{
				closeTime = value;
				if (value != DateTime.MinValue)
				{
					stayedTime = (float) (closeTime - openTime).TotalSeconds;
				}
			}
		}
	}
}