using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class AirSupplyService : ServiceBase
	{
		
		public void Init()
		{
			this.airSupplyCount = 0;
			this.scheduledAirSupplyList.Clear();
		}

		
		public void AirSupplyAnnounce(float announceTime)
		{
			announceTime -= 60f;
			float delay = (announceTime >= 0f) ? announceTime : 0f;
			base.StartCoroutine(CoroutineUtil.DelayedAction(delay, new Action(this.AirSupplyAnnounce)));
		}

		
		public void AirSupplyAnnounce()
		{
			List<Area> list = this.game.Area.GetAreasByState(AreaRestrictionState.Normal);
			list.Shuffle<Area>();
			list = list.GetRange(0, Mathf.RoundToInt((float)list.Count / 2f));
			List<AirSupplyInfo> list2 = new List<AirSupplyInfo>();
			foreach (Area area in list)
			{
				ItemSpawnPoint airSupplyPoint = this.game.Level.GetAirSupplyPoint(area.AreaCode);
				if (!(airSupplyPoint == null))
				{
					List<Item> list3 = GameDB.monster.SampleDropItem(new Func<int>(this.game.IncreaseAndGetItemId), this.GetAirSupplyDropGroupNo(), 1, true);
					if (list3.Count != 0)
					{
						ItemGrade itemGrade = (from item in list3
						select item.ItemData.itemGrade).Max<ItemGrade>();
						int objectId = this.game.World.IncrementAndGetObjectId();
						this.scheduledAirSupplyList.Add(new ScheduledAirSupply
						{
							objectId = objectId,
							itemSpawnPointCode = airSupplyPoint.code,
							position = airSupplyPoint.transform.position,
							rotation = airSupplyPoint.transform.rotation,
							highestIitemGrade = itemGrade,
							items = list3
						});
						list2.Add(new AirSupplyInfo
						{
							objectId = objectId,
							dropPosition = airSupplyPoint.transform.position,
							itemGrade = itemGrade
						});
					}
				}
			}
			this.airSupplyCount++;
			if (list2.Count > 0)
			{
				this.game.Announce.AirSupplyAnnounce();
				this.server.Broadcast(new RpcNoticeAirSupply
				{
					info = list2
				}, NetChannel.ReliableOrdered);
			}
		}

		
		public void AirSupply()
		{
			if (this.scheduledAirSupplyList.Count > 0)
			{
				List<SnapshotWrapper> spawnSnapshotList = new List<SnapshotWrapper>();
				this.scheduledAirSupplyList.ForEach(delegate(ScheduledAirSupply airSupply)
				{
					WorldAirSupplyItemBox worldAirSupplyItemBox = this.game.Spawn.SpawnAirSupplyItemBox(airSupply);
					spawnSnapshotList.Add(worldAirSupplyItemBox.CreateSnapshotWrapper());
				});
				this.server.Broadcast(new RpcSpawnAirSupply
				{
					spawnSnapshots = spawnSnapshotList
				}, NetChannel.ReliableOrdered);
				this.scheduledAirSupplyList.Clear();
			}
		}

		
		private int GetAirSupplyDropGroupNo()
		{
			return GameConstants.AIR_SUPPLY_DROP_GROUP + this.airSupplyCount;
		}

		
		private readonly List<ScheduledAirSupply> scheduledAirSupplyList = new List<ScheduledAirSupply>();

		
		private int airSupplyCount;
	}
}
