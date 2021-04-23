using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive2ShieldBreak)]
	public class ChiaraActive2ShieldBreak : SkillScript
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
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A2ShieldState).group, Caster.ObjectId);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}