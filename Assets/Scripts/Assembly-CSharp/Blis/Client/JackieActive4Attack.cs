using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive4Attack)]
	public class JackieActive4Attack : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanSkill04, true);
			PlayAnimation(Self, TriggerSkill04_2);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill04, false);
		}
	}
}