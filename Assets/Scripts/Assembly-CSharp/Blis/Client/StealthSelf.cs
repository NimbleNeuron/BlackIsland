using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.StealthSelf)]
	public class StealthSelf : LocalSkillScript
	{
		private const string Stealth = "FX_BI_Character_Skill";


		private const string Center = "Fx_Center";


		private const string Stealthkey = "Stealtheffect";


		private const string Skill03_Stelth = "FX_BI_Common_Stealth";


		private const string SteathSound = "Common_Stealth_Start";


		private const string SteathSound_End = "Common_Stealth_End";

		public override void Start()
		{
			PlayEffectChild(Self, "FX_BI_Common_Stealth");
			PlaySoundPoint(Self, "Common_Stealth_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			PlayEffectChildManual(Self, "Stealtheffect", "FX_BI_Character_Skill", "Fx_Center");
			PlaySoundPoint(Self, "Common_Stealth_End", 15);
		}
	}
}