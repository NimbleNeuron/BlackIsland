using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class IgnoreTargetService
	{
		private Dictionary<IgnoreType, HashSet<int>> ignoreUsers =
			new Dictionary<IgnoreType, HashSet<int>>(SingletonComparerEnum<IgnoreTypeComparer, IgnoreType>.Instance);

		public bool IsIgnoreUser(IgnoreType ignoreType, int targetObjectId)
		{
			return ignoreUsers.ContainsKey(ignoreType) && ignoreUsers[ignoreType].Contains(targetObjectId);
		}


		private void OnSwitchFlag(IgnoreType ignoreType, int objectId, bool isChangeFlag)
		{
			if (!ignoreUsers.ContainsKey(ignoreType))
			{
				ignoreUsers[ignoreType] = new HashSet<int>();
			}

			if (isChangeFlag)
			{
				ignoreUsers[ignoreType].Add(objectId);
				return;
			}

			ignoreUsers[ignoreType].Remove(objectId);
		}


		public void SendIgnoreRequest(IgnoreType ignoreType, bool isIgnore, List<int> objectIds,
			Action<bool> callback = null)
		{
			if (objectIds.Count <= 0)
			{
				return;
			}

			MonoBehaviourInstance<GameClient>.inst.Request<ReqIgnorePlayer, ResIgnorePlayer>(new ReqIgnorePlayer
			{
				targetObjectIds = objectIds,
				ignoreType = ignoreType,
				isChangeFlag = isIgnore
			}, delegate(ResIgnorePlayer res)
			{
				for (int i = 0; i < objectIds.Count; i++)
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.OnSwitchFlag(ignoreType, objectIds[i],
						isIgnore);
				}

				Action<bool> callback2 = callback;
				if (callback2 != null)
				{
					callback2(res.isIgnore);
				}

				if (ignoreType == IgnoreType.Ping)
				{
					SingletonMonoBehaviour<PlayerController>.inst.PingTarget.UpdatePing();
				}
			});
		}


		public void SetSnapShotIgnoreUsers(Dictionary<IgnoreType, HashSet<int>> snapshotIgnoreUsers)
		{
			if (snapshotIgnoreUsers != null)
			{
				ignoreUsers = snapshotIgnoreUsers;
				SingletonMonoBehaviour<PlayerController>.inst.PingTarget.UpdatePing();
			}
		}
	}
}