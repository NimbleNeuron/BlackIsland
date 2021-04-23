using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive3Buff)]
	public class HartActive3Buff : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
			Finish();
		}
	}
}