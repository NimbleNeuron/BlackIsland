using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CursorData
	{
		private readonly Dictionary<CursorOption, CursorInfo> cursorInfos = new Dictionary<CursorOption, CursorInfo>();


		public readonly CursorMode cursorMode;


		public readonly CursorTarget cursorTarget;

		public CursorData(CursorMode mode, CursorTarget target, string name, Vector2 uv)
		{
			cursorMode = mode;
			cursorTarget = target;
			AddOption(CursorOption.None, name, uv);
		}


		public CursorData AddOption(CursorOption option, string name, Vector2 uv)
		{
			if (cursorInfos.ContainsKey(option))
			{
				cursorInfos.Remove(option);
			}

			CursorInfo cursorInfo = new CursorInfo
			{
				tex = SingletonMonoBehaviour<ResourceManager>.inst.GetCursorImage(name)
			};
			if (cursorInfo.tex != null)
			{
				cursorInfo.pointPixel = new Vector2(cursorInfo.tex.width, cursorInfo.tex.height) * uv;
			}
			else
			{
				cursorInfo.pointPixel = Vector2.zero;
			}

			cursorInfos.Add(option, cursorInfo);
			return this;
		}


		public Vector2 GetPixelPoint(CursorOption option)
		{
			if (cursorInfos.ContainsKey(option))
			{
				return cursorInfos[option].pointPixel;
			}

			return cursorInfos[CursorOption.None].pointPixel;
		}


		public Texture2D GetCursorTexture(CursorOption option)
		{
			if (cursorInfos.ContainsKey(option))
			{
				return cursorInfos[option].tex;
			}

			return cursorInfos[CursorOption.None].tex;
		}


		private struct CursorInfo
		{
			public Vector2 pointPixel;


			public Texture2D tex;
		}
	}
}