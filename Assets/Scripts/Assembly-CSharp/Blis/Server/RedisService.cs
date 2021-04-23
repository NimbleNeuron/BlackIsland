using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using StackExchange.Redis;
using UnityEngine;

namespace Blis.Server
{
	
	public class RedisService : MonoBehaviourInstance<RedisService>
	{
		
		public void Init(string redisHost, string redisHostReadOnly, MatchingRegion region, string hostname, string host)
		{
			Log.V(string.Format("Begin: redisHost={0}, redisHostReadOnly={1}, region={2}, hostname={3}, host={4}", new object[]
			{
				redisHost,
				redisHostReadOnly,
				region,
				hostname,
				host
			}));
			this.SetHost(region, hostname, host);
			this._redisHost = redisHost;
			this._redisHostReadOnly = redisHostReadOnly;
			this.redisConnector = RedisService.BuildRedisConnector(this._redisHost, true);
			this.redisConnectorReadOnly = RedisService.BuildRedisConnector(this._redisHostReadOnly, false);
			this.redisConnector.ConnectIfNotConnected();
			this.redisConnectorReadOnly.ConnectIfNotConnected();
			Log.V(string.Format("End: redisHost={0}, redisHostReadOnly={1}, region={2}, hostname={3}, host={4}", new object[]
			{
				redisHost,
				redisHostReadOnly,
				region,
				hostname,
				host
			}));
		}

		
		private void SetHost(MatchingRegion region, string hostname, string host)
		{
			this.keyReadyBattleHost = string.Concat(new object[]
			{
				"ReadyBattleHost:",
				region,
				":",
				hostname,
				":",
				host
			});
			this.keyReadyBattleHostList = "ReadyBattleHostList:" + region;
			this.keyRunningBattleHost = string.Concat(new object[]
			{
				"RunningBattleHost:",
				region,
				":",
				hostname,
				":",
				host
			});
			this.host = host;
		}

		
		private static RedisConnector BuildRedisConnector(string redisHost, bool withSubscription)
		{
			return RedisConnector.Build(RedisConnector.BuildOptions(redisHost, withSubscription));
		}

		
		private ConnectionMultiplexer GetRedisConnection()
		{
			return this.redisConnector.GetConnection();
		}

		
		private ConnectionMultiplexer GetRedisConnectionReadOnly()
		{
			return this.redisConnectorReadOnly.GetConnection();
		}

		
		private IDatabase GetRedisDb()
		{
			return this.GetRedisConnection().GetDatabase(-1, null);
		}

		
		private IDatabase GetRedisDbReadOnly()
		{
			return this.GetRedisConnectionReadOnly().GetDatabase(-1, null);
		}

		
		public void OnServerReady()
		{
			Log.V("[RedisService] Server Ready");
			this.GetRedisDb().KeyDelete(this.keyRunningBattleHost, CommandFlags.None);
			this.coServerReady = base.StartCoroutine(this.PingAlive(this.keyReadyBattleHost, () => this.host, 5f, 3f));
			this.GetRedisConnection().GetSubscriber(null).Subscribe(this.keyReadyBattleHost, delegate(RedisChannel channel, RedisValue message)
			{
				this.reserveReceived = true;
			}, CommandFlags.None);
			this.GetRedisDb().ListRemove(this.keyReadyBattleHostList, this.keyReadyBattleHost, 0L, CommandFlags.None);
			Log.V("[RedisService] List Remove Key");
			this.GetRedisDb().ListLeftPush(this.keyReadyBattleHostList, this.keyReadyBattleHost, When.Always, CommandFlags.None);
			Log.V("[RedisService] List Left Push Key");
			base.StartCoroutine(this.WaitingReserved());
		}

		
		private void OnApplicationQuit()
		{
			this.Close();
		}

		
		private void Close()
		{
			Log.V("[RedisService] Finalize");
			if (this.coServerReady != null)
			{
				base.StopCoroutine(this.coServerReady);
				this.coServerReady = null;
			}
			if (this.coServerRunning != null)
			{
				base.StopCoroutine(this.coServerRunning);
				this.coServerRunning = null;
			}
			this.GetRedisDb().KeyDelete(this.keyReadyBattleHost, CommandFlags.None);
			this.GetRedisDb().KeyDelete(this.keyRunningBattleHost, CommandFlags.None);
		}

		
		private IEnumerator WaitingReserved()
		{
			while (!this.reserveReceived)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return base.StartCoroutine(this.ReserveHost());
			this.redisConnector.Close();
			this.redisConnector = RedisService.BuildRedisConnector(this._redisHost, false);
		}

		
		private IEnumerator PingAlive(string aliveKey, RedisService.DelAliveValue GetAliveValue, float expire, float interval)
		{
			do
			{
				try
				{
					string value = GetAliveValue();
					this.GetRedisDb().StringSet(aliveKey, value, new TimeSpan?(TimeSpan.FromSeconds((double)expire)), When.Always, CommandFlags.None);
				}
				catch (Exception e)
				{
					Log.E("Exception occurred StringSet(AliveKey) on PingAlive");
					Log.Exception(e);
				}
				Log.Gauge(1.0, "Alive");
				yield return new WaitForSeconds(interval);
			}
			while (this);
		}

		
		public IEnumerator ReserveHost()
		{
			if (this.hasReserved)
			{
				yield break;
			}
			this.hasReserved = true;
			this.reservedTime = Time.realtimeSinceStartup;
			Log.V("[RedisService] Recv request to reserve this host");
			if (this.coServerReady != null)
			{
				Log.V("[RedisService] StopCoroutine For Ready");
				base.StopCoroutine(this.coServerReady);
				this.coServerReady = null;
			}
			this.GetRedisDb().KeyDelete(this.keyReadyBattleHost, CommandFlags.None);
			this.coServerRunning = base.StartCoroutine(this.PingAlive(this.keyRunningBattleHost, () => this.runningUserCount.ToString(), 10f, 8f));
		}

		
		public bool IsReserved()
		{
			return this.hasReserved;
		}

		
		public float GetReservedTime()
		{
			return this.reservedTime;
		}

		
		public void GetBattleToken(string battleTokenKey, Action<BattleToken> callback)
		{
			string text = this.GetRedisDbReadOnly().StringGet("BattleToken:" + battleTokenKey, CommandFlags.None);
			if (string.IsNullOrEmpty(text))
			{
				throw new GameException(ErrorType.InvalidBattleToken, "[RedisService] GetBattleToken Failed. battleTokenKey: " + battleTokenKey);
			}
			BattleToken obj = Jsons.Deserialize<BattleToken>(text);
			callback(obj);
		}

		
		public void SetRunningUserCount(int count)
		{
			this.runningUserCount = count;
		}

		
		private RedisConnector redisConnector;

		
		private string _redisHost;

		
		private RedisConnector redisConnectorReadOnly;

		
		private string _redisHostReadOnly;

		
		private string host;

		
		private Coroutine coServerReady;

		
		private Coroutine coServerRunning;

		
		private string keyReadyBattleHost;

		
		private string keyReadyBattleHostList;

		
		private string keyRunningBattleHost;

		
		private volatile bool reserveReceived;

		
		private bool hasReserved;

		
		private float reservedTime;

		
		private int runningUserCount;

		
		private static class Prefix
		{
			
			public const string RunningBattleHost = "RunningBattleHost:";

			
			public const string BattleToken = "BattleToken:";

			
			public const string ReadyBattleHost = "ReadyBattleHost:";
		}

		
		private static class Key
		{
			
			public const string ReadyHostList = "ReadyBattleHostList";
		}

		
		private delegate string DelAliveValue();
	}
}
