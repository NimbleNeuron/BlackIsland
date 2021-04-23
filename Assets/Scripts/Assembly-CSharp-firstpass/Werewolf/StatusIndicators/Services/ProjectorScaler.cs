using System.Collections.Generic;
using UnityEngine;

namespace Werewolf.StatusIndicators.Services
{
	public class ProjectorScaler
	{
		public static void Resize(Projector projector, float scale)
		{
			if (projector != null)
			{
				projector.orthographicSize = scale / 2f;
			}
		}


		public static void Resize(Projector projector, ScalingType scaling, float scale, float width)
		{
			if (projector != null && scaling != ScalingType.None)
			{
				if (scaling == ScalingType.LengthOnly)
				{
					projector.aspectRatio = width / scale;
				}
				else
				{
					projector.aspectRatio = 1f;
				}

				projector.orthographicSize = scale / 2f;
			}
		}


		public static void Resize(List<Projector> projectors, ScalingType scaling, float scale, float width)
		{
			foreach (Projector projector in projectors)
			{
				Resize(projector, scaling, scale, width);
			}
		}


		public static void Resize(Projector[] projectors, float scale)
		{
			for (int i = 0; i < projectors.Length; i++)
			{
				Resize(projectors[i], scale);
			}
		}
	}
}