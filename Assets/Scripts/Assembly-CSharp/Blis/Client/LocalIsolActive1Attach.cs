using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolActive1Attach)]
	public class LocalIsolActive1Attach : LocalSkillScript
	{
		private const string Set_Character = "FX_BI_Isol_Skill01_other";


		private const string Upper_Explore = "FX_BI_Isol_Skill01_Upper_Explore";


		private Animator atorFx;


		private int attachStateCode;


		private GameObject goFx;


		private LocalCharacter targetCharacter;

		public override void Start()
		{
			attachStateCode = Singleton<IsolSkillActive1Data>.inst.AttachState;
			goFx = PlayEffectChild(Self, "FX_BI_Isol_Skill01_other");
			if (goFx != null)
			{
				atorFx = goFx.GetComponent<Animator>();
			}

			PlaySoundPoint(Self, "Isol_Skill01_Set_Character", 15);
			PlaySoundChildManual(Self, "Isol_Skill01_Set_Timer", 15);
			targetCharacter = Self as LocalCharacter;
			StartCoroutine(UpdateCor());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlaySoundPoint(Self, "Isol_Skill01_Set_Hit_2", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
			if (goFx != null)
			{
				Object.Destroy(goFx);
			}

			StopSoundChildManual(Self, "Isol_Skill01_Set_Timer");
			PlayEffectPoint(Self, "FX_BI_Isol_Skill01_Upper_Explore");
			PlaySoundPoint(Self, "Isol_Skill01_Set_Explosion", 15);
		}


		private IEnumerator UpdateCor()
		{
			for (;;)
			{
				CharacterStateValue characterStateValue = null;
				foreach (CharacterStateValue characterStateValue2 in targetCharacter.States)
				{
					if (characterStateValue2.code == attachStateCode)
					{
						if (characterStateValue == null)
						{
							characterStateValue = characterStateValue2;
						}
						else if (characterStateValue.Duration + characterStateValue.createdTime >
						         characterStateValue2.Duration + characterStateValue2.createdTime)
						{
							characterStateValue = characterStateValue2;
						}
					}
				}

				if (characterStateValue != null && goFx != null)
				{
					if (characterStateValue.CasterId == CasterId)
					{
						goFx.SetActive(true);
						float num = characterStateValue.Duration + characterStateValue.createdTime -
						            MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime;
						float normalizedTime = 1f - num / characterStateValue.OriginalDuration + 0.01f;
						if (atorFx != null)
						{
							atorFx.Play("FX_BI_Isol_Skill01_other", 0, normalizedTime);
						}
					}
					else
					{
						goFx.SetActive(false);
					}
				}

				yield return null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}