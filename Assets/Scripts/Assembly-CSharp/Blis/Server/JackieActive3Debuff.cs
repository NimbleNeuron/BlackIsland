using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive3Debuff)]
	public class JackieActive3Debuff : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}
	}
}