using System;

namespace Blis.Client
{
	[Serializable]
	public abstract class CloneableInstance
	{
		public abstract object Clone();
	}
}