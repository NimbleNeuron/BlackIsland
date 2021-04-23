using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ShoichiActive1Buff)]
	public class ShoichiActive1Buff : SkillScript
	{
		
		private float buffDuration;

		
		protected override void Start()
		{
			base.Start();
			if (buffDuration == 0f)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ShoichiSkillActive1Data>.inst.BuffStateCode);
				buffDuration = data != null ? data.duration : 0f;
			}

			SkillData skillData = Caster.GetSkillData(SkillSlotSet.Active1_1);
			float num = skillData != null ? -skillData.cooldown : SkillCooldown;
			num *= GameUtil.GetCooldowncooldownReduction(Caster.Owner.Stat.CooldownReduction);
			Caster.Owner.SetReservationCooldown(SkillSlotSet.Active1_1, num);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			float timeStack = 0f;
			while (buffDuration >= timeStack)
			{
				yield return WaitForFrame();
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (cancel && Caster.Owner.IsReservationCooldown(SkillSlotSet.Active1_1))
			{
				Caster.Owner.RemoveReservationCooldown(SkillSlotSet.Active1_1);
			}
		}
	}
}