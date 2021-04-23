using Blis.Common;

namespace Blis.Server
{
	
	public class SkillScriptManager : TypeAggregator<SkillId, SkillScriptAttribute, SkillScript>
	{
		
		
		
		public static SkillScriptManager inst { get; private set; }

		
		static SkillScriptManager()
		{
			if (SkillScriptManager.inst == null)
			{
				SkillScriptManager.inst = new SkillScriptManager();
			}
		}

		
		private SkillScriptManager()
		{
		}

		
		protected override SkillId GetAggregateType(SkillScriptAttribute attr)
		{
			if (attr == null)
			{
				return SkillId.None;
			}
			return attr.skillId;
		}
	}
}
