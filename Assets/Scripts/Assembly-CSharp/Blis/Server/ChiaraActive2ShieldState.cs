using System.Collections;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive2ShieldState)]
	public class ChiaraActive2ShieldState : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damage = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D collision;

		
		private ShieldState shieldState;

		
		protected override void Start()
		{
			base.Start();
			if (collision == null)
			{
				collision = new CollisionCircle3D(Caster.Position, SkillRange);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			OnRemoveThisState();
			WorldPlayerCharacter wpc = Caster.Character as WorldPlayerCharacter;
			if (wpc == null)
			{
				return;
			}

			wpc.CancelActionCasting(CastingCancelType.Action);
			if (wpc.IsRest)
			{
				wpc.StartActionCasting(ActionCostData.GetActionCostType(false), true, null,
					delegate { wpc.Rest(false, true); });
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			shieldState = extraData as ShieldState;
			yield return WaitForFrame();
		}

		
		private void OnRemoveThisState()
		{
			if (shieldState == null)
			{
				return;
			}

			if (Singleton<ChiaraSkillData>.inst.A2ShieldState != shieldState.StateData.code)
			{
				return;
			}

			if (shieldState.CurrentShieldAmount <= 0f)
			{
				PlaySkillAction(Target, SkillId.ChiaraActive2ShieldState, 1);
				return;
			}

			damage.Clear();
			damage.Add(SkillScriptParameterType.Damage, Singleton<ChiaraSkillData>.inst.A2BaseDamage[SkillLevel]);
			damage.Add(SkillScriptParameterType.DamageApCoef, Singleton<ChiaraSkillData>.inst.A2ApDamage);
			collision.UpdatePosition(Caster.Position);
			collision.UpdateRadius(SkillRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(collision);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, damage,
				Singleton<ChiaraSkillData>.inst.A2EffectSoundCode);
			PlaySkillAction(Target, SkillId.ChiaraActive2ShieldState, 2);
		}
	}
}