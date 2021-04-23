using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxActive4BlueSnake2)]
	public class LenoxActive4BlueSnake2 : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "FX_BI_Lenox_Skill04_Debuff2_key", "FX_BI_Lenox_Skill04_Debuff2");
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAfterSkillDamageProcess +=
				OnAfterSkillDamageProcess;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public void OnAfterSkillDamageProcess(LocalCharacter target, LocalCharacter.SkillDamageInfo info)
		{
			if (Self == null)
			{
				return;
			}

			if (target.ObjectId != Self.ObjectId)
			{
				return;
			}

			if (info.effectCode != Singleton<LenoxSkillActive4Data>.inst.BlueSnakeEffectAndSoundCode)
			{
				return;
			}

			PlayEffectChild(Self, "FX_BI_Lenox_Skill04_Debuff_Blood", "Bip001 Spine");
			PlayEffectChild(Self, "FX_BI_Lenox_Skill04_Debuff_Ground");
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Lenox_Skill04_Debuff2_key", true);
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAfterSkillDamageProcess -=
				OnAfterSkillDamageProcess;
		}
	}
}