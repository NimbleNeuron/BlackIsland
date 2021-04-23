using System;

namespace Blis.Client.UI
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class UIActionMappingAttribute : Attribute
	{
		public readonly Type[] actions;

		public UIActionMappingAttribute(params Type[] actions)
		{
			this.actions = actions;
		}
	}
}