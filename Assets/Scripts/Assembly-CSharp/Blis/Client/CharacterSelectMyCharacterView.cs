using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectMyCharacterView : BaseUI
	{
		private RotationTweener backgroundTweener;


		private Image blankIcon;


		private Image blankImage;


		private Text blankText;


		private Text characterName;


		private Image characterPortrait;


		private ColorTweener pickColorTweener;


		private Image pickPortrait;


		private ScaleTweener pickScaleTweener;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			backgroundTweener = GameUtil.Bind<RotationTweener>(gameObject, "Circle");
			blankImage = GameUtil.Bind<Image>(gameObject, "BlankPortrait");
			blankIcon = GameUtil.Bind<Image>(gameObject, "BlankPortrait/Icon");
			blankText = GameUtil.Bind<Text>(gameObject, "BlankPortrait/Text");
			characterPortrait = GameUtil.Bind<Image>(gameObject, "SelectedPortrait");
			characterName = GameUtil.Bind<Text>(gameObject, "SelectedPortrait/Name");
			pickPortrait = GameUtil.Bind<Image>(gameObject, "PickEffect");
			pickScaleTweener = GameUtil.Bind<ScaleTweener>(gameObject, "PickEffect");
			pickColorTweener = GameUtil.Bind<ColorTweener>(gameObject, "PickEffect");
			pickColorTweener.OnAnimationFinish += delegate { pickPortrait.enabled = false; };
			Clear();
		}


		public void Clear()
		{
			EnableBlank(true);
			EnableCharacter(false);
			backgroundTweener.StopAnimation();
			backgroundTweener.transform.localRotation = Quaternion.Euler(backgroundTweener.from);
			pickPortrait.enabled = false;
			pickScaleTweener.StopAnimation();
			pickColorTweener.StopAnimation();
			pickPortrait.transform.localScale = Vector3.one;
			pickPortrait.color = Color.white;
		}


		public void SetCharacter(Sprite characterSprite, string characterName)
		{
			EnableBlank(false);
			EnableCharacter(true);
			backgroundTweener.StopAnimation();
			backgroundTweener.PlayAnimation();
			characterPortrait.sprite = characterSprite;
			pickPortrait.sprite = characterSprite;
			this.characterName.text = characterName;
		}


		public void PickCharacter()
		{
			pickPortrait.enabled = true;
			pickScaleTweener.StopAnimation();
			pickScaleTweener.PlayAnimation();
			pickColorTweener.StopAnimation();
			pickColorTweener.PlayAnimation();
		}


		public void CancelPickCharacter()
		{
			pickPortrait.enabled = false;
		}


		private void EnableBlank(bool enable)
		{
			blankImage.enabled = enable;
			blankIcon.enabled = enable;
			blankText.enabled = enable;
		}


		private void EnableCharacter(bool enable)
		{
			characterPortrait.enabled = enable;
			characterName.enabled = enable;
		}
	}
}