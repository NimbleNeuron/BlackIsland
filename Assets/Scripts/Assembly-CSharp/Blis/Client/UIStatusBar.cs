using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIStatusBar : BaseTrackUI
	{
		public float EFFECT_SPEED = 10f;


		public float HEAL_DELAY = 0.03f;


		public float DAMAGE_START_DELAY = 0.07f;


		public float DAMAGE_APPEND_DELAY = 0.03f;


		[SerializeField] private RawImage separatorImage = default;


		[SerializeField] private Image hpImage = default;


		[SerializeField] private Image spImage = default;


		[SerializeField] private GameObject extraPointGaugeType = default;


		[SerializeField] private GameObject extraPointStackType = default;


		[SerializeField] private Image extraPointBar = default;


		[SerializeField] private RectTransform extraPointSeparator = default;


		[SerializeField] private GameObject extraPointStackSample = default;


		[SerializeField] private float bulletUiPullDownWhenEpShow = -9f;


		[SerializeField] private Image shieldImage = default;


		[SerializeField] private Image teamLine = default;


		[SerializeField] private Text levelText = default;


		[SerializeField] private Image healEffect = default;


		[SerializeField] private Image damageEffect = default;


		[SerializeField] private CanvasGroup canvasGroup = default;


		[SerializeField] private CanvasAlphaTweener alphaTweener = default;


		[SerializeField] private RectTransform bulletParent = default;


		[SerializeField] private UIBullet bullet1 = default;


		[SerializeField] private UIBullet bullet2 = default;


		[SerializeField] private UIBullet bullet3 = default;


		[SerializeField] private UIBullet bullet4 = default;


		[SerializeField] private UIBullet skillStackCount = default;


		[SerializeField] private GameObject throwObj = default;


		[SerializeField] private Image throwWeaponType = default;


		[SerializeField] private Text throwAmmoText = default;


		private UIBullet activeBullet = default;


		private float bulletParentOriginalPosY = default;


		private int deltaDamage = default;


		private float deltaDamageDelay = default;


		private int deltaHeal = default;


		private float deltaHealDelay = default;


		private ExtraPointDisplayData displayData = default;


		private int ep = default;


		private List<Image> epStacks = default;


		private int hp = default;


		private int lastMaxValue = default;


		private int maxEp = default;


		private int maxHp = default;


		private int maxSp = default;


		private RectTransform pingAnchor = default;


		private Material separatorMat = default;


		private int shield = default;


		private int sp = default;


		public RectTransform PingAnchor => pingAnchor;


		protected override void Awake()
		{
			base.Awake();
			pingAnchor = GameUtil.Bind<RectTransform>(gameObject, "PingAnchor");
			separatorMat = new Material(Shader.Find("Hidden/HealthBar"));
			separatorImage.material = separatorMat;
			bulletParentOriginalPosY = bulletParent.anchoredPosition.y;
		}


		private void LateUpdate()
		{
			deltaHeal = (int) (deltaHeal * (1f - Time.deltaTime * EFFECT_SPEED));
			if (deltaDamageDelay <= 0f)
			{
				deltaDamage = (int) (deltaDamage * (1f - Time.deltaTime * EFFECT_SPEED));
			}
			else
			{
				deltaDamageDelay -= Time.deltaTime;
			}

			UpdateUI();
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
			Destroy(separatorMat);
		}


		public void SetHp(int hp, int maxHp)
		{
			if (this.hp > 0)
			{
				int num = hp - this.hp;
				if (num > maxHp * 0.1f)
				{
					deltaHeal += num;
					deltaHealDelay = HEAL_DELAY;
				}
				else if (num < 0)
				{
					deltaDamage += Mathf.Abs(num);
					deltaDamageDelay += deltaDamageDelay <= 0f
						? DAMAGE_START_DELAY - deltaDamageDelay
						: DAMAGE_APPEND_DELAY;
				}
			}

			this.hp = hp;
			this.maxHp = maxHp;
		}


		public void SetDyingCondition(bool dyingCondition)
		{
			if (dyingCondition)
			{
				alphaTweener.PlayAnimation();
				return;
			}

			alphaTweener.StopAnimation();
			canvasGroup.alpha = 1f;
		}


		public void SetMaxExtraPoint(int maxEp, ExtraPointDisplayData displayData)
		{
			if (maxEp == 0 || displayData == null)
			{
				maxEp = 0;
				displayData = null;
				extraPointGaugeType.SetActive(false);
				extraPointStackType.SetActive(false);
				return;
			}

			bool flag = this.displayData == null;
			this.maxEp = maxEp;
			this.displayData = displayData;
			if (displayData.ExtraPointDisplayType == ExtraPointDisplayType.Gauge)
			{
				extraPointGaugeType.SetActive(true);
				extraPointStackType.SetActive(false);
				if (flag)
				{
					if (displayData.Dots.Count == 0)
					{
						extraPointSeparator.gameObject.SetActive(false);
					}
					else
					{
						RectTransform component = extraPointBar.GetComponent<RectTransform>();
						float width = component.rect.width;
						Vector2 anchoredPosition = component.anchoredPosition;
						extraPointSeparator.anchoredPosition =
							anchoredPosition + new Vector2(width * (displayData.Dots[0] - 0.5f), 0f);
						for (int i = 1; i < displayData.Dots.Count; i++)
						{
							RectTransform component2 = Instantiate<GameObject>(extraPointSeparator.gameObject)
								.GetComponent<RectTransform>();
							component2.SetParent(component.parent);
							component2.localScale = Vector3.one;
							component2.anchoredPosition3D =
								anchoredPosition + new Vector2(width * (displayData.Dots[i] - 0.5f), 0f);
						}
					}
				}
			}
			else if (displayData.ExtraPointDisplayType == ExtraPointDisplayType.Stack)
			{
				extraPointGaugeType.SetActive(false);
				extraPointStackType.SetActive(true);
				if (epStacks == null)
				{
					epStacks = new List<Image>();
				}

				int count = epStacks.Count;
				RectTransform component3 = extraPointStackSample.GetComponent<RectTransform>();
				extraPointStackSample.SetActive(false);
				for (int j = 0; j < count; j++)
				{
					if (j >= maxEp)
					{
						epStacks[j].gameObject.SetActive(false);
					}
					else
					{
						epStacks[j].gameObject.SetActive(true);
						epStacks[j].GetComponent<RectTransform>().anchoredPosition = component3.anchoredPosition +
							new Vector2(displayData.StackIconInterval * j, 0f);
					}
				}

				for (int k = count; k < maxEp; k++)
				{
					RectTransform component4 =
						Instantiate<GameObject>(extraPointStackSample).GetComponent<RectTransform>();
					component4.SetParent(component3.parent);
					component4.anchoredPosition3D = component3.anchoredPosition +
					                                new Vector2(displayData.StackIconInterval * k, 0f);
					component4.localScale = Vector3.one;
					component4.gameObject.SetActive(true);
					Image component5 = component4.GetComponent<Image>();
					component5.sprite = displayData.StackIconEmpty;
					component5.SetNativeSize();
					epStacks.Add(component5);
				}
			}

			if (flag)
			{
				Vector2 anchoredPosition2 = bulletParent.anchoredPosition;
				anchoredPosition2.y = bulletParentOriginalPosY + bulletUiPullDownWhenEpShow;
				bulletParent.anchoredPosition = anchoredPosition2;
			}
		}


		public void SetExtraPoint(int ep)
		{
			SetExtraPoint(ep, Color.clear);
		}


		public void SetExtraPoint(int ep, Color gaugeColor)
		{
			if (displayData == null)
			{
				return;
			}

			if (ep == this.ep && gaugeColor == Color.clear)
			{
				return;
			}

			this.ep = ep;
			if (displayData.ExtraPointDisplayType == ExtraPointDisplayType.Gauge)
			{
				extraPointBar.fillAmount = ep / (float) maxEp;
				extraPointBar.color = gaugeColor;
				return;
			}

			if (displayData.ExtraPointDisplayType == ExtraPointDisplayType.Stack && epStacks != null)
			{
				for (int i = 0; i < epStacks.Count; i++)
				{
					if (epStacks[i].gameObject.activeSelf)
					{
						epStacks[i].sprite = i < ep ? displayData.StackIconFill : displayData.StackIconEmpty;
					}
				}
			}
		}


		public void SetShield(int shield)
		{
			this.shield = shield;
		}


		public void SetSp(int sp, int maxSp)
		{
			this.sp = sp;
			this.maxSp = maxSp;
		}


		public void SetLevel(int characterLevel)
		{
			levelText.text = characterLevel.ToString();
		}


		public void SetTeamLine(int teamNumber)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer || teamNumber == 0 ||
			    !MonoBehaviourInstance<ClientService>.inst.IsTeamMode)
			{
				teamLine.gameObject.SetActive(false);
				return;
			}

			List<int> list = MonoBehaviourInstance<ClientService>.inst.GetTeams().Keys.ToList<int>();
			teamLine.gameObject.SetActive(true);
			teamLine.color = UIUtility.ObserverTeamColor(list.IndexOf(teamNumber));
		}


		public void SetForwardColor(Color color)
		{
			hpImage.color = color;
		}


		public void SetShieldColor(Color shieldColor)
		{
			shieldImage.color = shieldColor;
		}


		public void SetSpColor(Color spColor)
		{
			spImage.color = spColor;
		}


		public new void UpdateUI()
		{
			UpdateSeparator();
			int num = Mathf.Max(maxHp, hp + shield);
			num = Mathf.Max(num, 1);
			maxSp = Mathf.Max(maxSp, 1);
			hpImage.fillAmount = (hp - Mathf.Max(0, deltaHeal)) / (float) num;
			shieldImage.fillAmount = (hp + shield) / (float) num;
			spImage.fillAmount = sp / (float) maxSp;
			healEffect.fillAmount = hp / (float) num;
			damageEffect.fillAmount = (hp + deltaDamage) / (float) num;
		}


		private void UpdateSeparator()
		{
			int num = Mathf.Max(maxHp, hp + shield);
			if (lastMaxValue == num)
			{
				return;
			}

			lastMaxValue = num;
			Rect pixelAdjustedRect = separatorImage.GetPixelAdjustedRect();
			Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
			float num2 = pixelAdjustedRect.width * rootCanvas.scaleFactor;
			int value = 0;
			int num3;
			int num4;
			int value2;
			if (num <= 10)
			{
				num3 = num;
				num4 = Mathf.CeilToInt(num2 / num3);
				value2 = num4 * 10;
			}
			else if (num <= 100)
			{
				num3 = 0;
				num4 = 0;
				value2 = 0;
			}
			else
			{
				num /= 100;
				if (num < 30)
				{
					num3 = num;
					num4 = Mathf.RoundToInt(num2 / num3);
					value2 = num4 * 10;
				}
				else
				{
					num3 = Mathf.FloorToInt((10 - num / 10) * (num / 10f));
					num4 = Mathf.RoundToInt(num2 / num3);
					value2 = num4 * (10 - num / 10);
				}
			}

			int value3 = Mathf.RoundToInt((num2 - num3 * num4) * 0.5f);
			separatorMat.SetInt("_Thickness", value);
			separatorMat.SetInt("_SmallStep", num4);
			separatorMat.SetInt("_BigStep", value2);
			separatorMat.SetFloat("_Unit", 1f / num2);
			separatorMat.SetInt("_Margin", value3);
		}


		public override void ResetUI()
		{
			base.ResetUI();
			ResetStats();
			HideBullet();
			HideThrowAmmo();
			Vector2 anchoredPosition = bulletParent.anchoredPosition;
			anchoredPosition.y = bulletParentOriginalPosY;
			bulletParent.anchoredPosition = anchoredPosition;
		}


		private void ResetStats()
		{
			hp = 0;
			maxHp = 0;
			sp = 0;
			maxSp = 0;
			shield = 0;
			deltaHeal = 0;
			deltaHealDelay = 0f;
			deltaDamage = 0;
			deltaDamageDelay = 0f;
			displayData = null;
		}


		public void HideBullet()
		{
			activeBullet = null;
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
		}


		public void SetupBullets(int remainBullet, int capacity)
		{
			HideBullet();
			if (capacity == 1)
			{
				activeBullet = bullet1;
			}
			else if (1 < capacity && capacity <= 6)
			{
				activeBullet = bullet2;
			}
			else if (6 < capacity && capacity <= 10)
			{
				activeBullet = bullet3;
			}
			else
			{
				activeBullet = bullet4;
			}

			if (activeBullet != null)
			{
				activeBullet.gameObject.SetActive(true);
				activeBullet.Setup(remainBullet, capacity);
			}
		}


		public void UpdateBullet(int remainBullet)
		{
			if (activeBullet != null)
			{
				activeBullet.UpdateBullet(remainBullet);
			}
		}


		public void UpdateThrowAmmo(WeaponType weaponType, int ammo)
		{
			if (!throwObj.activeSelf)
			{
				throwObj.SetActive(true);
			}

			throwWeaponType.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetThrowAmmoWeaponType(weaponType);
			throwAmmoText.text = ammo.ToString();
		}


		public void HideThrowAmmo()
		{
			throwObj.SetActive(false);
		}

		private void Ref()
		{
			Reference.Use(skillStackCount);
		}
	}
}