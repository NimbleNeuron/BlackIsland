using System;

namespace Blis.Client
{
	[Serializable]
	public class StringTag : BaseParam
	{
		public string tag = "";


		public override object Clone()
		{
			return new StringTag
			{
				tag = tag
			};
		}
	}
}