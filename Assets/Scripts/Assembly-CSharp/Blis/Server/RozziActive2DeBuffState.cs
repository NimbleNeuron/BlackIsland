using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive2DeBuffState)]
	public class RozziActive2DeBuffState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}