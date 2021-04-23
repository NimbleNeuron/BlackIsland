using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaHumanStateInit)]
	public class LocalSilviaHumanStateInit : LocalSilviaHumanState
	{
		public override void Start()
		{
			base.Start();
			SetAnimatorLayer(Self, "Pistol", 1f);
			SetAnimatorLayer(Self, "PistolReload", 1f);
			ActiveWeaponObject(Self, WeaponMountType.Special_1, false);
			SetAnimatorLayer(Self, "Bike", 0f);
			SetAnimation(Self, bBikeState, false);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}