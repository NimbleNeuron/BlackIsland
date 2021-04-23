using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive1Cooldown)]
	public class LenoxActive1Cooldown : SkillScript
	{
		
		private float checkDuration;

		
		protected override void Start()
		{
			base.Start();
			checkDuration = StateDuration;
			Caster.Owner.RemoveReservationCooldown(SkillSlotSet.Active1_1);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.Owner.RemoveReservationCooldown(SkillSlotSet.Active1_1);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			int currentStack =
				Caster.GetStackByGroup(Singleton<LenoxSkillActive1Data>.inst.Active1BuffGroup, Caster.ObjectId);
			while (checkDuration > 0f && isPlaying)
			{
				checkDuration -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (!Caster.Owner.IsReservationCooldown(SkillSlotSet.Active1_1))
				{
					float num = currentStack *
					            GameUtil.GetCooldowncooldownReduction(Caster.Owner.Stat.CooldownReduction);
					Caster.Owner.SetReservationCooldown(SkillSlotSet.Active1_1, -num);
				}

				yield return WaitForFrame();
			}

			Finish();
		}
	}
}