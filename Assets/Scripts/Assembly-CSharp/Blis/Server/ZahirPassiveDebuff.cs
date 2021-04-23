using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ZahirPassiveDebuff)]
	public class ZahirPassiveDebuff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Caster.AttachSight(Target.WorldObject, 1f, StateDuration, true);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}
	}
}