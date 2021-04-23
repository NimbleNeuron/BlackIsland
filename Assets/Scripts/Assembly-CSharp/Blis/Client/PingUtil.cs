using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class PingUtil : SingletonMonoBehaviour<PingUtil>
	{
		private const float TIME_OUT = 5f;

		public void GetPingMs(string ipAddress, Action<int> callback)
		{
			if ((ipAddress == null || ipAddress.Equals("")) && callback != null)
			{
				callback(-1);
			}

			this.StartThrowingCoroutine(WaitPing(new Ping(ipAddress), callback),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][WaitPing] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void GetPingMs(MatchingRegion region, Action<int> callback)
		{
			string ip = GameDB.platform.GetIP(region);
			if (ip == null)
			{
				if (callback != null)
				{
					callback(-1);
				}

				return;
			}

			this.StartThrowingCoroutine(WaitPing(new Ping(ip), callback),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][WaitPing2] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator WaitPing(Ping ping, Action<int> callback)
		{
			float stratTime = Time.realtimeSinceStartup;
			while (!ping.isDone)
			{
				yield return null;
				if (Time.realtimeSinceStartup - stratTime > 5f)
				{
					if (callback != null)
					{
						callback(-1);
					}

					yield break;
				}
			}

			if (callback != null)
			{
				callback(ping.time);
			}
		}


		public void GetBestRegion(IEnumerable<MatchingRegion> regions, Action<MatchingRegion, int> callback)
		{
			this.StartThrowingCoroutine(WaitBestRegion(regions, callback),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][WaitBestRegion] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator WaitBestRegion(IEnumerable<MatchingRegion> regions, Action<MatchingRegion, int> callback)
		{
			Dictionary<MatchingRegion, int> results = new Dictionary<MatchingRegion, int>();
			List<Coroutine> list = new List<Coroutine>();
			foreach (MatchingRegion matchingRegion in regions)
			{
				string ip = GameDB.platform.GetIP(matchingRegion);
				if (ip != null)
				{
					MatchingRegion cached = matchingRegion;
					list.Add(this.StartThrowingCoroutine(
						WaitPing(new Ping(ip), delegate(int ms) { results[cached] = ms; }),
						delegate(Exception exception)
						{
							Log.E("[EXCEPTION][WaitPing] Message:" + exception.Message + ", StackTrace:" +
							      exception.StackTrace);
						}));
				}
			}

			foreach (Coroutine coroutine in list)
			{
				yield return coroutine;
			}

			MatchingRegion arg = MatchingRegion.None;
			int num = int.MaxValue;
			foreach (KeyValuePair<MatchingRegion, int> keyValuePair in results)
			{
				if (0 < keyValuePair.Value && keyValuePair.Value < num)
				{
					arg = keyValuePair.Key;
					num = keyValuePair.Value;
				}
			}

			if (callback != null)
			{
				callback(arg, num);
			}
		}
	}
}