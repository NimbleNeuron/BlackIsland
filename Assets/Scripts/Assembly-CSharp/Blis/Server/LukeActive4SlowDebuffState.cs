using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive4SlowDebuffState)]
	public class LukeActive4SlowDebuffState : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}