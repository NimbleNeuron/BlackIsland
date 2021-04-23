using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive2Attack)]
	public class FioraActive2Attack : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}