using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(CharacterFloatingUI))]
	public abstract class LocalSummonBase : LocalCharacter
	{
		protected const string TrapIndicator = "TrapIndicator";


		private const string _Color = "_Color";


		private const string _OutLineColor = "_OutlineColor";


		protected ClientCharacter clientCharacter;


		private int expireTime;


		private float expireTimer;


		private SummonHostileAgent hostileAgent;


		protected List<Renderer> indicatorRenderers;


		private LocalPlayerCharacter owner;


		private int ownerId;


		protected List<Renderer> renderers = new List<Renderer>();


		protected SplatManager splatManager;


		private SummonData summonData;


		public SummonData SummonData => summonData;


		public SplatManager SplatManager => splatManager;


		public LocalPlayerCharacter Owner => owner;


		public int OwnerId => ownerId;


		protected virtual void OnDestroy()
		{
			floatingUi.Hide();
			LocalPlayerCharacter localPlayerCharacter = owner;
			if (localPlayerCharacter != null)
			{
				localPlayerCharacter.RemoveOwnSummon(this);
			}

			if (summonData.destroyEffectAndSoundCode != 0)
			{
				PlayLocalEffectWorldPoint(summonData.destroyEffectAndSoundCode, GetPosition());
			}
		}

		protected override int GetTeamNumber()
		{
			LocalPlayerCharacter localPlayerCharacter = Owner;
			if (localPlayerCharacter == null)
			{
				return 0;
			}

			return localPlayerCharacter.TeamNumber;
		}


		protected override int GetCharacterCode()
		{
			return summonData.code;
		}


		protected override GameObject GetCharacterPrefab()
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(Owner.CharacterCode, Owner.SkinIndex,
					summonData.prefabPath);
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(summonData.prefabPath);
		}


		protected override string GetNickname()
		{
			return Ln.Get(LnType.SummonData_Name, summonData.code.ToString());
		}


		protected override HostileAgent GetHostileAgent()
		{
			return hostileAgent;
		}


		protected override bool GetIsOutSight()
		{
			return !(SightAgent != null) || SightAgent.IsOutSight;
		}


		public override void Init(byte[] snapshotData)
		{
			SummonSnapshot summonSnapshot = serializer.Deserialize<SummonSnapshot>(snapshotData);
			summonData = GameDB.character.GetSummonData(summonSnapshot.summonId);
			ownerId = summonSnapshot.ownerId;
			if (summonSnapshot.ownerId > 0)
			{
				owner = MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(summonSnapshot
					.ownerId);
				owner.AddOwnSummon(this);
			}

			base.Init(snapshotData);
			if (owner != null && summonData.sightShare)
			{
				sightAgent.SetOwner(owner.SightAgent);
			}

			hostileAgent = new SummonHostileAgent(this, ownerId);
			sightAgent.SetDetect(summonData.detectShare, summonData.detectInvisible);
			expireTime = (int) summonData.duration;
			expireTimer = expireTime;
			characterObject.GetComponentsInChildren<Renderer>(true, renderers);
			SetCharacterAnimatorTrigger("appear", false);
			pickable.ForcePickableDisable(summonData.isInvincibility);
			pickable.EnablePickable(!summonData.isInvincibility);
			if (summonData.isInvincibility)
			{
				CharacterFloatingUI floatingUi = this.floatingUi;
				if (floatingUi != null)
				{
					floatingUi.SetAutoVisible(false);
				}

				CharacterFloatingUI floatingUi2 = this.floatingUi;
				if (floatingUi2 != null)
				{
					floatingUi2.Hide();
				}
			}

			clientCharacter = characterObject.GetComponentInChildren<ClientCharacter>();
			if (clientCharacter != null)
			{
				clientCharacter.Init(this, objectId);
			}

			if (activeOnHostileType != null)
			{
				activeOnHostileType.Init(this);
			}

			if (summonData.spawnEffectAndSoundCode != 0)
			{
				PlayLocalEffectWorldPoint(summonData.spawnEffectAndSoundCode, GetPosition());
			}

			SetIsInCombat(summonSnapshot.isInCombat);
		}


		protected override void InitStatus(byte[] snapshot)
		{
			SummonStatusSnapshot snapshot2 = serializer.Deserialize<SummonStatusSnapshot>(snapshot);
			Status = new CharacterStatus(snapshot2);
		}


		protected override LocalCharacterStat CreateCharacterStat()
		{
			return new LocalCharacterStat();
		}


		public override string GetLocalizedName(bool includeColor)
		{
			return GetNickname();
		}


		public override void UpdateInvisible(bool isInvisible)
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			if (SightAgent != null)
			{
				SightAgent.SetIsInvisible(isInvisible);
				if (!IsOutSight)
				{
					SetOpacity(IsInvisible ? 0.55f : 1f);
				}
			}

			if (characterRenderer != null)
			{
				characterRenderer.SetMaterial(MaterialSwitchType.Stealth, IsInvisible);
			}

			if (clientCharacter != null)
			{
				clientCharacter.SetStealth(isInvisible);
			}
		}


		protected virtual void SetOpacity(float opacity)
		{
			if (opacity > 0f)
			{
				foreach (Renderer renderer in renderers)
				{
					if (renderer != null)
					{
						renderer.enabled = true;
						foreach (Material material in renderer.materials)
						{
							if (material.HasProperty("_Color"))
							{
								material.color = new Color(material.color.r, material.color.g, material.color.b,
									opacity);
							}

							if (material.HasProperty("_OutlineColor"))
							{
								Color color = material.GetColor("_OutlineColor");
								material.SetColor("_OutlineColor",
									new Color(color.r, color.g, color.b, opacity < 1f ? 0f : 1f));
							}
						}
					}
				}
			}
		}


		protected virtual void ShowIndicator() { }


		protected virtual void HideIndicator() { }


		public override void ShowMiniMapIcon(MiniMapIconType miniMapIconType)
		{
			if (summonData.detectShare)
			{
				MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateSummonCamera(ObjectId, GetPosition(),
					GetMapIcon(), miniMapIconType);
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.Minimap.UIMap.CreateNonPlayer(ObjectId, GetPosition(), GetMapIcon(),
				miniMapIconType);
		}


		public override void ShowMapIcon(MiniMapIconType miniMapIconType) { }


		public override void HideMapIcon(MiniMapIconType miniMapIconType) { }


		public override void OnDamage(int attackerId, bool isSkillDamage, int damage, bool isCritical, int effectCode)
		{
			if (summonData.isInvincibility)
			{
				return;
			}

			base.OnDamage(attackerId, isSkillDamage, damage, isCritical, effectCode);
		}


		public override void DestroySelf()
		{
			base.DestroySelf();
			if (characterObject == null)
			{
				return;
			}

			characterObject.transform.parent = null;
			Destroy(characterObject, summonData.attackDelay + summonData.localDestroyDelay);
		}


		protected override Sprite GetMapIcon()
		{
			bool flag = owner != null
				? MonoBehaviourInstance<ClientService>.inst.IsAlly(owner)
				: MonoBehaviourInstance<ClientService>.inst.IsAlly(this);
			if (summonData.detectShare)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(
					flag ? "Ico_Map_Ally_SummonCamera" : "Ico_Map_Enemy_SummonCamera");
			}

			return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(flag
				? "Ico_Map_Ally_SummonObject"
				: "Ico_Map_Enemy_SummonObject");
		}


		public bool IsOwner(int objectId)
		{
			return owner != null && objectId == owner.ObjectId;
		}


		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			if (summonData != null && !summonData.isInvincibility)
			{
				expireTimer -= Time.deltaTime;
				int sp = Mathf.CeilToInt(expireTimer);
				CharacterFloatingUI floatingUi = this.floatingUi;
				if (floatingUi != null)
				{
					floatingUi.SetSp(sp, expireTime);
				}

				TargetInfoHud targetInfoHud = this.targetInfoHud;
				if (targetInfoHud == null)
				{
					return;
				}

				targetInfoHud.SetTargetStatSpBar(sp, expireTime);
			}
		}


		public void ResetDuration(float duration)
		{
			expireTime = (int) duration;
			expireTimer = duration;
		}


		public override void SetNoParentEffectByTag(string tag)
		{
			base.SetNoParentEffectByTag(tag);
			if (clientCharacter != null)
			{
				clientCharacter.SetNoParentEffectByTag(tag);
			}
		}


		public override void StopLocalEffectByTag(string tag)
		{
			base.StopLocalEffectByTag(tag);
			if (clientCharacter != null)
			{
				clientCharacter.StopEffectByTag(tag);
			}
		}


		public override void StopLocalSoundByTag(string tag)
		{
			base.StopLocalSoundByTag(tag);
			if (clientCharacter != null)
			{
				clientCharacter.StopSoundByTag(tag);
			}
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			if (base.SetCursor(myPlayer))
			{
				return true;
			}

			if (!IsAlive)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.None);
				return true;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Ally);
				return true;
			}

			if (!myPlayer.IsEquippedWeapon())
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Disarmed);
				return true;
			}

			MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.EnemySummon);
			return true;
		}


		public override void InSight()
		{
			base.InSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetInSight(true);
			}
		}


		public override void OutSight()
		{
			base.OutSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetInSight(false);
			}
		}


		public override void OnVisible()
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			FogHiderOnCenter.InSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(true);
			}

			EnableSpringManager(true);
			pickable.EnablePickable(true);
			ShowIndicator();
			SetOpacity(IsInvisible ? 0.55f : 1f);
			characterObject.SetActive(true);
			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(true);
				characterRenderer.SetMaterial(MaterialSwitchType.Stealth, IsInvisible);
			}

			if (summonData != null && !summonData.isInvincibility && floatingUi != null)
			{
				floatingUi.Show();
				floatingUi.SetAutoVisible(true);
			}

			ShowMiniMapIcon(MiniMapIconType.Sight);
		}


		public override void OnInvisible()
		{
			if (MonoBehaviourInstance<ClientService>.inst == null)
			{
				return;
			}

			FogHiderOnCenter.OutSight();
			if (clientCharacter != null)
			{
				clientCharacter.SetVisible(false);
			}

			EnableSpringManager(false);
			pickable.EnablePickable(false);
			HideIndicator();
			SetOpacity(0f);
			characterObject.SetActive(false);
			if (characterRenderer != null)
			{
				characterRenderer.EnableRenderer(false);
			}

			if (floatingUi != null)
			{
				floatingUi.Hide();
				floatingUi.SetAutoVisible(false);
			}

			HideMiniMapIcon(MiniMapIconType.Sight);
		}


		protected virtual void CreateIndicator() { }


		public virtual void SetSummonRangeIndicator(bool isActive, float range) { }


		public override GameObject LoadProjectile(string projectileName)
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadProjectile(Owner.CharacterCode, Owner.SkinIndex,
					projectileName);
			}

			return LoadCommonProjectile(projectileName);
		}


		public override GameObject LoadObject(string objectName)
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadObject(Owner.CharacterCode, Owner.SkinIndex,
					objectName);
			}

			return LoadCommonObject(objectName);
		}


		public override GameObject LoadEffect(string effectName)
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect(Owner.CharacterCode, Owner.SkinIndex,
					effectName);
			}

			return LoadCommonEffect(effectName);
		}


		public override AudioClip LoadFXSound(string soundName)
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(Owner.CharacterCode, Owner.SkinIndex,
					soundName);
			}

			return LoadCommonFXSound(soundName);
		}


		public override AudioClip LoadVoice(string characterResource, string voiceName, int randomCount = 0)
		{
			if (Owner != null)
			{
				return SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(Owner.CharacterCode, Owner.SkinIndex,
					characterResource, voiceName, randomCount);
			}

			return LoadCommonVoice(voiceName);
		}


		protected override void OnBushEvent(bool isInBush, int objectId) { }
	}
}