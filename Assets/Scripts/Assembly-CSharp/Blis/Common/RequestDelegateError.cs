using System;
using Neptune.Http;

namespace Blis.Common
{
	
	public class RequestDelegateError
	{
		public enum Type
		{
			server,
			http,
			exception
		}
		
		public readonly RestErrorType errorType;
		public readonly string message;
		public readonly Type type;
		
		public RequestDelegateError(Type type, string message)
		{
			this.type = type;
			errorType = RestErrorType.UNKNOWN;
			this.message = message;
			Log.V("[RequestDelegateError] {0}: {1}", type, message);
		}

		
		public RequestDelegateError(int code, RestErrorType restErrorType)
		{
			type = Type.server;
			errorType = restErrorType;
			message = code.ToString();
		}

		
		public static RequestDelegateError CreateByServerError(int code, RestErrorType restErrorType)
		{
			return new RequestDelegateError(code, restErrorType);
		}

		
		public static RequestDelegateError CreateByException(Exception e)
		{
			string text = e.Message;
			if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
			{
				text = e.InnerException.Message;
			}

			return new RequestDelegateError(Type.exception, text);
		}

		
		public static RequestDelegateError CreateHttpError(HttpResultStates httpStatus, string msg)
		{
			string str = "Http" + httpStatus;
			if (!string.IsNullOrEmpty(msg))
			{
				str = str + "_" + msg;
			}

			return new RequestDelegateError(Type.http, str);
		}
	}
}