using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxActive3KnockBack)]
	public class LenoxActive3KnockBack : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Target.LockRotation(true);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Target.LockRotation(false);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			AirborneState airborneState = CreateState<AirborneState>(Target, 2000001, 0, StateDuration);
			airborneState.Init(StateDuration, Singleton<LenoxSkillActive3Data>.inst.AirbornPower);
			Target.AddState(airborneState, Caster.ObjectId);
			KnockbackState knockbackState = CreateState<KnockbackState>(Target, 2000010, 0, StateDuration);
			knockbackState.Init(Caster.Forward, Singleton<LenoxSkillActive3Data>.inst.KnockbackDistance, StateDuration,
				EasingFunction.Ease.EaseOutQuad, true);
			Target.AddState(knockbackState, Caster.ObjectId);
			yield return WaitForSeconds(StateDuration);
			Finish();
		}
	}
}