using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DualSwordActive_2)]
	public class DualSwordActive_2 : DualSwordActive_1
	{
		
		protected override bool IsChangedSkillSequence()
		{
			return true;
		}
	}
}