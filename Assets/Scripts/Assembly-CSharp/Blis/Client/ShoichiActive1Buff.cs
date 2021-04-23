using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiActive1Buff)]
	public class ShoichiActive1Buff : LocalSkillScript
	{
		private const string Weapon_Special_1 = "Weapon_Special_1";


		private const string Shoichi_Skill01_S = "Shoichi_Skill01_S";


		private const string Shoichi_Skill01_L = "Shoichi_Skill01_L";


		private const string FX_BI_Shoichi_Skill01_2_Buff_Loop = "FX_BI_Shoichi_Skill01_2_Buff_Loop";


		private const string FX_BI_Shoichi_Skill01_2_Buff_Start = "FX_BI_Shoichi_Skill01_2_Buff_Start";


		private static readonly int tSkill01_r = Animator.StringToHash("tSkill01_t");


		private static readonly int tSkill01_r_02 = Animator.StringToHash("tSkill01_t_02");

		public override void Start()
		{
			PlayEffectChildManual(Self, "Shoichi_Skill01_S", "FX_BI_Shoichi_Skill01_2_Buff_Start", "Weapon_Special_1");
			PlayEffectChildManual(Self, "Shoichi_Skill01_L", "FX_BI_Shoichi_Skill01_2_Buff_Loop", "Weapon_Special_1");
			PlayAnimation(Self, tSkill01_r);
			PlaySoundPoint(Self, "Shoichi_Skill01_Ing", 15);
			if (Self.ObjectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId)
			{
				StartCoroutine(CooldownCheck());
			}
		}


		private IEnumerator CooldownCheck()
		{
			for (float cooldown = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(SkillSlotIndex.Active1)
					.Cooldown.GetCooldown();
				cooldown > 0f;
				cooldown = MonoBehaviourInstance<GameUI>.inst.SkillHud.FindSkillSlot(SkillSlotIndex.Active1).Cooldown
					.GetCooldown())
			{
				yield return new WaitForFixedUpdate();
			}

			float skillCooldown = LocalPlayerCharacter.GetSkillCooldown(SkillSlotSet.Active1_2);
			MonoBehaviourInstance<GameUI>.inst.SkillHud.SetCastWaitTime(SkillSlotIndex.Active1, 0f,
				SkillData.cooldown - skillCooldown);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Shoichi_Skill01_S", true);
			StopEffectChildManual(Self, "Shoichi_Skill01_L", true);
			PlayAnimation(Self, tSkill01_r_02);
		}
	}
}