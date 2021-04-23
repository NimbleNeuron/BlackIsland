using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using DG.Tweening;
using UnityEngine;

namespace Blis.Client
{
	public class KoreaGameGradeUtil : SingletonMonoBehaviour<KoreaGameGradeUtil>
	{
		public delegate void OnPlayTimeNotice(int hour);


		private const int OneHour = 3600;


		public bool IsKorea;


		public bool GameGradeShown;


		public float LastHourTime;


		public int CurHour;


		private DOTweenAnimation GameGradeToast;


		private bool isTimeNotice;


		private bool UsingCompleteGameGrade;


		private Vector3 vPos;


		public void Update()
		{
			TimeNoticeCheck();
		}

		
		
		public event OnPlayTimeNotice onPlayTimeNotice = delegate { };


		protected override void OnAwakeSingleton()
		{
			base.OnAwakeSingleton();
			DontDestroyOnLoad(this);
		}


		public void SetGameGradeToast(GameObject gameGardeToast)
		{
			GameGradeToast = gameGardeToast != null ? gameGardeToast.GetComponent<DOTweenAnimation>() : null;
		}


		public void GradeToastReStart()
		{
			if (GameGradeToast == null)
			{
				return;
			}

			if (!IsKorea)
			{
				return;
			}

			if (!GameGradeToast.gameObject.activeSelf)
			{
				GameGradeToast.gameObject.SetActive(true);
			}

			GameGradeToast.DORestart();
		}


		public void GradeToastRewind()
		{
			if (GameGradeToast == null)
			{
				return;
			}

			if (!IsKorea)
			{
				return;
			}

			GameGradeToast.DORewind();
		}


		public IEnumerator ShowLobbyStart()
		{
			if (GameGradeShown)
			{
				if (IsKorea)
				{
					isTimeNotice = true;
				}

				yield break;
			}

			bool IsShowAccessTermsPopup = false;
			bool IsShowGameGrade = false;
			if (GlobalUserData.showGrade)
			{
				IsShowAccessTermsPopup = true;
				IsShowGameGrade = true;
				IsKorea = true;
				isTimeNotice = true;
			}
			else
			{
				if (GlobalUserData.showGrade || Ln.GetCurrentLanguage() != SupportLanguage.Korean)
				{
					GameGradeShown = true;
					yield break;
				}

				IsShowAccessTermsPopup = true;
				IsShowGameGrade = true;
				IsKorea = true;
				isTimeNotice = true;
			}

			if (IsShowAccessTermsPopup)
			{
				string str = SteamApi.GetSteamID().m_SteamID.ToString();
				string text = "AccessTerms_" + str;
				if ((PlayerPrefs.HasKey(text) ? PlayerPrefs.GetInt(text) : 0) != 2)
				{
					MonoBehaviourInstance<Popup>.inst.BlackCurtain.SetActive(false);
					MonoBehaviourInstance<Popup>.inst.AccessTermsUI.ShowAccessTermsUI(text);
				}
				else
				{
					IsShowAccessTermsPopup = false;
				}
			}

			bool IsCheckedAccessTermsPopup = IsShowAccessTermsPopup;
			while (IsCheckedAccessTermsPopup)
			{
				IsCheckedAccessTermsPopup = !MonoBehaviourInstance<Popup>.inst.AccessTermsUI.IsAccessTermsAgreement();
				yield return null;
			}

			if (IsShowGameGrade)
			{
				MonoBehaviourInstance<Popup>.inst.BlackCurtain.SetActive(false);
				MonoBehaviourInstance<Popup>.inst.GameGrade.SetActive(true);
			}

			yield return new WaitForFixedUpdate();
			if (IsShowAccessTermsPopup)
			{
				MonoBehaviourInstance<Popup>.inst.AccessTermsUI.CloseAccessTermsUI();
			}

			GameGradeShown = true;
		}


		public void GameGradeLogoPlay()
		{
			GameObject gameObject;
			if (IsKorea && !UsingCompleteGameGrade)
			{
				UsingCompleteGameGrade = true;
				gameObject = MonoBehaviourInstance<Popup>.inst.GameGrade;
			}
			else
			{
				gameObject = MonoBehaviourInstance<Popup>.inst.BlackCurtain;
			}

			DOTweenAnimation[] componentsInChildren = gameObject.GetComponentsInChildren<DOTweenAnimation>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].DOPlay();
				}
			}
		}


		public void TimeNoticeCheck()
		{
			if (!IsKorea || !isTimeNotice)
			{
				return;
			}

			if (3600f <= Time.realtimeSinceStartup - LastHourTime)
			{
				CurHour = (int) (Time.realtimeSinceStartup / 3600f);
				LastHourTime = CurHour * 3600;
				OnPlayTimeNotice onPlayTimeNotice = this.onPlayTimeNotice;
				if (onPlayTimeNotice == null)
				{
					return;
				}

				onPlayTimeNotice(CurHour);
			}
		}
	}
}