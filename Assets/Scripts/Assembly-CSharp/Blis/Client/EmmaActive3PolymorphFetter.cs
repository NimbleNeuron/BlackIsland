using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive3PolymorphFetter)]
	public class EmmaActive3PolymorphFetter : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Emma_Skill03_Rabbit_Snare", "FX_BI_Emma_Skill03_Rabbit_Snare", "Root", false);
			PlaySoundPoint(Self, "Emma_Skill03_Snare", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Emma_Skill03_Rabbit_Snare", true);
		}
	}
}