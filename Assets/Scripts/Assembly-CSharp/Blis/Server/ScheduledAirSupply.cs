using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class ScheduledAirSupply
	{
		
		public int objectId;

		
		public int itemSpawnPointCode;

		
		public Vector3 position;

		
		public Quaternion rotation;

		
		public List<Item> items;

		
		public ItemGrade highestIitemGrade;
	}
}
