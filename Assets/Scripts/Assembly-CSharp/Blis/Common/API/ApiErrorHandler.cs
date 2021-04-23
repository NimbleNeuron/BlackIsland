using System;
using Blis.Client;
using Blis.Common.Utils;
using Neptune.Http;
using UnityEngine;

namespace Blis.Common
{
	public static class ApiErrorHandler
	{
		public static void ShowPopup(string message, Action action)
		{
			MonoBehaviourInstance<Popup>.inst.Error(message, delegate
			{
				if (action != null)
				{
					action();
				}
			});
		}


		public static bool OnHttpError(HttpResultStates state, string msg)
		{
			Log.V("[OnServerError] {0} | {1}", state, msg);
			return true;
		}


		public static bool OnServerError(RestErrorType errorType, string msg)
		{
			Log.V("[OnServerError] {0} | {1}", errorType, msg);
			if (errorType <= RestErrorType.MALFORMED_SESSION)
			{
				if (errorType != RestErrorType.UNKNOWN)
				{
					switch (errorType)
					{
						case RestErrorType.SESSION_HEADER_REQUIRED:
							break;
						case RestErrorType.UNAVAILALBE_REQUEST:
							return false;
						case RestErrorType.UNDER_MAINTENANCE:
							ShowMaintenancePopup();
							return true;
						case RestErrorType.INVALID_VERSION:
							MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("클라이언트 버전 업데이트 이후 이용 가능합니다."), Exit);
							return true;
						default:
							if (errorType != RestErrorType.MALFORMED_SESSION)
							{
								return false;
							}

							break;
					}
				}
			}
			else if (errorType != RestErrorType.SESSION_EXPIRED)
			{
				if (errorType == RestErrorType.BAN_USER)
				{
					DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
					Popup inst = MonoBehaviourInstance<Popup>.inst;
					string msg2 = "접속이 제한된 계정입니다.";
					DateTime time = dateTime.AddSeconds(long.Parse(msg) / 1000L);
					Popup.Button[] array = new Popup.Button[2];
					int num = 0;
					Popup.Button button = new Popup.Button();
					button.text = Ln.Get("고객문의");
					button.callback = delegate { Application.OpenURL(Ln.Get("고객지원 링크")); };
					array[num] = button;
					array[1] = new Popup.Button
					{
						text = Ln.Get("닫기"),
						callback = Exit
					};
					inst.Message(msg2, time, array);
					return true;
				}

				if (errorType != RestErrorType.PERMANENT_BAN_USER)
				{
					return false;
				}

				Popup inst2 = MonoBehaviourInstance<Popup>.inst;
				string msg3 = Ln.Get("영구 정지");
				Popup.Button[] array2 = new Popup.Button[2];
				int num2 = 0;
				Popup.Button button2 = new Popup.Button();
				button2.text = Ln.Get("고객문의");
				button2.callback = delegate { Application.OpenURL(Ln.Get("고객지원 링크")); };
				array2[num2] = button2;
				array2[1] = new Popup.Button
				{
					text = Ln.Get("닫기"),
					callback = Exit
				};
				inst2.Message(msg3, array2);
				return true;
			}

			MonoBehaviourInstance<Popup>.inst.Error(Ln.Get(string.Format("ServerError/{0}", (int) errorType)), Restart);
			return true;
		}


		private static void ShowMaintenancePopup()
		{
			RequestDelegate.request<MaintenanceResult>(LobbyApi.Maintenance(),
				delegate(RequestDelegateError err, MaintenanceResult res)
				{
					if (err == null)
					{
						LnUtil.GetServerTime(res.maintenanceStart, res.maintenanceEnd);
					}

					Popup inst = MonoBehaviourInstance<Popup>.inst;
					string msg = Ln.Get("OutOfService_Temp");
					Popup.Button[] array = new Popup.Button[2];
					int num = 0;
					Popup.Button button = new Popup.Button();
					button.text = Ln.Get("기본 커뮤니티");
					button.callback = delegate { Application.OpenURL(Ln.Get("기본 커뮤니티 공지 링크")); };
					button.type = Popup.ButtonType.Link;
					array[num] = button;
					array[1] = new Popup.Button
					{
						text = Ln.Get("닫기"),
						callback = Exit,
						type = Popup.ButtonType.Cancel
					};
					inst.Message(msg, array);
				});
		}


		private static void Exit()
		{
			Application.Quit();
		}


		private static void Restart()
		{
			Lobby inst = Lobby.inst;
			if (inst != null)
			{
				inst.ResetUser();
			}

			SingletonMonoBehaviour<UserService>.inst.SetUser(null);
			Singleton<SoundControl>.inst.CleanUp();
			SingletonMonoBehaviour<Bootstrap>.inst.LoadLobby();
		}
	}
}