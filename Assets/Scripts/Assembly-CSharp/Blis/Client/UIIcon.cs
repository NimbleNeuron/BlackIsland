using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIIcon : BaseUI
	{
		private const float innerSmallSize = 0.75f;


		private const float innerBigSize = 1f;


		private Image alertMask;


		private ColorTweener alertTweener;


		private Image arrow;


		private RectTransform inner;


		private MiniMapIconType miniMapIconType;


		private Text nickname;


		private RectTransform observerTeam;


		private Image observerTeamColor;


		private Text observerTeamNumber;


		private RectTransform outer;


		private Image portrait;


		private Image teamColor;

		protected override void Awake()
		{
			base.Awake();
			outer = GameUtil.Bind<RectTransform>(gameObject, "Outer");
			inner = GameUtil.Bind<RectTransform>(gameObject, "Inner");
			arrow = GameUtil.Bind<Image>(outer.gameObject, "Arrow");
			arrow.enabled = false;
			teamColor = GameUtil.Bind<Image>(inner.gameObject, "Bg");
			teamColor.enabled = false;
			portrait = GameUtil.Bind<Image>(inner.gameObject, "Face");
			portrait.enabled = false;
			alertMask = GameUtil.Bind<Image>(inner.gameObject, "Alert");
			alertMask.enabled = false;
			nickname = GameUtil.Bind<Text>(inner.gameObject, "Name");
			nickname.enabled = false;
			alertTweener = alertMask.GetComponent<ColorTweener>();
			alertTweener.StopAnimation();
			observerTeam = GameUtil.Bind<RectTransform>(inner.gameObject, "Team");
			observerTeamColor = GameUtil.Bind<Image>(observerTeam.gameObject, "ObserverBg");
			observerTeamNumber = GameUtil.Bind<Text>(observerTeam.gameObject, "Txt_Number");
			outer.localScale = Vector3.zero;
			inner.localScale = Vector3.one * 1f;
		}


		public void SetColor(Color color)
		{
			arrow.color = color;
		}


		public void SetBackground(Sprite sprite)
		{
			teamColor.enabled = sprite != null;
			teamColor.sprite = sprite;
			teamColor.SetNativeSize();
			teamColor.preserveAspect = true;
			teamColor.transform.localScale = Vector3.one;
		}


		public void SetPortrait(Sprite sprite)
		{
			portrait.enabled = sprite != null;
			portrait.sprite = sprite;
			portrait.SetNativeSize();
			portrait.preserveAspect = true;
			portrait.transform.localScale = Vector3.one;
		}


		public void SetNickname(string nickname)
		{
			if (string.IsNullOrEmpty(nickname))
			{
				return;
			}

			this.nickname.text = nickname;
			this.nickname.enabled = true;
		}


		public void SetTeam(int teamNumber)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer || teamNumber == 0 ||
			    !MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				observerTeam.gameObject.SetActive(false);
				return;
			}

			List<int> list = MonoBehaviourInstance<ClientService>.inst.GetTeams().Keys.ToList<int>();
			observerTeam.gameObject.SetActive(true);
			int num = list.IndexOf(teamNumber);
			observerTeamColor.color = UIUtility.ObserverTeamColor(num);
			observerTeamNumber.text = (num + 1).ToString();
		}


		public void Alive()
		{
			alertMask.enabled = false;
			alertTweener.StopAnimation();
		}


		public void DyingCondition()
		{
			alertMask.enabled = true;
			alertTweener.PlayAnimation();
		}


		public void Out(Quaternion rotation)
		{
			outer.localScale = Vector3.one;
			inner.localScale = Vector2.one * 0.75f;
			outer.transform.rotation = rotation;
			arrow.enabled = true;
		}


		public void In()
		{
			outer.localScale = Vector3.zero;
			inner.localScale = Vector2.one * 1f;
			arrow.enabled = false;
		}


		public void SetMiniMapIconType(MiniMapIconType miniMapIconType)
		{
			this.miniMapIconType = miniMapIconType;
		}


		public MiniMapIconType GetMiniMapIconType()
		{
			return miniMapIconType;
		}


		public void Clear()
		{
			arrow.enabled = false;
			teamColor.enabled = false;
			portrait.enabled = false;
			alertMask.enabled = false;
			alertTweener.StopAnimation();
			miniMapIconType = MiniMapIconType.None;
		}
	}
}