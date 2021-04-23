using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinPassiveDebuff)]
	public class LocalHyejinPassiveDebuff : LocalSkillScript
	{
		public override void Start()
		{
			LocalCharacter localCharacter = Self as LocalCharacter;
			if (localCharacter == null)
			{
				return;
			}

			CharacterStateData data = GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.PassiveStackState);
			int num;
			if (CasterId == SingletonMonoBehaviour<PlayerController>.inst.myObjectId)
			{
				num = localCharacter.GetStateStackByGroup(data.group,
					SingletonMonoBehaviour<PlayerController>.inst.myObjectId);
			}
			else if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				if (localCharacter.GetStateStackByGroup(data.group,
					SingletonMonoBehaviour<PlayerController>.inst.myObjectId) != 0)
				{
					return;
				}

				num = localCharacter.GetMaxStateStackByGroup(data.group);
			}
			else
			{
				num = localCharacter.GetMaxStateStackByGroup(data.group);
			}

			string effectName = "FX_BI_Hyejin_Passive_Debuff" + num;
			StopEffectChildManual(Self, "Hyejin_Passive_Debuff", true);
			PlayEffectChildManual(Self, "Hyejin_Passive_Debuff", effectName, "Root");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Hyejin_Passive_Debuff", true);
		}
	}
}