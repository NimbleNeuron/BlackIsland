using System;
using System.Collections.Generic;
using System.IO;
using Neptune.Http;
using Newtonsoft.Json;
using UnityEngine;

namespace Blis.Common
{
	public class GameDBPersist
	{
		private const string VersionCacheFile = "VersionCache";
		private static string dataServerUrl = ApiConstants.DataServerUrl + "/metaData";
		private readonly Dictionary<string, long> cacheVersions;
		private readonly string dataPath;
		public bool isInitialized;
		private Dictionary<string, long> versions;

		public GameDBPersist(string dataPath)
		{
			this.dataPath = dataPath;
			if (!Directory.Exists(dataPath))
			{
				Directory.CreateDirectory(dataPath);
			}

			cacheVersions = LoadVersionData();
		}


		private Dictionary<string, long> LoadVersionData()
		{
			Dictionary<string, long> result;
			try
			{
				result = JsonConvert.DeserializeObject<Dictionary<string, long>>(
					SecureFileHandler.ReadEncryptFile(Path.Combine(dataPath, "VersionCache")));
			}
			catch (Exception ex)
			{
				Log.V("Failed to load version data:");
				Log.V(ex.ToString());
				result = new Dictionary<string, long>();
			}

			return result;
		}


		public void SaveVersionData()
		{
			SecureFileHandler.WriteEncryptFile(Path.Combine(dataPath, "VersionCache"),
				JsonConvert.SerializeObject(versions));
		}


		public Coroutine Init(Action<string> callback)
		{
			return RequestDelegate.requestCoroutine<Dictionary<string, long>>(GameDBApi.GetVersionData(),
				delegate(RequestDelegateError err, Dictionary<string, long> res)
				{
					if (err != null)
					{
						callback(err.message);
						return;
					}

					versions = res;
					isInitialized = true;
					callback(null);
				}, false);
		}


		private void SaveFile<T>(Entry entry, T obj)
		{
			string data = JsonConvert.SerializeObject(obj);
			SecureFileHandler.WriteEncryptFile(GetFilePath(entry), data);
			if (cacheVersions.ContainsKey(entry.key))
			{
				cacheVersions[entry.key] = entry.newestVersion;
				return;
			}

			cacheVersions.Add(entry.key, entry.newestVersion);
		}


		public long GetCacheVersion(string key)
		{
			if (cacheVersions.TryGetValue(key, out long result))
			{
				return result;
			}

			return 0L;
		}


		public void Load<T>(string key, Action<string, List<T>> cb) where T : class
		{
			if (versions == null || !versions.TryGetValue(key, out long newestVersion))
			{
				Log.H("[GameDB] No version data: " + key);
				return;
			}

			Entry entry = new Entry(key, newestVersion);
			if (entry.newestVersion == GetCacheVersion(key))
			{
				try
				{
					LoadFromFile<List<T>>(entry, cb);
					return;
				}
				catch (Exception)
				{
					LoadFromServer<List<T>>(entry, cb);
					return;
				}
			}

			Log.H("[GameDB] Entry(" + entry.key + ") needs to be updated.");
			LoadFromServer<List<T>>(entry, cb);
		}


		public void LoadFromFile<T>(string key, Action<string, List<T>> cb) where T : class
		{
			Entry entry = new Entry(key, 0L);
			try
			{
				LoadFromFile<List<T>>(entry, cb);
			}
			catch (Exception)
			{
				LoadFromServer<List<T>>(entry, cb);
			}
		}


		private string GetFilePath(Entry entry)
		{
			return Path.Combine(dataPath, entry.GetFilename());
		}


		private void LoadFromFile<T>(Entry entry, Action<string, T> cb) where T : class
		{
			T arg = JsonConvert.DeserializeObject<T>(SecureFileHandler.ReadEncryptFile(GetFilePath(entry)));
			cb(null, arg);
		}


		private void LoadFromServer<T>(Entry entry, Action<string, T> cb) where T : class
		{
			RequestDelegate.request<T>(entry.Request(), delegate(RequestDelegateError err, T res)
			{
				if (res == null)
				{
					Log.E("[GameDB] " + entry.key + " : Failed to receive game data from server");
					cb(err.message, default);
					return;
				}

				try
				{
					SaveFile<T>(entry, res);
				}
				catch (Exception ex)
				{
					Log.E("[GameDB] Save File Exception = " + ex);
				}

				try
				{
					cb(null, res);
				}
				catch (Exception ex2)
				{
					Log.E("[GameDB] Callback Exception = " + ex2);
				}
			}, false);
		}


		private class Entry
		{
			public readonly string key;


			public readonly long newestVersion;


			public Entry(string key, long newestVersion)
			{
				this.key = key;
				this.newestVersion = newestVersion;
			}


			public Func<HttpRequest> Request()
			{
				return GameDBApi.GetGameData(key);
			}


			public string GetFilename()
			{
				return key;
			}
		}
	}
}