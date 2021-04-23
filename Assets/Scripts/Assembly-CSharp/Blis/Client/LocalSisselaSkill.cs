using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public abstract class LocalSisselaSkill : LocalSkillScript
	{
		protected const string tServantTrigerUstart = "tUstart_Wilson";


		protected const string tServantTrigerSkill01 = "tSkill01_Wilson";


		protected const string tServantTrigerSkill01_2 = "tSkill01_2_Wilson";


		protected const string bServantTrigerSkill01 = "bSkill01_Wilson";


		protected const string tWilsonSeperate = "tSeperate";


		protected const string bWilsonSeperate = "bSeperate";


		private LocalWilsonData _localWilsonData;


		private Transform _trsfWilson;


		private LocalSummonServant _wilson;


		private Coroutine playCoroutineSetWilsonTongueLength;


		protected LocalSummonServant wilson {
			get
			{
				if (_wilson == null)
				{
					_wilson = GetWilson(LocalPlayerCharacter);
				}

				return _wilson;
			}
		}


		protected Transform trsfWilson {
			get
			{
				if (_trsfWilson == null)
				{
					if (wilson == null)
					{
						return null;
					}

					_trsfWilson = wilson.transform;
				}

				return _trsfWilson;
			}
		}


		protected LocalWilsonData localWilsonData {
			get
			{
				if (_localWilsonData == null)
				{
					if (wilson == null)
					{
						return null;
					}

					_localWilsonData = (LocalWilsonData) wilson.ServantData;
				}

				return _localWilsonData;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1004 && wilson != null)
			{
				PlayEffectPoint(wilson, "FX_BI_Sissela_Union_End");
			}
		}


		protected void SetWilsonTongueLength(float length)
		{
			if (playCoroutineSetWilsonTongueLength != null)
			{
				StopCoroutine(playCoroutineSetWilsonTongueLength);
			}

			playCoroutineSetWilsonTongueLength = StartCoroutine(playSetWilsonTongueLength(length));
		}


		private IEnumerator playSetWilsonTongueLength(float length)
		{
			while (localWilsonData == null)
			{
				yield return null;
			}

			if (length <= 0f)
			{
				localWilsonData.WilsonTongue.localScale = Vector3.zero;
			}
			else
			{
				localWilsonData.WilsonTongue.localScale = new Vector3(length, 1f, 1f);
			}

			playCoroutineSetWilsonTongueLength = null;
		}


		private static bool Condition(LocalSummonBase summon)
		{
			return summon.SummonData.code == Singleton<SisselaSkillData>.inst.WilsonSummonCode;
		}


		public static LocalSummonServant GetWilson(LocalPlayerCharacter owner)
		{
			return (LocalSummonServant) owner.GetOwnSummon(Condition);
		}


		public class LocalWilsonData : LocalSummonServant.LocalSummonServantData
		{
			private bool isDoingGrab;


			public LocalWilsonData(Transform originalWilsonParent, Transform wilsonResourcePrefab,
				Transform wilsonTongue)
			{
				OriginalWilsonParent = originalWilsonParent;
				WilsonResourcePrefab = wilsonResourcePrefab;
				WilsonTongue = wilsonTongue;
				isDoingGrab = false;
			}


			public Transform OriginalWilsonParent { get; }


			public Transform WilsonResourcePrefab { get; }


			public Transform WilsonTongue { get; }


			public bool IsDoingGrab => isDoingGrab;


			public void SetIsDoingGrab(bool isDoingGrab)
			{
				this.isDoingGrab = isDoingGrab;
			}
		}
	}
}