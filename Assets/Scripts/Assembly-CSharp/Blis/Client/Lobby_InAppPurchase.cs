using Blis.Common;
using Blis.Common.Utils;
using Steamworks;

namespace Blis.Client
{
	public class Lobby_InAppPurchase : MonoBehaviourInstance<Lobby_InAppPurchase>
	{
		private string iapProductIdCache;

		private void Start()
		{
			SteamApi.InitMicroTxn(CallBackSteamTxn);
		}


		public bool PurchaseItem(string productId)
		{
			try
			{
				RequestPayLoad(productId);
			}
			catch
			{
				return false;
			}

			return true;
		}


		private void RequestPayLoad(string productId)
		{
			if (MonoBehaviourInstance<LobbyService>.inst.LobbyState != LobbyState.Ready)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 중 결제 불가"), new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("확인")
				});
				return;
			}

			iapProductIdCache = productId;
			RequestDelegate.request<ProductApi.PayloadResult>(ProductApi.MakePayload(productId),
				delegate(RequestDelegateError err, ProductApi.PayloadResult res)
				{
					if (err != null)
					{
						MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
					}
				});
		}


		private void CallBackSteamTxn(MicroTxnAuthorizationResponse_t callback)
		{
			Log.H(string.Format("스팀 결제 승인 결과 : {0}", callback.m_bAuthorized > 0));
			MonoBehaviourInstance<Lobby_InAppPurchaseResult>.inst.RequestIAPReward_Steam(iapProductIdCache,
				callback.m_ulOrderID.ToString());
		}
	}
}