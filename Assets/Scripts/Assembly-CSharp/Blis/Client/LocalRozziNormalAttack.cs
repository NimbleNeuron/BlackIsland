using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziNormalAttack)]
	public class LocalRozziNormalAttack : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "RozziNormalAttackCancel";


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerAttack01);
				return;
			}

			if (action == 2)
			{
				PlayAnimation(Self, TriggerAttack02);
			}
		}


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "RozziNormalAttackCancel");
				StopSoundByTag(Self, "RozziNormalAttackCancel");
			}
		}
	}
}