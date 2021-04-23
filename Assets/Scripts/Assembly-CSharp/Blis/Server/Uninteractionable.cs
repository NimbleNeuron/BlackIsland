using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Uninteractionable)]
	public class Uninteractionable : CrowdControlScript
	{
		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}