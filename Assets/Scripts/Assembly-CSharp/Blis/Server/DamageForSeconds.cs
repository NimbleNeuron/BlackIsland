using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class DamageForSeconds : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameters = SkillScriptParameterCollection.Create();

		
		private float damageRemain;

		
		private int lastTick;

		
		private float prevRemainTickTime;

		
		private float remainTickTime;

		
		private int resultDamage;

		
		private float tickDamage;

		
		private float tickDamageApCoef;

		
		
		protected abstract int intervalCount { get; }

		
		
		protected abstract float intervalTime { get; }

		
		
		protected abstract Dictionary<int, int> damageByLevel { get; }

		
		
		protected abstract Dictionary<int, float> damageApCoefByLevel { get; }

		
		protected override void Start()
		{
			base.Start();
			prevRemainTickTime += remainTickTime;
			int stackByGroup = Target.GetStackByGroup(info.stateData.group, Caster.ObjectId);
			int level = GameDB.characterState.GetData(StateCode).level;
			resultDamage = damageByLevel[level] * stackByGroup;
			float num = damageApCoefByLevel[level] * stackByGroup;
			damageRemain = 0f;
			lastTick = 0;
			remainTickTime = 0f;
			tickDamage = resultDamage / (float) intervalCount;
			tickDamageApCoef = num / intervalCount;
		}

		
		public override IEnumerator Play(object exteraData)
		{
			Start();
			if (intervalTime <= prevRemainTickTime)
			{
				prevRemainTickTime -= intervalTime;
				DirectDamageTo(tickDamage, tickDamageApCoef);
			}

			while (lastTick < intervalCount)
			{
				remainTickTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (intervalTime <= remainTickTime)
				{
					remainTickTime -= intervalTime;
					Damage();
					lastTick++;
				}

				yield return WaitForFrame();
			}

			Finish();
		}

		
		private void Damage()
		{
			int num;
			if (lastTick + 1 == intervalCount)
			{
				num = resultDamage;
			}
			else
			{
				num = Mathf.FloorToInt(tickDamage + damageRemain);
				resultDamage -= num;
				damageRemain += tickDamage - num;
			}

			DirectDamageTo(num, tickDamageApCoef);
		}

		
		private void DirectDamageTo(float damage, float damageApCoef)
		{
			parameters.Clear();
			parameters.Add(SkillScriptParameterType.Damage, damage);
			parameters.Add(SkillScriptParameterType.DamageApCoef, damageApCoef);
			base.DirectDamageTo(Target, DamageType.Skill, DamageSubType.Dot, 0, parameters, 0, false, 0, 1f, false);
		}

		
		protected override void Finish(bool cancel = false)
		{
			prevRemainTickTime = 0f;
			base.Finish(cancel);
		}
	}
}