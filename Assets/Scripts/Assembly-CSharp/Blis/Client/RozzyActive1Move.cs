using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive1Move)]
	public class RozzyActive1Move : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill01_2);
				PlayEffectPoint(Self, "FX_BI_Rozzi_Skill01_Move");
				PlaySoundPoint(Self, "Rozzi_Skill01_Move", 15);
			}
		}


		public override void Finish(bool cancel) { }
	}
}