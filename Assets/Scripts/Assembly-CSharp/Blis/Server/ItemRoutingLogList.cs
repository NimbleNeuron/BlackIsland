using System;
using System.Collections.Generic;

namespace Blis.Server
{
	
	public class ItemRoutingLogList
	{
		
		public List<ItemRoutingLog> logs = new List<ItemRoutingLog>();

		
		public ItemRoutingLogList(int boxId, DateTime openTime, DateTime closeTime, int itemId)
		{
			logs.Add(new ItemRoutingLog(boxId, openTime, closeTime, itemId));
		}

		
		public void OpenItemBoxAction(int boxId)
		{
			logs.Add(new ItemRoutingLog(boxId, DateTime.Now, DateTime.MinValue, 0));
		}

		
		public void CloseBox(int boxId)
		{
			if (logs.Count > 0 && logs[logs.Count - 1].boxId == boxId)
			{
				logs[logs.Count - 1].CloseTime = DateTime.Now;
				return;
			}

			logs.Add(new ItemRoutingLog(boxId, DateTime.MinValue, DateTime.Now, 0));
		}

		
		public void AddRoutingLog(int boxId, int itemId)
		{
			if (logs.Count > 0 && logs[logs.Count - 1].boxId == boxId)
			{
				logs[logs.Count - 1].itemIds.Add(itemId);
				return;
			}

			logs.Add(new ItemRoutingLog(boxId, DateTime.MinValue, DateTime.MinValue, itemId));
		}
	}
}