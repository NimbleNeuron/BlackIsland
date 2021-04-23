using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.EmmaActive3)]
	public class EmmaActive3 : EmmaSkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			LookAtPosition(Caster, Target.Position);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			if (Target != null)
			{
				LaunchMagicRabbitBeamProjectile(SkillSlotIndex.Active3, Target.ObjectId);
			}

			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}
	}
}