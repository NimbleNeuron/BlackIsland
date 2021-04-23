using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Suppressed)]
	public class Suppressed : CrowdControlScript
	{
		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}