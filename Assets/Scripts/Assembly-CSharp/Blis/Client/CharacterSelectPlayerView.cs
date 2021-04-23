using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectPlayerView : CharacterSelectView
	{
		private readonly Color afterPickColor = new Color(0f, 1f, 0.91f, 1f);


		private readonly Color beforePickColor = new Color(1f, 0.62f, 0.21f, 1f);


		private readonly Color beforeSelectColor = new Color(0.44f, 0.44f, 0.44f, 1f);


		private CharacterSelectCharacterSelect characterSelect;


		private CharacterSelectCharacterView characterView;


		private Button exitBtn;


		private CanvasAlphaTweener leftTitle;


		private Button pickBtn;


		private ButtonSound pickBtnSound;


		private bool pickedMyCharacter;


		private Image pickImg;


		private LnText pickText;


		private CanvasAlphaTweener rightTitle;


		private bool selectedMyCharacter;


		private CharacterSelectSkinSelect skinSelect;


		private CharacterSelectWeaponSelect weaponSelect;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			characterSelect = GameUtil.Bind<CharacterSelectCharacterSelect>(gameObject, "CharacterSelect");
			skinSelect = GameUtil.Bind<CharacterSelectSkinSelect>(gameObject, "SkinSelect");
			characterView = GameUtil.Bind<CharacterSelectCharacterView>(gameObject, "CharacterView");
			weaponSelect = GameUtil.Bind<CharacterSelectWeaponSelect>(gameObject, "WeaponSelect");
			rightTitle = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "RightTitle");
			leftTitle = GameUtil.Bind<CanvasAlphaTweener>(gameObject, "LeftTitle");
			pickBtn = GameUtil.Bind<Button>(gameObject, "ButtonPick");
			pickBtnSound = GameUtil.Bind<ButtonSound>(gameObject, "ButtonPick");
			pickText = GameUtil.Bind<LnText>(pickBtn.gameObject, "Label");
			pickImg = GameUtil.Bind<Image>(pickBtn.gameObject, "FX_Glow/IMG_Light");
			pickBtn.onClick.AddListener(OnClickPick);
			exitBtn = GameUtil.Bind<Button>(gameObject, "ButtonExit");
			exitBtn.transform.localScale = Vector3.zero;
			exitBtn.onClick.AddListener(OnClickExit);
			pickText.color = beforeSelectColor;
			pickImg.enabled = false;
		}


		public override void Open()
		{
			base.Open();
			selectedMyCharacter = false;
			pickedMyCharacter = false;
			exitBtn.transform.localScale = GlobalUserData.IsStandaloneMode() ? Vector3.one : Vector3.zero;
			UpdateUI();
			characterSelect.Open();
			skinSelect.Close();
			characterView.Clear();
			weaponSelect.Clear();
			pickBtn.gameObject.SetActive(true);
		}


		public override void Close()
		{
			base.Close();
			skinSelect.Close();
			characterSelect.Close();
		}


		private void UpdateUI()
		{
			UpdateTitle();
			UpdateButton();
		}


		private void UpdateSkinList(int characterCode, int skinCode = 0)
		{
			if (pickedMyCharacter)
			{
				characterSelect.Close();
				skinSelect.Open(characterCode, skinCode);
				return;
			}

			characterSelect.Open();
			skinSelect.Close();
		}


		private void UpdateTitle()
		{
			if (!selectedMyCharacter)
			{
				rightTitle.StopAnimation();
				rightTitle.GetComponent<CanvasGroup>().alpha = 1f;
				leftTitle.StopAnimation();
				leftTitle.PlayAnimation();
				return;
			}

			if (pickedMyCharacter)
			{
				rightTitle.StopAnimation();
				leftTitle.StopAnimation();
				rightTitle.GetComponent<CanvasGroup>().alpha = 1f;
				leftTitle.GetComponent<CanvasGroup>().alpha = 1f;
				return;
			}

			rightTitle.StopAnimation();
			rightTitle.PlayAnimation();
			leftTitle.StopAnimation();
			leftTitle.PlayAnimation();
		}


		private void UpdateButton()
		{
			if (!selectedMyCharacter)
			{
				pickText.color = beforeSelectColor;
				pickText.text = Ln.Get("준비 완료");
				pickImg.enabled = false;
				pickBtnSound.enabled = false;
				return;
			}

			if (pickedMyCharacter)
			{
				pickText.color = afterPickColor;
				pickText.text = Ln.Get("준비 취소");
				pickImg.enabled = false;
				pickBtnSound.enabled = false;
				return;
			}

			pickText.color = beforePickColor;
			pickText.text = Ln.Get("준비 완료");
			pickImg.enabled = true;
			pickBtnSound.enabled = true;
		}


		public override void CharacterSelect(int characterCode, int startingDataCode, bool mySelect)
		{
			characterSelect.UpdateCharacters(false);
			if (mySelect)
			{
				UpdateMyCharacter(characterCode);
				weaponSelect.UpdateWeapons(characterCode, startingDataCode);
				selectedMyCharacter = true;
				UpdateUI();
			}
		}


		public override void SkinSelect(int characterCode, int skinCode, bool mySelect)
		{
			if (mySelect)
			{
				selectedMyCharacter = true;
				UpdateUI();
				UpdateMyCharacter(characterCode, skinCode);
				UpdateSkinList(characterCode, skinCode);
			}
		}


		public override void WeaponSelect(int startingDataCode)
		{
			weaponSelect.SelectWeapon(startingDataCode);
		}


		private void UpdateMyCharacter(int selectedCharacter, int skinCode = 0)
		{
			int skinIndex = 0;
			CharacterData characterData = GameDB.character.GetCharacterData(selectedCharacter);
			CharacterSkinData skinData = GameDB.character.GetSkinData(skinCode);
			if (skinData != null)
			{
				skinIndex = skinData.index;
			}

			characterView.UpdateMyCharacter(
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterFullSprite(characterData.code, skinIndex),
				LnUtil.GetCharacterName(characterData.code));
		}


		public override void UpdateMyTeam(MatchingService.MatchingUser userInfo)
		{
			characterView.UpdateMyTeam(userInfo);
		}


		public override void PickMyCharacter(int characterCode, int skinCode, bool isSingle)
		{
			Singleton<SoundControl>.inst.Play2DSound(characterCode, 0, "selected_1");
			Singleton<SoundControl>.inst.PlayUISound("oui_matchClick2");
			pickedMyCharacter = true;
			characterView.PickMyCharacter();
			weaponSelect.PickMyCharacter();
			if (isSingle)
			{
				characterSelect.UpdateCharacters(false);
			}

			UpdateUI();
			UpdateSkinList(skinCode);
		}


		public override void CharacterCancelMyPick(int characterCode)
		{
			pickedMyCharacter = false;
			characterView.CancelPickMyCharacter();
			weaponSelect.CancelPickMyCharacter();
			UpdateUI();
			UpdateSkinList(characterCode);
			UpdateMyCharacter(characterCode);
		}


		public override void CharacterCancelPick(int characterCode)
		{
			characterSelect.UpdateCharacters(false);
			UpdateUI();
		}


		public override void StandBy()
		{
			base.StandBy();
			HidePickBtn();
		}


		public void HidePickBtn()
		{
			pickBtn.gameObject.SetActive(false);
		}


		public override void PickCharacter(MatchingService.MatchingUser matchingTeamMember)
		{
			characterView.PickCharacter(matchingTeamMember);
			characterSelect.UpdateCharacters(false);
		}


		private void OnClickPick()
		{
			MatchingService inst = MonoBehaviourInstance<MatchingService>.inst;
			if (inst == null)
			{
				return;
			}

			inst.PickCharacter();
		}


		private void OnClickExit()
		{
			MatchingService inst = MonoBehaviourInstance<MatchingService>.inst;
			if (inst == null)
			{
				return;
			}

			inst.Exit();
		}
	}
}