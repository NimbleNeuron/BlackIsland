using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive4Passive)]
	public class NadineActive4Passive : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectPoint(Self, "FX_BI_Nadine_Skill04_Appear", "SkillShotPos_Appear");
				PlayEffectPoint(Self, "FX_BI_Nadine_Skill04_Smoke_2", "SkillShotPos_Appear");
				PlaySoundPoint(Self, "nadine_Skill04_Start", 15);
			}

			if (actionNo == 2)
			{
				PlayEffectPoint(Self, "FX_BI_Nadine_Skill04", "SkillShotPos");
			}
		}


		public override void Finish(bool cancel) { }
	}
}