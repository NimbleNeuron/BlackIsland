using System;

namespace Blis.Common
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class SnapshotAttr : Attribute
	{
		public readonly ObjectType objectType;


		public SnapshotAttr(ObjectType objectType)
		{
			this.objectType = objectType;
		}
	}
}