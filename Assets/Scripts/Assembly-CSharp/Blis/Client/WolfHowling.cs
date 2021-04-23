using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.WolfHowling)]
	public class WolfHowling : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
		}
	}
}