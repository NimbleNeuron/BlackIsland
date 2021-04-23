using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public static class LocalPingUtil
	{
		private static readonly Color selectColor = new Color32(203, 203, 203, byte.MaxValue);


		private static readonly Color targetColor = new Color32(221, 71, 42, byte.MaxValue);


		private static readonly Color escapeColor = new Color32(221, 71, 42, byte.MaxValue);


		private static readonly Color helpColor = new Color32(93, 204, 228, byte.MaxValue);


		private static readonly Color warningColor = new Color32(227, 217, 134, byte.MaxValue);


		private static readonly Color runColor = new Color32(218, 135, 221, byte.MaxValue);


		public static Color GetPingColor(PingType type)
		{
			Color black = Color.black;
			switch (type)
			{
				case PingType.Run:
					black = runColor;
					break;
				case PingType.Warning:
					black = warningColor;
					break;
				case PingType.Escape:
					black = escapeColor;
					break;
				case PingType.Help:
					black = helpColor;
					break;
				case PingType.Target:
					black = targetColor;
					break;
				case PingType.Select:
					black = selectColor;
					break;
			}

			return black;
		}
	}
}