using Blis.Common;

namespace Blis.Server
{
	
	public class HealInfo
	{
		
		
		public int Hp
		{
			get
			{
				return this.hp;
			}
		}

		
		
		public int Sp
		{
			get
			{
				return this.sp;
			}
		}

		
		
		public WorldCharacter Healer
		{
			get
			{
				return this.healer;
			}
		}

		
		
		public int EffectAndSoundCode
		{
			get
			{
				return this.effectAndSoundCode;
			}
		}

		
		
		public int StateCode
		{
			get
			{
				return this.stateCode;
			}
		}

		
		
		public bool ShowUI
		{
			get
			{
				return this.showUI;
			}
		}

		
		
		public bool NeedApplyHealRatio
		{
			get
			{
				return this.needApplyHealRatio;
			}
		}

		
		private HealInfo()
		{
		}

		
		private HealInfo(int hp, int sp)
		{
			this.hp = hp;
			this.sp = sp;
		}

		
		public static HealInfo Create(int hp, int sp)
		{
			return new HealInfo(hp, sp);
		}

		
		public void SetHealer(WorldCharacter healer)
		{
			this.healer = healer;
		}

		
		public void SetStateCode(int stateCode)
		{
			this.stateCode = stateCode;
		}

		
		public void SetEffectAndSoundCode(int effectAndSoundCode)
		{
			this.effectAndSoundCode = effectAndSoundCode;
		}

		
		public void SetShowUI(bool showUI)
		{
			this.showUI = showUI;
		}

		
		public void SetNeedApplyHealRatio(bool needApplyHealRatio)
		{
			this.needApplyHealRatio = needApplyHealRatio;
		}

		
		public LocalCharacterCommandPacket GetHealPacket(int deltaHp, int deltaSp)
		{
			if (this.effectAndSoundCode != 0 && this.stateCode != 0)
			{
				return new CmdHealEffectAndStateCode
				{
					addHp = deltaHp,
					addSp = deltaSp,
					stateCode = this.stateCode,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.EffectAndSoundCode != 0)
			{
				return new CmdHealEffectCode
				{
					addHp = deltaHp,
					addSp = deltaSp,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.StateCode != 0)
			{
				return new CmdHealStateCode
				{
					addHp = deltaHp,
					addSp = deltaSp,
					stateCode = this.StateCode,
					showUI = this.ShowUI
				};
			}
			return new CmdHeal
			{
				addHp = deltaHp,
				addSp = deltaSp,
				showUI = this.ShowUI
			};
		}

		
		public LocalCharacterCommandPacket GetHpHealPacket(int deltaHp)
		{
			if (this.effectAndSoundCode != 0 && this.stateCode != 0)
			{
				return new CmdHealHpEffectAndStateCode
				{
					addHp = deltaHp,
					stateCode = this.stateCode,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.EffectAndSoundCode != 0)
			{
				return new CmdHealHpEffectCode
				{
					addHp = deltaHp,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.StateCode != 0)
			{
				return new CmdHealHpStateCode
				{
					addHp = deltaHp,
					stateCode = this.StateCode,
					showUI = this.ShowUI
				};
			}
			return new CmdHealHp
			{
				addHp = deltaHp,
				showUI = this.ShowUI
			};
		}

		
		public LocalCharacterCommandPacket GetSpHealPacket(int deltaSp)
		{
			if (this.effectAndSoundCode != 0 && this.stateCode != 0)
			{
				return new CmdHealSpEffectAndStateCode
				{
					addSp = deltaSp,
					stateCode = this.stateCode,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.EffectAndSoundCode != 0)
			{
				return new CmdHealSpEffectCode
				{
					addSp = deltaSp,
					effectCode = this.EffectAndSoundCode,
					showUI = this.ShowUI
				};
			}
			if (this.StateCode != 0)
			{
				return new CmdHealSpStateCode
				{
					addSp = deltaSp,
					stateCode = this.StateCode,
					showUI = this.ShowUI
				};
			}
			return new CmdHealSp
			{
				addSp = deltaSp,
				showUI = this.ShowUI
			};
		}

		
		private int hp;

		
		private int sp;

		
		private WorldCharacter healer;

		
		private int effectAndSoundCode;

		
		private int stateCode;

		
		private bool showUI = true;

		
		private bool needApplyHealRatio = true;
	}
}
