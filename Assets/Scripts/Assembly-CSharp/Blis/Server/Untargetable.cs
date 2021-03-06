using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Untargetable)]
	public class Untargetable : SkillScript
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