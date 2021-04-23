using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.GlovePassive)]
	public class GlovePassive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<GloveSkillActiveData>.inst.BuffState[SkillLevel]);
			Caster.RemoveStateByGroup(data.GroupData.group, Caster.ObjectId);
		}
	}
}