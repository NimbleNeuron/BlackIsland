using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive4Buff)]
	public class NadineActive4Buff_1 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.BuffState2[SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
		}
	}
}