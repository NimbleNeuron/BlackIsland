using System;
using System.Collections;
using System.Collections.Generic;
using BIFog;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class CharacterFloatingUI : MonoBehaviour, ISightEventHandler, IHoverble
	{
		private const float BURST_DAMAGE_THRESHOLD = 0.3f;


		private const float DEFUALT_DAMAGE_FONT_HALFSIZE = 15.5f;


		private const float EMOTION_LIFE_TIME = 5f;


		private readonly Queue<string> floatingTextQueue = new Queue<string>();


		private readonly Color LEVEL_UP_COLOR = new Color32(byte.MaxValue, 221, 104, byte.MaxValue);


		private readonly Color NON_PLAYER_SP_COLOR = new Color(255f, 255f, 255f);


		private readonly Color PLAYER_SP_COLOR = new Color(0f, 160f, 215f);


		private readonly Color SHIELD_COLOR = new Color32(248, 215, 67, byte.MaxValue);


		private Camera _camera;


		private bool autoVisible = true;


		private Color? epGaugeColor;


		private bool hidePlayerName;


		private bool isMineCharacter;


		private bool isShowing;


		private LocalCharacter localCharacter;


		private bool showDyingCondition;


		private bool showObjectText;


		private UIStatusBar statusBar;


		private UISurvivableTimer survivableTimer;


		private ExtraPointDisplayData targetEpDisplayData;


		private Coroutine textEffectScheduler;


		private UIEmotionIcon uiEmotionIcon;


		private UIName uiName;


		public UIStatusBar StatusBar => statusBar;


		private Camera Camera {
			get
			{
				if (_camera == null)
				{
					_camera = Camera.main;
				}

				return _camera;
			}
		}


		private void OnEnable()
		{
			textEffectScheduler = this.StartThrowingCoroutine(FloatingTextScheduler(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][FloatingTextScheduler] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			if (MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.OnKeyPressed += OnKeyPress;
				MonoBehaviourInstance<GameInput>.inst.OnKeyRelease += OnKeyRelease;
			}
		}


		private void OnDisable()
		{
			if (textEffectScheduler != null)
			{
				StopCoroutine(textEffectScheduler);
			}

			Release();
			if (MonoBehaviourInstance<GameInput>.inst != null)
			{
				MonoBehaviourInstance<GameInput>.inst.OnKeyPressed -= OnKeyPress;
				MonoBehaviourInstance<GameInput>.inst.OnKeyRelease -= OnKeyRelease;
			}
		}


		public void HoverOn()
		{
			if (uiName && !showObjectText && localCharacter != null && !localCharacter.IsAlive)
			{
				uiName.ShowBg(true);
			}
		}


		public void HoverOff()
		{
			if (uiName && !showObjectText)
			{
				uiName.ShowBg(false);
			}
		}


		public void OnSight()
		{
			if (!autoVisible)
			{
				return;
			}

			Show();
		}


		public void OnHide()
		{
			Hide();
		}


		// co: set lambda
		Transform ISightEventHandler.transform => base.transform;


		public void Init(LocalCharacter localCharacter)
		{
			this.localCharacter = localCharacter;
			if (GetComponent<FogHiderOnCenter>().IsInSight)
			{
				OnSight();
			}

			if (localCharacter != null)
			{
				isMineCharacter = localCharacter.ObjectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId;
				return;
			}

			isMineCharacter = false;
		}


		public void SetDyingCondition(bool dyingCondition)
		{
			showDyingCondition = dyingCondition;
			UIStatusBar uistatusBar = statusBar;
			if (uistatusBar != null)
			{
				uistatusBar.SetDyingCondition(dyingCondition);
			}

			UIStatusBar uistatusBar2 = statusBar;
			if (uistatusBar2 == null)
			{
				return;
			}

			uistatusBar2.SetForwardColor(GetHpForwardColor());
		}


		public void SetHp(int hp, int maxHp)
		{
			if (showDyingCondition)
			{
				maxHp = 500;
			}

			UIStatusBar uistatusBar = statusBar;
			if (uistatusBar == null)
			{
				return;
			}

			uistatusBar.SetHp(hp, maxHp);
		}


		public void SetSp(int sp, int maxSp)
		{
			UIStatusBar uistatusBar = statusBar;
			if (uistatusBar == null)
			{
				return;
			}

			uistatusBar.SetSp(sp, maxSp);
		}


		public void SetShield(int shield)
		{
			UIStatusBar uistatusBar = statusBar;
			if (uistatusBar == null)
			{
				return;
			}

			uistatusBar.SetShield(shield);
		}


		public void ShowEmotionIcon(EmotionIconData emotionIconData)
		{
			if (uiEmotionIcon != null)
			{
				uiEmotionIcon.DestroyAfterSecond(0f);
			}

			GameObject gameObject = MonoBehaviourInstance<GameUI>.inst.UITracker.CreateEmotionPrefab(
				emotionIconData.prefab, isMineCharacter ? UITracker.Order.MineParent : UITracker.Order.OtherParent);
			uiEmotionIcon = gameObject.GetComponent<UIEmotionIcon>();
			if (uiEmotionIcon != null)
			{
				uiEmotionIcon.SetTrackingTarget(base.transform);
				Vector3 a = Vector3.up * GameDB.character.GetCharacterData(localCharacter.CharacterCode).uiHeight;
				uiEmotionIcon.SetTrackingOffset(a + Vector3.up * 0.7f);
				uiEmotionIcon.DestroyAfterSecond(5f);
				if (isShowing)
				{
					uiEmotionIcon.PlaySound(base.transform, emotionIconData.sound);
					return;
				}

				uiEmotionIcon.Hide();
			}
		}


		public void ShowHpHeal(int heal, float offset = 2f)
		{
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingPosition(base.transform.position);
			uiDamage.SetTrackingOffset(Vector3.up * offset);
			float num = heal / (float) localCharacter.Stat.MaxHp;
			UIDamage.AnimationType aniType = UIDamage.AnimationType.HpRecovery;
			uiDamage.SetHpHeal(aniType, heal,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void ShowSpHeal(int heal, float offset = 2.2f)
		{
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingPosition(base.transform.position);
			uiDamage.SetTrackingOffset(Vector3.up * offset);
			float num = heal / (float) localCharacter.Stat.MaxSp;
			UIDamage.AnimationType aniType = UIDamage.AnimationType.SpRecovery;
			uiDamage.SetSpHeal(aniType, heal,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void ShowDamage(bool isSkillDamage, int damage, bool isCritical, Vector3 direction, float offset = 2f)
		{
			UIDamage.Direction direction2 = CalcUIDamageDirection(direction);
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingPosition(base.transform.position);
			uiDamage.SetTrackingOffset(Vector3.up * offset);
			float num = damage / (float) localCharacter.Stat.MaxHp;
			UIDamage.AnimationType aniType = UIDamage.AnimationType.NormalDamaged;
			if (isCritical)
			{
				aniType = UIDamage.AnimationType.CriticalDamaged;
			}
			else if (num >= 0.3f)
			{
				aniType = UIDamage.AnimationType.BurstDamaged;
			}

			uiDamage.SetDamage(isSkillDamage, aniType, direction2, damage,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void ShowSpDamage(int damage, Vector3 direction)
		{
			UIDamage.Direction direction2 = CalcUIDamageDirection(direction);
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingPosition(base.transform.position);
			uiDamage.SetTrackingOffset(Vector3.up * 1.5f);
			float num = damage / (float) localCharacter.Stat.MaxSp;
			UIDamage.AnimationType aniType = UIDamage.AnimationType.NormalDamaged;
			if (num >= 0.3f)
			{
				aniType = UIDamage.AnimationType.BurstDamaged;
			}

			uiDamage.SetSpDamage(aniType, direction2, damage,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		private UIDamage.Direction CalcUIDamageDirection(Vector3 direction)
		{
			UIDamage.Direction result = UIDamage.Direction.Right;
			if (direction.sqrMagnitude > Mathf.Epsilon)
			{
				Camera camera = Camera;
				if ((camera.WorldToViewportPoint(localCharacter.GetPosition() - direction) -
				     camera.WorldToViewportPoint(localCharacter.GetPosition())).x >= 0f)
				{
					result = UIDamage.Direction.Right;
				}
				else
				{
					result = UIDamage.Direction.Left;
				}
			}

			return result;
		}


		public void ShowBlock(int damage)
		{
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingTarget(base.transform);
			uiDamage.SetTrackingOffset(Vector3.up * 1.5f);
			uiDamage.SetLabel(damage.ToString(), 25, Color.gray, Color.black,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void ShowEvasion()
		{
			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingTarget(base.transform);
			uiDamage.SetTrackingOffset(Vector3.up * 1.5f);
			uiDamage.SetLabel(UIDamage.AnimationType.SpRecovery, UIDamage.Direction.Right, Ln.Get("Evasion"), 25,
				Color.yellow, Color.black,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void UpdateLevel(int level)
		{
			if (statusBar != null)
			{
				statusBar.SetLevel(localCharacter.Status.Level);
			}
		}


		public void GainExp(int exp)
		{
			if (!IsEnable())
			{
				return;
			}

			UIDamage uiDamage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(UITracker.Order.None);
			uiDamage.SetTrackingTarget(base.transform);
			uiDamage.SetTrackingOffset(new Vector3(-0.35f, 1f, -0.21f));
			uiDamage.SetLabel(exp.ToString(), 30, Color.yellow, Color.black,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		public void OnMasteryLevelUp(MasteryType masteryType)
		{
			floatingTextQueue.Enqueue(LnUtil.GetMasteryName(masteryType) + " " + Ln.Get("레벨업!"));
		}


		private IEnumerator FloatingTextScheduler()
		{
			for (;;)
			{
				if (floatingTextQueue.Count > 0)
				{
					SetFloatingText(floatingTextQueue.Dequeue(), LEVEL_UP_COLOR, Color.black);
				}

				yield return new WaitForSeconds(1f);
			}
		}


		public void SetFloatingText(string text, Color color, Color outlineColor)
		{
			if (!IsEnable())
			{
				return;
			}

			UIDamage uiDamage =
				MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIDamage>(isMineCharacter
					? UITracker.Order.MineParent
					: UITracker.Order.OtherParent);
			uiDamage.SetTrackingTarget(base.transform);
			uiDamage.SetTrackingOffset(new Vector3(0f, -1.5f, 0f));
			uiDamage.SetLabel(text, 20, color, outlineColor,
				delegate { MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIDamage>(uiDamage); });
		}


		private Vector3 GetUIOffset()
		{
			if (localCharacter != null)
			{
				ObjectType objectType = localCharacter.ObjectType;
				if (objectType == ObjectType.PlayerCharacter)
				{
					return Vector3.up * GameDB.character.GetCharacterData(localCharacter.CharacterCode).uiHeight;
				}

				if (objectType == ObjectType.Monster)
				{
					return Vector3.up * GameDB.monster.GetMonsterData(localCharacter.CharacterCode).uiHeight;
				}

				if (objectType - ObjectType.SummonCamera <= 2)
				{
					return Vector3.up * GameDB.character.GetSummonData(localCharacter.CharacterCode).uiHeight;
				}
			}

			return Vector3.up * 1.5f;
		}


		private Color GetHpForwardColor()
		{
			if (localCharacter is LocalPlayerCharacter && ((LocalPlayerCharacter) localCharacter).IsDyingCondition)
			{
				return GameConstants.TeamMode.DYINGCONDITION_COLOR;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			    MonoBehaviourInstance<ClientService>.inst.MyObjectId == localCharacter.ObjectId)
			{
				return GameConstants.TeamMode.MY_COLOR;
			}

			if (localCharacter.IsTypeOf<LocalSummonBase>() && MonoBehaviourInstance<ClientService>.inst.MyObjectId ==
				((LocalSummonBase) localCharacter).OwnerId)
			{
				return GameConstants.TeamMode.MY_COLOR;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(localCharacter))
			{
				return GameConstants.TeamMode.ENEMY_COLOR;
			}

			return GameConstants.TeamMode.ALLY_COLOR;
		}


		public void Show()
		{
			if (localCharacter != null)
			{
				if (uiName == null)
				{
					uiName = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIName>(isMineCharacter
						? UITracker.Order.MineParent
						: UITracker.Order.OtherParent);
				}

				Vector3 vector = GetUIOffset();
				ObjectType objectType = localCharacter.ObjectType;
				if (objectType != ObjectType.PlayerCharacter)
				{
					if (objectType != ObjectType.Monster)
					{
						if (objectType - ObjectType.SummonCamera > 2)
						{
							vector = Vector3.up * 1.5f;
						}
						else
						{
							vector = Vector3.up * GameDB.character.GetSummonData(localCharacter.CharacterCode).uiHeight;
						}
					}
					else
					{
						vector = Vector3.up * GameDB.monster.GetMonsterData(localCharacter.CharacterCode).uiHeight;
					}
				}
				else
				{
					vector = Vector3.up * GameDB.character.GetCharacterData(localCharacter.CharacterCode).uiHeight;
				}

				if (localCharacter.IsAlive && statusBar == null)
				{
					statusBar = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIStatusBar>(isMineCharacter
						? UITracker.Order.MineParent
						: UITracker.Order.OtherParent);
				}

				if (uiName != null)
				{
					uiName.SetName(localCharacter.Nickname);
					uiName.SetTrackingTarget(base.transform);
					if (statusBar != null)
					{
						uiName.SetStickToTrackUi(statusBar, Vector3.up * 18f);
					}
					else
					{
						uiName.SetStickToTrackUi(null, Vector3.zero);
					}

					uiName.SetTrackingOffset(localCharacter.IsAlive ? vector + Vector3.up * 0.25f : Vector3.up * 1.5f);
				}

				if (statusBar != null)
				{
					Color hpForwardColor = GetHpForwardColor();
					statusBar.SetForwardColor(hpForwardColor);
					statusBar.SetDyingCondition(hpForwardColor == GameConstants.TeamMode.DYINGCONDITION_COLOR);
					statusBar.SetShieldColor(SHIELD_COLOR);
					statusBar.SetTrackingTarget(base.transform);
					statusBar.SetTrackingOffset(vector);
					statusBar.SetHp(localCharacter.Status.Hp, localCharacter.Stat.MaxHp);
					statusBar.SetSp(localCharacter.Status.Hp, localCharacter.Stat.MaxHp);
					statusBar.SetShield(localCharacter.Status.Shield);
					statusBar.SetLevel(localCharacter.Status.Level);
					statusBar.SetSpColor(localCharacter.IsTypeOf<LocalPlayerCharacter>()
						? PLAYER_SP_COLOR
						: NON_PLAYER_SP_COLOR);
					statusBar.SetTeamLine(localCharacter.TeamNumber);
					targetEpDisplayData = ExtraPointDisplayManager.instance.GetData(localCharacter.CharacterCode);
					if (targetEpDisplayData == null)
					{
						statusBar.SetMaxExtraPoint(0, null);
					}
					else
					{
						statusBar.SetMaxExtraPoint(localCharacter.Stat.MaxExtraPoint, targetEpDisplayData);
						UpdateExtraPoint(localCharacter.Status.ExtraPoint);
					}

					LocalPlayerCharacter localPlayerCharacter = localCharacter as LocalPlayerCharacter;
					if (localPlayerCharacter != null)
					{
						Item weapon = localPlayerCharacter.Equipment.GetWeapon();
						ItemWeaponData itemWeaponData = weapon != null ? weapon.GetItemData<ItemWeaponData>() : null;
						if (itemWeaponData != null)
						{
							if (itemWeaponData.IsGunType())
							{
								int bulletCapacity = GameDB.item.GetBulletCapacity(itemWeaponData.code);
								SetupBullet(localPlayerCharacter.Status.Bullet, bulletCapacity);
							}
							else if (itemWeaponData.IsThrowType())
							{
								UpdateThrowAmmo(itemWeaponData.weaponType,
									localPlayerCharacter.Equipment.GetWeapon().Bullet);
							}
						}
						else
						{
							HideThrowAmmo();
							HideBullet();
						}
					}

					statusBar.UpdateUI();
				}

				if (MonoBehaviourInstance<GameInput>.inst != null)
				{
					if (MonoBehaviourInstance<GameInput>.inst.IsKeyHoldEvent(GameInputEvent.ShowObjectText))
					{
						OnKeyPress(GameInputEvent.ShowObjectText, Vector3.zero);
					}
					else
					{
						OnKeyRelease(GameInputEvent.ShowObjectText, Vector3.zero);
					}
				}
			}

			if (uiEmotionIcon != null)
			{
				uiEmotionIcon.Show();
			}

			isShowing = true;
			FlagPlayerName();
		}


		public void Hide()
		{
			Release();
			HideSurvivableTimer();
			if (uiEmotionIcon != null)
			{
				uiEmotionIcon.Hide();
			}

			isShowing = false;
		}


		public void SetDead()
		{
			if (uiName != null)
			{
				uiName.SetTrackingOffset(Vector3.up * 1.5f);
				uiName.SetName(localCharacter.Nickname);
				uiName.SetStickToTrackUi(null, Vector3.zero);
			}

			if (statusBar != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIStatusBar>(statusBar);
				statusBar = null;
			}
		}


		private void Release()
		{
			if (uiName != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIName>(uiName);
				uiName = null;
			}

			if (statusBar != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIStatusBar>(statusBar);
				statusBar = null;
			}
		}


		private bool IsEnable()
		{
			return uiName != null && statusBar != null;
		}


		public void SetAutoVisible(bool autoVisible)
		{
			this.autoVisible = autoVisible;
		}


		public void ShowSurvivableTimer()
		{
			if (isShowing)
			{
				localCharacter.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
				{
					if (survivableTimer != null)
					{
						return;
					}

					survivableTimer =
						MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UISurvivableTimer>(isMineCharacter
							? UITracker.Order.MineParent
							: UITracker.Order.OtherParent);
					if (survivableTimer != null)
					{
						survivableTimer.UpdateTimer(player.SurvivableTime);
						survivableTimer.SetTrackingTarget(base.transform);
						survivableTimer.SetTrackingOffset(GetUIOffset() + Vector3.up * 1.5f);
					}
				});
			}
		}


		public void HideSurvivableTimer()
		{
			if (survivableTimer != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UISurvivableTimer>(survivableTimer);
				survivableTimer = null;
			}
		}


		public void UpdateSurvivalTimer(float remainTime)
		{
			if (survivableTimer != null)
			{
				survivableTimer.UpdateTimer(remainTime);
			}
		}


		public void SetupBullet(int remainBullet, int capacity)
		{
			if (statusBar != null)
			{
				statusBar.SetupBullets(remainBullet, capacity);
			}
		}


		public void UpdateBullet(int remainBullet)
		{
			if (statusBar != null)
			{
				statusBar.UpdateBullet(remainBullet);
			}
		}


		public void HideBullet()
		{
			if (statusBar != null)
			{
				statusBar.HideBullet();
			}
		}


		public void UpdateExtraPoint(int setExtraPoint)
		{
			if (statusBar != null && targetEpDisplayData != null)
			{
				if (targetEpDisplayData.ExtraPointDisplayType == ExtraPointDisplayType.Stack)
				{
					statusBar.SetExtraPoint(setExtraPoint);
					return;
				}

				if (targetEpDisplayData.ExtraPointDisplayType == ExtraPointDisplayType.Gauge)
				{
					if (epGaugeColor != null)
					{
						statusBar.SetExtraPoint(setExtraPoint, epGaugeColor.Value);
						return;
					}

					Color gaugeColor = Color.clear;
					float num = setExtraPoint / (float) localCharacter.Stat.MaxExtraPoint;
					int num2 = 0;
					while (num2 < targetEpDisplayData.Colors.Count && targetEpDisplayData.ColorStarts[num2] <= num)
					{
						gaugeColor = targetEpDisplayData.Colors[num2];
						num2++;
					}

					statusBar.SetExtraPoint(setExtraPoint, gaugeColor);
				}
			}
		}


		public void SettingForceEpGaugeColor(Color? clr)
		{
			epGaugeColor = clr;
			UpdateExtraPoint(localCharacter.Status.ExtraPoint);
		}


		public void UpdateThrowAmmo(WeaponType weaponType, int ammo)
		{
			if (statusBar != null)
			{
				statusBar.UpdateThrowAmmo(weaponType, ammo);
			}
		}


		public void HideThrowAmmo()
		{
			if (statusBar != null)
			{
				statusBar.HideThrowAmmo();
			}
		}


		public void SetStateName(string stateName)
		{
			if (uiName == null || localCharacter == null)
			{
				return;
			}

			uiName.SetCCName(stateName);
		}


		public void ResetStateName()
		{
			if (uiName == null || localCharacter == null)
			{
				return;
			}

			uiName.ResetCCName(localCharacter.Nickname);
		}


		public void ResetName(string text)
		{
			if (uiName == null)
			{
				return;
			}

			uiName.ResetCCName(text);
		}


		private void OnKeyPress(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.ShowObjectText)
			{
				OnShowObjectText();
				return;
			}

			if (inputEvent != GameInputEvent.ChangeEnableTrackerName)
			{
				return;
			}

			hidePlayerName = !hidePlayerName;
			FlagPlayerName();
		}


		private void OnKeyRelease(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.ShowObjectText)
			{
				UIName uiname = uiName;
				if (uiname != null)
				{
					uiname.EnableClick(false);
				}

				showObjectText = false;
			}
		}


		private void OnShowObjectText()
		{
			if (!isMineCharacter && uiName != null)
			{
				uiName.SetLeftClickCallback(delegate
				{
					LocalObject component = base.transform.GetComponent<LocalObject>();
					if (component != null)
					{
						SingletonMonoBehaviour<PlayerController>.inst.OnSelectEvent(component, component.GetPosition(),
							component.GetPosition(), true);
					}
				});
				uiName.SetRightClickCallback(delegate
				{
					LocalObject component = base.transform.GetComponent<LocalObject>();
					if (component != null)
					{
						SingletonMonoBehaviour<PlayerController>.inst.OnMoveEvent(false, component,
							component.GetPosition(), true);
					}
				});
				uiName.EnableClick(true);
				showObjectText = true;
			}
		}


		private void FlagPlayerName()
		{
			if (localCharacter != null && localCharacter.ObjectType == ObjectType.PlayerCharacter && uiName != null)
			{
				uiName.transform.localScale = hidePlayerName ? Vector3.zero : Vector3.one;
			}
		}
	}
}