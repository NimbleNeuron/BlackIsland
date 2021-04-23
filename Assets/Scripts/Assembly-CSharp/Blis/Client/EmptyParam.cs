using System;

namespace Blis.Client
{
	[Serializable]
	public class EmptyParam : BaseParam
	{
		public override object Clone()
		{
			return new EmptyParam();
		}
	}
}