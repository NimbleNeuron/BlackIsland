using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class SummonObjectController : MonoBehaviour
	{
		
		public void Init(WorldMovableCharacter self)
		{
			this.self = self;
		}

		
		public void PushLifeLinkPool(WorldSummonTrap summonTrap)
		{
			this.Cleanup();
			this.waitingLifeLinkSummons.Add(summonTrap);
		}

		
		public WorldSummonTrap FindLifeLinkPairSummonObject(WorldSummonTrap summonTrap, int lifeLinkSummonDataCode, float minRange, float maxRange)
		{
			this.Cleanup();
			List<WorldSummonTrap> list = new List<WorldSummonTrap>();
			foreach (WorldSummonTrap worldSummonTrap in this.waitingLifeLinkSummons)
			{
				if (worldSummonTrap.IsAlive && worldSummonTrap.SummonData.code == lifeLinkSummonDataCode)
				{
					float num = GameUtil.DistanceOnPlane(worldSummonTrap.GetPosition(), summonTrap.GetPosition());
					if (num <= maxRange && num >= minRange)
					{
						list.Add(worldSummonTrap);
					}
				}
			}
			WorldSummonTrap worldSummonTrap2 = list.NearestOne(summonTrap.GetPosition());
			if (worldSummonTrap2 == null)
			{
				return null;
			}
			this.waitingLifeLinkSummons.Remove(worldSummonTrap2);
			return worldSummonTrap2;
		}

		
		private void Cleanup()
		{
			for (int i = this.waitingLifeLinkSummons.Count - 1; i >= 0; i--)
			{
				if (this.waitingLifeLinkSummons[i] == null)
				{
					this.waitingLifeLinkSummons.RemoveAt(i);
				}
				else if (!this.waitingLifeLinkSummons[i].IsAlive)
				{
					this.waitingLifeLinkSummons.RemoveAt(i);
				}
			}
		}

		
		private WorldMovableCharacter self;

		
		private List<WorldSummonTrap> waitingLifeLinkSummons = new List<WorldSummonTrap>();
	}
}
