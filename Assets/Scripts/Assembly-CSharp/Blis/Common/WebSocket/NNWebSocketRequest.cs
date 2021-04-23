using System;
using System.Collections;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class NNWebSocketRequest : IEnumerator
	{
		protected Exception exception;
		protected DateTime requestTime = DateTime.UtcNow;
		protected NNWebSocketMessage response;

		public NNWebSocketRequest(WebSocketRequest req, Type responseType)
		{
			RequestId = req.id;
			RequestMethod = req.method;
			ResultType = responseType;
			State = NNWebSocketRequestStates.Unsent;
		}

		public long RequestId { get; protected set; }
		
		public NNWebSocketRequestStates State { get; protected set; }
		
		public Type ResultType { get; protected set; }
		
		public string RequestMethod { get; protected set; }
		
		public NNWebSocketMessage Response {
			get => response;
			internal set
			{
				if (!State.IsFinal())
				{
					response = value;
					if (response == null)
					{
						Exception = new Exception("Null response");
						return;
					}

					if (response.code == 200)
					{
						State = NNWebSocketRequestStates.Done;
						return;
					}

					Exception = new NNWebSocketException(response);
				}
			}
		}
		
		public Exception Exception {
			get => exception;
			set
			{
				if (!State.IsFinal())
				{
					exception = value;
					if (exception is TimeoutException)
					{
						State = NNWebSocketRequestStates.TimedOut;
						return;
					}

					State = NNWebSocketRequestStates.Error;
				}
			}
		}

		public DateTime RequestTime {
			get => requestTime;
			internal set
			{
				if (State == NNWebSocketRequestStates.Unsent)
				{
					requestTime = value;
					State = NNWebSocketRequestStates.Sent;
				}
			}
		}

		public TimeSpan Elasped => DateTime.UtcNow - RequestTime;
		public object Current => null;

		public bool MoveNext()
		{
			return !State.IsFinal();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public T Result<T>()
		{
			T result;
			try
			{
				result = JsonConvert.DeserializeObject<WebSocketResult<T>>(Response.json).result;
			}
			catch (Exception ex)
			{
				Log.E("[IgnameReqest] e = {0}", ex.Message);
				result = default;
			}

			return result;
		}
	}
}