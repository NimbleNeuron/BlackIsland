using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class LenoxBlueSnake : SkillScript
	{
		
		protected bool NoneCheckDistanceTiming;

		
		protected Vector3 NoneCheckPosition;

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected Vector3 PrevPosition;

		
		protected float totalDistance;

		
		protected override void Start()
		{
			base.Start();
			NoneCheckDistanceTiming = false;
			PrevPosition = Target.Position;
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterHyperLoopWarpEvent = (Action<WorldPlayerCharacter, Vector3, int>) Delegate.Combine(
				inst.OnAfterHyperLoopWarpEvent,
				new Action<WorldPlayerCharacter, Vector3, int>(OnAfterHyperLoopWarpEvent));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterHyperLoopWarpEvent = (Action<WorldPlayerCharacter, Vector3, int>) Delegate.Remove(
				inst.OnAfterHyperLoopWarpEvent,
				new Action<WorldPlayerCharacter, Vector3, int>(OnAfterHyperLoopWarpEvent));
			Damgage();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			float timeStack = 0f;
			while (isPlaying)
			{
				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (!NoneCheckDistanceTiming)
				{
					float num = GameUtil.DistanceOnPlane(PrevPosition, Target.Position);
					totalDistance += num;
				}
				else if (NoneCheckPosition.Equals(Target.Position))
				{
					NoneCheckDistanceTiming = false;
				}
				else
				{
					float num2 = GameUtil.DistanceOnPlane(PrevPosition, Target.Position);
					totalDistance += num2;
				}

				PrevPosition = Target.Position;
				if (Singleton<LenoxSkillActive4Data>.inst.BlueSnakeActiveTime <= timeStack)
				{
					timeStack -= Singleton<LenoxSkillActive4Data>.inst.BlueSnakeActiveTime;
					Damgage();
				}

				yield return WaitForFrame();
			}
		}

		
		private void Damgage()
		{
			if (totalDistance >= Singleton<LenoxSkillActive4Data>.inst.BlueSnakeMinDistance &&
			    totalDistance <= Singleton<LenoxSkillActive4Data>.inst.BlueSnakeMaxDistance)
			{
				float meterPerDamage = GetMeterPerDamage();
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage, totalDistance * meterPerDamage);
				DirectDamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
					Singleton<LenoxSkillActive4Data>.inst.BlueSnakeEffectAndSoundCode);
			}

			totalDistance = 0f;
		}

		
		protected abstract float GetMeterPerDamage();

		
		public void OnAfterHyperLoopWarpEvent(WorldPlayerCharacter playerCharacter, Vector3 warpPosition, int areaCode)
		{
			if (!Target.IsAlive)
			{
				return;
			}

			if (!Target.ObjectId.Equals(playerCharacter.ObjectId))
			{
				return;
			}

			NoneCheckPosition = warpPosition;
			NoneCheckDistanceTiming = true;
		}
	}
}