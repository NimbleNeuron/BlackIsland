using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.PistolActiveBuff)]
	public class PistolActiveBuff : LocalSkillScript
	{
		private const string PistolSkill_Sfx = "Pistol_skill_D";


		protected static readonly int TriggerPistolSkill = Animator.StringToHash("tPistol_Skill");


		public override void Start()
		{
			PlaySoundChildManual(Self, "Pistol_skill_D", 15);
			PlayEffectChild(Self, "FX_BI_WSkill_Pistol_01b");
			PlayEffectPoint(Self, "FX_BI_WSkill_Pistol_01a");
			PlayAnimation(Self, TriggerPistolSkill);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}