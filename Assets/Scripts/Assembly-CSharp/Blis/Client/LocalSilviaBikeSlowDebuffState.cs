using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeSlowDebuffState)]
	public class LocalSilviaBikeSlowDebuffState : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Silvia_Skill04_Spin", "FX_BI_Silvia_Skill04_Spin", "Root");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}