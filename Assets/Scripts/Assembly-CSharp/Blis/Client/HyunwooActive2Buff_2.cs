using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive2Buff_2)]
	public class HyunwooActive2Buff_2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChild(Self, "FX_BI_Hyunwoo_Skill02_Buff01");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}