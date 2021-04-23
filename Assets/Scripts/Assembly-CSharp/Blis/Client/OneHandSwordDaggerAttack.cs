using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.OneHandSwordDaggerAttack)]
	public class OneHandSwordDaggerAttack : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tOneHandSword_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bOneHandSword_Skill");


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill, true);
			PlayAnimation(Self, TriggerSkill);
			PlayEffectChildManual(Self, "WSkill_OnHandSword_Attack", "FX_BI_WSkill_OneHandSword_01", "TrailPoint");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill, false);
			StopEffectChildManual(Self, "WSkill_OnHandSword_Attack", true);
		}
	}
}