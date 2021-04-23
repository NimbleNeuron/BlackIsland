using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.MagnusPassive)]
	public class MagnusPassive : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<MagnusSkillPassiveData>.inst.BuffState[SkillLevel]);
			AddState(Caster, data.code);
			yield return WaitForFrame();
		}
	}
}