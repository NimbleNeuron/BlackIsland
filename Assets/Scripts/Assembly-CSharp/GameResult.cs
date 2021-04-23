using System.Linq;
using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;


public class GameResult : BaseUI
{
	
	protected override void OnAwakeUI()
	{
		base.OnAwakeUI();
		this.observerResult = GameUtil.Bind<CanvasGroup>(base.gameObject, "ObserverContent");
		this.observerCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(base.gameObject, "ObserverContent");
		this.DisableObserverResult();
		this.normalResult = GameUtil.Bind<CanvasGroup>(base.gameObject, "Content");
		this.normalCanvasAlphaTweener = GameUtil.Bind<CanvasAlphaTweener>(base.gameObject, "Content");
		this.DisableResult();
		this.bg = GameUtil.Bind<Image>(base.gameObject, "Bg");
		this.bgTweener = GameUtil.Bind<ColorTweener>(base.gameObject, "Bg");
		this.DisableBg();
		this.textTitle = GameUtil.Bind<LnText>(base.gameObject, "Content/Title");
		this.textRank = GameUtil.Bind<LnText>(base.gameObject, "Content/Result/Rank/Rank");
		this.textKiller = GameUtil.Bind<LnText>(base.gameObject, "Content/ResultType/DeadEnd_P/KillerNickname");
		this.textKillerTempNickname = GameUtil.Bind<LnText>(base.gameObject, "Content/ResultType/DeadEnd_P/KillerTempNickname");
		this.textNickname = GameUtil.Bind<LnText>(base.gameObject, "Content/Result/Name/Nickname");
		this.textTotalUser = GameUtil.Bind<LnText>(base.gameObject, "Content/Result/Rank/MaxPlayer");
		this.deadEnd_RMsg = GameUtil.Bind<LnText>(base.gameObject, "Content/ResultType/DeadEnd_R/Text");
		this.deadEnd_P = GameUtil.Bind<Transform>(base.gameObject, "Content/ResultType/DeadEnd_P").gameObject;
		this.deadEnd_R = GameUtil.Bind<Transform>(base.gameObject, "Content/ResultType/DeadEnd_R").gameObject;
		this.windEnd = GameUtil.Bind<Transform>(base.gameObject, "Content/ResultType/WinEnd").gameObject;
		this.imgKiller = GameUtil.Bind<Image>(this.deadEnd_P, "KillerImage/Image");
		this.imgPlayer = GameUtil.Bind<Image>(base.gameObject, "Content/MyChaImage");
		this.reportBtn = GameUtil.Bind<Button>(base.gameObject, "Content/ReportBtn");
		this.watchBtn = GameUtil.Bind<Button>(base.gameObject, "Content/WatchBtn");
		this.textWinnerTempNickname = GameUtil.Bind<LnText>(base.gameObject, "Content/Result/Name/TempNickname");
		this.rank = null;
		this.isObserving = false;
	}

	
	private void SetResultType(GameResult.ResultType resultType)
	{
		this.deadEnd_P.SetActive(false);
		this.deadEnd_R.SetActive(false);
		this.windEnd.SetActive(false);
		this.textWinnerTempNickname.gameObject.SetActive(false);
		switch (resultType)
		{
		case GameResult.ResultType.DeadEnd_P:
			this.deadEnd_P.SetActive(true);
			return;
		case GameResult.ResultType.DeadEnd_R:
			this.deadEnd_RMsg.text = Ln.Get("금지구역 사망");
			this.deadEnd_R.SetActive(true);
			return;
		case GameResult.ResultType.DeadEnd_M:
			this.deadEnd_RMsg.text = Ln.Get("야생동물 사망");
			this.deadEnd_R.SetActive(true);
			return;
		case GameResult.ResultType.WinEnd:
			this.windEnd.SetActive(true);
			this.textWinnerTempNickname.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	
	public void Deactive()
	{
		base.gameObject.SetActive(false);
		this.DisableResult();
		this.DisableObserverResult();
		this.DisableBg();
	}

	
	private void DisableResult()
	{
		this.normalCanvasAlphaTweener.enabled = false;
		this.normalResult.alpha = 0f;
		this.normalResult.interactable = false;
		this.normalResult.blocksRaycasts = false;
	}

	
	private void DisableObserverResult()
	{
		this.observerCanvasAlphaTweener.enabled = false;
		this.observerResult.alpha = 0f;
		this.observerResult.interactable = false;
		this.observerResult.blocksRaycasts = false;
	}

	
	private void DisableBg()
	{
		this.bgTweener.enabled = false;
		this.bg.enabled = false;
	}

	
	public void ShowObserverResult()
	{
		base.gameObject.SetActive(true);
		this.DisableResult();
		if (!this.observerCanvasAlphaTweener.IsPlaying)
		{
			this.observerCanvasAlphaTweener.PlayAnimation();
		}
		this.observerResult.interactable = true;
		this.observerResult.blocksRaycasts = true;
		this.bg.enabled = true;
		if (!this.bgTweener.IsPlaying)
		{
			this.bgTweener.PlayAnimation();
		}
	}

	
	public void ShowResult(int? rank, int? total, int killerObjectId, string killerName, string killerTempName, string myNickname, Sprite killerSkin, Sprite mySkin, bool isMonster, int pkCount, int mkCount, bool finishedGame)
	{
		base.gameObject.SetActive(true);
		this.rank = rank;
		this.DisableObserverResult();
		if (!this.normalCanvasAlphaTweener.IsPlaying)
		{
			this.normalCanvasAlphaTweener.PlayAnimation();
		}
		this.normalResult.interactable = true;
		this.normalResult.blocksRaycasts = true;
		this.bg.enabled = true;
		if (!this.bgTweener.IsPlaying)
		{
			this.bgTweener.PlayAnimation();
		}
		if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene && MonoBehaviourInstance<ClientService>.inst != null && MonoBehaviourInstance<ClientService>.inst.IsPlayer && MonoBehaviourInstance<ClientService>.inst.MyPlayer.IsObserving)
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.TeamMembers.Any((LocalPlayerCharacter x) => x.IsAlive))
			{
				foreach (LocalPlayerCharacter localPlayerCharacter in MonoBehaviourInstance<ClientService>.inst.MyPlayer.TeamMembers)
				{
					Log.V(string.Format("[OBSERVING BUG][ShowResult] Team is Alive. Rank({0}), Member({1}), IsAlive({2}), IsDyingCondition({3}), HP({4})", new object[]
					{
						rank ?? -999,
						localPlayerCharacter.Nickname,
						localPlayerCharacter.IsAlive,
						localPlayerCharacter.IsDyingCondition,
						localPlayerCharacter.Status.Hp
					}));
				}
			}
		}
		if (this.textRank != null)
		{
			if (rank != null)
			{
				this.textRank.text = string.Format("{0}", rank.Value);
			}
			else
			{
				this.textRank.text = null;
			}
		}
		if (this.textTotalUser != null)
		{
			if (total != null)
			{
				this.textTotalUser.text = string.Format("/{0}", total.Value);
			}
			else
			{
				this.textTotalUser.text = null;
			}
		}
		if (this.textKiller != null)
		{
			this.textKiller.text = killerName;
		}
		if (this.textNickname != null)
		{
			this.textNickname.text = myNickname;
		}
		if (this.textKillerTempNickname != null)
		{
			this.textKillerTempNickname.text = (string.IsNullOrEmpty(killerTempName) ? "" : ("[" + killerTempName + "]"));
		}
		if (this.textWinnerTempNickname != null)
		{
			this.textWinnerTempNickname.text = (string.IsNullOrEmpty(killerTempName) ? "" : ("[" + killerTempName + "]"));
		}
		if (MonoBehaviourInstance<GameClient>.inst.IsTutorial && this.textTitle != null)
		{
			this.textTitle.text = Ln.Get("완료");
		}
		if (rank != null && rank.Value <= 1)
		{
			this.SetResultType(GameResult.ResultType.WinEnd);
		}
		else if (killerObjectId == 0)
		{
			this.SetResultType(GameResult.ResultType.DeadEnd_R);
		}
		else
		{
			if (killerSkin != null)
			{
				this.imgKiller.sprite = killerSkin;
			}
			this.SetResultType(isMonster ? GameResult.ResultType.DeadEnd_M : GameResult.ResultType.DeadEnd_P);
		}
		if (mySkin != null)
		{
			this.imgPlayer.sprite = mySkin;
		}
		this.ShowReportButton(false);
		this.ShowWatchButton(finishedGame, rank == null);
	}

	
	private void ShowReportButton(bool showReport)
	{
		MatchingMode matchingMode = GlobalUserData.matchingMode;
		if (matchingMode - MatchingMode.Normal > 1)
		{
			showReport = false;
		}
		if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
		{
			showReport = false;
		}
		this.reportBtn.gameObject.SetActive(showReport);
	}

	
	private void ShowWatchButton(bool finishedGame, bool showWatch)
	{
		if (finishedGame)
		{
			this.watchBtn.gameObject.SetActive(false);
			return;
		}
		MatchingMode matchingMode = MonoBehaviourInstance<GameClient>.inst.MatchingMode;
		if (matchingMode > MatchingMode.Rank)
		{
			if (matchingMode != MatchingMode.Custom)
			{
				showWatch = false;
			}
			else if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				if (this.rank != null && 2 < this.rank.Value)
				{
					this.watchBtn.gameObject.SetActive(true);
					return;
				}
			}
			else
			{
				showWatch = false;
			}
		}
		else
		{
			MatchingTeamMode matchingTeamMode = MonoBehaviourInstance<GameClient>.inst.MatchingTeamMode;
			if (matchingTeamMode == MatchingTeamMode.Solo)
			{
				showWatch = false;
			}
		}
		this.watchBtn.gameObject.SetActive(showWatch && !this.isObserving);
	}

	
	public void GoToLobby()
	{
		if (MonoBehaviourInstance<GameClient>.inst.IsTutorial || !MonoBehaviourInstance<ClientService>.inst.IsPlayer)
		{
			MonoBehaviourInstance<ClientService>.inst.GoToLobby();
			return;
		}
		if (MonoBehaviourInstance<GameClient>.inst.IsConnected)
		{
			MonoBehaviourInstance<GameClient>.inst.Request(new ReqExitGame(), NetChannel.ReliableOrdered);
			return;
		}
		MonoBehaviourInstance<ClientService>.inst.GoToLobby();
	}

	
	public void StartWatching()
	{
		if (MonoBehaviourInstance<ClientService>.inst.IsPlayer && MonoBehaviourInstance<GameClient>.inst.MatchingMode == MatchingMode.Custom && this.rank != null)
		{
			this.CustomObserving();
			return;
		}
		this.StartObserving();
	}

	
	private void CustomObserving()
	{
		if (MonoBehaviourInstance<GameClient>.inst.IsTeamMode)
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.TeamMembers.Any((LocalPlayerCharacter x) => x.IsAlive))
			{
				this.StartObserving();
				return;
			}
		}
		MonoBehaviourInstance<GameClient>.inst.Request(new ReqChangeToObserver(), NetChannel.ReliableOrdered);
	}

	
	private void StartObserving()
	{
		this.Deactive();
		MonoBehaviourInstance<GameUI>.inst.MainHud.SetActive(false);
		MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.SetWatchMode();
		MonoBehaviourInstance<GameUI>.inst.NavigationHud.gameObject.SetActive(false);
		MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowNaviAreaItem(false);
		MonoBehaviourInstance<GameUI>.inst.HudButton.SetWatchMode();
		SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Normal);
		MonoBehaviourInstance<ClientService>.inst.SetGamePlayMode(ClientService.GamePlayMode.ObserveTeam);
		MonoBehaviourInstance<GameUI>.inst.BloodFx.Stop();
		MonoBehaviourInstance<MobaCamera>.inst.TrackingAliveTeamPlayer(true);
		MonoBehaviourInstance<GameUI>.inst.WatchingHud.SetActive(true);
		MonoBehaviourInstance<GameClient>.inst.Request(new ReqReqObserving(), NetChannel.ReliableOrdered);
		this.isObserving = true;
	}

	
	private CanvasGroup observerResult;

	
	private CanvasAlphaTweener observerCanvasAlphaTweener;

	
	private CanvasGroup normalResult;

	
	private CanvasAlphaTweener normalCanvasAlphaTweener;

	
	private Image bg;

	
	private ColorTweener bgTweener;

	
	private LnText textTitle;

	
	private LnText textRank;

	
	private LnText textKiller;

	
	private LnText textKillerTempNickname;

	
	private LnText textNickname;

	
	private LnText textTotalUser;

	
	private LnText deadEnd_RMsg;

	
	private GameObject deadEnd_P;

	
	private GameObject deadEnd_R;

	
	private GameObject windEnd;

	
	private Image imgKiller;

	
	private Image imgPlayer;

	
	private Button reportBtn;

	
	private Button watchBtn;

	
	private LnText textWinnerTempNickname;

	
	private int? rank;

	
	private bool isObserving;

	
	private bool RequestedExitTeamGame;

	
	private Coroutine goToLobby;

	
	private enum ResultType
	{
		
		DeadEnd_P,
		
		DeadEnd_R,
		
		DeadEnd_M,
		
		WinEnd
	}
}
