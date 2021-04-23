using System;
using System.Collections.Generic;

namespace Blis.Server
{
	
	public class ItemRoutingRecoder : Singleton<ItemRoutingRecoder>
	{
		
		public void OpenBox(long userId, int boxId)
		{
			if (this.itemRoutingLogDic.ContainsKey(userId))
			{
				this.itemRoutingLogDic[userId].OpenItemBoxAction(boxId);
				return;
			}
			this.itemRoutingLogDic.Add(userId, new ItemRoutingLogList(boxId, DateTime.Now, DateTime.MinValue, 0));
		}

		
		public void CloseBox(long userid, int boxId)
		{
			if (this.itemRoutingLogDic.ContainsKey(userid))
			{
				this.itemRoutingLogDic[userid].CloseBox(boxId);
				return;
			}
			this.itemRoutingLogDic.Add(userid, new ItemRoutingLogList(boxId, DateTime.MinValue, DateTime.Now, 0));
		}

		
		public void AddRoutingLog(long userid, int boxId, int itemId)
		{
			if (this.itemRoutingLogDic.ContainsKey(userid))
			{
				this.itemRoutingLogDic[userid].AddRoutingLog(boxId, itemId);
				return;
			}
			this.itemRoutingLogDic.Add(userid, new ItemRoutingLogList(boxId, DateTime.MinValue, DateTime.MinValue, itemId));
		}

		
		public Dictionary<long, ItemRoutingLogList> GetRoutingLog()
		{
			return this.itemRoutingLogDic;
		}

		
		public ItemRoutingLogList GetUserRoutingLog(long userId)
		{
			return this.itemRoutingLogDic[userId];
		}

		
		private Dictionary<long, ItemRoutingLogList> itemRoutingLogDic = new Dictionary<long, ItemRoutingLogList>();
	}
}
