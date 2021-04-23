using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.TonfaHighSpeedSpin)]
	public class TonfaHighSpeedSpin : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}