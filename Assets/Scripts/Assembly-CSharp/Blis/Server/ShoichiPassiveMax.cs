using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiPassiveMax)]
	public class ShoichiPassiveMax : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}