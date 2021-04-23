using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaWilsonUnion)]
	public class SisselaWilsonUnion : SisselaSkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}
	}
}