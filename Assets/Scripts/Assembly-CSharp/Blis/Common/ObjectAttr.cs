using System;

namespace Blis.Common
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ObjectAttr : Attribute
	{
		public readonly ObjectType objectType;

		public ObjectAttr(ObjectType objectType)
		{
			this.objectType = objectType;
		}
	}
}