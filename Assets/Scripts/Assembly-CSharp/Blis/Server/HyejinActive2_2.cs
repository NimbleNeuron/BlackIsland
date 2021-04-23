using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinActive2_2)]
	public class HyejinActive2_2 : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			HyejinActive2_2 hyejinActive22 = this;
			hyejinActive22.Start();
			if (hyejinActive22.SkillCastingTime1 > 0.0)
			{
				yield return hyejinActive22.FirstCastingTime();
			}

			WorldPlayerCharacter character = hyejinActive22.Caster.Character as WorldPlayerCharacter;
			if (character == null)
			{
				Log.E("HyeJin W Skill Seq1 can not cast WorldPlayerCharacter");
				hyejinActive22.Finish();
			}
			else
			{
				WorldSummonBase ownSummon = character.GetOwnSummon(Condition);
				if (ownSummon == null)
				{
					hyejinActive22.Finish();
				}
				else
				{
					ownSummon.Dead(DamageType.Normal);
					if (hyejinActive22.SkillFinishDelayTime > 0.0)
					{
						yield return hyejinActive22.FinishDelayTime();
					}

					hyejinActive22.Finish();
				}
			}

			bool Condition(WorldSummonBase summon)
			{
				return summon.SummonData.code == Singleton<HyejinSkillData>.inst.A2SummonCodeBow ||
				       summon.SummonData.code == Singleton<HyejinSkillData>.inst.A2SummonCodeShuriken;
			}

			// this.Start();
			// if (base.SkillCastingTime1 > 0f)
			// {
			// 	yield return base.FirstCastingTime();
			// }
			// WorldPlayerCharacter worldPlayerCharacter = base.Caster.Character as WorldPlayerCharacter;
			// if (worldPlayerCharacter == null)
			// {
			// 	Log.E("HyeJin W Skill Seq1 can not cast WorldPlayerCharacter");
			// 	this.Finish(false);
			// 	yield break;
			// }
			// WorldSummonBase ownSummon = worldPlayerCharacter.GetOwnSummon(new Func<WorldSummonBase, bool>(HyejinActive2_2.<>c.<>9.<Play>g__Condition|1_0));
			// if (ownSummon == null)
			// {
			// 	this.Finish(false);
			// 	yield break;
			// }
			// ownSummon.Dead(DamageType.Normal);
			// if (base.SkillFinishDelayTime > 0f)
			// {
			// 	yield return base.FinishDelayTime();
			// }
			// this.Finish(false);
			// yield break;
		}
	}
}