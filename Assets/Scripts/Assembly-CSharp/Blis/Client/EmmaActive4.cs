using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaActive4)]
	public class EmmaActive4 : LocalEmmaSkillScript
	{
		private List<LocalSummonTrap> listFireworkHat;


		private List<LocalSummonServant> listPigeon;


		private CollisionCircle3D skillCollision;


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill04);
			SetAnimation(Self, BooleanMotionWait, true);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1 && targetPosition != null)
			{
				LocalCharacter.PlayLocalEffectWorldPoint(
					Singleton<EmmaSkillActive2Data>.inst.FireworkHatDropEffectAndSoundCode, targetPosition.Value);
			}

			if (action == 4 && targetPosition != null)
			{
				Vector3 direction = GameUtil.Direction(targetPosition.Value, Self.GetPosition());
				LocalCharacter.PlayLocalEffectWorldPoint(
					Singleton<EmmaSkillActive4Data>.inst.WarpPrevEffectAndSoundCode, targetPosition.Value,
					GameUtil.LookRotation(direction));
			}

			if (action == 5)
			{
				if (targetPosition != null)
				{
					Vector3 direction2 = GameUtil.Direction(Self.GetPosition(), targetPosition.Value);
					LocalCharacter.PlayLocalEffectWorldPoint(
						Singleton<EmmaSkillActive4Data>.inst.WarpAfterEffectAndSoundCode, Self.GetPosition(),
						GameUtil.LookRotation(direction2));
				}

				PlaySoundPoint(Self, "Emma_Skill04_Arrive", 15);
			}

			if (action == 6)
			{
				LocalSummonBase localSummonBase = target as LocalSummonBase;
				if (IsPigeon(localSummonBase) && IsMySummon(localSummonBase))
				{
					LocalProjectile ownProjectile = localSummonBase.GetOwnProjectile(IsPigeonLineProjectile);
					if (ownProjectile != null)
					{
						LineRenderer componentInChildren = ownProjectile.GetComponentInChildren<LineRenderer>();
						if (componentInChildren != null)
						{
							componentInChildren.SetPosition(0, target.GetPosition());
							componentInChildren.SetPosition(1, Self.GetPosition());
						}
					}

					PlayEffectChild(target, "FX_BI_Emma_Skill04_Pigeon_LineGlow", "Root");
				}
			}
		}


		public override void Finish(bool cancel) { }


		public override UseSkillErrorCode IsCanUseSkill(LocalObject hitTarget, Vector3? cursorPosition)
		{
			if (hitTarget == null)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			LocalMovableCharacter localMovableCharacter = hitTarget as LocalMovableCharacter;
			if (localMovableCharacter != null &&
			    localMovableCharacter.GetCharacterStateValueByGroup(
				    Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode, LocalPlayerCharacter.ObjectId) !=
			    null)
			{
				return UseSkillErrorCode.None;
			}

			if (hitTarget.ObjectType != ObjectType.SummonServant && hitTarget.ObjectType != ObjectType.SummonTrap)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			LocalSummonBase localSummonBase = hitTarget as LocalSummonBase;
			if (localSummonBase == null)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (!IsMySummon(localSummonBase))
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (!IsPigeon(localSummonBase) && !IsFireworkHat(localSummonBase))
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			return UseSkillErrorCode.None;
		}


		public override UseSkillErrorCode IsEnableSkillSlot()
		{
			int num = GetPigeons(LocalPlayerCharacter) != null ? GetPigeons(LocalPlayerCharacter).Count : 0;
			if (0 < num)
			{
				return UseSkillErrorCode.None;
			}

			int num2 = GetFireworkHats(LocalPlayerCharacter) != null ? GetFireworkHats(LocalPlayerCharacter).Count : 0;
			if (0 < num2)
			{
				return UseSkillErrorCode.None;
			}

			if (skillCollision == null)
			{
				skillCollision = new CollisionCircle3D(Self.GetPosition(), SkillRange);
			}
			else
			{
				skillCollision.UpdatePosition(Self.GetPosition());
			}

			foreach (LocalCharacter localCharacter in GetCharacterWithinRange(skillCollision, false, true))
			{
				LocalMovableCharacter localMovableCharacter = localCharacter as LocalMovableCharacter;
				if (localMovableCharacter != null &&
				    localMovableCharacter.GetCharacterStateValueByGroup(
					    Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode, Self.ObjectId) != null)
				{
					return UseSkillErrorCode.None;
				}
			}

			return UseSkillErrorCode.NotAvailableNow;
		}


		public override bool PickingOrderCompare(LocalObject prevTarget, LocalObject nextTarget)
		{
			EmmaPickingOrder emmaPickingOrder = GetEmmaPickingOrder(prevTarget);
			EmmaPickingOrder emmaPickingOrder2 = GetEmmaPickingOrder(nextTarget);
			return emmaPickingOrder == emmaPickingOrder2 || emmaPickingOrder < emmaPickingOrder2;
		}


		private EmmaPickingOrder GetEmmaPickingOrder(LocalObject target)
		{
			LocalMovableCharacter localMovableCharacter = target as LocalMovableCharacter;
			if (localMovableCharacter != null &&
			    localMovableCharacter.GetCharacterStateValueByGroup(
				    Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateGroupCode, LocalPlayerCharacter.ObjectId) !=
			    null)
			{
				return EmmaPickingOrder.Rabbit;
			}

			LocalSummonBase localSummonBase = target as LocalSummonBase;
			if (localSummonBase == null)
			{
				return EmmaPickingOrder.None;
			}

			if (!IsMySummon(localSummonBase))
			{
				return EmmaPickingOrder.None;
			}

			if (target.ObjectType == ObjectType.SummonServant)
			{
				return EmmaPickingOrder.Pigeon;
			}

			if (target.ObjectType == ObjectType.SummonTrap)
			{
				return EmmaPickingOrder.FireworkHat;
			}

			return EmmaPickingOrder.None;
		}


		private bool IsPigeonLineProjectile(LocalProjectile projectile)
		{
			return projectile.ProjectileData.code == Singleton<EmmaSkillActive4Data>.inst.PigeonLineProjectileCode;
		}


		public override void OnDisplaySkillIndicator(Splat indicator)
		{
			base.OnDisplaySkillIndicator(indicator);
			SetForcePickable(true);
		}


		public override void OnHideSkillIndicator(Splat indicator)
		{
			base.OnHideSkillIndicator(indicator);
			SetForcePickable(false);
		}


		private void SetForcePickable(bool forcePickable)
		{
			listPigeon = GetPigeons(LocalPlayerCharacter);
			listFireworkHat = GetFireworkHats(LocalPlayerCharacter);
			if (listPigeon != null)
			{
				foreach (LocalSummonServant localSummonServant in listPigeon)
				{
					localSummonServant.Pickable.ForcePickableDisable(!forcePickable);
				}
			}

			if (listFireworkHat != null)
			{
				foreach (LocalSummonTrap localSummonTrap in listFireworkHat)
				{
					localSummonTrap.Pickable.ForcePickableDisable(!forcePickable);
				}
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<EmmaSkillActive4Data>.inst.PigeonAttackDamageBySkillLevel[skillData.level]
						.ToString();
				case 1:
					return ((int) (Singleton<EmmaSkillActive4Data>.inst.PigeonAttackDamageApCoef *
					               SelfStat.AttackPower)).ToString();
				case 2:
					return Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionDamageBySkillLevel[skillData.level]
						.ToString();
				case 3:
					return ((int) (Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionDamageApCoef *
					               SelfStat.AttackPower)).ToString();
				case 4:
					return GameDB.characterState
						.GetData(Singleton<EmmaSkillActive4Data>.inst.PigeonDealerFetterStateCode).duration.ToString();
				case 5:
					return GameDB.characterState.GetData(Singleton<EmmaSkillActive4Data>.inst.MagicRabbitStateCode)
						.duration.ToString();
				case 6:
					return GameDB.characterState
						.GetData(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitFetterStateCode).duration.ToString();
				case 7:
					return Math.Abs(GameDB.characterState
							.GetData(Singleton<EmmaSkillActive3Data>.inst.MagicRabbitStateCode[skillData.level])
							.statValue1)
						.ToString();
				case 8:
					return skillData.maxStack.ToString();
				case 9:
					return skillData.stackUseIntervalTime.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/PigeonDamage";
				case 1:
					return "ToolTipType/HatDamage";
				case 2:
					return "ToolTipType/ChargingTime";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<EmmaSkillActive4Data>.inst.PigeonAttackDamageBySkillLevel[level].ToString();
				case 1:
					return Singleton<EmmaSkillActive4Data>.inst.FireworkHatExplosionDamageBySkillLevel[level]
						.ToString();
				case 2:
					return skillData.cooldown.ToString();
				default:
					return "";
			}
		}
	}
}