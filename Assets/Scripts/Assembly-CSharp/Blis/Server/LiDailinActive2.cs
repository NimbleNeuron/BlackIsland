using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinActive2)]
	public class LiDailinActive2 : SkillScript
	{
		
		private bool isReinforce;

		
		protected override void Start()
		{
			base.Start();
			isReinforce = Caster.Status.ExtraPoint >= Singleton<LiDailinSkillData>.inst.SkillReinforceExtraPoint;
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			AddState(Caster, 10007, 0f);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			if (Caster.IsHaveStateByGroup(10007, Caster.ObjectId))
			{
				Caster.RemoveStateByGroup(10007, Caster.ObjectId);
			}

			if (isReinforce && Caster.Status.ExtraPoint != Caster.Stat.MaxExtraPoint)
			{
				AddState(Caster, Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackStateCode);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			int charageStack = 0;
			while (Singleton<LiDailinSkillData>.inst.A2ChargeCount > charageStack)
			{
				yield return WaitForSeconds(Singleton<LiDailinSkillData>.inst.A2ChargeTerm);
				Caster.ExtraPointModifyTo(Caster, Singleton<LiDailinSkillData>.inst.A2ChargeAmount);
				int num = charageStack + 1;
				charageStack = num;
			}

			FinishConcentration(false);
			Caster.RemoveStateByGroup(10007, Caster.ObjectId);
			ExtensionCommonState extensionCommonState =
				CreateState<ExtensionCommonState>(Caster, Singleton<LiDailinSkillData>.inst.A2SkillEndBuff);
			extensionCommonState.SetExtraValue(Caster.Status.ExtraPoint *
			                                   Singleton<LiDailinSkillData>.inst.A2SkillEndBuffEffectAmountPerAlcohol);
			AddState(Caster, extensionCommonState);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}