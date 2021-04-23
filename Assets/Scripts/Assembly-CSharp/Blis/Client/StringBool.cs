using System;

namespace Blis.Client
{
	[Serializable]
	public class StringBool : BaseParam
	{
		public string key = "";


		public bool value;


		public override object Clone()
		{
			return new StringBool
			{
				key = key,
				value = value
			};
		}
	}
}