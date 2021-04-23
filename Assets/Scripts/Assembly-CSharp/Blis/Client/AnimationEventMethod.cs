using System;

namespace Blis.Client
{
	public class AnimationEventMethod : Attribute
	{
		public string category;
		public string desc;
		public Type descType;
		public string label;
		public Type type;
	}
}