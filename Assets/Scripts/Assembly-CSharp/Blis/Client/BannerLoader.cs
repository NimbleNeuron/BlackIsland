using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Blis.Common;
using Blis.Common.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

namespace Blis.Client
{
	public class BannerLoader : SingletonMonoBehaviour<BannerLoader>
	{
		public delegate void ResponseCallback<T>(T image);


		private const float WEB_REQUEST_TIMEOUT = 3f;


		private const string REQUEST_URL_EDGE = "notice/";


		public float untilTimeOut;


		private readonly List<LoadedBanner> loadedBigBanner = new List<LoadedBanner>();


		private readonly List<LoadedBanner> loadedSmallBanner = new List<LoadedBanner>();


		private LoadedBanner loadedPlayBanner;


		private BannerPersist persist;

		public Coroutine Load(string dataPath)
		{
			persist = new BannerPersist(dataPath);
			return this.StartThrowingCoroutine(RequestAll(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][BANNER] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public IEnumerator RequestAll()
		{
			bool isBigRequestFinish = false;
			bool isSmallRequestFinish = false;
			bool isPlayRequestFinish = false;
			loadedBigBanner.Clear();
			loadedSmallBanner.Clear();
			loadedPlayBanner = null;
			persist.RemoveExpiredFile();
			RequestBanners(LobbyApi.NoticeType.BIG_BANNER, loadedBigBanner, delegate(List<LobbyApi.LobbyNotice> result)
			{
				BannerService.UpdateBigBanner(result);
				isBigRequestFinish = true;
			});
			RequestBanners(LobbyApi.NoticeType.SMALL_BANNER, loadedSmallBanner,
				delegate(List<LobbyApi.LobbyNotice> result)
				{
					BannerService.UpdateSmallBanner(result);
					isSmallRequestFinish = true;
				});
			RequestBanner(LobbyApi.NoticeType.PLAY_BANNER, delegate(LobbyApi.LobbyNotice result)
			{
				BannerService.UpdatePlayBanner(result);
				isPlayRequestFinish = true;
			}, delegate(LoadedBanner loadedBanner) { loadedPlayBanner = loadedBanner; });
			while (!isBigRequestFinish || !isSmallRequestFinish || !isPlayRequestFinish)
			{
				yield return new WaitForSeconds(0.1f);
			}

			untilTimeOut = 3f;
			while (!BannerService.IsFinishBigBannerImgLoad(loadedBigBanner.Count) ||
			       !BannerService.IsFinishSmallBannerImgLoad(loadedSmallBanner.Count) ||
			       !BannerService.IsFinishPlayBannerImgLoad(loadedPlayBanner))
			{
				untilTimeOut -= 0.1f;
				if (untilTimeOut <= 0f)
				{
					break;
				}

				yield return new WaitForSeconds(0.1f);
			}

			persist.SaveEndDtmData();
			ApplyBannerUI();
		}


		private void RequestBanners(LobbyApi.NoticeType bannerType, List<LoadedBanner> loadedBanners,
			Action<List<LobbyApi.LobbyNotice>> requestFinish)
		{
			RequestDelegate.requestCoroutine<LobbyApi.LobbyNoticeResult>(
				LobbyApi.GetLobbyBannerData(bannerType, Ln.GetCurrentLanguage()),
				delegate(RequestDelegateError err, LobbyApi.LobbyNoticeResult res)
				{
					if (err != null)
					{
						Action<List<LobbyApi.LobbyNotice>> requestFinish2 = requestFinish;
						if (requestFinish2 == null)
						{
							return;
						}

						requestFinish2(null);
					}
					else
					{
						if (res.lobbyNoticeResults != null && res.lobbyNoticeResults.Count > 0)
						{
							using (List<LobbyApi.LobbyNotice>.Enumerator enumerator =
								res.lobbyNoticeResults.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									LobbyApi.LobbyNotice bannerData = enumerator.Current;
									if (bannerData != null && bannerData.bannerUrl != null)
									{
										persist.AddLatestEndDtms(bannerData.bannerUrl, bannerData.endDtm);
										Load(bannerType, bannerData, delegate(Texture2D result)
										{
											result.wrapMode = TextureWrapMode.Clamp;
											loadedBanners.Add(new LoadedBanner(bannerData, result));
										});
									}
								}
							}
						}

						Action<List<LobbyApi.LobbyNotice>> requestFinish3 = requestFinish;
						if (requestFinish3 == null)
						{
							return;
						}

						requestFinish3(res.lobbyNoticeResults);
					}
				});
		}


		private void RequestBanner(LobbyApi.NoticeType bannerType, Action<LobbyApi.LobbyNotice> requestFinish,
			Action<LoadedBanner> imgLoadFinish)
		{
			RequestDelegate.requestCoroutine<LobbyApi.LobbyNoticeResult>(
				LobbyApi.GetLobbyBannerData(bannerType, Ln.GetCurrentLanguage()),
				delegate(RequestDelegateError err, LobbyApi.LobbyNoticeResult res)
				{
					if (err != null)
					{
						Log.V("RequestBanner Error: " + err.errorType);
						Action<LobbyApi.LobbyNotice> requestFinish2 = requestFinish;
						if (requestFinish2 == null)
						{
							return;
						}

						requestFinish2(null);
					}
					else if (res.lobbyNoticeResults == null || res.lobbyNoticeResults.Count <= 0)
					{
						Action<LobbyApi.LobbyNotice> requestFinish3 = requestFinish;
						if (requestFinish3 == null)
						{
							return;
						}

						requestFinish3(null);
					}
					else
					{
						if (res.lobbyNoticeResults.Count > 1)
						{
							res.lobbyNoticeResults.Sort((x, y) => x.order.CompareTo(y.order));
						}

						LobbyApi.LobbyNotice bannerData = res.lobbyNoticeResults[0];
						if (bannerData != null && bannerData.bannerUrl != null)
						{
							persist.AddLatestEndDtms(bannerData.bannerUrl, bannerData.endDtm);
							Load(bannerType, bannerData, delegate(Texture2D result)
							{
								result.wrapMode = TextureWrapMode.Clamp;
								Action<LoadedBanner> imgLoadFinish2 = imgLoadFinish;
								if (imgLoadFinish2 == null)
								{
									return;
								}

								imgLoadFinish2(new LoadedBanner(bannerData, result));
							});
						}

						Action<LobbyApi.LobbyNotice> requestFinish4 = requestFinish;
						if (requestFinish4 == null)
						{
							return;
						}

						requestFinish4(bannerData);
					}
				});
		}


		public void Load(LobbyApi.NoticeType bannerType, LobbyApi.LobbyNotice bannerData, Action<Texture2D> callback)
		{
			if (File.Exists(persist.GetFilePath(bannerData.bannerUrl)))
			{
				try
				{
					persist.LoadFromFile(bannerType, bannerData.bannerUrl, callback);
					return;
				}
				catch (Exception)
				{
					this.StartThrowingCoroutine(LoadFromServer(bannerData.bannerUrl, bannerData.endDtm, callback),
						delegate(Exception exception)
						{
							Log.E("[EXCEPTION][BANNER][LoadFromServer] Message:" + exception.Message + ", StackTrace:" +
							      exception.StackTrace);
						});
					return;
				}
			}

			Log.H("[BannerLoader] Entry(" + persist.GetFilename(bannerData.bannerUrl) + ") needs to be updated.");
			this.StartThrowingCoroutine(LoadFromServer(bannerData.bannerUrl, bannerData.endDtm, callback),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][BANNER][LoadFromServer2] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator LoadFromServer(string url, DateTime endDtm, Action<Texture2D> callback)
		{
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
			DownloadHandlerTexture handler = new DownloadHandlerTexture(true);
			request.downloadHandler = handler;
			yield return request.SendWebRequest();
			if (request.isHttpError || handler.texture == null)
			{
				Log.E("[BannerLoader] " + url + " : Failed to receive game data from server");
				yield break;
			}

			try
			{
				persist.SaveFile(url, endDtm, handler.texture);
				if (callback != null)
				{
					callback(handler.texture);
				}
			}
			catch (Exception arg)
			{
				Log.E(string.Format("[BannerLoader] Save File Exception = {0}", arg));
			}
		}


		public void ApplyBannerUI()
		{
			List<LoadedBanner> list = new List<LoadedBanner>();
			loadedBigBanner.Sort((x, y) => x.data.order.CompareTo(y.data.order));
			for (int i = 0; i < loadedBigBanner.Count; i++)
			{
				list.Add(loadedBigBanner[i]);
			}

			BannerService.SetLoadedBigBanners(list);
			list.Clear();
			loadedSmallBanner.Sort((x, y) => x.data.order.CompareTo(y.data.order));
			for (int j = 0; j < loadedSmallBanner.Count; j++)
			{
				list.Add(loadedSmallBanner[j]);
			}

			BannerService.SetLoadedSmallBanners(list);
			BannerService.SetLoadedPlayBanner(loadedPlayBanner);
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.InitBanners();
		}


		public class LoadedBanner
		{
			public LoadedBanner(LobbyApi.LobbyNotice data, Texture2D image)
			{
				this.data = data;
				this.image = image;
			}


			public LobbyApi.LobbyNotice data { get; }


			public Texture2D image { get; }
		}


		public class BannerPersist
		{
			private const string endDtmCacheFile = "BannerCacheFile";


			private readonly int[] BIG_BANNER_SIZE =
			{
				980,
				551
			};


			private readonly List<string> cacheEndDtms;


			private readonly string dataPath;


			private readonly List<string> newEndDtms = new List<string>();


			private readonly int[] PLAY_BANNER_SIZE =
			{
				462,
				237
			};


			private readonly int[] SMALL_BANNER_SIZE =
			{
				340,
				170
			};

			public BannerPersist(string dataPath)
			{
				this.dataPath = dataPath + "/Banner/";
				if (!Directory.Exists(this.dataPath))
				{
					Directory.CreateDirectory(this.dataPath);
				}

				cacheEndDtms = LoadEndDtmData();
			}


			private List<string> LoadEndDtmData()
			{
				List<string> result;
				try
				{
					result = JsonConvert.DeserializeObject<List<string>>(
						SecureFileHandler.ReadEncryptFile(Path.Combine(dataPath, "BannerCacheFile")));
				}
				catch (Exception ex)
				{
					Log.V("Failed to load version data:");
					Log.V(ex.ToString());
					result = new List<string>();
				}

				return result;
			}


			public void AddLatestEndDtms(string url, DateTime endDtm)
			{
				string item = string.Format("{0}_{1}", GetFilename(url), endDtm);
				newEndDtms.Add(item);
			}


			public void SaveEndDtmData()
			{
				SecureFileHandler.WriteEncryptFile(Path.Combine(dataPath, "BannerCacheFile"),
					JsonConvert.SerializeObject(newEndDtms));
			}


			public void SaveFile(string url, DateTime endDtm, Texture2D obj)
			{
				string data = JsonConvert.SerializeObject(obj.EncodeToPNG());
				SecureFileHandler.WriteEncryptFile(GetFilePath(url), data);
			}


			public string GetFilePath(string fullUrl)
			{
				return Path.Combine(dataPath, GetFilename(fullUrl));
			}


			public string GetFilename(string fullUrl)
			{
				if (fullUrl.Contains("notice/"))
				{
					int startIndex = fullUrl.IndexOf("notice/") + "notice/".Length;
					return fullUrl.Substring(startIndex).Replace(".png", string.Empty);
				}

				return fullUrl;
			}


			public void LoadFromFile(LobbyApi.NoticeType bannerType, string url, Action<Texture2D> callback)
			{
				byte[] data =
					JsonConvert.DeserializeObject<byte[]>(SecureFileHandler.ReadEncryptFile(GetFilePath(url)));
				Texture2D texture2D;
				if (bannerType == LobbyApi.NoticeType.BIG_BANNER)
				{
					texture2D = new Texture2D(BIG_BANNER_SIZE[0], BIG_BANNER_SIZE[1], TextureFormat.ARGB32, false);
				}
				else if (bannerType == LobbyApi.NoticeType.PLAY_BANNER)
				{
					texture2D = new Texture2D(PLAY_BANNER_SIZE[0], PLAY_BANNER_SIZE[1], TextureFormat.ARGB32, false);
				}
				else
				{
					texture2D = new Texture2D(SMALL_BANNER_SIZE[0], SMALL_BANNER_SIZE[1], TextureFormat.ARGB32, false);
				}

				texture2D.LoadImage(data);
				if (texture2D.graphicsFormat == GraphicsFormat.R8G8B8_SRGB)
				{
					File.Delete(GetFilePath(url));
					return;
				}

				callback(texture2D);
			}


			public void RemoveExpiredFile()
			{
				foreach (string text in cacheEndDtms)
				{
					if (!newEndDtms.Contains(text) && File.Exists(Path.Combine(dataPath, text)))
					{
						File.Delete(Path.Combine(dataPath, text));
					}
				}
			}
		}
	}
}