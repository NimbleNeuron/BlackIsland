using System;

namespace Blis.Client
{
	[Serializable]
	public class StringInfo : BaseParam
	{
		public string resourceName = "";


		public string tag = "";


		public override object Clone()
		{
			return new StringInfo
			{
				resourceName = resourceName,
				tag = tag
			};
		}
	}
}