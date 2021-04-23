using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DummySkill)]
	public class DummySkill : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			WorldSkillDummy worldSkillDummy = Caster.Owner as WorldSkillDummy;
			int dummySkillStateCode = worldSkillDummy.DummySkillStateCode;
			if (worldSkillDummy.SkillTarget != null)
			{
				CharacterState characterState = CreateState(worldSkillDummy.SkillTarget, dummySkillStateCode, 0,
					worldSkillDummy.DummySkillStateDuration);
				if (dummySkillStateCode.Equals(2000016))
				{
					(characterState as GrabState).Init(Caster.Character, 8f);
				}

				AddState(worldSkillDummy.SkillTarget, characterState);
				yield return WaitForSeconds(worldSkillDummy.DummySkillStateDuration);
				Finish();
			}
			else
			{
				AddState(Caster, dummySkillStateCode);
			}

			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.Character.DestroySelf();
		}
	}
}