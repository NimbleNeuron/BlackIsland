using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using Neptune.Http;

namespace Blis.Client
{
	public static class RouteApi
	{
		private static List<Favorite> recommendfavoriteList;


		private static List<Favorite> customFavoriteList;


		public static List<Favorite> userFavorites = new List<Favorite>();

		public static void SetFavoritesList(int characterCode, Action successCallBack, Action failCallBack)
		{
			userFavorites.Clear();
			RequestDelegate.request<UserRoute>(GetUserRoute(characterCode), false,
				delegate(RequestDelegateError err, UserRoute res)
				{
					if (err != null || res == null)
					{
						Action failCallBack2 = failCallBack;
						if (failCallBack2 == null)
						{
							return;
						}

						failCallBack2();
					}
					else
					{
						if (res.userWeaponRouteResults != null && res.userWeaponRouteResults.Count != 0 ||
						    res.initRecommendWeaponRoutes != null && res.initRecommendWeaponRoutes.Count != 0)
						{
							var sorted = res.userWeaponRouteResults.Select(e => e.userWeaponRoute).ToList();
							sorted.Sort((x, y) => x.order.CompareTo(y.order));
							ProcessCustom(sorted);
							foreach (Favorite item in customFavoriteList)
							{
								userFavorites.Add(item);
							}

							ProcessRecommend(res.initRecommendWeaponRoutes);
							foreach (Favorite item2 in recommendfavoriteList)
							{
								userFavorites.Add(item2);
							}

							successCallBack();
							return;
						}

						Action failCallBack3 = failCallBack;
						if (failCallBack3 == null)
						{
							return;
						}

						failCallBack3();
					}
				});
		}


		private static void ProcessCustom(List<UserWeaponRoute> userWeaponRoutes)
		{
			customFavoriteList = new List<Favorite>();
			foreach (UserWeaponRoute userWeaponRoute in userWeaponRoutes)
			{
				List<int> integerListFromString = GetIntegerListFromString(userWeaponRoute.weaponCodes);
				List<int> integerListFromString2 = GetIntegerListFromString(userWeaponRoute.paths);
				Favorite item = new Favorite(-1L, userWeaponRoute.characterCode, userWeaponRoute.slotId,
					userWeaponRoute.title, userWeaponRoute.weaponType, integerListFromString, integerListFromString2,
					userWeaponRoute.recommendWeaponRouteId, userWeaponRoute.recommendUserNickname,
					userWeaponRoute.share, false, userWeaponRoute.version, userWeaponRoute.teamMode,
					userWeaponRoute.order, userWeaponRoute.shareWeaponRouteId);
				customFavoriteList.Add(item);
			}
		}


		private static void ProcessRecommend(List<InitRecommendWeaponRoute> initRecommendWeaponRoutes)
		{
			recommendfavoriteList = new List<Favorite>();
			foreach (InitRecommendWeaponRoute initRecommendWeaponRoute in initRecommendWeaponRoutes)
			{
				List<int> integerListFromString = GetIntegerListFromString(initRecommendWeaponRoute.weaponCodes);
				List<int> integerListFromString2 = GetIntegerListFromString(initRecommendWeaponRoute.paths);
				Favorite item = new Favorite(-1L, initRecommendWeaponRoute.characterCode, -1,
					Ln.Format("{0} 추천 루트",
						Ln.Get(string.Format("WeaponType/{0}", initRecommendWeaponRoute.weaponType))),
					initRecommendWeaponRoute.weaponType, integerListFromString, integerListFromString2, -1L,
					"NimbleNeuron", true, false, BSERVersion.VERSION, RouteFilterType.ALL, 0, -1L);
				recommendfavoriteList.Add(item);
			}
		}


		public static List<int> GetIntegerListFromString(string str)
		{
			List<int> list = new List<int>();
			if (!string.IsNullOrEmpty(str))
			{
				string[] array = str.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					int item = int.Parse(array[i]);
					list.Add(item);
				}
			}

			return list;
		}


		public static string GetStringFromIntegerList(List<int> intergerList)
		{
			if (intergerList == null || intergerList.Count == 0)
			{
				return "";
			}

			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < intergerList.Count; i++)
			{
				if (i == intergerList.Count - 1)
				{
					stringBuilder.Append(intergerList[i]);
				}
				else
				{
					stringBuilder.Append(intergerList[i]);
					stringBuilder.Append(", ");
				}
			}

