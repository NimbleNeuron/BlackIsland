namespace Blis.Client
{
	public abstract class LocalShieldStateSkill : LocalSkillScript
	{
		protected abstract string effectKey { get; }


		protected abstract string effectName { get; }


		protected abstract string soundName { get; }


		public override void Start()
		{
			if (effectKey != null && effectName != null)
			{
				PlayEffectChildManual(Self, effectKey, effectName);
			}

			if (soundName != null)
			{
				PlaySoundPoint(Self, soundName);
			}
		}


		public override void Finish(bool cancel)
		{
			if (effectKey != null)
			{
				StopEffectChildManual(Self, effectKey, true);
			}
		}
	}
}