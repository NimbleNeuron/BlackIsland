using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaActive4)]
	public class SisselaActive4 : SisselaSkillScript
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

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			List<WorldPlayerCharacter> list =
				MonoBehaviourInstance<GameService>.inst.World.FindAll<WorldPlayerCharacter>(x => x.IsAlive);
			int skillLevel = SkillLevel;
			foreach (WorldPlayerCharacter worldPlayerCharacter in list)
			{
				if (worldPlayerCharacter.ObjectId == Caster.ObjectId ||
				    Caster.GetHostileType(worldPlayerCharacter) == HostileType.Enemy)
				{
					AddState(worldPlayerCharacter.SkillAgent,
						Singleton<SisselaSkillData>.inst.A4MarkStateCode[skillLevel]);
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}