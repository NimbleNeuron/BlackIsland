using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public static class BannerService
	{
		private static readonly List<LobbyApi.LobbyNotice> bigBannerDatas = new List<LobbyApi.LobbyNotice>();


		private static readonly List<LobbyApi.LobbyNotice> smallBannerDatas = new List<LobbyApi.LobbyNotice>();


		private static LobbyApi.LobbyNotice playBannerData = null;


		private static readonly List<BannerLoader.LoadedBanner>
			loadedBigBanners = new List<BannerLoader.LoadedBanner>();


		private static readonly List<BannerLoader.LoadedBanner> loadedSmallBanners =
			new List<BannerLoader.LoadedBanner>();


		private static BannerLoader.LoadedBanner loadedPlayBanner = null;

		public static void UpdateBigBanner(List<LobbyApi.LobbyNotice> bannerList)
		{
			bigBannerDatas.Clear();
			if (bannerList != null)
			{
				foreach (LobbyApi.LobbyNotice item in bannerList)
				{
					bigBannerDatas.Add(item);
				}
			}
		}


		public static void SetLoadedBigBanners(List<BannerLoader.LoadedBanner> loadedBanners)
		{
			loadedBigBanners.Clear();
			foreach (BannerLoader.LoadedBanner item in loadedBanners)
			{
				loadedBigBanners.Add(item);
			}
		}


		public static void GetBigBanner(int index, out LobbyApi.LobbyNotice data, out Texture2D img)
		{
			data = loadedBigBanners[index].data;
			img = loadedBigBanners[index].image;
		}


		public static bool IsFinishBigBannerImgLoad(int loadedCount)
		{
			if (bigBannerDatas.Count == 0)
			{
				return true;
			}

			int num = 0;
			foreach (LobbyApi.LobbyNotice lobbyNotice in bigBannerDatas)
			{
				if (lobbyNotice != null && !string.IsNullOrEmpty(lobbyNotice.bannerUrl))
				{
					num++;
				}
			}

			return num == loadedCount;
		}


		public static int GetLoadedBigBannerCount()
		{
			return loadedBigBanners.Count;
		}


		public static void UpdateSmallBanner(List<LobbyApi.LobbyNotice> bannerList)
		{
			smallBannerDatas.Clear();
			if (bannerList != null)
			{
				foreach (LobbyApi.LobbyNotice item in bannerList)
				{
					smallBannerDatas.Add(item);
				}
			}
		}


		public static void SetLoadedSmallBanners(List<BannerLoader.LoadedBanner> loadedBanners)
		{
			loadedSmallBanners.Clear();
			foreach (BannerLoader.LoadedBanner item in loadedBanners)
			{
				loadedSmallBanners.Add(item);
			}
		}


		public static void GetSmallBanner(int index, out LobbyApi.LobbyNotice data, out Texture2D img)
		{
			data = loadedSmallBanners[index].data;
			img = loadedSmallBanners[index].image;
		}


		public static bool IsFinishSmallBannerImgLoad(int loadedCount)
		{
			if (smallBannerDatas.Count == 0)
			{
				return true;
			}

			int num = 0;
			foreach (LobbyApi.LobbyNotice lobbyNotice in smallBannerDatas)
			{
				if (lobbyNotice != null && !string.IsNullOrEmpty(lobbyNotice.bannerUrl))
				{
					num++;
				}
			}

			return num == loadedCount;
		}


		public static int GetLoadedSmallBannerCount()
		{
			return loadedSmallBanners.Count;
		}


		public static void UpdatePlayBanner(LobbyApi.LobbyNotice lobbyNotice)
		{
			playBannerData = lobbyNotice;
		}


		public static bool IsFinishPlayBannerImgLoad(BannerLoader.LoadedBanner loadedBanner)
		{
			return playBannerData == null || string.IsNullOrEmpty(playBannerData.bannerUrl) || loadedBanner != null;
		}


		public static void SetLoadedPlayBanner(BannerLoader.LoadedBanner loadedBanner)
		{
			loadedPlayBanner = loadedBanner;
		}


		public static bool ApplyPlayBannerUI(LobbyBannerItem banner)
		{
			if (loadedPlayBanner != null)
			{
				banner.Init(loadedPlayBanner.data.title, loadedPlayBanner.data.message, loadedPlayBanner.image,
					loadedPlayBanner.data.targetUrl);
				return true;
			}

			return false;
		}
	}
}