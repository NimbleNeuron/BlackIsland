using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UICooldown : BaseUI
	{
		public enum FillAmountType
		{
			NONE,

			FORWARD,

			OVERLAP,

			RADIAL,

			HORIZONTAL,

			STACK_FORWARD,

			BULLET_FORWARD
		}


		private const float GLOBAL_COOLDOWN = 0.1f;


		public Image radial;


		public Image horizontal;


		[SerializeField] private Image stackForward = default;


		[SerializeField] private Image bulletForward = default;


		private Image animateImage;


		private float cooldown;


		private FillAmountType curFillAmountType;


		private Action finishAction;


		private Image forward;


		private bool isHold;


		private float maxCooldown;


		public Action OnSkill;


		private Image overlap;


		private int? pastTimerText;


		private float pauseCooldownRemainTime;


		private bool playInvoke;


		private float stackIntervalTime;


		private float stackIntervalTimeMax;


		private LnText timer;


		private bool usingTimer;


		private void Update()
		{
			if (isHold)
			{
				return;
			}

			if (stackIntervalTime > 0f)
			{
				stackIntervalTime -= Time.deltaTime;
				if (stackIntervalTime <= 0f)
				{
					InitIntervalTime();
				}
			}

			if (curFillAmountType == FillAmountType.NONE)
			{
				if (stackIntervalTime > 0f)
				{
					UpdateUI();
				}

				return;
			}

			float num = Time.deltaTime;
			if (pauseCooldownRemainTime > 0f)
			{
				pauseCooldownRemainTime -= num;
				if (pauseCooldownRemainTime > 0f)
				{
					return;
				}

				num = -pauseCooldownRemainTime;
				pauseCooldownRemainTime = 0f;
			}

			if (0f < cooldown && 0f < maxCooldown)
			{
				UpdateUI();
				cooldown -= num;
				if (cooldown <= 0f)
				{
					Finish();
				}
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			overlap = GameUtil.Bind<Image>(gameObject, "Reverse");
			forward = GameUtil.Bind<Image>(gameObject, "Forward");
			timer = GameUtil.Bind<LnText>(gameObject, "Text");
			Init();
		}


		public void Init()
		{
			cooldown = 0f;
			maxCooldown = 0f;
			curFillAmountType = FillAmountType.NONE;
			isHold = false;
			ResetUI();
		}


		public void InitIntervalTime()
		{
			stackIntervalTimeMax = 0f;
			stackIntervalTime = 0f;
			forward.fillAmount = 0f;
		}


		private void ResetUI()
		{
			if (stackIntervalTime == 0f)
			{
				forward.fillAmount = 0f;
			}

			overlap.fillAmount = 0f;
			if (radial != null)
			{
				radial.fillAmount = 0f;
			}

			if (stackForward != null)
			{
				stackForward.fillAmount = 0f;
			}

			if (horizontal != null)
			{
				horizontal.fillAmount = 0f;
			}

			if (bulletForward != null)
			{
				bulletForward.fillAmount = 0f;
			}

			timer.text = null;
			pastTimerText = null;
		}


		public void Hold(bool isHold)
		{
			this.isHold = isHold;
			if (isHold)
			{
				if (animateImage != null)
				{
					animateImage.fillAmount = 0f;
				}

				forward.fillAmount = 1f;
				if (usingTimer)
				{
					timer.text = null;
					pastTimerText = null;
				}
			}
			else if (animateImage != forward)
			{
				forward.fillAmount = 0f;
			}
		}


		public bool RemainCooldown()
		{
			return 0f < cooldown;
		}


		public float GetCooldown()
		{
			return cooldown;
		}


		public bool IsUsable()
		{
			return true;
		}


		public void FillAmountTypeChange(FillAmountType fillAmountType)
		{
			if (curFillAmountType == FillAmountType.NONE)
			{
				return;
			}

			if (fillAmountType == FillAmountType.NONE)
			{
				return;
			}

			if (curFillAmountType == fillAmountType)
			{
				return;
			}

			curFillAmountType = fillAmountType;
			ResetUI();
			AnimationCooldown(fillAmountType);
		}


		public void SetCooldown(float cooldown, float maxCooldown, FillAmountType fillAmountType, bool usingTimer,
			Action finishAction = null)
		{
			if (cooldown <= 0f && maxCooldown <= 0f)
			{
				return;
			}

			if (this.cooldown > 0f && cooldown <= 0f)
			{
				Finish();
			}

			this.finishAction = finishAction;
			this.cooldown = Mathf.Max(0f, cooldown);
			this.maxCooldown = Mathf.Max(0.1f, maxCooldown);
			bool flag = curFillAmountType != fillAmountType;
			curFillAmountType = fillAmountType;
			isHold = false;
			this.usingTimer = usingTimer;
			if (!usingTimer)
			{
				timer.text = null;
				pastTimerText = null;
			}

			if (flag)
			{
				ResetUI();
				AnimationCooldown(fillAmountType);
				return;
			}

			if (cooldown == 0f)
			{
				AnimationCooldown(fillAmountType);
			}
		}


		public void SetStackIntervalTime(float stackIntervalTime, float stackIntervalTimeMax)
		{
			this.stackIntervalTime = stackIntervalTime;
			this.stackIntervalTimeMax = stackIntervalTimeMax;
		}


		public void SetPauseTime(float pauseEndTime)
		{
			pauseCooldownRemainTime = pauseEndTime - MonoBehaviourInstance<ClientService>.inst.CurrentServerFrameTime;
		}


		public void AddCooldown(float addValue)
		{
			cooldown += addValue;
			if (cooldown <= 0f)
			{
				Finish();
			}
		}


		private void AnimationCooldown(FillAmountType fillAmountType)
		{
			switch (fillAmountType)
			{
				case FillAmountType.FORWARD:
					animateImage = forward;
					playInvoke = true;
					break;
				case FillAmountType.OVERLAP:
					animateImage = overlap;
					playInvoke = false;
					break;
				case FillAmountType.RADIAL:
					animateImage = radial;
					playInvoke = false;
					break;
				case FillAmountType.HORIZONTAL:
					animateImage = horizontal;
					playInvoke = true;
					break;
				case FillAmountType.STACK_FORWARD:
					animateImage = stackForward;
					playInvoke = true;
					break;
				case FillAmountType.BULLET_FORWARD:
					animateImage = bulletForward;
					playInvoke = true;
					break;
			}

			UpdateUI();
		}


		public void Finish()
		{
			Init();
			if (playInvoke)
			{
				Action onSkill = OnSkill;
				if (onSkill != null)
				{
					onSkill();
				}
			}

			Action action = finishAction;
			if (action == null)
			{
				return;
			}

			action();
		}


		private void UpdateUI()
		{
			if (stackIntervalTime > 0f)
			{
				timer.text = null;
				pastTimerText = null;
				animateImage.fillAmount = 0f;
				forward.fillAmount = stackIntervalTime / stackIntervalTimeMax;
				return;
			}

			if (pauseCooldownRemainTime > 0f)
			{
				return;
			}

			if (isHold)
			{
				forward.fillAmount = 1f;
				return;
			}

			if (animateImage != null)
			{
				animateImage.fillAmount = cooldown / maxCooldown;
			}

			if (usingTimer)
			{
				int num = Mathf.CeilToInt(cooldown);
				int? num2 = pastTimerText;
				int num3 = num;
				if (!((num2.GetValueOrDefault() == num3) & (num2 != null)))
				{
					pastTimerText = num;
					timer.text = (float) num > 0f ? num.ToString() : null;
				}
			}
		}
	}
}