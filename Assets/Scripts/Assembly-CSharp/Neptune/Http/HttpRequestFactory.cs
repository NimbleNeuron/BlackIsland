using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Neptune.Http
{
	public static class HttpRequestFactory
	{
		public static HttpRequest Open(string method, string url, object tag = null)
		{
			ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpRequestFactory.Validator);
			HttpRequest request = null;
			request = new NetHttpRequest(method, url);
			Uri uri = new Uri(url);
			
			Blis.Common.Log.V("<color=#86E57F>[API: SEND][{1}] {0}</color>", url, method.ToUpper());
			
			HttpRequestFactory.headers.FindAll((HttpRequestFactory.Header header) => header.Matches(uri)).ForEach(delegate(HttpRequestFactory.Header header)
			{
				request.SetRequestHeader(header.headerName, header.headerValue);
			});
			
			request.KeepAlive = HttpRequestFactory.KeepAlive;
			request.UseCaches = HttpRequestFactory.UseCaches;
			request.Timeout = HttpRequestFactory.Timeout;
			request.MaxRetry = HttpRequestFactory.MaxRetry;
			request.Tag = tag;
			return request;
		}

		
		private static HttpRequest GetInternal(string url, object tag = null)
		{
			HttpRequest httpRequest = HttpRequestFactory.Open("GET", url, tag);
			httpRequest.Send();
			return httpRequest;
		}

		
		public static Func<HttpRequest> Get(string url, object tag = null)
		{
			return () => HttpRequestFactory.GetInternal(url, tag);
		}

		
		private static HttpRequest Delete(string url, object tag = null)
		{
			HttpRequest httpRequest = HttpRequestFactory.Open("DELETE", url, tag);
			httpRequest.Send();
			return httpRequest;
		}

		
		private static HttpRequest Put(string url, object data, object tag = null)
		{
			HttpRequest httpRequest = HttpRequestFactory.Open("PUT", url, tag);
			httpRequest.SendObject(data);
			return httpRequest;
		}

		
		private static HttpRequest PostInternal(string url, object data, object tag = null)
		{
			HttpRequest httpRequest = HttpRequestFactory.Open("POST", url, tag);
			httpRequest.SendObject(data);
			return httpRequest;
		}

		
		public static Func<HttpRequest> Post(string url, object data, object tag = null)
		{
			return () => HttpRequestFactory.PostInternal(url, data, tag);
		}

		
		public static void GetHeaders(string url, IDictionary<string, string> headers)
		{
			Uri uri = new Uri(url);
			HttpRequestFactory.headers.FindAll((HttpRequestFactory.Header header) => header.Matches(uri)).ForEach(delegate(HttpRequestFactory.Header header)
			{
				headers[header.headerName] = header.headerValue;
			});
		}

		
		private static void SetHeader(HttpRequestFactory.Header header)
		{
			HttpRequestFactory.Header header2 = HttpRequestFactory.headers.Find((HttpRequestFactory.Header h) => h.Matches(header));
			if (header2 == null)
			{
				if (header.headerValue != null)
				{
					HttpRequestFactory.headers.Add(header);
				}
				return;
			}
			if (header.headerValue != null)
			{
				header2.headerValue = header.headerValue;
				return;
			}
			HttpRequestFactory.headers.Remove(header2);
		}

		
		public static void SetHeader(string hostName, int port, string headerName, string headerValue)
		{
			HttpRequestFactory.SetHeader(new HttpRequestFactory.Header
			{
				hostName = hostName,
				port = new int?(port),
				headerName = headerName,
				headerValue = headerValue
			});
		}

		
		public static void SetHeader(string hostName, string headerName, string headerValue)
		{
			HttpRequestFactory.SetHeader(new HttpRequestFactory.Header
			{
				hostName = hostName,
				headerName = headerName,
				headerValue = headerValue
			});
		}

		
		public static void SetHeader(string headerName, string headerValue)
		{
			HttpRequestFactory.SetHeader(new HttpRequestFactory.Header
			{
				headerName = headerName,
				headerValue = headerValue
			});
		}

		
		public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		
		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			Debug.Log("Certificate error: " + sslPolicyErrors);
			return false;
		}

		
		public static bool CheckValidationResult(ServicePoint sp, X509Certificate certificate, WebRequest request, int error)
		{
			return error == 0;
		}

		
		public static void CheckValidate()
		{
		}

		
		private static List<HttpRequestFactory.Header> headers = new List<HttpRequestFactory.Header>();

		
		public static bool KeepAlive = true;

		
		public static bool UseCaches = false;

		
		public static int Timeout = 30000;

		
		public static int MaxRetry = 1;

		
		private class Header
		{
			
			internal bool Matches(Uri uri)
			{
				return (this.hostName == null || !(this.hostName != uri.Host)) && (this.port == null || this.port.Value == uri.Port);
			}

			
			internal bool Matches(HttpRequestFactory.Header header)
			{
				if (this.hostName != header.hostName)
				{
					return false;
				}
				int? num = this.port;
				int? num2 = header.port;
				return (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)) && !(this.headerName != header.headerName);
			}

			
			internal string hostName;

			
			internal int? port;

			
			internal string headerName;

			
			internal string headerValue;
		}
	}
}
