using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive2AddAttack)]
	public class LukeActive2AddAttack : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			SetAnimation(Self, BooleanSkill04_02, false);
			StopEffectChildManual(Self, "Effect_SKill2_Loop_key", true);
		}
	}
}