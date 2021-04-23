using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaWilsonUnion)]
	public class LocalSisselaWilsonUnion : LocalSisselaSkill
	{
		public override void Start()
		{
			StartCoroutine(PlayUnion());
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
		}


		private IEnumerator PlayUnion()
		{
			while (localWilsonData == null)
			{
				yield return null;
			}

			localWilsonData.WilsonResourcePrefab.SetParent(
				LocalPlayerCharacter.WeaponMount.GetTransform(WeaponMountType.Special_1));
			localWilsonData.WilsonResourcePrefab.localPosition = Vector3.zero;
			localWilsonData.WilsonResourcePrefab.localRotation = Quaternion.identity;
			localWilsonData.WilsonResourcePrefab.localScale = Vector3.one;
			wilson.SetCharacterAnimatorBool("bSkill01_Wilson", false);
			trsfWilson.SetParent(LocalPlayerCharacter.transform);
			trsfWilson.localPosition = Vector3.zero;
			wilson.SetCharacterAnimatorTrigger("tUstart_Wilson");
			wilson.SetCharacterAnimatorBool("bSeperate", false);
			PlaySoundPoint(Self, "Sissela_Passive_Union", 15);
		}
	}
}