using System;
using System.Collections;
using System.Net;
using Neptune.Http;
using UnityEngine;

namespace Blis.Common
{
	public class RequestDelegate : SingletonMonoBehaviour<RequestDelegate>
	{
		public delegate void ResponseCallback<T>(RequestDelegateError error, T result);


		public delegate void ServerDisruption(Action retryAction);


		public delegate void StartWaitRequest();


		public delegate void StopWaitRequest();


		public ServerDisruption OnServerDisruption;


		public StartWaitRequest OnStartWaitRequest;


		public StopWaitRequest OnStopWaitRequest;

		public static void request<T>(Func<HttpRequest> req, ResponseCallback<T> responseCallback) where T : class
		{
			request<T>(req, responseCallback, true);
		}


		public static void request<T>(Func<HttpRequest> req, ResponseCallback<T> responseCallback, bool isRestApi)
			where T : class
		{
			request<T>(req, true, responseCallback, isRestApi);
		}


		public static void request<T>(Func<HttpRequest> req, bool isHandleHttpError,
			ResponseCallback<T> responseCallback) where T : class
		{
			request<T>(req, isHandleHttpError, responseCallback, true);
		}


		public static void request<T>(Func<HttpRequest> req, bool isHandleHttpError,
			ResponseCallback<T> responseCallback, bool isRestApi) where T : class
		{
			Instance._request<T>(req, isHandleHttpError, responseCallback, isRestApi);
		}


		public void _request<T>(Func<HttpRequest> req, bool isHandleHttpError, ResponseCallback<T> responseCallback,
			bool isRestApi) where T : class
		{
			StartCoroutine(waitingResponse<T>(req, isHandleHttpError, responseCallback, isRestApi));
		}


		public static Coroutine requestCoroutine<T>(Func<HttpRequest> req, ResponseCallback<T> responseCallback)
			where T : class
		{
			return requestCoroutine<T>(req, responseCallback, true);
		}


		public static Coroutine requestCoroutine<T>(Func<HttpRequest> req, ResponseCallback<T> responseCallback,
			bool isRestApi) where T : class
		{
			return Instance._requestCoroutine<T>(req, responseCallback, isRestApi);
		}


		public Coroutine _requestCoroutine<T>(Func<HttpRequest> req, ResponseCallback<T> responseCallback,
			bool isRestApi) where T : class
		{
			return StartCoroutine(waitingResponse<T>(req, true, responseCallback, isRestApi));
		}


		private IEnumerator waitingResponse<T>(Func<HttpRequest> request, bool isHandleHttpError,
			ResponseCallback<T> responseCallback, bool isRestApi) where T : class
		{
			HttpRequest req = request();
			StartWaitRequest onStartWaitRequest = OnStartWaitRequest;
			if (onStartWaitRequest != null)
			{
				onStartWaitRequest();
			}

			yield return StartCoroutine(req);
			StopWaitRequest onStopWaitRequest = OnStopWaitRequest;
			if (onStopWaitRequest != null)
			{
				onStopWaitRequest();
			}

			if (!req.ResultState.IsOK())
			{
				Log.W("HttpRequest Error [{2}] : method={0}, url={1}, responseText={3}", req.Method, req.Url,
					req.StatusCode, req.ResponseText);
				if (responseCallback != null)
				{
					string text = "";
					if (req.Exception != null)
					{
						text = req.Exception is WebException ? "" : req.Exception.ToString();
					}

					Log.E("HandledError: {0}", isHandleHttpError);
					if (isHandleHttpError)
					{
						responseCallback(RequestDelegateError.CreateHttpError(req.ResultState, text), default);
						if (ApiErrorHandler.OnHttpError(req.ResultState, req.Exception.ToString()))
						{
							req.Dispose();
							yield break;
						}
					}

					Log.E("Result State: {0} | Message: {1}", req.ResultState, text);
					if (OnServerDisruption != null)
					{
						OnServerDisruption(delegate
						{
							RequestRetry<T>(request, isHandleHttpError, responseCallback, isRestApi);
						});
					}
					else
					{
						responseCallback(RequestDelegateError.CreateHttpError(req.ResultState, text), default);
						req.Dispose();
					}
				}

				yield break;
			}

			ApiResultContainer<T> apiResultContainer = null;
			try
			{
				if (isRestApi)
				{
					apiResultContainer = req.ResponseJson<ApiResultContainer<T>>();
				}
				else
				{
					T results = req.ResponseJson<T>();
					apiResultContainer = new ApiResultContainer<T>
					{
						code = 200,
						msg = "",
						results = results
					};
				}
			}
			catch (Exception e)
			{
				if (responseCallback != null)
				{
					responseCallback(RequestDelegateError.CreateByException(e), default);
					yield break;
				}
			}
			finally
			{
				req.Dispose();
			}

			if (apiResultContainer == null)
			{
				if (responseCallback != null)
				{
					responseCallback(null, Activator.CreateInstance<T>());
				}

				yield break;
			}

			if (apiResultContainer.code != 200 &&
			    ApiErrorHandler.OnServerError((RestErrorType) apiResultContainer.code, apiResultContainer.msg))
			{
				yield break;
			}

			T t = apiResultContainer.results;
			if (t == null)
			{
				Log.W("[RequestDelegate] result empty");
				t = Activator.CreateInstance<T>();
			}

			if (responseCallback != null)
			{
				RequestDelegateError error = null;
				if (apiResultContainer.code != 200)
				{
					error = RequestDelegateError.CreateByServerError(apiResultContainer.code,
						(RestErrorType) apiResultContainer.code);
				}

				responseCallback(error, t);
			}
		}


		public static void RequestRetry<T>(Func<HttpRequest> req, bool isHandleHttpError,
			ResponseCallback<T> responseCallback, bool isRestApi) where T : class
		{
			request<T>(req, isHandleHttpError, responseCallback, isRestApi);
		}
	}
}