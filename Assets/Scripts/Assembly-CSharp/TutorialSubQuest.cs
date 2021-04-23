using System.Collections.Generic;
using Blis.Client;
using UnityEngine;
using UnityEngine.UI;


public class TutorialSubQuest : MonoBehaviour
{
	
	
	public int Code
	{
		get
		{
			return this.code;
		}
	}

	
	
	public int Index
	{
		get
		{
			return this.index;
		}
	}

	
	
	public bool RemoveFlag
	{
		get
		{
			return this.removeFlag;
		}
	}

	
	
	public bool Moving
	{
		get
		{
			return this.moving;
		}
	}

	
	
	public Button BtnDetail
	{
		get
		{
			return this.btnDetail;
		}
	}

	
	
	public PositionTweener PositionTweener
	{
		get
		{
			return this.positionTweener;
		}
	}

	
	
	public List<TutorialQuestInfo> TutorialQuestInfos
	{
		get
		{
			return this.tutorialQuestInfos;
		}
	}

	
	public void SetCode(int code)
	{
		this.code = code;
	}

	
	public void SetIndex(int index)
	{
		this.index = index;
	}

	
	public void SetkMark(bool complete)
	{
		if (complete)
		{
			this.imgBG.color = new Color(0f, 1f, 1f, 1f);
		}
		else
		{
			this.imgBG.color = Color.white;
		}
		this.imgCheckMark.gameObject.SetActive(complete);
		this.imgExclamationMark.gameObject.SetActive(!complete);
	}

	
	public void SetTutorialInfos(List<TutorialQuestInfo> tutorialQuestInfos)
	{
		this.tutorialQuestInfos = tutorialQuestInfos;
	}

	
	public void SetComment(string comment)
	{
		this.txtComment.text = Ln.Get(comment ?? "");
	}

	
	public void Success(TutorialQuestType tutorialQuestType)
	{
		if (this.isComplete)
		{
			return;
		}
		TutorialQuestInfo tutorialQuestInfo = this.tutorialQuestInfos.Find((TutorialQuestInfo x) => x.TutorialQuestType == tutorialQuestType);
		if (tutorialQuestInfo == null)
		{
			return;
		}
		tutorialQuestInfo.Success();
		int num = 0;
		using (List<TutorialQuestInfo>.Enumerator enumerator = this.tutorialQuestInfos.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsSuccess)
				{
					num++;
				}
			}
		}
		if (num == this.tutorialQuestInfos.Count)
		{
			this.isComplete = true;
			this.SetkMark(true);
			this.TweenPositionOut();
		}
	}

	
	public void SetPosition()
	{
		float y = -156f - 38f * (float)this.index;
		base.transform.localPosition = new Vector3(400f, y, 0f);
	}

	
	public void TweenPositionIn()
	{
		this.moving = true;
		float y = -156f - 38f * (float)this.index;
		Vector3 from = new Vector3(0f, y, 0f);
		Vector3 to = new Vector3(400f, y, 0f);
		this.positionTweener.enabled = false;
		this.positionTweener.from = from;
		this.positionTweener.to = to;
		this.positionTweener.speed = 0.6f;
		this.positionTweener.SetCurve(this.curveIn);
		this.positionTweener.PlayAnimation();
		this.positionTweener.enabled = true;
		this.positionTweener.OnAnimationFinish += delegate()
		{
			this.moving = false;
			this.OnFinishIn();
		};
	}

	
	public void TweenPositionOut()
	{
		this.moving = true;
		float y = -156f - 38f * (float)this.index;
		Vector3 from = new Vector3(400f, y, 0f);
		Vector3 to = new Vector3(0f, y, 0f);
		this.positionTweener.enabled = false;
		this.positionTweener.from = from;
		this.positionTweener.to = to;
		this.positionTweener.speed = 0.8f;
		this.positionTweener.SetCurve(this.curveOut);
		this.positionTweener.PlayAnimation();
		this.positionTweener.enabled = true;
		this.positionTweener.OnAnimationFinish += delegate()
		{
			this.moving = false;
			this.removeFlag = true;
			this.OnFinishOut();
		};
	}

	
	[SerializeField]
	private Image imgBG = default;

	
	[SerializeField]
	private Image imgCheckMark = default;

	
	[SerializeField]
	private Image imgExclamationMark = default;

	
	[SerializeField]
	private Text txtComment = default;

	
	[SerializeField]
	private Button btnDetail = default;

	
	[SerializeField]
	private PositionTweener positionTweener = default;

	
	[SerializeField]
	private AnimationCurve curveIn = default;

	
	[SerializeField]
	private AnimationCurve curveOut = default;

	
	private int code = default;

	
	private int index = default;

	
	private bool removeFlag = default;

	
	private bool moving = default;

	
	private bool isComplete = default;

	
	public TutorialSubQuest.TweenPositionFinishEvent OnFinishIn = default;

	
	public TutorialSubQuest.TweenPositionFinishEvent OnFinishOut = default;

	
	private List<TutorialQuestInfo> tutorialQuestInfos = new List<TutorialQuestInfo>();

	
	private const float mainQuestHeight = 156f;

	
	private const float subQuestHeight = 28f;

	
	private const float subQuestSpacing = 10f;

	
	// (Invoke) Token: 0x06004DDB RID: 19931
	public delegate void TweenPositionFinishEvent();
}
