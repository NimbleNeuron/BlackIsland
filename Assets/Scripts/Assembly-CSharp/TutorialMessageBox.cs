using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Client;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;


public class TutorialMessageBox : BaseWindow
{
	
	protected override void OnAwakeUI()
	{
		base.OnAwakeUI();
	}

	
	public void Show(int group, Action closeCallback)
	{
		SingletonMonoBehaviour<PlayerController>.inst.LockInput(true);
		this.closeCallback = closeCallback;
		this.currentSequence = 0;
		this.tmDatas = GameDB.tutorial.GetTutorialMessageBoxDatas(group);
		this.SetActiveButtons(this.currentSequence);
		this.ShowTutorialMessageBox(this.tmDatas[this.currentSequence]);
		this.StartThrowingCoroutine(this.ShowChangeTimeScale(), null);
	}

	
	private IEnumerator ShowChangeTimeScale()
	{
		yield return new WaitUntil(() => base.CanvasGroup.alpha >= 1f);
		Time.timeScale = 0f;
	}

	
	protected override void OnClose()
	{
		base.OnClose();
		SingletonMonoBehaviour<PlayerController>.inst.LockInput(false);
		Time.timeScale = 1f;
		this.Hide();
	}

	
	public void Hide()
	{
		if (this.closeCallback != null)
		{
			this.closeCallback();
		}
	}

	
	private void SetActiveButtons(int sequence)
	{
		this.btnClose.SetActive(this.tmDatas.Count == sequence + 1);
		this.btnBottomClose.SetActive(this.tmDatas.Count == sequence + 1);
		this.btnContinue.SetActive(this.tmDatas.Count > sequence + 1);
	}

	
	private void ShowTutorialMessageBox(TutorialMessageBoxData tmData)
	{
		this.txtTitle.text = Ln.Get(tmData.title ?? "");
		this.txtsubTitle.text = Ln.Get(tmData.subTitle ?? "");
		this.txtDesc.text = Ln.Get(tmData.desc ?? "");
		this.SetTutorialImage(tmData.imgName);
		this.SetTutorialItems(tmData.dicItems);
	}

	
	private void SetTutorialImage(string imgName)
	{
		if (imgName.Equals(string.Empty))
		{
			this.imgTutorial.gameObject.SetActive(false);
			return;
		}
		this.imgTutorial.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(imgName);
		this.imgTutorial.gameObject.SetActive(true);
	}

	
	private void SetTutorialItems(Dictionary<int, TutorialItemDataInfo> dicItems)
	{
		foreach (TutorialMessageBox.TutorialItemDataSlot tutorialItemDataSlot in this.tiSlots)
		{
			tutorialItemDataSlot.obj.SetActive(false);
		}
		if (dicItems == null)
		{
			this.imgSlotBG.gameObject.SetActive(false);
			return;
		}
		this.imgSlotBG.gameObject.SetActive(true);
		int num = 0;
		foreach (KeyValuePair<int, TutorialItemDataInfo> keyValuePair in dicItems)
		{
			this.tiSlots[num].obj.SetActive(true);
			ItemData itemData = GameDB.item.FindItemByCode(keyValuePair.Key);
			this.tiSlots[num].ItemDataSlot.InitSlot();
			this.tiSlots[num].ItemDataSlot.SetItemData(itemData);
			this.tiSlots[num].ItemDataSlot.SetSlotType(SlotType.None);
			this.tiSlots[num].ItemDataSlot.SetSprite(itemData.GetSprite());
			this.tiSlots[num].ItemDataSlot.SetBackground(itemData.GetGradeSprite());
			this.tiSlots[num].ItemDataSlot.EnableOwnMark(keyValuePair.Value.ownMark);
			this.tiSlots[num].ItemDataSlot.EnableNeedMark(keyValuePair.Value.needMark);
			this.tiSlots[num].txtDesc.text = Ln.Get(keyValuePair.Value.itemDesc ?? "");
			num++;
		}
	}

	
	public void ClickedContinue()
	{
		if (this.tmDatas.Count - 1 == this.currentSequence)
		{
			return;
		}
		this.currentSequence++;
		this.SetActiveButtons(this.currentSequence);
		this.ShowTutorialMessageBox(this.tmDatas[this.currentSequence]);
	}

	
	public void ClickedClose()
	{
		this.Close();
	}

	
	[SerializeField]
	private Text txtTitle = default;

	
	[SerializeField]
	private Text txtsubTitle = default;

	
	[SerializeField]
	private Text txtDesc = default;

	
	[SerializeField]
	private Image imgTutorial = default;

	
	[SerializeField]
	private Image imgSlotBG = default;

	
	[SerializeField]
	private GameObject btnClose = default;

	
	[SerializeField]
	private GameObject btnBottomClose = default;

	
	[SerializeField]
	private GameObject btnContinue = default;

	
	[SerializeField]
	private List<TutorialMessageBox.TutorialItemDataSlot> tiSlots = default;

	
	private List<TutorialMessageBoxData> tmDatas = default;

	
	private int currentSequence = default;

	
	private Action closeCallback = default;

	
	[Serializable]
	public class TutorialItemDataSlot
	{
		
		public GameObject obj = default;

		
		public ItemDataSlot ItemDataSlot = default;

		
		public Text txtDesc = default;
	}
}
