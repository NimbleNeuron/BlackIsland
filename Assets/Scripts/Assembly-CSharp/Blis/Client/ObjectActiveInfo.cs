using System;

namespace Blis.Client
{
	[Serializable]
	public class ObjectActiveInfo : BaseParam
	{
		public string objectPath = "";


		public bool active;


		public override object Clone()
		{
			return new ObjectActiveInfo
			{
				objectPath = objectPath,
				active = active
			};
		}
	}
}