using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class DamageInfo
	{
		
		
		public int Damage
		{
			get
			{
				return this.damage;
			}
		}

		
		
		public DamageType DamageType
		{
			get
			{
				return this.damageType;
			}
		}

		
		
		public DamageSubType DamageSubType
		{
			get
			{
				return this.damageSubType;
			}
		}

		
		
		public int DamageDataCode
		{
			get
			{
				return this.damageDataCode;
			}
		}

		
		
		public int DamageId
		{
			get
			{
				return this.damageId;
			}
		}

		
		
		public bool TargetInCombat
		{
			get
			{
				return this.targetInCombat;
			}
		}

		
		
		public WorldCharacter Attacker
		{
			get
			{
				return this.attacker;
			}
		}

		
		
		public int UndefendedDamage
		{
			get
			{
				return this.undefendedDamage;
			}
		}

		
		
		public int damageForMastery
		{
			get
			{
				if (this.damageMasteryModifier != 1f)
				{
					return Mathf.FloorToInt((float)this.maxDamageOnHp * this.damageMasteryModifier);
				}
				return this.maxDamageOnHp;
			}
		}

		
		
		public Vector3? DamagePoint
		{
			get
			{
				return this.damagePoint;
			}
		}

		
		
		public bool IsCritical
		{
			get
			{
				return this.isCritical;
			}
		}

		
		
		public SkillSlotSet? SkillSlotSet
		{
			get
			{
				return this.skillSlotSet;
			}
		}

		
		
		public int EffectAndSoundCode
		{
			get
			{
				return this.effectAndSoundCode;
			}
		}

		
		
		public int MinRemain
		{
			get
			{
				return this.minRemain;
			}
		}

		
		private DamageInfo()
		{
			this.damage = 0;
			this.damageType = DamageType.Normal;
			this.damageSubType = DamageSubType.Normal;
			this.damageDataCode = 0;
			this.damageId = 0;
			this.minRemain = 0;
		}

		
		private DamageInfo(int damage, DamageType damageType, DamageSubType damageSubType, int damageDataCode, int damageId, int minRemain)
		{
			this.damage = damage;
			this.damageType = damageType;
			this.damageSubType = damageSubType;
			this.damageDataCode = damageDataCode;
			this.damageId = damageId;
			this.minRemain = minRemain;
		}

		
		public static DamageInfo Create(int damage, DamageType damageType, DamageSubType damageSubType, int damageDataCode, int damageId, int minRemain)
		{
			return new DamageInfo(damage, damageType, damageSubType, damageDataCode, damageId, minRemain);
		}

		
		public void SetAttacker(WorldCharacter attacker)
		{
			this.attacker = attacker;
		}

		
		public void SetDamage(int damage)
		{
			this.damage = damage;
		}

		
		public void SetDamagePoint(Vector3? damagePoint)
		{
			this.damagePoint = damagePoint;
		}

		
		public void SetCritical(bool isCritical)
		{
			this.isCritical = isCritical;
		}

		
		public void SetEffectAndSoundCode(int effectAndSoundCode)
		{
			this.effectAndSoundCode = effectAndSoundCode;
		}

		
		public void SetUndefendedDamage(int undefendedDamage)
		{
			this.undefendedDamage = undefendedDamage;
		}

		
		public void SetMaxDamageOnHp(int actualDamage)
		{
			this.maxDamageOnHp = actualDamage;
		}

		
		public void SetSkillInfo(SkillSlotSet skillSlotSet)
		{
			this.skillSlotSet = new SkillSlotSet?(skillSlotSet);
		}

		
		public void SetTargetInCombat(bool targetInCombat)
		{
			this.targetInCombat = targetInCombat;
		}

		
		public void SetDamageMasteryModifier(float damageMasteryModifier)
		{
			this.damageMasteryModifier = damageMasteryModifier;
		}

		
		private int damage;

		
		private DamageType damageType;

		
		private DamageSubType damageSubType;

		
		private int damageDataCode;

		
		private int damageId;

		
		private bool targetInCombat = true;

		
		private WorldCharacter attacker;

		
		private int undefendedDamage;

		
		private int maxDamageOnHp;

		
		private float damageMasteryModifier = 1f;

		
		private Vector3? damagePoint;

		
		private bool isCritical;

		
		private SkillSlotSet? skillSlotSet;

		
		private int effectAndSoundCode;

		
		private int minRemain;
	}
}
