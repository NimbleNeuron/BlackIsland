using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraPassiveDebuff)]
	public class FioraPassiveDebuff : LocalSkillScript
	{
		private string prevEffectName = "";


		public override void Start()
		{
			CharacterStateData stateData =
				GameDB.characterState.GetData(Singleton<FioraSkillPassiveData>.inst.DebuffState[data.level]);
			int stack = 0;
			Self.IfTypeOf<LocalCharacter>(delegate(LocalCharacter character)
			{
				stack = character.GetStateStackByGroup(stateData.group,
					SingletonMonoBehaviour<PlayerController>.inst.myObjectId);
				if (stack == 0)
				{
					stack = character.GetStateStackByGroup(stateData.group, 0);
				}
			});
			if (stack > stateData.maxStack)
			{
				return;
			}

			string text;
			if (stateData.maxStack == stack)
			{
				text = string.Format("FX_BI_Fiora_Passive_{0}", stateData.maxStack);
			}
			else
			{
				text = string.Format("FX_BI_Fiora_Passive_Count{0}_{1}", stateData.maxStack, stack);
			}

			if (text.Equals(prevEffectName))
			{
				return;
			}

			StopEffectChildManual(Self, "Fiora_Skill01_Debuff", true);
			PlayEffectChildManual(Self, "Fiora_Skill01_Debuff", text);
			prevEffectName = text;
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Fiora_Skill01_Debuff", true);
		}
	}
}