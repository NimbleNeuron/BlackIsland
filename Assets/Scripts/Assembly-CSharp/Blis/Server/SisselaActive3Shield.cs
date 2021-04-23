using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive3Shield)]
	public class SisselaActive3Shield : SisselaSkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return FirstCastingTime();
		}
	}
}