using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SilviaBikeState)]
	public class LocalSilviaBikeState : LocalSkillScript
	{
		private const string bikeRunSoundName = "Silvia_Bike_FastRun";


		private bool isOnRun;


		public override void Start()
		{
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			SetAnimatorLayer(Self, "Pistol", 0f);
			SetAnimatorLayer(Self, "PistolReload", 0f);
			SetAnimatorLayer(Self, "Bike", 1f);
			ActiveWeaponObject(Self, WeaponMountType.Special_1, true);
			ActiveWeaponObject(Self, WeaponMountType.Special_2, false);
			SetAnimation(Self, LocalSilviaHumanState.bBikeState, true);
			PlayAnimation(Self, TriggerSkill04);
			PlayEffectPoint(Self, "FX_BI_Silvia_Skill04_Fog");
			PlaySoundPoint(Self, "Silvia_Skill04_Fog", 15);
			StartCoroutine(UpdateBikeRunSound());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopCoroutines();
			if (isOnRun)
			{
				isOnRun = false;
				StopSoundChildManual(Self, "Silvia_Bike_FastRun");
			}
		}


		private IEnumerator UpdateBikeRunSound()
		{
			isOnRun = false;
			for (;;)
			{
				yield return null;
				if (LocalPlayerCharacter.IsDyingCondition || !LocalPlayerCharacter.IsAlive)
				{
					if (!isOnRun)
					{
						continue;
					}

					isOnRun = false;
					StopSoundChildManual(Self, "Silvia_Bike_FastRun");
				}

				if (LocalPlayerCharacter.MoveVelocity > 0f)
				{
					if (!isOnRun)
					{
						isOnRun = true;
						PlaySoundChildManual(Self, "Silvia_Bike_FastRun", 15, true);
					}
				}
				else if (isOnRun)
				{
					isOnRun = false;
					StopSoundChildManual(Self, "Silvia_Bike_FastRun");
				}
			}
		}
	}
}