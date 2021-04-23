using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraPassiveStigmaDebuff)]
	public class LocalChiaraPassiveStigmaDebuff : LocalSkillScript
	{
		private const string Passive_effect_name = "FX_BI_Chiara_Passive_0";


		private const string Passive_effect_full_name = "FX_BI_Chiara_Passive_04c";


		private const string Passive_effect_key = "Chiara_Passive_Debuff";


		private const string Passive_effect_full_key = "Chiara_Passive_Debuff";


		private const string Passive_point = "Root";


		private const string Passive_Get_Sfx = "Chiara_Passive_Get";


		private const string Passive_Full_Sfx = "Chiara_Passive_Full";


		private int pastDisplayStack;


		public override void Start()
		{
			LocalCharacter localCharacter = Self as LocalCharacter;
			if (localCharacter == null)
			{
				return;
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[1]);
			int num;
			if (CasterId == SingletonMonoBehaviour<PlayerController>.inst.myObjectId)
			{
				num = localCharacter.GetStateStackByGroup(data.group,
					SingletonMonoBehaviour<PlayerController>.inst.myObjectId);
			}
			else if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CharacterCode ==
				    Singleton<ChiaraSkillData>.inst.ChiaraCharacterCode)
				{
					if (localCharacter.ObjectId != SingletonMonoBehaviour<PlayerController>.inst.myObjectId)
					{
						return;
					}

					num = localCharacter.GetMaxStateStackByGroup(data.group);
				}
				else
				{
					num = localCharacter.GetMaxStateStackByGroup(data.group);
				}
			}
			else
			{
				num = localCharacter.GetMaxStateStackByGroup(data.group);
			}

			if (num == 4)
			{
				StopEffectChildManual(Self, "Chiara_Passive_Debuff", true);
				StopEffectChildManual(Self, "Chiara_Passive_Debuff", true);
				PlayEffectChildManual(Self, "Chiara_Passive_Debuff", "FX_BI_Chiara_Passive_04c", "Root");
			}
			else
			{
				string effectName = "FX_BI_Chiara_Passive_0" + num;
				StopEffectChildManual(Self, "Chiara_Passive_Debuff", true);
				PlayEffectChildManual(Self, "Chiara_Passive_Debuff", effectName, "Root");
			}

			if (pastDisplayStack == num)
			{
				return;
			}

			pastDisplayStack = num;
			PlaySoundPoint(Self, num == 4 ? "Chiara_Passive_Full" : "Chiara_Passive_Get", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Chiara_Passive_Debuff", true);
			StopEffectChildManual(Self, "Chiara_Passive_Debuff", true);
			pastDisplayStack = 0;
		}
	}
}