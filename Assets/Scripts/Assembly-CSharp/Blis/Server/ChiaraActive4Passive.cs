using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive4Passive)]
	public class ChiaraActive4Passive : SkillScript
	{
		
		private readonly List<ServerSightAgent> cachedSight = new List<ServerSightAgent>();

		
		private int passiveMaxStack;

		
		private int passiveStateGroup;

		
		private float sightShareTime;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteChangedStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst2.OnCompleteChangedStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			cachedSight.Clear();
			if (passiveStateGroup == 0)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[1]);
				sightShareTime = data.duration;
				passiveStateGroup = data.group;
				passiveMaxStack = data.maxStack;
			}

			MonoBehaviourInstance<GameService>.inst.World.FindAllDoAction<WorldCharacter>(
				delegate(WorldCharacter character)
				{
					if (!character.IsAlive)
					{
						return;
					}

					CharacterState characterState =
						character.StateEffector.FindStateByGroup(passiveStateGroup, Caster.ObjectId);
					if (characterState == null)
					{
						return;
					}

					if (characterState.StackCount < passiveMaxStack)
					{
						return;
					}

					ServerSightAgent item = Caster.AttachSight(character,
						Singleton<ChiaraSkillData>.inst.A4SightShareRange, characterState.RemainTime(), false);
					cachedSight.Add(item);
				});
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteChangedStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst2.OnCompleteChangedStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		private void OnCompleteAddState(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (!target.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId == target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (passiveStateGroup != characterState.StateData.group)
			{
				return;
			}

			if (characterState.StackCount < passiveMaxStack)
			{
				return;
			}

			for (int i = cachedSight.Count - 1; i >= 0; i--)
			{
				if (cachedSight[i] == null)
				{
					cachedSight.RemoveAt(i);
				}
				else if (cachedSight[i].ObjectId == target.ObjectId)
				{
					Caster.ResetSightDestroyTime(cachedSight[i], sightShareTime);
					return;
				}
			}

			ServerSightAgent item = Caster.AttachSight(target, Singleton<ChiaraSkillData>.inst.A4SightShareRange,
				sightShareTime, false);
			cachedSight.Add(item);
		}
	}
}