using System;
using Blis.Common;

namespace Blis.Client
{
	public class LocalSkillScriptManager : TypeAggregator<SkillId, SkillScriptAttribute, LocalSkillScript>
	{
		static LocalSkillScriptManager()
		{
			if (inst == null)
			{
				inst = new LocalSkillScriptManager();
			}
		}


		private LocalSkillScriptManager() { }


		
		public static LocalSkillScriptManager inst { get; }


		protected override SkillId GetAggregateType(SkillScriptAttribute attr)
		{
			if (attr == null)
			{
				return SkillId.None;
			}

			return attr.skillId;
		}


		public SkillId GetSkillId(LocalSkillScript localSkillScript)
		{
			Type type = localSkillScript.GetType();
			SkillId result;
			if (!revertTypeMap.TryGetValue(type, out result))
			{
				throw new Exception("SkillScript has no own attribute : " + type);
			}

			return result;
		}
	}
}