using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraNormalAttack)]
	public class LocalChiaraNormalAttack : LocalSkillScript
	{
		private const string NormalAttackCancelTag = "ChiaraNormalAttackCancel";


		private int playCount;

		public override void Start()
		{
			SetAnimation(Self, BooleanSkill03, false);
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


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (cancel)
			{
				StopEffectByTag(Self, "ChiaraNormalAttackCancel");
				StopSoundByTag(Self, "ChiaraNormalAttackCancel");
			}
		}
	}
}