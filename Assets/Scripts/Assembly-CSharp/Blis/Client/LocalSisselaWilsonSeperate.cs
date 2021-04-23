using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaWilsonSeperate)]
	public class LocalSisselaWilsonSeperate : LocalSisselaSkill
	{
		private static readonly Vector3 wilsonSeperatePos = new Vector3(0f, 0.7f, 0f);


		public override void Start()
		{
			StartCoroutine(PlaySeperate());
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
		}


		private IEnumerator PlaySeperate()
		{
			while (localWilsonData == null)
			{
				yield return null;
			}

			localWilsonData.WilsonResourcePrefab.SetParent(trsfWilson);
			localWilsonData.WilsonResourcePrefab.localPosition = wilsonSeperatePos;
			localWilsonData.WilsonResourcePrefab.localRotation = Quaternion.identity;
			localWilsonData.WilsonResourcePrefab.localScale = Vector3.one;
			trsfWilson.SetParent(localWilsonData.OriginalWilsonParent);
			wilson.SetCharacterAnimatorTrigger("tSeperate");
			wilson.SetCharacterAnimatorBool("bSeperate", true);
		}
	}
}