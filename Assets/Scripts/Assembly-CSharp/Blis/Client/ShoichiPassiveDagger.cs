using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiPassiveDagger)]
	public class ShoichiPassiveDagger : LocalSkillScript
	{
		private static readonly int tSkillp_01 = Animator.StringToHash("tSkillp_01");


		private static readonly int bSkillp_01 = Animator.StringToHash("bSkillp_01");


		public override void Start()
		{
			PlayAnimation(Self, tSkillp_01);
			SetAnimation(Self, bSkillp_01, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, bSkillp_01, false);
			ResetAnimatorTrigger(Self, tSkillp_01);
		}
	}
}