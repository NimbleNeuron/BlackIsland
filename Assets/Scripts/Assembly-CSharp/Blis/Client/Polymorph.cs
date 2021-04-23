using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Polymorph)]
	public class Polymorph : LocalSkillScript
	{
		public override void Start()
		{
			LocalMovableCharacter localMovableCharacter = Self as LocalMovableCharacter;
			if (localMovableCharacter.GetCharacterStateValueByGroup(
				Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode, CasterId) != null)
			{
				if (localMovableCharacter != null)
				{
					localMovableCharacter.ChangePolymorphObject(GetEffectCaster(),
						Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode);
				}

				PlayEffectChild(Self, "FX_BI_Emma_Skill03_Rabbit_Start");
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			LocalMovableCharacter localMovableCharacter = Self as LocalMovableCharacter;
			if (localMovableCharacter != null)
			{
				localMovableCharacter.ReturnPolymorphObject();
			}

			SetPolymorphEndEffect(localMovableCharacter);
		}


		private void SetPolymorphEndEffect(LocalMovableCharacter targetPolymorph)
		{
			if (targetPolymorph.GetCharacterStateValueByGroup(
				Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode, CasterId) != null)
			{
				PlayEffectChild(Self, "FX_BI_Emma_Skill03_Rabbit_End");
				PlaySoundPoint(Self, "Emma_Skill03_End", 15);
			}
		}
	}
}