			return stringBuilder.ToString();
		}


		public static void Save(Favorite favorite, Action callBack)
		{
			RequestDelegate.request<NullResponse>(SaveUserRoute(FavoriteConvertToUserWeaponRoute(favorite)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}

					callBack();
				});
		}


		public static void Delete(Favorite favorite, Action callBack)
		{
			RequestDelegate.request<NullResponse>(
				DeleteUserRoute(new UserRouteSlotParam(favorite.characterCode, favorite.slotId)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}

					callBack();
				});
		}


		public static void Share(Favorite favorite, Action callBack)
		{
			RequestDelegate.request<NullResponse>(
				ShareUserRoute(new UserRouteSlotParam(favorite.characterCode, favorite.slotId)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}

					callBack();
				});
		}


		public static void ShareCancel(Favorite favorite, Action callBack)
		{
			RequestDelegate.request<NullResponse>(
				ShareCancelUserRoute(new UserRouteSlotParam(favorite.characterCode, favorite.slotId)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}

					callBack();
				});
		}


		public static void Copy(Favorite favorite, Action callBack)
		{
			RequestDelegate.request<NullResponse>(
				CopyRecommendRoute(new RecommendCopyParam(favorite.characterCode, favorite.slotId,
					favorite.recommendWeaponRouteId, favorite.init, favorite.weaponType, favorite.order)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}

					callBack();
				});
		}


		public static void Use(Favorite favorite)
		{
			if (favorite == null)
			{
				Log.E("[FavoriteAPI] Use(Favorite favorite) favorite is Null");
				return;
			}

			if (favorite.slotId == 0)
			{
				Log.E("[FavoriteAPI] Use(Favorite favorite) favorite is RecommendFavorite");
				return;
			}

			RequestDelegate.request<NullResponse>(
				AddRecommedRouteUseCount(new RecommendUseParam(favorite.characterCode, favorite.slotId)), false,
				delegate(RequestDelegateError err, NullResponse res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}
				});
		}


		private static UserWeaponRoute FavoriteConvertToUserWeaponRoute(Favorite favorite)
		{
			string stringFromIntegerList = GetStringFromIntegerList(favorite.weaponCodes);
			string stringFromIntegerList2 = GetStringFromIntegerList(favorite.paths);
			return new UserWeaponRoute(favorite.characterCode, favorite.slotId, favorite.title, favorite.weaponType,
				stringFromIntegerList, stringFromIntegerList2, favorite.recommendWeaponRouteId,
				favorite.recommendUserNickname, favorite.share, favorite.init, favorite.version,
				favorite.routeFilterType, favorite.order, favorite.shareWeaponRouteId);
		}


		public static Func<HttpRequest> GetUserRoute(int characterCode)
		{
			return HttpRequestFactory.Get(
				ApiConstants.Url(string.Format("/weaponRoute/?characterCode={0}", characterCode),
					Array.Empty<object>()));
		}


		public static Func<HttpRequest> GetBringRoute(RecommendSearchParam searchParam)
		{
			return HttpRequestFactory.Get(ApiConstants.Url(
				string.Format("/weaponRoute/recommends/?sortType={0}&characterCode={1}", searchParam.sortType,
					searchParam.characterCode) +
				string.Format("&weaponType={0}&pageIndex={1}", searchParam.weaponType, searchParam.pageIndex) +
				string.Format("&teamMode={0}&recentlyVersion={1}", searchParam.teamMode, searchParam.recentlyVersion),
				Array.Empty<object>()));
		}


		public static Func<HttpRequest> SaveUserRoute(UserWeaponRoute userWeaponRoute)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/add", Array.Empty<object>()),
				userWeaponRoute);
		}


		public static Func<HttpRequest> DeleteUserRoute(UserRouteSlotParam userRouteSlotParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/delete", Array.Empty<object>()),
				userRouteSlotParam);
		}


		public static Func<HttpRequest> ShareUserRoute(UserRouteSlotParam userRouteSlotParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/share", Array.Empty<object>()),
				userRouteSlotParam);
		}


		public static Func<HttpRequest> ShareCancelUserRoute(UserRouteSlotParam userRouteSlotParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/share/cancel", Array.Empty<object>()),
				userRouteSlotParam);
		}


		public static Func<HttpRequest> CopyRecommendRoute(RecommendCopyParam recommendCopyParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/getRecommend", Array.Empty<object>()),
				recommendCopyParam);
		}


		public static Func<HttpRequest> AddRecommedRouteUseCount(RecommendUseParam recommendUseParam)
		{
			return HttpRequestFactory.Post(
				ApiConstants.Url("/weaponRoute/addRecommendWeaponRouteCount", Array.Empty<object>()),
				recommendUseParam);
		}


		public static Func<HttpRequest> GetRecommendRouteById(UserWeaponRouteParam userWeaponRouteParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/getRecommendById", Array.Empty<object>()),
				userWeaponRouteParam);
		}


		public static Func<HttpRequest> ChangeOrdersRoute(List<UserWeaponRouteParam> userWeaponRouteParam)
		{
			return HttpRequestFactory.Post(ApiConstants.Url("/weaponRoute/changeOrders", Array.Empty<object>()),
				userWeaponRouteParam);
		}


		public static List<Favorite> GetFavoritesByFilterType(RouteFilterType routeFilterType)
		{
			List<Favorite> list = new List<Favorite>();
			foreach (Favorite item in userFavorites)
			{
				list.Add(item);
			}

			if (routeFilterType == RouteFilterType.ALL)
			{
				return list;
			}

			if (routeFilterType == RouteFilterType.SOLO || routeFilterType == RouteFilterType.DUO ||
			    routeFilterType == RouteFilterType.SQUAD)
			{
				list.RemoveAll(x =>
					!x.routeFilterType.ToString().Contains(routeFilterType.ToString()) &&
					x.routeFilterType > RouteFilterType.ALL);
			}
			else if (routeFilterType == RouteFilterType.SOLO_DUO)
			{
				list.RemoveAll(x =>
					x.routeFilterType == RouteFilterType.SQUAD && x.routeFilterType > RouteFilterType.ALL);
			}
			else if (routeFilterType == RouteFilterType.SOLO_SQUAD)
			{
				list.RemoveAll(x =>
					x.routeFilterType == RouteFilterType.DUO && x.routeFilterType > RouteFilterType.ALL);
			}
			else if (routeFilterType == RouteFilterType.DUO_SQUAD)
			{
				list.RemoveAll(
					x => x.routeFilterType == RouteFilterType.SOLO && x.routeFilterType > RouteFilterType.ALL);
			}

			return list;
		}


		public class UserRoute
		{
			public int freeMaxSlotCount;
			public int paidMaxSlotCount;
			public int paidSlotCount;

			public List<InitRecommendWeaponRoute> initRecommendWeaponRoutes;
			public List<UserWeaponRouteResult> userWeaponRouteResults;
		}


		public class BringRouteInfo
		{
			public InitRecommendWeaponRoute initRecommendWeaponRoute;


			public List<RecommendWeaponRoute> myRecommendWeaponRoutes;

			public List<RecommendWeaponRoute> recommendWeaponRoutes;


			public int recommendWeaponRoutesCount;
		}


		public class UserWeaponRouteResult
		{
			public UserWeaponRoute userWeaponRoute;
			public int recommendWeaponRouteRouteVersion; // 0x18
			public int like; // 0x1C
			public int unLike; // 0x20
			public int accumulateLike; // 0x24
			public int accumulateUnLike; // 0x28
			public bool myLike; // 0x2C
			public bool myUnLike; // 0x2D
			public UserWeaponRouteDesc userWeaponRouteDesc; // 0x30
		}
		
		public class UserWeaponRouteDesc // TypeDefIndex: 13595
		{
			// Fields
			public int characterCode; // 0x10
			public int slotId; // 0x14
			public string qDesc; // 0x18
			public string wDesc; // 0x20
			public string eDesc; // 0x28
			public string rDesc; // 0x30
			public string tDesc; // 0x38
			public string dDesc; // 0x40
			public string skillPath; // 0x48
			public string descTitle; // 0x50
			public string desc; // 0x58

			// RVA: 0x224BCE0 Offset: 0x224A6E0 VA: 0x18224BCE0
			public UserWeaponRouteDesc(RecommendWeaponRouteDesc recommendWeaponRouteDesc, int characterCode, int slotId) { }
		}
		
		public class RecommendWeaponRouteDesc // TypeDefIndex: 13596
		{
			// Fields
			public long recommendWeaponRouteId; // 0x10
			public string qDesc; // 0x18
			public string wDesc; // 0x20
			public string eDesc; // 0x28
			public string rDesc; // 0x30
			public string tDesc; // 0x38
			public string dDesc; // 0x40
			public string skillPath; // 0x48
			public string descTitle; // 0x50
			public string desc; // 0x58
		}
	}
}