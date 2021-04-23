using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[Serializable]
	public class Cookie
	{
		
		
		public bool IsValid
		{
			get
			{
				bool result = true;
				if (this.Name == null)
				{
					Debug.LogWarning("Invalid value for Cookie.Name: " + this.Name);
					result = false;
				}
				if (this.Value == null)
				{
					Debug.LogWarning("Invalid value for Cookie.Value: " + this.Value);
					result = false;
				}
				if (this.Domain == null || !this.Domain.Contains(".") || this.Domain.Contains("/"))
				{
					Debug.LogWarning("Invalid value for Cookie.Domain: " + this.Domain);
					result = false;
				}
				if (this.Path == null)
				{
					Debug.LogWarning("Invalid value for Cookie.Path: " + this.Path);
					result = false;
				}
				return result;
			}
		}

		
		public static Cookie FromJson(string serializedCookie)
		{
			if (serializedCookie == "null")
			{
				return null;
			}
			return JsonUtility.FromJson<Cookie>(serializedCookie);
		}

		
		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		
		public override string ToString()
		{
			return this.ToJson();
		}

		
		public string Name;

		
		public string Value;

		
		public string Domain;

		
		public string Path = "/";

		
		public int ExpirationDate;

		
		public bool HttpOnly;

		
		public bool Secure;
	}
}
