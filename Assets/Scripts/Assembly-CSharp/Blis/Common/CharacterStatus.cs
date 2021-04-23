using System;
using Blis.Server;

namespace Blis.Common
{
	public class CharacterStatus
	{
		private int bullet;
		public Action<int> ChangeHpEvent;
		private int exp;
		private int extraPoint;
		private int level = 1;
		private int monsterKill;
		private float moveSpeed;
		private OnChangeStatusValue onChangeStatusValue;
		private int playerKill;
		private int playerKillAssist;
		private int shield;
		private int sp;

		public CharacterStatus() { }

		public CharacterStatus(CharacterStatusSnapshot snapshot)
		{
			InitCharacterStatus(snapshot);
		}

		public CharacterStatus(PlayerStatusSnapshot snapshot)
		{
			InitCharacterStatus(snapshot);
			InitPlayerStatus(snapshot);
		}

		public CharacterStatus(SummonStatusSnapshot snapshot)
		{
			InitCharacterStatus(snapshot);
		}

		public CharacterStatus(MonsterStatusSnapshot snapshot)
		{
			InitCharacterStatus(snapshot);
		}
		
		private int hp {
			set
			{
				if (onChangeStatusValue.SetHp(value))
				{
					OnChangeHp(Hp);
				}
			}
		}

		public int Hp => onChangeStatusValue.Hp;
		public int Sp => sp;
		public int Level => level;
		public int Shield => shield;
		public float MoveSpeed => moveSpeed;
		public int ExtraPoint => extraPoint;
		public int Exp => exp;
		public int Bullet => bullet;
		public int MonsterKill => monsterKill;
		public int PlayerKill => playerKill;
		public int PlayerKillAssist => playerKillAssist;
		public int DamageToPlayer { get; private set; }
		public int DamageToMonster { get; private set; }
		
		public int TrapDamageToPlayer { get; private set; }
		
		public int BasicAttackDamageToPlayer { get; private set; }
		
		public int SkillDamageToPlayer { get; private set; }
		
		public int CraftUncommon { get; private set; }
		
		public int CraftRare { get; private set; }
		
		public int CraftEpic { get; private set; }
		
		public int CraftLegend { get; private set; }

		private void InitCharacterStatus(CharacterStatusSnapshot snapshot)
		{
			hp = snapshot.hp;
			sp = snapshot.sp;
			level = snapshot.level;
			shield = snapshot.shield;
			moveSpeed = snapshot.moveSpeed;
		}


		private void InitPlayerStatus(PlayerStatusSnapshot snapshot)
		{
			extraPoint = snapshot.extraPoint;
			exp = snapshot.exp;
			bullet = snapshot.bullet;
			monsterKill = snapshot.monsterKill;
			playerKill = snapshot.playerKill;
			playerKillAssist = snapshot.playerKillAssist;
		}


		public void SetHp(int hp)
		{
			this.hp = hp;
		}


		public void AddHp(int addHp)
		{
			hp = Hp + addHp;
		}


		public void SubHp(int subHp)
		{
			hp = Math.Max(0, Hp - subHp);
		}


		public int SubHpCompare(int subHp, int compareHp)
		{
			int hp = Hp;
			this.hp = Math.Max(compareHp, Hp - subHp);
			return hp - Hp;
		}


		public void SetSp(int sp)
		{
			this.sp = sp;
		}


		public void AddSp(int addSp)
		{
			sp += addSp;
		}


		public void SubSp(int subSp)
		{
			sp = Math.Max(0, sp - subSp);
		}


		public void SetLevel(int nextLevel)
		{
			level = nextLevel;
		}


		public void SetShield(int shield)
		{
			this.shield = shield;
		}


		public void SetMoveSpeed(float moveSpeed)
		{
			this.moveSpeed = moveSpeed;
		}


		public void SetExtraPoint(int ep)
		{
			extraPoint = ep;
		}


		public void SubEp(int ep)
		{
			ModifyExtraPoint(-ep);
		}


		public void ModifyExtraPoint(int modifyEp)
		{
			extraPoint += modifyEp;
			if (extraPoint < 0)
			{
				extraPoint = 0;
			}
		}


		public void AddPlayerKillCount()
		{
			playerKill++;
		}


		public void AddPlayerKillAssistCount()
		{
			playerKillAssist++;
		}


		public void AddMonsterKillCount()
		{
			monsterKill++;
		}


		public void AddDamageToPlayer(DamageInfo damageInfo)
		{
			DamageToPlayer += damageInfo.Damage;
			if (damageInfo.DamageType != DamageType.Normal)
			{
				AddSkillDamageToPlayer(damageInfo.Damage);
				return;
			}

			if (damageInfo.DamageSubType == DamageSubType.Trap)
			{
				AddTrapDamageToPlayer(damageInfo.Damage);
				return;
			}

			AddBasicAttackDamageToPlayer(damageInfo.Damage);
		}


		private void AddTrapDamageToPlayer(int value)
		{
			TrapDamageToPlayer += value;
		}


		private void AddBasicAttackDamageToPlayer(int value)
		{
			BasicAttackDamageToPlayer += value;
		}


		private void AddSkillDamageToPlayer(int value)
		{
			SkillDamageToPlayer += value;
		}


		public void AddDamageToMonster(int value)
		{
			DamageToMonster += value;
		}


		public void UpdateBullet(int bullet)
		{
			this.bullet = bullet;
		}


		public void ConsumeBullet()
		{
			bullet--;
		}


		private void OnChangeHp(int hp)
		{
			Action<int> changeHpEvent = ChangeHpEvent;
			if (changeHpEvent == null)
			{
				return;
			}

			changeHpEvent(hp);
		}


		public void SetExp(int exp)
		{
			this.exp = exp;
		}


		public void AddItemCraftCount(ItemGrade itemGrade)
		{
			switch (itemGrade)
			{
				case ItemGrade.Uncommon:
					AddCraftUncommon();
					return;
				case ItemGrade.Rare:
					AddCraftRare();
					return;
				case ItemGrade.Epic:
					AddCraftEpic();
					return;
				case ItemGrade.Legend:
					AddCraftLegend();
					return;
				default:
					return;
			}
		}


		private void AddCraftUncommon()
		{
			int craftUncommon = CraftUncommon;
			CraftUncommon = craftUncommon + 1;
		}


		private void AddCraftRare()
		{
			int craftRare = CraftRare;
			CraftRare = craftRare + 1;
		}


		private void AddCraftEpic()
		{
			int craftEpic = CraftEpic;
			CraftEpic = craftEpic + 1;
		}


		private void AddCraftLegend()
		{
			int craftLegend = CraftLegend;
			CraftLegend = craftLegend + 1;
		}


		protected struct OnChangeStatusValue
		{
			public int Hp => hp;


			public bool SetHp(int hp)
			{
				if (hp == this.hp)
				{
					return false;
				}

				this.hp = hp;
				return true;
			}


			private int hp;
		}
	}
}