using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WolfHowlingBuff)]
	public class WolfHowlingBuff : SkillScript
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
			WorldMonster worldMonster = Target.Character as WorldMonster;
			if (worldMonster == null)
			{
				return;
			}

			if (worldMonster.ObjectId == Caster.ObjectId)
			{
				return;
			}

			WorldObject target = Caster.GetTarget();
			if (target != null)
			{
				worldMonster.Controller.TargetOn(target);
			}
		}
	}
}