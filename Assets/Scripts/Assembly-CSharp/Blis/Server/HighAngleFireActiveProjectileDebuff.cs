using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HighAngleFireActiveProjectileDebuff)]
	public class HighAngleFireActiveProjectileDebuff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Target.BlockAllySight(BlockedSightType.HighAngleFireActiveProjectileDebuff, true);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Target.BlockAllySight(BlockedSightType.HighAngleFireActiveProjectileDebuff, false);
		}
	}
}