using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SilviaBikeSlowDebuffState)]
	public class SilviaBikeSlowDebuffState : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}