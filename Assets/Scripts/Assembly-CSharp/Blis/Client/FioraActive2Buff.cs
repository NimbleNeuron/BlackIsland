using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive2Buff)]
	public class FioraActive2Buff : LocalSkillScript
	{
		private const string fiora_Skill02_Active = "fiora_Skill02_Active";


		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "FioraActive2Buff", "FX_BI_Fiora_Skill02_Hand", "Fx_Hand_R");
				createdEffect = true;
			}

			PlaySoundPoint(Self, "fiora_Skill02_Active");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FioraActive2Buff", true);
			createdEffect = false;
		}
	}
}