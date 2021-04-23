using System;
using System.Collections;
using UnityEngine;

namespace Blis.Common
{
	public abstract class AuthTokenAcquirer
	{
		private const float TIME_OUT = 10f;

		public readonly AuthProvider authProvider;
		private AuthToken authToken;
		private string err;
		private bool isFinish;

		protected AuthTokenAcquirer()
		{
			authProvider = AuthProvider.NONE;
		}

		protected AuthTokenAcquirer(AuthProvider authProvider)
		{
			this.authProvider = authProvider;
		}

		public IEnumerator FetchToken()
		{
			float time = 0f;
			isFinish = false;

			FetchTokenInternal();
			
			while (!isFinish)
			{
				time += Time.deltaTime;
				Log.V("AuthTokenRequest - time: " + time);
				if (time > 10f)
				{
					isFinish = true;
					break;
				}

				yield return null;
			}
		}

		public bool HasError()
		{
			return !string.IsNullOrEmpty(err);
		}

		public string GetError()
		{
			return err;
		}

		public bool HasToken()
		{
			return authToken != null;
		}

		public AuthToken GetToken()
		{
			return authToken;
		}
		
		public bool IsTimeOut()
		{
			return isFinish && err == null && authToken == null;
		}

		protected void Finish(string error, AuthToken token)
		{
			isFinish = true;
			err = error;
			authToken = token;
		}

		protected abstract void FetchTokenInternal();
		
		public static AuthTokenAcquirer Create(AuthProvider authProvider)
		{
			switch (authProvider)
			{
				case AuthProvider.MACHINE:
					throw new NotImplementedException();
				case AuthProvider.STEAM:
					return new AuthTokenAcquirer_Steam();
			}

			return new AuthTokenAcquirer_Guest();
		}
	}
}