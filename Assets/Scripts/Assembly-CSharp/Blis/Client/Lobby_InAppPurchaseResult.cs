using System;
using Blis.Common;
using Blis.Common.Utils;
using Neptune.Http;
using UnityEngine;

namespace Blis.Client
{
	public class Lobby_InAppPurchaseResult : MonoBehaviourInstance<Lobby_InAppPurchaseResult>
	{
		private readonly int maxRetryCount = default;
		private int currentRetryCount = default;
		
		private string inAppProductId = "";

		public void RequestIAPReward_Steam(string productId, string orderId)
		{
			currentRetryCount = 0;
			Func<HttpRequest> req = ProductApi.InAppReceiptSteam(orderId);
			RequestIAPResult(req, ProductApi.InAppReceiptSteam(orderId), productId, orderId);
		}
		
		private void RequestIAPResult(Func<HttpRequest> req, Func<HttpRequest> retryHttp, string productId,
			string orderId)
		{
			inAppProductId = productId;
			RequestDelegate.request<ProductApi.PurchaseResult>(req, false,
				(err, res) =>
				{
					if (err != null)
					{
						if (err.errorType == RestErrorType.PAYMENT_PURCHASE_CANCELED)
						{
							MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("결제를 취소 하였습니다"), new Popup.Button
							{
								type = Popup.ButtonType.Confirm,
								text = Ln.Get("확인"),
								callback =
									(Action) (() => MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Close())
							});
						}
						else if (currentRetryCount < maxRetryCount)
						{
							MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("상품 수령 실패"), new Popup.Button
							{
								type = Popup.ButtonType.Confirm,
								text = Ln.Get("재시도"),
								callback = (Action) (() =>
								{
									++currentRetryCount;
									RequestIAPResult(retryHttp, retryHttp, productId, orderId);
								})
							}, new Popup.Button
							{
								type = Popup.ButtonType.Cancel,
								text = Ln.Get("확인")
							});
						}
						else
						{
							MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("상품 구매 오류 발생", orderId),
								new Popup.Button
								{
									type = Popup.ButtonType.Confirm,
									text = Ln.Get("고객문의"),
									callback = (Action) (() => Application.OpenURL(Ln.Get("고객지원 링크")))
								}, new Popup.Button
								{
									type = Popup.ButtonType.Cancel,
									text = Ln.Get("확인"),
									callback = (Action) (() =>
										MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Close())
								});
						}
					}
					else
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("구매가 완료되었습니다."), new Popup.Button
						{
							type = Popup.ButtonType.Confirm,
							text = Ln.Get("확인"),
							callback = (Action) (() => MonoBehaviourInstance<LobbyUI>.inst.ShopProductWindow.Close())
						});
					}
				});
		}


		public void RequestIAPRestore(string failedReceipt)
		{
			currentRetryCount = 0;
			Func<HttpRequest> req = ProductApi.InAppPurchaseRestore(failedReceipt);
			RequestIAPRestore(req, ProductApi.InAppPurchaseRestore(failedReceipt), failedReceipt);
		}


		private void RequestIAPRestore(Func<HttpRequest> req, Func<HttpRequest> retryHttp, string failedReceipt)
		{
			RequestDelegate.request<ProductApi.PurchaseResult>(req, false, (err, res) =>
			{
				if (err != null)
				{
					if (currentRetryCount < maxRetryCount)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("상품 수령 실패"), new Popup.Button
						{
							type = Popup.ButtonType.Confirm,
							text = Ln.Get("재시도"),
							callback = (Action) (() =>
							{
								++currentRetryCount;
								RequestIAPRestore(retryHttp, retryHttp, failedReceipt);
							})
						}, new Popup.Button
						{
							type = Popup.ButtonType.Cancel,
							text = Ln.Get("확인")
						});
					}
					else
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("상품 수령 오류 발생", failedReceipt),
							new Popup.Button
							{
								type = Popup.ButtonType.Confirm,
								text = Ln.Get("고객문의"),
								callback = (Action) (() => Application.OpenURL(Ln.Get("고객지원 링크")))
							}, new Popup.Button
							{
								type = Popup.ButtonType.Cancel,
								text = Ln.Get("확인")
							});
					}
				}
				else
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("수령 완료"), new Popup.Button
					{
						type = Popup.ButtonType.Confirm,
						text = Ln.Get("확인")
					});
				}
			});
		}
	}
}