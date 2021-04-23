using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirActive2Passive)]
	public class ZahirActive2Passive : LocalSkillScript
	{
		private const int ChangedStackActionNo = 1;


		private const int RemovedStackActionNo = 2;


		protected static readonly int IntCount = Animator.StringToHash("count");


		protected static readonly int TriggerCount01 = Animator.StringToHash("tCount01");


		protected static readonly int TriggerCount02 = Animator.StringToHash("tCount02");


		protected static readonly int TriggerCount03 = Animator.StringToHash("tCount03");


		protected static readonly int TriggerCount04 = Animator.StringToHash("tCount04");


		protected static readonly int TriggerCount05 = Animator.StringToHash("tCount05");


		private int prevCount;

		public override void Start()
		{
			CharacterStateData data = GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState);
			prevCount = GetStateStackByGroup(Self, data.group, Self.ObjectId);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState);
				int stateStackByGroup = GetStateStackByGroup(Self, data.group, Self.ObjectId);
				int num = prevCount;
				prevCount = stateStackByGroup;
				AppearChakram(stateStackByGroup);
				return;
			}

			if (actionNo == 2)
			{
				DisappearChakram(prevCount);
			}
		}


		private IEnumerator StartChakram(int stack)
		{
			yield return new WaitForSeconds(0.03f);
			switch (stack)
			{
				case 1:
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_01", "A_Main");
					break;
				case 2:
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_02", "B_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_03", "C_Main");
					break;
				case 3:
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_04", "C_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_05", "B_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_06", "D_Main");
					break;
				case 4:
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_07", "A_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_08", "B_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_09", "D_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_10", "E_Main");
					break;
				case 5:
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_11", "A_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_12", "B_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_13", "C_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_14", "D_Main");
					PlayEffectChild(Self, "FX_BI_Zahir_Skill02_Start_15", "E_Main");
					break;
			}
		}


		private void AppearChakram(int stack)
		{
			CharacterStateData data = GameDB.characterState.GetData(Singleton<ZahirSkillActive2Data>.inst.BuffState);
			PlayWeaponAnimation(Self, WeaponMountType.Special_1, IntCount, stack);
			if (1 <= stack && stack <= data.maxStack)
			{
				switch (stack)
				{
					case 1:
						PlayWeaponAnimation(Self, WeaponMountType.Special_1, TriggerCount01);
						return;
					case 2:
						PlayWeaponAnimation(Self, WeaponMountType.Special_1, TriggerCount02);
						return;
					case 3:
						PlayWeaponAnimation(Self, WeaponMountType.Special_1, TriggerCount03);
						return;
					case 4:
						PlayWeaponAnimation(Self, WeaponMountType.Special_1, TriggerCount04);
						return;
					case 5:
						PlayWeaponAnimation(Self, WeaponMountType.Special_1, TriggerCount05);
						break;
					default:
						return;
				}
			}
		}


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				DisappearChakram(prevCount);
			}
		}


		public override void OnUpdateWeapon()
		{
			AppearChakram(prevCount);
		}


		private void DisappearChakram(int prevCount)
		{
			switch (prevCount)
			{
				case 1:
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_01", "A_Main");
					break;
				case 2:
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_02", "B_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_03", "C_Main");
					break;
				case 3:
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_04", "C_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_05", "B_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_06", "D_Main");
					break;
				case 4:
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_07", "A_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_08", "B_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_09", "D_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_10", "E_Main");
					break;
				case 5:
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_11", "A_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_12", "B_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_13", "C_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_14", "D_Main");
					PlayEffectPoint(Self, "FX_BI_Zahir_Skill02_End_15", "E_Main");
					break;
			}

			PlayWeaponAnimation(Self, WeaponMountType.Special_1, IntCount, 0);
			this.prevCount = 0;
		}
	}
}