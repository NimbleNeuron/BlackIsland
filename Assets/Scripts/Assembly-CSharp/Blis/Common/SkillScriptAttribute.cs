using System;

namespace Blis.Common
{
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class SkillScriptAttribute : Attribute
	{
		
		public SkillScriptAttribute(SkillId skillId)
		{
			this.skillId = skillId;
		}

		
		public readonly SkillId skillId;
	}
}
