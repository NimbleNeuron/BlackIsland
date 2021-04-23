using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.YukiActive1Buff)]
	public class YukiActive1Buff : LocalSkillScript
	{
		private const string FX_BI_Yuki_Skill01_Buff = "FX_BI_Yuki_Skill01_Buff";


		private const string FX_BI_Yuki_Skill01_Buff_DualSword = "FX_BI_Yuki_Skill01_Buff_DualSword";


		private const string Fx_Hand_L = "Fx_Hand_L";


		private const string Fx_Hand_R = "Fx_Hand_R";


		private const string Yuki_Skill01_L = "Yuki_Skill01_L";


		private const string Yuki_Skill01_R = "Yuki_Skill01_R";


		private const string Yuki_Skill01_Active = "Yuki_Skill01_Active";


		private const string FX_BI_Yuki_Skill01_Buff02 = "FX_BI_Yuki_Skill01_Buff02";


		private const string FX_BI_Yuki_Skill01_Buff02_DualSword = "FX_BI_Yuki_Skill01_Buff02_DualSword";


		private const string FX_BI_Yuki_Skill01_Buff03 = "FX_BI_Yuki_Skill01_Buff03";


		private const string FX_BI_Yuki_Skill01_Buff03_DualSword = "FX_BI_Yuki_Skill01_Buff03_DualSword";


		private const string TwoHandSword_01 = "TwoHandSword_01";


		private const string DualSword_L_01 = "DualSword_L_01";


		private const string DualSword_R_01 = "DualSword_R_01";


		private const string Yuki_Skill01 = "Yuki_Skill01";


		private const string Yuki_Skill01_1 = "Yuki_Skill01_1";


		private const string Yuki_Skill01_2 = "Yuki_Skill01_2";


		public override void Start()
		{
			LocalPlayerCharacter target = Self as LocalPlayerCharacter;
			if (GetEquipWeaponMasteryType(target) == MasteryType.TwoHandSword)
			{
				PlayEffectChildManual(Self, "Yuki_Skill01", "FX_BI_Yuki_Skill01_Buff02", "TwoHandSword_01");
				PlayEffectChildManual(Self, "Yuki_Skill01_R", "FX_BI_Yuki_Skill01_Buff", "Fx_Hand_R");
			}
			else
			{
				PlayEffectChildManual(Self, "Yuki_Skill01_L", "FX_BI_Yuki_Skill01_Buff_DualSword", "Fx_Hand_L");
				PlayEffectChildManual(Self, "Yuki_Skill01_R", "FX_BI_Yuki_Skill01_Buff_DualSword", "Fx_Hand_R");
				PlayEffectChildManual(Self, "Yuki_Skill01_1", "FX_BI_Yuki_Skill01_Buff02_DualSword", "DualSword_L_01");
				PlayEffectChildManual(Self, "Yuki_Skill01_2", "FX_BI_Yuki_Skill01_Buff02_DualSword", "DualSword_R_01");
			}

			PlaySoundPoint(Self, "Yuki_Skill01_Active");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			LocalPlayerCharacter target = Self as LocalPlayerCharacter;
			int equipWeaponMasteryType = (int) GetEquipWeaponMasteryType(target);
			StopEffectChildManual(Self, "Yuki_Skill01_L", true);
			StopEffectChildManual(Self, "Yuki_Skill01_R", true);
			StopEffectChildManual(Self, "Yuki_Skill01", false);
			StopEffectChildManual(Self, "Yuki_Skill01_1", false);
			StopEffectChildManual(Self, "Yuki_Skill01_2", false);
			if (equipWeaponMasteryType == 16)
			{
				PlayEffectChild(Self, "FX_BI_Yuki_Skill01_Buff03", "TwoHandSword_01");
				return;
			}

			PlayEffectChild(Self, "FX_BI_Yuki_Skill01_Buff03_DualSword", "DualSword_L_01");
			PlayEffectChild(Self, "FX_BI_Yuki_Skill01_Buff03_DualSword", "DualSword_R_01");
		}
	}
}