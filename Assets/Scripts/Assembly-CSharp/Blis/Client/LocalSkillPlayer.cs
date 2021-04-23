using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class LocalSkillPlayer
	{
		private readonly Dictionary<SkillId, LocalSkillScript> forDataSkillScripts =
			new Dictionary<SkillId, LocalSkillScript>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);


		private readonly Dictionary<SkillId, LocalSkillScript> forStartSkillScripts =
			new Dictionary<SkillId, LocalSkillScript>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);


		private readonly LocalSkillScriptManager localSkillScriptManager = LocalSkillScriptManager.inst;


		public readonly Dictionary<SkillId, LocalSkillScript> playingScripts =
			new Dictionary<SkillId, LocalSkillScript>(SingletonComparerEnum<SkillIdComparer, SkillId>.Instance);


		private readonly LocalObject self;


		private Action<SkillData> OnBeforeStartSkill;


		public LocalSkillPlayer(LocalObject self)
		{
			this.self = self;
		}

		public bool IsPlaying(SkillId skillId)
		{
			return playingScripts.ContainsKey(skillId);
		}


		public void SetOnBeforeStartSkillAction(Action<SkillData> action)
		{
			OnBeforeStartSkill = action;
		}


		public void Init(SkillControllerSnapshot skillControllerSnapshot)
		{
			if (skillControllerSnapshot != null)
			{
				List<SkillScriptSnapshot> passiveScripts = skillControllerSnapshot.passiveScripts;
				if (passiveScripts != null)
				{
					passiveScripts.ForEach(delegate(SkillScriptSnapshot x)
					{
						Start(x.skillId, GameDB.skill.GetSkillData(x.skillCode), x.skillEvolutionLevel,
							x.targetObjectId, x.cursorPosition.ToVector3(), x.releasePosition.ToVector3());
					});
				}

				List<SkillScriptSnapshot> list = skillControllerSnapshot.playingScripts;
				if (list != null)
				{
					list.ForEach(delegate(SkillScriptSnapshot x)
					{
						Start(x.skillId, GameDB.skill.GetSkillData(x.skillCode), x.skillEvolutionLevel,
							x.targetObjectId, x.cursorPosition.ToVector3(), x.releasePosition.ToVector3());
					});
				}

				List<SkillScriptSnapshot> playingStateScripts = skillControllerSnapshot.playingStateScripts;
				if (playingStateScripts == null)
				{
					return;
				}

				playingStateScripts.ForEach(delegate(SkillScriptSnapshot x)
				{
					StartStateSkill(x.skillId, GameDB.skill.GetSkillData(x.skillCode), x.skillEvolutionLevel,
						x.cursorPosition.ToVector3(), x.releasePosition.ToVector3(), x.casterId);
				});
			}
		}


		public bool GetSkillRange(SkillId skillId, SkillData skillData, ref float minRange, ref float maxRange)
		{
			return GetSkillScript(skillId, skillData, false).GetSkillRange(ref minRange, ref maxRange);
		}


		public float GetSkillWidth(SkillId skillId, SkillData skillData)
		{
			return GetSkillScript(skillId, skillData, false).GetSkillWidth();
		}


		public float GetSkillInnerRange(SkillId skillId, SkillData skillData)
		{
			return GetSkillScript(skillId, skillData, false).GetSkillInnerRange();
		}


		public float GetSkillLength(SkillId skillId, SkillData skillData)
		{
			return GetSkillScript(skillId, skillData, false).GetSkillLength();
		}


		public float GetSkillAngle(SkillId skillId, SkillData skillData)
		{
			return GetSkillScript(skillId, skillData, false).GetSkillAngle();
		}


		private LocalSkillScript GetSkillScript(SkillId skillId, SkillData skillData, bool isForStartSkill)
		{
			LocalSkillScript localSkillScript = null;
			Dictionary<SkillId, LocalSkillScript> dictionary =
				isForStartSkill ? forStartSkillScripts : forDataSkillScripts;
			if (dictionary.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.Init(self, skillData);
			}
			else
			{
				localSkillScript = localSkillScriptManager.Create(skillId);
				localSkillScript.Init(self, skillData);
				dictionary.Add(skillId, localSkillScript);
			}

			return localSkillScript;
		}


		public void Start(SkillId skillId, SkillData skillData, int evolutionLevel, int targetObjectId,
			Vector3 cursorPosition, Vector3 releasePosition)
		{
			Start(skillId, skillData, evolutionLevel, targetObjectId);
		}


		public void Start(SkillId skillId, SkillData skillData, int evolutionLevel, int targetObjectId)
		{
			LocalSkillScript skillScript;
			if (!playingScripts.TryGetValue(skillId, out skillScript))
			{
				skillScript = GetSkillScript(skillId, skillData, true);
				playingScripts.Add(skillId, skillScript);
			}
			else
			{
				skillScript.Init(self, skillData);
			}

			LocalObject target = null;
			MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalObject>(targetObjectId, ref target);
			Action<SkillData> onBeforeStartSkill = OnBeforeStartSkill;
			if (onBeforeStartSkill != null)
			{
				onBeforeStartSkill(skillData);
			}

			skillScript.Setup(self, evolutionLevel, target);
			skillScript.Start();
			skillScript.StartInternal();
		}


		public void StartPassiveSkill(SkillId skillId, SkillData skillData, int evolutionLevel, int targetObjectId)
		{
			LocalSkillScript skillScript;
			if (!playingScripts.TryGetValue(skillId, out skillScript))
			{
				skillScript = GetSkillScript(skillId, skillData, true);
				playingScripts.Add(skillId, skillScript);
			}
			else
			{
				skillScript.Init(self, skillData);
			}

			LocalObject target = null;
			MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalObject>(targetObjectId, ref target);
			skillScript.Setup(self, evolutionLevel, target);
			skillScript.Start();
			skillScript.StartInternal();
		}


		private void StartStateSkill(SkillId skillId, SkillData skillData, int evolutionLevel, Vector3 cursorPosition,
			Vector3 releasePosition, int stateCasterId)
		{
			StartStateSkill(skillId, skillData, evolutionLevel, stateCasterId);
		}


		public void StartStateSkill(SkillId skillId, SkillData skillData, int evolutionLevel, int stateCasterId)
		{
			LocalSkillScript skillScript;
			if (!playingScripts.TryGetValue(skillId, out skillScript))
			{
				skillScript = GetSkillScript(skillId, skillData, true);
				playingScripts.Add(skillId, skillScript);
			}
			else
			{
				skillScript.Init(self, skillData);
			}

			LocalObject caster = MonoBehaviourInstance<ClientService>.inst.World.Find<LocalObject>(stateCasterId);
			skillScript.Setup(caster, evolutionLevel);
			skillScript.Start();
			skillScript.StartInternal();
		}


		public void Play(SkillId skillId, int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 0 || actionNo < 0)
			{
				return;
			}

			LocalSkillScript localSkillScript = null;
			if (forStartSkillScripts.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.Play(actionNo, target, targetPosition);
				return;
			}

			Log.W("Not Found PlaySkill : {0}({1})", skillId, actionNo);
		}


		public void Finish(SkillId skillId, bool cancel)
		{
			LocalSkillScript localSkillScript;
			if (playingScripts.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.Finish(cancel);
				localSkillScript.FinishInternal();
				playingScripts.Remove(skillId);
			}
		}


		public void FinishPassiveSkill(SkillId skillId, bool cancel)
		{
			LocalSkillScript localSkillScript;
			if (playingScripts.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.Finish(cancel);
				localSkillScript.FinishInternal();
				playingScripts.Remove(skillId);
			}
		}


		public void FinishStateSkill(SkillId skillId, bool cancel)
		{
			LocalSkillScript localSkillScript;
			if (playingScripts.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.Finish(cancel);
				localSkillScript.FinishInternal();
				playingScripts.Remove(skillId);
			}
		}


		public void FinishAll()
		{
			foreach (LocalSkillScript localSkillScript in playingScripts.Values)
			{
				localSkillScript.Finish(true);
				localSkillScript.FinishInternal();
			}

			playingScripts.Clear();
		}


		public string[] GetTooltipParam(SkillData skillData, int evolutionLevel)
		{
			LocalSkillScript skillScript =
				GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId,
					skillData, false);
			skillScript.Init(self, skillData);
			skillScript.Setup(self, evolutionLevel);
			return skillScript.GetTooltipParams(skillData);
		}


		public string[] GetNextLevelTooltipParam(SkillData skillData, int evolutionLevel)
		{
			if (skillData.level == 0)
			{
				return null;
			}

			LocalSkillScript skillScript =
				GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId,
					skillData, false);
			skillScript.Init(self, skillData);
			skillScript.Setup(self, evolutionLevel);
			return skillScript.GetNextlevelTooltipParams(skillData);
		}


		public string[] GetNextLevelTooltipValue(SkillData skillData, SkillData nextSkillData, int evolutionLevel)
		{
			if (skillData.level == 0)
			{
				return null;
			}

			LocalSkillScript skillScript =
				GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId,
					skillData, false);
			skillScript.Init(self, skillData);
			skillScript.Setup(self, evolutionLevel);
			return skillScript.GetNextlevelTooltipValues(skillData, nextSkillData);
		}


		public bool IsPlayingCanAdditionalActionActiveSkill()
		{
			foreach (KeyValuePair<SkillId, LocalSkillScript> keyValuePair in playingScripts)
			{
				SkillGroupData skillGroupDataByActiveSkillId =
					GameDB.skill.GetSkillGroupDataByActiveSkillId(keyValuePair.Key);
				if (skillGroupDataByActiveSkillId != null && skillGroupDataByActiveSkillId.canAdditionalAction)
				{
					return true;
				}
			}

			return false;
		}


		public void StartConcentration(SkillId skillId)
		{
			LocalSkillScript localSkillScript;
			if (playingScripts.TryGetValue(skillId, out localSkillScript))
			{
				localSkillScript.StartConcentration();
			}
		}


		public void OnUpdateWeapon()
		{
			foreach (KeyValuePair<SkillId, LocalSkillScript> keyValuePair in playingScripts)
			{
				keyValuePair.Value.OnUpdateWeapon();
			}
		}


		public void OnDisplaySkillIndicator(SkillData skillData, Splat indicator)
		{
			GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId, skillData,
				false).OnDisplaySkillIndicator(indicator);
		}


		public void OnHideSkillIndicator(SkillData skillData, Splat indicator)
		{
			if (skillData == null)
			{
				return;
			}

			if (indicator == null)
			{
				return;
			}

			GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId, skillData,
				false).OnHideSkillIndicator(indicator);
		}


		public Vector3 GetSkillMyCharacterPos(SkillData skillData)
		{
			return GetSkillScript(skillData.SkillId == SkillId.None ? skillData.PassiveSkillId : skillData.SkillId,
				skillData, false).GetSkillMyCharacterPos();
		}


		public UseSkillErrorCode IsCanUseSkill(SkillData skillData, LocalObject hitTarget, Vector3? cursorPosition)
		{
			return GetSkillScript(skillData.SkillId, skillData, false).IsCanUseSkill(hitTarget, cursorPosition);
		}


		public UseSkillErrorCode IsSkillSlotCanUseInSkillScript(SkillData skillData)
		{
			if (skillData.SkillId == SkillId.None)
			{
				return UseSkillErrorCode.InvalidAction;
			}

			return GetSkillScript(skillData.SkillId, skillData, false).IsEnableSkillSlot();
		}


		public bool CompareObjectOrder(LocalObject prevTarget, LocalObject nextTarget, SkillId skillId)
		{
			LocalSkillScript localSkillScript = null;
			if (forDataSkillScripts.TryGetValue(skillId, out localSkillScript))
			{
				return localSkillScript.PickingOrderCompare(prevTarget, nextTarget);
			}

			return prevTarget.GetObjectOrder() > nextTarget.GetObjectOrder();
		}
	}
}