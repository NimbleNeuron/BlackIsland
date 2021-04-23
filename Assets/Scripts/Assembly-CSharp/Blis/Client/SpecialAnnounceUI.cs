using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SpecialAnnounceUI : BaseUI
	{
		[SerializeField] private Image thumbnail = default;


		[SerializeField] private Text announceMessage = default;


		// [TupleElementNames(new string[]
		// {
		// 	"none",
		// 	"create",
		// 	"createDimmed",
		// 	"dead"
		// })]
		// private ValueTuple<int, int, int, int> animationState;
		// co : tuple
		private (int none, int create, int createDimmed, int dead) animationState;


		private Animator animator;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			animator = GetComponent<Animator>();
			animationState = new ValueTuple<int, int, int, int>(Animator.StringToHash("None"),
				Animator.StringToHash("Create"), Animator.StringToHash("CreateDimmed"), Animator.StringToHash("Dead"));
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		public void StartWaitForAnnounce()
		{
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.IsWaitAnotherAnnounce = false;
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.Next();
		}


		public void ShowMessage(AnnounceType type, string message)
		{
			StopCurrentPlayAnnounce();
			PlaySound(type);
			PlayAnimation(type);
			SetThumbnail(type);
			SetMessage(message);
		}


		private void StopCurrentPlayAnnounce()
		{
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.IsWaitAnotherAnnounce = true;
			MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.Stop();
		}


		private void PlaySound(AnnounceType type)
		{
			switch (type)
			{
				case AnnounceType.Meteor_CreateExpected:
				case AnnounceType.Metoer_Appear:
					break;
				case AnnounceType.TreeOfLife_CreateExpected:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.TreeOfLifeCreateExpected);
					return;
				case AnnounceType.TreeOfLife_Appear:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.TreeOfLifeAppear);
					return;
				case AnnounceType.Wickeline_CreateExpected:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.WicklineCreateExpected);
					return;
				case AnnounceType.Wickeline_Appear:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.WicklineAppear);
					return;
				case AnnounceType.Wickeline_Dead:
					Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.WicklineDead);
					break;
				default:
					return;
			}
		}


		private void PlayAnimation(AnnounceType type)
		{
			switch (type)
			{
				case AnnounceType.Meteor_CreateExpected:
				case AnnounceType.Metoer_Appear:
					break;
				case AnnounceType.TreeOfLife_CreateExpected:
				case AnnounceType.Wickeline_CreateExpected:
					animator.Play(animationState.Item3);
					return;
				case AnnounceType.TreeOfLife_Appear:
				case AnnounceType.Wickeline_Appear:
					animator.Play(animationState.Item2);
					return;
				case AnnounceType.Wickeline_Dead:
					animator.Play(animationState.Item4);
					break;
				default:
					return;
			}
		}


		private void SetThumbnail(AnnounceType type)
		{
			switch (type)
			{
				case AnnounceType.Meteor_CreateExpected:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_Meteor_Ready");
					return;
				case AnnounceType.Metoer_Appear:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_Meteor");
					return;
				case AnnounceType.TreeOfLife_CreateExpected:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_TreeOfLife_Ready");
					return;
				case AnnounceType.TreeOfLife_Appear:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_TreeOfLife");
					return;
				case AnnounceType.Wickeline_CreateExpected:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_Wickeline_Ready");
					return;
				case AnnounceType.Wickeline_Appear:
				case AnnounceType.Wickeline_Dead:
					thumbnail.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Announce_Wickeline");
					return;
				default:
					return;
			}
		}


		private void SetMessage(string message)
		{
			announceMessage.text = message;
		}
	}
}