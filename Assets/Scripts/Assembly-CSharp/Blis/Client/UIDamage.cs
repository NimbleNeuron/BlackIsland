using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIDamage : BaseTrackUI
	{
		public enum AnimationType
		{
			None,

			NormalDamaged,

			CriticalDamaged,

			BurstDamaged,

			HpRecovery,

			SpRecovery
		}


		public enum Direction
		{
			None,

			Right = -1,

			Left = 1
		}


		[SerializeField] private int fontSize = default;


		private readonly int criFontSize = 45;


		private readonly Color hpHealColor = Color.green;


		private readonly Color normalDamageColor = Color.red;


		private readonly int normalFontSize = 30;


		private readonly Color skillDamageColor = Color.magenta;


		private readonly Color spDamageColor = new Color(0.67f, 0.84f, 0.9f);


		private readonly Color spHealColor = Color.cyan;


		private Animator animator;


		private CanvasGroup canvasGroup;


		private LnText label;


		private RectTransform layout;


		private Action onFinish;


		private Outline outline;


		private Vector3 sortPosition;

		protected override void Awake()
		{
			base.Awake();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			layout = GameUtil.Bind<RectTransform>(gameObject, "Layout");
			label = GameUtil.Bind<LnText>(gameObject, "Layout/Text");
			outline = GameUtil.Bind<Outline>(gameObject, "Layout/Text");
			GameUtil.Bind<Animator>(gameObject, ref animator);
		}


		public void FinishEvent()
		{
			animator.enabled = false;
			canvasGroup.alpha = 0f;
			foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
			{
				if (animatorControllerParameter.type == AnimatorControllerParameterType.Trigger)
				{
					animator.ResetTrigger(animatorControllerParameter.name);
				}
				else if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
				{
					animator.SetFloat(animatorControllerParameter.name, 0f);
				}
			}

			layout.anchoredPosition = Vector3.zero;
			Action action = onFinish;
			if (action == null)
			{
				return;
			}

			action();
		}


		public override void ResetUI()
		{
			base.ResetUI();
			animator.enabled = true;
			label.text = null;
		}


		protected override void UpdateUI()
		{
			base.UpdateUI();
			transform.position += sortPosition;
			sortPosition = Vector3.zero;
			if (label != null && label.fontSize != fontSize)
			{
				label.fontSize = fontSize;
			}
		}


		public void SetDamage(bool isSkillDamage, AnimationType aniType, Direction direction, int damage,
			Action onFinish)
		{
			SetUI(aniType, direction, damage, isSkillDamage ? skillDamageColor : normalDamageColor, onFinish);
		}


		public void SetSpDamage(AnimationType aniType, Direction direction, int damage, Action onFinish)
		{
			SetUI(aniType, direction, damage, spDamageColor, onFinish);
		}


		public void SetHpHeal(AnimationType aniType, int heal, Action onFinish)
		{
			SetUI(aniType, Direction.None, heal, hpHealColor, onFinish);
		}


		public void SetSpHeal(AnimationType aniType, int heal, Action onFinish)
		{
			SetUI(aniType, Direction.None, heal, spHealColor, onFinish);
		}


		private void SetUI(AnimationType aniType, Direction direction, int damage, Color damageColor, Action onFinish)
		{
			this.onFinish = onFinish;
			label.text = damage.ToString();
			label.color = damageColor;
			outline.effectColor = Color.black;
			animator.SetFloat("Direction", (float) direction);
			animator.SetTrigger(aniType.ToString());
		}


		public void SetLabel(string text, int fontSize, Color color, Color outlineColor, Action onFinish)
		{
			this.onFinish = onFinish;
			label.fontSize = fontSize;
			label.text = text;
			label.color = color;
			outline.effectColor = outlineColor;
			animator.SetTrigger(AnimationType.None.ToString());
		}


		public void SetLabel(AnimationType aniType, Direction direction, string text, int fontSize, Color color,
			Color outlineColor, Action onFinish)
		{
			this.onFinish = onFinish;
			label.fontSize = fontSize;
			label.text = text;
			label.color = color;
			outline.effectColor = outlineColor;
			animator.SetFloat("Direction", (float) direction);
			animator.SetTrigger(aniType.ToString());
		}

		private void Ref()
		{
			Reference.Use(normalFontSize);
			Reference.Use(criFontSize);
		}
	}
}