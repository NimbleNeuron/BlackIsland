using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive1Buff)]
	public class HartActive1Buff : LocalSkillScript
	{
		private const string Hart_Skill01_Charge = "Hart_Skill01_Charge";


		private const string FX_BI_Hart_Skill01_ChargeBase = "FX_BI_Hart_Skill01_ChargeBase";


		private const string ShotPoint = "ShotPoint";


		private const string FX_BI_Hart_Skill01_ChargeBase_End = "FX_BI_Hart_Skill01_ChargeBase_End";


		private const string Hart_Skill01_Charge_1 = "Hart_Skill01_Charge_1";


		private const string Hart_Skill01_Charge_2 = "Hart_Skill01_Charge_2";


		private const string Hart_Skill01_Charge_3 = "Hart_Skill01_Charge_3";


		private const string Hart_Skill01_Charge_4 = "Hart_Skill01_Charge_4";


		private const string Hart_Skill01_ChargeBase_4 = "Hart_Skill01_ChargeBase_4";


		private const string Hart_Skill01_ChargeBase_3 = "Hart_Skill01_ChargeBase_3";


		private const string Hart_Skill01_ChargeBase_2 = "Hart_Skill01_ChargeBase_2";


		private const string Hart_Skill01_ChargeBase_1 = "Hart_Skill01_ChargeBase_1";


		private const string FX_BI_Hart_Skill01_FullCharge = "FX_BI_Hart_Skill01_FullCharge";


		private const string hart_Skill01_Charged_Maintain = "hart_Skill01_Charged_Maintain";


		private readonly Dictionary<int, string> Charge1Sec_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Charge2Sec_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Charge3Sec_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> ChargeStart_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_0 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_1 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_2 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_3 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_4 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_5 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_6 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_7 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> Effect_Skill_8 = new Dictionary<int, string>();


		private readonly Dictionary<int, string> FullCharged_SFX = new Dictionary<int, string>();


		private readonly Dictionary<int, string> SkillActive_SFX = new Dictionary<int, string>();


		public HartActive1Buff()
		{
			Effect_Skill_0.Add(0, "FX_BI_Hart_Skill01_Start");
			Effect_Skill_0.Add(1, "FX_BI_Hart_Skill01_Start_Evolve");
			Effect_Skill_0.Add(2, "FX_BI_Hart_Skill01_Start_Evolve");
			Effect_Skill_1.Add(0, "FX_BI_Hart_Skill01_Charge_1");
			Effect_Skill_1.Add(1, "FX_BI_Hart_Skill01_Charge_Evolve_1");
			Effect_Skill_1.Add(2, "FX_BI_Hart_Skill01_Charge_Evolve_1");
			Effect_Skill_2.Add(0, "FX_BI_Hart_Skill01_Charge_2");
			Effect_Skill_2.Add(1, "FX_BI_Hart_Skill01_Charge_Evolve_2");
			Effect_Skill_2.Add(2, "FX_BI_Hart_Skill01_Charge_Evolve_2");
			Effect_Skill_3.Add(0, "FX_BI_Hart_Skill01_Charge_3");
			Effect_Skill_3.Add(1, "FX_BI_Hart_Skill01_Charge_Evolve_3");
			Effect_Skill_3.Add(2, "FX_BI_Hart_Skill01_Charge_Evolve_3");
			Effect_Skill_4.Add(0, "FX_BI_Hart_Skill01_Charge_4");
			Effect_Skill_4.Add(1, "FX_BI_Hart_Skill01_Charge_Evolve_4");
			Effect_Skill_4.Add(2, "FX_BI_Hart_Skill01_Charge_Evolve_4");
			Effect_Skill_5.Add(0, "FX_BI_Hart_Skill01_ChargeBase_4");
			Effect_Skill_5.Add(1, "FX_BI_Hart_Skill01_ChargeBase_Evolve_4");
			Effect_Skill_5.Add(2, "FX_BI_Hart_Skill01_ChargeBase_Evolve_4");
			Effect_Skill_6.Add(0, "FX_BI_Hart_Skill01_ChargeBase_3");
			Effect_Skill_6.Add(1, "FX_BI_Hart_Skill01_ChargeBase_Evolve_3");
			Effect_Skill_6.Add(2, "FX_BI_Hart_Skill01_ChargeBase_Evolve_3");
			Effect_Skill_7.Add(0, "FX_BI_Hart_Skill01_ChargeBase_2");
			Effect_Skill_7.Add(1, "FX_BI_Hart_Skill01_ChargeBase_Evolve_2");
			Effect_Skill_7.Add(2, "FX_BI_Hart_Skill01_ChargeBase_Evolve_2");
			Effect_Skill_8.Add(0, "FX_BI_Hart_Skill01_ChargeBase_1");
			Effect_Skill_8.Add(1, "FX_BI_Hart_Skill01_ChargeBase_Evolve_1");
			Effect_Skill_8.Add(2, "FX_BI_Hart_Skill01_ChargeBase_Evolve_1");
			ChargeStart_SFX.Add(0, "hart_Skill01_Charge");
			ChargeStart_SFX.Add(1, "hart_Skill01_Evo_Charge");
			ChargeStart_SFX.Add(2, "hart_Skill01_Evo_Charge");
			SkillActive_SFX.Add(0, "hart_Skill01_Active");
			SkillActive_SFX.Add(1, "hart_Skill01_Evo_Active");
			SkillActive_SFX.Add(2, "hart_Skill01_Evo_Active");
			Charge1Sec_SFX.Add(0, "hart_Skill01_1sec_Charge");
			Charge1Sec_SFX.Add(1, "hart_Skill01_Evo_1sec_Charge");
			Charge1Sec_SFX.Add(2, "hart_Skill01_Evo_1sec_Charge");
			Charge2Sec_SFX.Add(0, "hart_Skill01_2sec_Charge");
			Charge2Sec_SFX.Add(1, "hart_Skill01_Evo_2sec_Charge");
			Charge2Sec_SFX.Add(2, "hart_Skill01_Evo_2sec_Charge");
			Charge3Sec_SFX.Add(0, "hart_Skill01_3sec_Charge");
			Charge3Sec_SFX.Add(1, "hart_Skill01_Evo_3sec_Charge");
			Charge3Sec_SFX.Add(2, "hart_Skill01_Evo_3sec_Charge");
			FullCharged_SFX.Add(0, "hart_Skill01_Full_Charged");
			FullCharged_SFX.Add(1, "hart_Skill01_Evo_Full_Charged");
			FullCharged_SFX.Add(2, "hart_Skill01_Evo_Full_Charged");
		}


		public override void Start()
		{
			SetAnimation(Self, BooleanSkill01, true);
			PlayEffectChildManual(Self, "Hart_Skill01_Charge", "FX_BI_Hart_Skill01_ChargeBase");
			PlayEffectChild(Self, Effect_Skill_0[evolutionLevel]);
			PlaySoundChildManual(Self, ChargeStart_SFX[evolutionLevel], 15);
			PlaySoundPoint(Self, SkillActive_SFX[evolutionLevel], 15);
			StartCoroutine(PlayEffects());
		}


		private IEnumerator PlayEffects()
		{
			PlayEffectChildManual(Self, "Hart_Skill01_Charge_1", Effect_Skill_1[evolutionLevel]);
			yield return new WaitForSeconds(1f);
			PlayEffectChildManual(Self, "Hart_Skill01_ChargeBase_4", Effect_Skill_5[evolutionLevel]);
			PlayEffectChildManual(Self, "Hart_Skill01_Charge_2", Effect_Skill_2[evolutionLevel]);
			PlaySoundPoint(Self, Charge1Sec_SFX[evolutionLevel], 15);
			yield return new WaitForSeconds(1f);
			PlayEffectChildManual(Self, "Hart_Skill01_ChargeBase_3", Effect_Skill_6[evolutionLevel]);
			PlayEffectChildManual(Self, "Hart_Skill01_Charge_3", Effect_Skill_3[evolutionLevel]);
			PlaySoundPoint(Self, Charge2Sec_SFX[evolutionLevel], 15);
			yield return new WaitForSeconds(1f);
			PlayEffectChildManual(Self, "Hart_Skill01_ChargeBase_2", Effect_Skill_7[evolutionLevel]);
			PlayEffectChildManual(Self, "Hart_Skill01_Charge_4", Effect_Skill_4[evolutionLevel]);
			PlaySoundPoint(Self, Charge3Sec_SFX[evolutionLevel], 15);
			yield return new WaitForSeconds(1f);
			PlayEffectChildManual(Self, "Hart_Skill01_ChargeBase_1", Effect_Skill_8[evolutionLevel]);
			PlayEffectChild(Self, "FX_BI_Hart_Skill01_FullCharge");
			PlaySoundPoint(Self, FullCharged_SFX[evolutionLevel], 15);
			PlaySoundChildManual(Self, "hart_Skill01_Charged_Maintain", 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			StopEffectChildManual(Self, "Hart_Skill01_Charge", true);
			StopEffectChildManual(Self, "Hart_Skill01_ChargeBase_4", true);
			StopEffectChildManual(Self, "Hart_Skill01_ChargeBase_3", true);
			StopEffectChildManual(Self, "Hart_Skill01_ChargeBase_2", true);
			StopEffectChildManual(Self, "Hart_Skill01_ChargeBase_1", true);
			StopEffectChildManual(Self, "Hart_Skill01_Charge_1", true);
			StopEffectChildManual(Self, "Hart_Skill01_Charge_2", true);
			StopEffectChildManual(Self, "Hart_Skill01_Charge_3", true);
			StopEffectChildManual(Self, "Hart_Skill01_Charge_4", true);
			PlayEffectChild(Self, "FX_BI_Hart_Skill01_ChargeBase_End");
			StopSoundChildManual(Self, "hart_Skill01_Charged_Maintain");
			StopSoundChildManual(Self, ChargeStart_SFX[evolutionLevel]);
			StopCoroutines();
		}
	}
}