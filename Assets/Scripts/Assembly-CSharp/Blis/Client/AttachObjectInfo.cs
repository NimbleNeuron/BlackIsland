using System;
using UnityEngine;

namespace Blis.Client
{
	[Serializable]
	public class AttachObjectInfo : BaseParam
	{
		public string childObjectPath = "";


		public string resourceName = "";


		public Vector3 rotation;


		public string tag = "";

		public override string GetDisplayName()
		{
			return resourceName;
		}


		public override object Clone()
		{
			return new AttachObjectInfo
			{
				childObjectPath = childObjectPath,
				resourceName = resourceName,
				rotation = rotation,
				tag = tag
			};
		}
	}
}