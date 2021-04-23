using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive4Buff)]
	public class JackieActive4Buff : LocalSkillScript
	{
		public override void Start()
		{
			ResetWeaponLayers(Self);
			SetAnimatorLayer(Self, "ElectricSaw", 1f);
			SetAnimatorLayer(Self, "ElectricSawUpperBody", 1f);
			PlayEffectChildManual(Self, "Jackie_Skill04_Buff", "FX_BI_Jackie_Skill04_01", "Fx_Center");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetCurrentWeaponAnimatorLayer(Self);
			SetAnimatorLayer(Self, "ElectricSaw", 0f);
			StopEffectChildManual(Self, "Jackie_Skill04_Buff", true);
		}
	}
}