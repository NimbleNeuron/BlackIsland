using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.GloveUppercutAttack)]
	public class GloveUppercutAttack : LocalSkillScript
	{
		protected static readonly int TriggerSkill = Animator.StringToHash("tGlove_Skill");


		protected static readonly int BooleanSkill = Animator.StringToHash("bGlove_Skill");


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill);
			SetAnimation(Self, BooleanSkill, true);
			PlayEffectChildManual(Self, "WSkill_Glove_Attack", "FX_BI_WSkill_Glove_02", "Fx_Hand_R");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "WSkill_Glove_Attack", true);
			SetAnimation(Self, BooleanSkill, false);
		}
	}
}