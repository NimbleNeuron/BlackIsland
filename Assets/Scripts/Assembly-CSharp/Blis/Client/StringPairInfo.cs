using System;

namespace Blis.Client
{
	[Serializable]
	public class StringPairInfo : BaseParam
	{
		public string value1 = "";


		public string value2 = "";


		public string tag = "";


		public override object Clone()
		{
			return new StringPairInfo
			{
				value1 = value1,
				value2 = value2,
				tag = tag
			};
		}
	}
}