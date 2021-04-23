using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanStateNonSkill)]
	public class LocalSilviaHumanStateNonSkill : LocalSilviaHumanState
	{
		public override void Start()
		{
			base.Start();
			SetAnimatorLayer(Self, "Pistol", 1f);
			SetAnimatorLayer(Self, "PistolReload", 1f);
			SetAnimatorLayer(Self, "Bike", 0f);
			ActiveWeaponObject(Self, WeaponMountType.Special_1, false);
			SetAnimation(Self, bBikeState, false);
			PlayEffectPoint(Self, "FX_BI_Silvia_Skill04_Fog");
			PlaySoundPoint(Self, "Silvia_Skill08_Fog", 15);
			StopEffectChildManual(Self, "Silvia_Skill04_Spin", true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}