using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public abstract class LocalSilviaHumanState : LocalSkillScript
	{
		public const string SetAnimatorLayerWeight_Pistol = "Pistol";


		public const string SetAnimatorLayerWeight_Pistol_Reload = "PistolReload";


		public const string SetAnimatorLayerWeight_Bike = "Bike";


		public const string Bool_BikeState = "bBike_State";


		public const string Spinwheel = "FX_BI_Silvia_Skill04_Spin";


		public const string Spinwheel_key = "Silvia_Skill04_Spin";


		public static readonly int bBikeState = Animator.StringToHash("bBikeState");


		public override void Start()
		{
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}
	}
}