using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive1MoveState)]
	public class RozzyActive1MoveState : SkillScript
	{
		
		private bool injected;

		
		private Vector3? preHitPoint;

		
		protected override void Start()
		{
			base.Start();
			injected = false;
			preHitPoint = null;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			CommonState commonState = extraData as CommonState;
			if (commonState != null && commonState.ExtraData != null)
			{
				preHitPoint = (Vector3) commonState.ExtraData;
			}

			if (preHitPoint != null)
			{
				OnMove(preHitPoint.Value);
			}

			while (!injected && MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <=
				StateDuration)
			{
				yield return WaitForFrame();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override bool UseOnMove()
		{
			int group = GameDB.characterState.GetData(Singleton<RozziSkillActive2Data>.inst.Active2SpeedUpStateCode)
				.group;
			return !Caster.IsHaveStateByGroup(group, Caster.ObjectId);
		}

		
		public override void OnMove(Vector3 hitPoint)
		{
			if (injected)
			{
				return;
			}

			injected = true;
			SkillData skillData = GameDB.skill.GetSkillData(Singleton<RozziSkillActive1Data>.inst.Active1MoveSkillCode);
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(info.caster, info.target, skillData, info.skillSlotSet,
				MasteryType.None, info.skillEvolutionLevel, hitPoint, info.releasePosition, null, true);
			(Caster.Character as WorldPlayerCharacter).UseInjectSkill(skillUseInfo);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
		}

		
		public override bool UseOnTargetOn()
		{
			return UseOnMove();
		}

		
		public override void OnTargetOn(WorldObject target)
		{
			base.OnTargetOn(target);
			Vector3 position = target.GetPosition();
			OnMove(position);
		}
	}
}