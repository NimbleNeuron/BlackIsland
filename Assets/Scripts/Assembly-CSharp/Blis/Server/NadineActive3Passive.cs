using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive3Passive)]
	public class NadineActive3Passive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			OnUpgradePassiveSkill();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive3Data>.inst.BuffState1[SkillLevel]);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				if (Caster.FindStateByGroup(data.group, Caster.ObjectId).Level < data.level)
				{
					Caster.OverwriteState(data.code, Caster.ObjectId);
				}
			}
			else
			{
				AddState(Caster, data.code);
			}
		}
	}
}