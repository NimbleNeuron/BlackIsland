using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Vuplex.WebView
{
	
	public static class Utils
	{
		
		public static byte[] ConvertAndroidByteArray(AndroidJavaObject arrayObject)
		{
			// co: sbyte[] to byte[]
			return (byte[])AndroidJNIHelper.ConvertFromJNIArray<byte[]>(arrayObject.GetRawObject());
		}

		
		public static Material CreateDefaultMaterial()
		{
			return new Material(Resources.Load<Material>("DefaultViewportMaterial"));
		}

		
		public static string GetGraphicsApiErrorMessage(GraphicsDeviceType activeGraphicsApi, GraphicsDeviceType[] acceptableGraphicsApis)
		{
			if (Array.IndexOf<GraphicsDeviceType>(acceptableGraphicsApis, activeGraphicsApi) != -1)
			{
				return null;
			}
			IEnumerable<string> source = from api in acceptableGraphicsApis.ToList<GraphicsDeviceType>()
			select api.ToString();
			string arg = string.Join(" or ", source.ToArray<string>());
			return string.Format("Unsupported graphics API: Vuplex 3D WebView requires {0} for this platform, but the selected graphics API is {1}. Please go to Player Settings and set \"Graphics APIs\" to {0}.", arg, activeGraphicsApi);
		}

		
		public static void ThrowExceptionIfAbnormallyLarge(int width, int height)
		{
			if (width * height > 14700000)
			{
				throw new ArgumentException(string.Format("The application specified an abnormally large webview size ({0}px x {1}px), and webviews of this size are normally only created by mistake. A webview's default resolution is 1300px per Unity unit, so it's likely that you specified a large physical size by mistake or need to adjust the resolution. For more information, please see IWebView.SetResolution: https://developer.vuplex.com/webview/IWebView#SetResolution", width, height));
			}
		}
	}
}
