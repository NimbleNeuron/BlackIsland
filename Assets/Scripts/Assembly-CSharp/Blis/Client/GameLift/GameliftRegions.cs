using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Blis.Common;
using Common.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Blis.Client.GameLift
{
	public static class GameliftRegions
	{
		private static readonly Dictionary<string, string> RegionalEndpointsGlobal;


		private static readonly Dictionary<string, int> RegionalLatencies;


		private static readonly List<string> AcceptableRegions;


		// Note: this type is marked as 'beforefieldinit'.
		static GameliftRegions()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["us-east-2"] = "gamelift.us-east-2.amazonaws.com";
			dictionary["us-east-1"] = "gamelift.us-east-1.amazonaws.com";
			dictionary["us-west-1"] = "gamelift.us-west-1.amazonaws.com";
			dictionary["us-west-2"] = "gamelift.us-west-2.amazonaws.com";
			dictionary["ap-south-1"] = "gamelift.ap-south-1.amazonaws.com";
			dictionary["ap-northeast-2"] = "gamelift.ap-northeast-2.amazonaws.com";
			dictionary["ap-southeast-1"] = "gamelift.ap-southeast-1.amazonaws.com";
			dictionary["ap-southeast-2"] = "gamelift.ap-southeast-2.amazonaws.com";
			dictionary["ap-northeast-1"] = "gamelift.ap-northeast-1.amazonaws.com";
			dictionary["ca-central-1"] = "gamelift.ca-central-1.amazonaws.com";
			dictionary["eu-central-1"] = "gamelift.eu-central-1.amazonaws.com";
			dictionary["eu-west-1"] = "gamelift.eu-west-1.amazonaws.com";
			dictionary["eu-west-2"] = "gamelift.eu-west-2.amazonaws.com";
			dictionary["sa-east-1"] = "gamelift.sa-east-1.amazonaws.com";
			RegionalEndpointsGlobal = dictionary;
			RegionalLatencies = new Dictionary<string, int>();
			AcceptableRegions = new List<string>();
		}

		public static Dictionary<string, string> GetRegionalEndpoints()
		{
			return RegionalEndpointsGlobal;
		}


		private static List<string> GetAcceptableRegions()
		{
			if (!AcceptableRegions.Any<string>())
			{
				return GetRegionalEndpoints().Keys.ToList<string>();
			}

			return AcceptableRegions;
		}


		public static void UpdateAcceptableRegions(IEnumerable<string> regions)
		{
			AcceptableRegions.Clear();
			AcceptableRegions.AddRange(regions);
		}


		[NotNull]
		public static Dictionary<string, int> GetCoordinatedRegionalLatencies()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(RegionalLatencies);
			Dictionary<string, int> dictionary2;
			if (AcceptableRegions.Any<string>())
			{
				dictionary2 = new Dictionary<string, int>();
				using (Dictionary<string, int>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, int> keyValuePair = enumerator.Current;
						string key = keyValuePair.Key;
						int value = keyValuePair.Value;
						dictionary2.Add(key, value);
					}

					goto IL_65;
				}
			}

			dictionary2 = dictionary;
			IL_65:
			if (dictionary2.Any<KeyValuePair<string, int>>())
			{
				KeyValuePair<string, int> keyValuePair2 = dictionary2.Aggregate(
					delegate(KeyValuePair<string, int> l, KeyValuePair<string, int> r)
					{
						if (l.Value >= r.Value)
						{
							return r;
						}

						return l;
					});
				if (keyValuePair2.Value >= 50)
				{
					dictionary2[keyValuePair2.Key] = 49;
				}
			}

			return dictionary2;
		}


		public static async Task Update()
		{
			try
			{
				await UpdateRegions();
				await UpdatePings();
			}
			catch (Exception ex)
			{
				Log.E("Exception occurred while update regions and pings: exception=" + ex.Message);
			}
		}


		private static async Task UpdateRegions()
		{
			await Task.Run(() => { });
		}


		private static async Task UpdatePings()
		{
			List<Task> list = new List<Task>();
			foreach (string region in GetAcceptableRegions())
			{
				list.Add(UpdatePing(region));
			}

			await Task.WhenAll(list);
		}


		private static async Task UpdatePing(string region)
		{
			string orDefault = GetRegionalEndpoints().GetOrDefault(region);
			if (orDefault != null)
			{
				IPAddress ipaddress = (await Dns.GetHostEntryAsync(orDefault)).AddressList.FirstOrDefault<IPAddress>();
				string text = ipaddress != null ? ipaddress.ToString() : null;
				if (text != null)
				{
					int tryCount = 0;
					Ping ping = new Ping(text);
					await Task.Delay(TimeSpan.FromSeconds(1.0));
					while (!ping.isDone)
					{
						await Task.Delay(TimeSpan.FromSeconds(1.0));
						if (++tryCount >= 5)
						{
							return;
						}
					}

					if (ping.time >= 1)
					{
						RegionalLatencies[region] = ping.time;
					}
				}
			}
		}
	}
}