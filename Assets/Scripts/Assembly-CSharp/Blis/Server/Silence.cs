using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Silence)]
	public class Silence : CrowdControlScript
	{
		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}