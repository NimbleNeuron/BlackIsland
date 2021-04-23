using Blis.Common;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyAccountInfo : BaseUI
	{
		private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


		private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();
		private CanvasAlphaTweener canvasAlphaTweener;


		private int currentExp;


		private EventTrigger eventTrigger;


		private Image imgEmblem;


		private Image imgExpValue;


		private int level;


		private int sectionExp;


		private Text txtLevelValue;


		private Text txtNickName;


		private Text txtTooltip;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txtNickName = GameUtil.Bind<Text>(gameObject, "TXT_NickName");
			txtLevelValue = GameUtil.Bind<Text>(gameObject, "TXT_LevelValue");
			imgExpValue = GameUtil.Bind<Image>(gameObject, "IMG_Exp/IMG_ExpValue");
			imgEmblem = GameUtil.Bind<Image>(gameObject, "IMG_Emblem");
			txtTooltip = GameUtil.Bind<Text>(gameObject, "TXT_ExpToolTip");
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref canvasAlphaTweener);
			GameUtil.BindOrAdd<EventTrigger>(imgExpValue.gameObject, ref eventTrigger);
			eventTrigger.triggers.Clear();
			onEnterEvent.AddListener(ExpVarOnPointerEnter);
			onExitEvent.AddListener(ExpVarOnPointerExit);
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerEnter,
				callback = onEnterEvent
			});
			eventTrigger.triggers.Add(new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerExit,
				callback = onExitEvent
			});
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		public void SetNickName(string nickName)
		{
			#if UNITY_EDITOR
			txtNickName.text = "<color=orange>admin</color>";
			#else
			txtNickName.text = nickName;
			#endif
		}


		public void SetLevel(int level)
		{
			this.level = level;
			txtLevelValue.text = string.Format("Lv.{0}", level);
		}


		public void SetExp(int needExp)
		{
			sectionExp = GameDB.user.GetNeedXP(level);
			currentExp = sectionExp - needExp;
			imgExpValue.fillAmount = currentExp / (float) sectionExp;
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


		private void ExpVarOnPointerEnter(BaseEventData eventData)
		{
			txtTooltip.gameObject.SetActive(true);
			txtTooltip.text = string.Format("{0}/{1}", currentExp, sectionExp);
		}


		private void ExpVarOnPointerExit(BaseEventData eventData)
		{
			txtTooltip.gameObject.SetActive(false);
		}
	}
}