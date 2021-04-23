using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive2ShieldState)]
	public class LocalChiaraActive2ShieldState : LocalSkillScript
	{
		private const string Chiara_Skill03_End_Sfx = "Chiara_Skill03_End";


		private const string Chiara_Skill03_Fire_Sfx = "Chiara_Skill03_Fire";


		private const string Chiara_Skill03_explore = "FX_BI_Chiara_Skill03_Explosion";


		private const string Chiara_Skill03_Barrier = "Chiara_Skill03_Barrier";


		private const string Chiara_Skill03_end = "FX_BI_Chiara_Skill03_End";


		private const string Setpoint = "Root";


		private const string Chiara_Skill03_Start_Sfx = "Chiara_Skill03_Start";


		public override void Start()
		{
			PlaySoundChildManual(Self, "Chiara_Skill03_Start", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				StopSoundChildManual(Self, "Chiara_Skill03_Start");
				PlayEffectChild(Self, "FX_BI_Chiara_Skill03_End", "Root");
				PlaySoundPoint(Self, "Chiara_Skill03_End", 15);
				StopEffectChildManual(Self, "Chiara_Skill03_Barrier", true);
				return;
			}

			if (action == 2)
			{
				StopSoundChildManual(Self, "Chiara_Skill03_Start");
				StopEffectChildManual(Self, "Chiara_Skill03_Barrier", true);
				PlaySoundPoint(Self, "Chiara_Skill03_Fire", 15);
				PlayEffectPoint(Self, "FX_BI_Chiara_Skill03_Explosion");
				PlayAnimation(Self, TriggerSkill03);
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Chiara_Skill03_Barrier", true);
		}
	}
}