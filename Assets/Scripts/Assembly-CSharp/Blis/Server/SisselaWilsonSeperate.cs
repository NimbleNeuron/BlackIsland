using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaWilsonSeperate)]
	public class SisselaWilsonSeperate : SisselaSkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}
	}
}