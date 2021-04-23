using UnityEngine;

namespace Blis.Client
{
	public abstract class LocalDamageForSeconds : LocalSkillScript
	{
		private bool createdEffect;


		protected abstract string effectTarget { get; }


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "DamageForSeconds", effectTarget, "Fx_Center");
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "DamageForSeconds", true);
			createdEffect = false;
		}
	}
}