using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(CanvasAlphaTweener))]
	public class LobbyCommunityLinkUI : BaseUI, ILnEventHander
	{
		[SerializeField] private Transform globalLink = default;


		[SerializeField] private Transform chineseSimplifiedLink = default;


		[SerializeField] private QQGroupSelect qqGroupSelect = default;


		[SerializeField] private Button showTwitchDrops = default;


		private CanvasAlphaTweener canvasAlphaTweener;


		public void OnLnDataChange()
		{
			if (Ln.GetCurrentLanguage() == SupportLanguage.ChineseSimplified)
			{
				globalLink.gameObject.SetActive(false);
				chineseSimplifiedLink.gameObject.SetActive(true);
				return;
			}

			globalLink.gameObject.SetActive(true);
			chineseSimplifiedLink.gameObject.SetActive(false);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref canvasAlphaTweener);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			showTwitchDrops.onClick.AddListener(ClickedShowTwitchDrops);
		}


		private void Show()
		{
			canvasAlphaTweener.from = 0f;
			canvasAlphaTweener.to = 1f;
			canvasAlphaTweener.PlayAnimation();
		}


		private void Hide()
		{
			canvasAlphaTweener.from = 1f;
			canvasAlphaTweener.to = 0f;
			canvasAlphaTweener.PlayAnimation();
		}


		public void ClickedQQ()
		{
			qqGroupSelect.Open();
		}


		public void ClickedShowTwitchDrops()
		{
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnClickShowTwitchDrops();
		}
	}
}