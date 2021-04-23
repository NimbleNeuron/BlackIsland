using System.Collections.Generic;
using Blis.Common;
using StackExchange.Redis;

namespace Blis.Server
{
	
	public class RedisConnector
	{
		
		public static RedisConnector Build(ConfigurationOptions options)
		{
			return new RedisConnector(options);
		}

		
		public static ConfigurationOptions BuildOptions(string redisHost, bool withSubscription)
		{
			ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(redisHost);
			configurationOptions.ConnectTimeout = 20000;
			configurationOptions.SyncTimeout = 20000;
			configurationOptions.ResponseTimeout = 20000;
			if (!withSubscription)
			{
				configurationOptions.CommandMap = CommandMap.Create(new Dictionary<string, string>
				{
					{
						"SUBSCRIBE",
						null
					}
				});
			}
			configurationOptions.AbortOnConnectFail = false;
			return configurationOptions;
		}

		
		private RedisConnector(ConfigurationOptions options)
		{
			this._options = options;
		}

		
		public ConnectionMultiplexer GetConnection()
		{
			this.ConnectIfNotConnected();
			return this._connection;
		}

		
		public void ConnectIfNotConnected()
		{
			if (this._connection == null)
			{
				this._connection = RedisConnector.ImplConnect(this._options);
			}
			else if (!this._connection.IsConnected)
			{
				this._connection.Close(true);
				this._connection = RedisConnector.ImplConnect(this._options);
			}
			if (!this._connection.IsConnected)
			{
				Log.E("Failed to Connect to Redis");
			}
		}

		
		public void Close()
		{
			if (this._connection == null)
			{
				return;
			}
			this._connection.Close(true);
			this._connection = null;
			Log.V("Invoked");
		}

		
		private static ConnectionMultiplexer ImplConnect(ConfigurationOptions options)
		{
			return ConnectionMultiplexer.Connect(options, null);
		}

		
		private readonly ConfigurationOptions _options;

		
		private ConnectionMultiplexer _connection;
	}
}
