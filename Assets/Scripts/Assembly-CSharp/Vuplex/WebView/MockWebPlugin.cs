using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	internal class MockWebPlugin : IWebPlugin
	{
		
		
		public static MockWebPlugin Instance
		{
			get
			{
				if (MockWebPlugin._instance == null)
				{
					MockWebPlugin._instance = new MockWebPlugin();
				}
				return MockWebPlugin._instance;
			}
		}

		
		public void ClearAllData()
		{
		}

		
		public void CreateTexture(float width, float height, Action<Texture2D> callback)
		{
			Dispatcher.RunOnMainThread(delegate
			{
				callback(new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false));
			});
		}

		
		public void CreateMaterial(Action<Material> callback)
		{
			Material material = new Material(Resources.Load<Material>("MockViewportMaterial"));
			Texture2D texture2D = new Texture2D(material.mainTexture.width, material.mainTexture.height, (material.mainTexture as Texture2D).format, true);
			Graphics.CopyTexture(material.mainTexture, texture2D);
			material.mainTexture = texture2D;
			Dispatcher.RunOnMainThread(delegate
			{
				callback(material);
			});
		}

		
		public void CreateVideoMaterial(Action<Material> callback)
		{
			callback(null);
		}

		
		public virtual IWebView CreateWebView()
		{
			return MockWebView.Instantiate();
		}

		
		public void SetIgnoreCertificateErrors(bool ignore)
		{
		}

		
		public void SetStorageEnabled(bool enabled)
		{
		}

		
		public void SetUserAgent(bool mobile)
		{
		}

		
		public void SetUserAgent(string userAgent)
		{
		}

		
		private static MockWebPlugin _instance;
	}
}
