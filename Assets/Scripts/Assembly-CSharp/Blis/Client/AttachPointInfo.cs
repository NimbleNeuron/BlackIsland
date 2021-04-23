using System;
using UnityEngine;

namespace Blis.Client
{
	[Serializable]
	public class AttachPointInfo : BaseParam
	{
		public Vector3 positionOffset;


		public Vector3 rotation;


		public string resourceName = "";


		public string tag = "";


		public override string GetDisplayName()
		{
			return resourceName;
		}


		public override object Clone()
		{
			return new AttachPointInfo
			{
				positionOffset = positionOffset,
				rotation = rotation,
				resourceName = resourceName,
				tag = tag
			};
		}
	}
}