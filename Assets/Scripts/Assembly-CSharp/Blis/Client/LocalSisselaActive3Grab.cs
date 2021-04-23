using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive3Grab)]
	public class LocalSisselaActive3Grab : LocalCapsuleSisselaGrab
	{
		private const string ServantTrigerSkill03_E = "tSkill03_E_Wilson";


		private const string ServantTrigerSkill03_A = "tSkill03_A_Wilson";


		public override void Start()
		{
			StartCoroutine(WilsonTonguePull());
		}


		private IEnumerator WilsonTonguePull()
		{
			if (wilson == null)
			{
				Log.E("WilsonTonguePull wilson is null");
				yield break;
			}

			LocalSisselaSkill.LocalWilsonData localWilsonData = (LocalSisselaSkill.LocalWilsonData) wilson.ServantData;
			if (localWilsonData == null)
			{
				Log.E("WilsonTonguePull wilson is null");
				yield break;
			}

			localWilsonData.SetIsDoingGrab(true);
			Transform tongue = localWilsonData.WilsonTongue;
			for (;;)
			{
				wilson.transform.LookAt(Self.GetPosition());
				float num = GameUtil.Distance(wilson.GetPosition(), Self.GetPosition());
				tongue.localScale = new Vector3(num / Singleton<SisselaSkillData>.inst.WilsonTongueBaseLength, 1f, 1f);
				yield return null;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
			if (action == 1)
			{
				wilson.SetCharacterAnimatorTrigger("tSkill03_E_Wilson");
				return;
			}

			if (action != 2)
			{
				return;
			}

			wilson.SetCharacterAnimatorTrigger("tSkill03_A_Wilson");
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
			if (wilson == null)
			{
				return;
			}

			LocalSisselaSkill.LocalWilsonData localWilsonData = (LocalSisselaSkill.LocalWilsonData) wilson.ServantData;
			if (localWilsonData == null)
			{
				return;
			}

			localWilsonData.WilsonTongue.localScale = Vector3.zero;
			localWilsonData.SetIsDoingGrab(false);
			if (wilson.MoveAgent.IsMoving && wilson.MoveAgent.IsLockRotation())
			{
				wilson.LockRotation(false, wilson.transform.rotation);
			}
		}
	}
}