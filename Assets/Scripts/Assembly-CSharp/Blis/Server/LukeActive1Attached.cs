using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive1Attached)]
	public class LukeActive1Attached : SkillScript
	{
		
		private ServerSightAgent sharedSightAgent;

		
		private int sharedSightAgentTargetId;

		
		protected override void Start()
		{
			base.Start();
			if (Target != null)
			{
				sharedSightAgentTargetId = Target.ObjectId;
				sharedSightAgent = Caster.AttachSight(Target.WorldObject,
					Singleton<LukeSkillActive1_1Data>.inst.DebuffSightRange, 0f, false);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.RemoveStateByGroup(Singleton<LukeSkillActive1_1Data>.inst.BuffGroupCode, Caster.ObjectId);
			if (sharedSightAgent != null)
			{
				Caster.RemoveSight(sharedSightAgent, sharedSightAgentTargetId);
			}
		}
	}
}