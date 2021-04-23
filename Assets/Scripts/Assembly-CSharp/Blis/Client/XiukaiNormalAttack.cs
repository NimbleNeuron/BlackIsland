using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiNormalAttack)]
	public class XiukaiNormalAttack : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "XiukaiNormalAttackCancel";


		private int playCount;

		public override void Start()
		{
			if (3f < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}
			else
			{
				playCount++;
			}

			lastNormalAttackTime = Time.time;
			PlayAnimation(Self, playCount % 2 == 0 ? TriggerAttack01 : TriggerAttack02);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "XiukaiNormalAttackCancel");
				StopSoundByTag(Self, "XiukaiNormalAttackCancel");
			}
		}
	}
}