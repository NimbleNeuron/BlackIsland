using System;

namespace Blis.Client
{
	[Serializable]
	public abstract class BaseParam : CloneableInstance
	{
		public virtual bool UseWeapon => false;


		public virtual string GetDisplayName()
		{
			return "";
		}
	}
}