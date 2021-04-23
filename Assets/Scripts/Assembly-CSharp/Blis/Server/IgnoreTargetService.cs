using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class IgnoreTargetService : ServiceBase
	{
		
		public void OnSwitchFlag(int requesterObjectId, List<int> targetObjectIds, IgnoreType ignoreType, bool isChangeFlag)
		{
			for (int i = 0; i < targetObjectIds.Count; i++)
			{
				if (!this.ignoreUsers.ContainsKey(requesterObjectId))
				{
					if (isChangeFlag)
					{
						this.ignoreUsers[requesterObjectId] = new Dictionary<IgnoreType, HashSet<int>>(SingletonComparerEnum<IgnoreTypeComparer, IgnoreType>.Instance)
						{
							{
								ignoreType,
								new HashSet<int>
								{
									targetObjectIds[i]
								}
							}
						};
					}
				}
				else if (!this.ignoreUsers[requesterObjectId].ContainsKey(ignoreType))
				{
					if (isChangeFlag)
					{
						this.ignoreUsers[requesterObjectId][ignoreType] = new HashSet<int>
						{
							targetObjectIds[i]
						};
					}
				}
				else if (isChangeFlag)
				{
					this.ignoreUsers[requesterObjectId][ignoreType].Add(targetObjectIds[i]);
				}
				else
				{
					this.ignoreUsers[requesterObjectId][ignoreType].Remove(targetObjectIds[i]);
				}
			}
		}

		
		public Dictionary<IgnoreType, HashSet<int>> GetIgnoreUsers(int objectId)
		{
			if (!this.ignoreUsers.ContainsKey(objectId))
			{
				return null;
			}
			return this.ignoreUsers[objectId];
		}

		
		private readonly Dictionary<int, Dictionary<IgnoreType, HashSet<int>>> ignoreUsers = new Dictionary<int, Dictionary<IgnoreType, HashSet<int>>>();
	}
}
