using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive4AttachStackState)]
	public class RozziActive4AttachStackState : LocalSkillScript
	{
		private const string Set_Character = "FX_BI_Rozzi_Skill04_Set_Character";


		private const string Set_Character_Count = "FX_BI_Rozzi_Skill04_Set_Character_Count";


		private const string Upper_Explore = "FX_BI_Rozzi_Skill04_Upper_Explore";


		private Animator atorFx;


		private int attachStateCode;


		private GameObject goFx;


		private GameObject goFx_Count;


		private LocalCharacter targetCharacter;


		public override void Start()
		{
			attachStateCode = Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[SkillData.level];
			goFx = PlayEffectChild(Self, "FX_BI_Rozzi_Skill04_Set_Character");
			if (goFx != null)
			{
				atorFx = goFx.GetComponent<Animator>();
			}

			goFx_Count = PlayEffectChild(Self, "FX_BI_Rozzi_Skill04_Set_Character_Count");
			PlaySoundChildManual(Self, "Rozzi_Skill04_Hit", 15);
			targetCharacter = Self as LocalCharacter;
			StartCoroutine(UpdateCor());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlaySoundPoint(Self, "Rozzi_Skill04_Hit2", 15);
			}
		}


		public override void Finish(bool cancel)
		{
			StopCoroutines();
			if (goFx != null)
			{
				Object.Destroy(goFx);
			}

			if (goFx_Count != null)
			{
				Object.Destroy(goFx_Count);
			}

			StopSoundChildManual(Self, "Rozzi_Skill04_Hit");
			PlayEffectPoint(Self, "FX_BI_Rozzi_Skill04_Upper_Explore");
			PlaySoundPoint(Self, "Rozzi_Skill04_Bomb", 15);
			int maxStack = GameDB.characterState
				.GetData(Singleton<RozziSkillActive4Data>.inst.AttachDebuffStateCodeByLevel[SkillData.level]).maxStack;
			if (targetCharacter.GetMaxStateStackByGroup(Singleton<RozziSkillActive4Data>.inst
				.AttachDebuffStateGroupId) >= maxStack)
			{
				PlayEffectPoint(Self, "FX_BI_Rozzi_Skill04_Upper_Explore2");
			}
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

				if (characterStateValue != null && goFx != null && goFx_Count != null)
				{
					if (characterStateValue.CasterId == CasterId)
					{
						goFx.SetActive(true);
						goFx_Count.SetActive(true);
						float num = characterStateValue.Duration + characterStateValue.createdTime -
						            MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime;
						float normalizedTime = 1f - num / characterStateValue.OriginalDuration + 0.01f;
						if (atorFx != null)
						{
							atorFx.Play("FX_BI_Rozzi_Skill04_Set_Character", 0, normalizedTime);
						}
					}
					else
					{
						goFx.SetActive(false);
						goFx_Count.SetActive(false);
					}
				}

				yield return null;
			}
		}
	}
}