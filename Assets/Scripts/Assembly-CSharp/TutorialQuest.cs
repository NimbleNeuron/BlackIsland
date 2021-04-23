using System;
using System.Collections.Generic;
using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;


public class TutorialQuest : MonoBehaviour
{
	
	public void InitMainQuestStack()
	{
		this.mainQuestStack = 0;
	}

	
	public void SuccessMainQuest()
	{
		this.mainQuest.obj.SetActive(false);
		if (this.mainQuestCompletedAction != null)
		{
			this.mainQuestCompletedAction();
		}
	}

	
	public void AddMainQuest(int code, Action mainQuestCompletedAction)
	{
		base.gameObject.SetActive(true);
		this.mainQuestCompletedAction = mainQuestCompletedAction;
		TutorialQuestData turorialQuestData = GameDB.tutorial.GetTurorialQuestData(code);
		this.ShowMainQuest(turorialQuestData.mainQuestData);
		this.subQuestDatas = turorialQuestData.subQuestDatas;
	}

	
	private void ShowMainQuest(TutorialMainQuestData mainQuestData)
	{
		this.mainQuestData = mainQuestData;
		this.mainQuest.imgMain.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(mainQuestData.characterCode, 0);
		this.UpdateMainQuestTitle();
		this.mainQuest.txtMainComment.text = Ln.Get(mainQuestData.mainComment ?? "");
		bool active = mainQuestData.detailGroupId != -1;
		this.mainQuest.btnDetail.gameObject.SetActive(active);
		this.mainQuest.btnDetail.onClick.AddListener(delegate()
		{
			this.questDetailButtonClick(mainQuestData.detailGroupId);
		});
	}

	
	private void UpdateMainQuestTitle()
	{
		switch (this.mainQuestData.tutorialType)
		{
		case TutorialType.BasicGuide:
		case TutorialType.Trace:
		case TutorialType.FinalSurvival:
			this.mainQuest.txtMainTitle.text = Ln.Get(this.mainQuestData.mainTitle ?? "");
			return;
		case TutorialType.Hunt:
		case TutorialType.PowerUp:
			this.mainQuest.txtMainTitle.text = string.Format("{0} ({1}/{2})", Ln.Get(this.mainQuestData.mainTitle ?? ""), this.mainQuestStack, this.mainQuestData.targetStack);
			return;
		default:
			return;
		}
	}

	
	public void AddMainQuestStack()
	{
		this.mainQuestStack++;
		if (this.mainQuestData != null)
		{
			this.mainQuestStack = Mathf.Min(this.mainQuestStack, this.mainQuestData.targetStack);
			this.UpdateMainQuestTitle();
		}
	}

	
	public void AddSubQuest(int code, Action completedAction = null)
	{
		if (this.historySubQuests.Exists((int x) => x == code))
		{
			return;
		}
		if (this.subQuestDatas == null)
		{
			return;
		}
		TutorialSubQuestData subQuestData = this.subQuestDatas.Find((TutorialSubQuestData x) => x.code == code);
		TutorialSubQuest subQuest = UnityEngine.Object.Instantiate<TutorialSubQuest>(this.subQuestPrefab, base.transform);
		subQuest.SetCode(code);
		subQuest.SetIndex(this.subQuests.Count);
		subQuest.SetkMark(false);
		subQuest.SetTutorialInfos(subQuestData.tutorialQuestInfos);
		subQuest.SetComment(subQuestData.comment);
		subQuest.TweenPositionIn();
		bool active = subQuestData.detailGroupId != -1;
		subQuest.BtnDetail.gameObject.SetActive(active);
		subQuest.BtnDetail.onClick.AddListener(delegate()
		{
			if (!subQuest.Moving)
			{
				this.questDetailButtonClick(subQuestData.detailGroupId);
			}
		});
		subQuest.OnFinishIn = delegate()
		{
			this.RepositionSubQuests();
			this.AlreadySuccssSubQuest(subQuest);
		};
		subQuest.OnFinishOut = delegate()
		{
			this.RepositionSubQuests();
			if (completedAction != null)
			{
				completedAction();
			}
		};
		this.subQuests.Add(subQuest);
		this.historySubQuests.Add(code);
	}

	
	private void AlreadySuccssSubQuest(TutorialSubQuest subQuest)
	{
		foreach (TutorialQuestInfo tutorialQuestInfo in subQuest.TutorialQuestInfos)
		{
			foreach (TutorialQuestInfo tutorialQuestInfo2 in MonoBehaviourInstance<TutorialController>.inst.AlreadySuccessQuestInfos)
			{
				if (tutorialQuestInfo.TutorialQuestType == tutorialQuestInfo2.TutorialQuestType)
				{
					subQuest.Success(tutorialQuestInfo.TutorialQuestType);
				}
			}
		}
	}

	
	public void SuccessSubQuest(TutorialQuestType tutorialQuestType)
	{
		foreach (TutorialSubQuest tutorialSubQuest in this.subQuests)
		{
			tutorialSubQuest.Success(tutorialQuestType);
		}
	}

	
	private void RepositionSubQuests()
	{
		using (List<TutorialSubQuest>.Enumerator enumerator = this.subQuests.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.PositionTweener.IsPlaying)
				{
					return;
				}
			}
		}
		List<TutorialSubQuest> list = new List<TutorialSubQuest>();
		for (int i = 0; i < this.subQuests.Count; i++)
		{
			if (this.subQuests[i].RemoveFlag)
			{
				UnityEngine.Object.Destroy(this.subQuests[i].gameObject);
				this.subQuests[i] = null;
				list.Add(this.subQuests[i]);
			}
		}
		this.subQuests.RemoveAll(new Predicate<TutorialSubQuest>(list.Contains));
		for (int j = 0; j < this.subQuests.Count; j++)
		{
			this.subQuests[j].PositionTweener.StopAnimation();
			this.subQuests[j].SetIndex(j);
			this.subQuests[j].SetPosition();
		}
	}

	
	public void Hide()
	{
		base.gameObject.SetActive(false);
		for (int i = 0; i < this.subQuests.Count; i++)
		{
			UnityEngine.Object.Destroy(this.subQuests[i].gameObject);
		}
		this.subQuests.Clear();
		this.historySubQuests.Clear();
	}

	
	[SerializeField]
	private TutorialSubQuest subQuestPrefab = default;

	
	[SerializeField]
	private TutorialQuest.MainQuest mainQuest = default;

	
	private TutorialMainQuestData mainQuestData;

	
	private List<TutorialSubQuestData> subQuestDatas;

	
	private List<TutorialSubQuest> subQuests = new List<TutorialSubQuest>();

	
	private List<int> historySubQuests = new List<int>();

	
	public TutorialQuest.DetailButtonClick questDetailButtonClick;

	
	private Action mainQuestCompletedAction;

	
	private int mainQuestStack;

	
	[Serializable]
	public class MainQuest
	{
		
		public GameObject obj = default;

		
		public Image imgMain = default;

		
		public Text txtMainTitle = default;

		
		public Text txtMainComment = default;

		
		public Button btnDetail = default;
	}

	
	// (Invoke) Token: 0x06004DCF RID: 19919
	public delegate void DetailButtonClick(int detailGroupId);
}
