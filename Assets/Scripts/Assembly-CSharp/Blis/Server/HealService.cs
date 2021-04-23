namespace Blis.Server
{
	
	public class HealService : Singleton<HealService>
	{
		
		public void HealTo(WorldCharacter self, WorldCharacter healer, HealCalculator calculator, bool showUI, int effectAndSoundCode)
		{
			HealInfo healInfo = calculator.Calculate(self, healer);
			healInfo.SetHealer(healer);
			healInfo.SetEffectAndSoundCode(effectAndSoundCode);
			healInfo.SetShowUI(showUI);
			self.Heal(healInfo);
		}

		
		public void LostHpHealTo(WorldCharacter self, WorldCharacter healer, LostHpHealCalculator calculator, bool showUI, int effectAndSoundCode)
		{
			HealInfo healInfo = calculator.Calculate(self, healer);
			healInfo.SetHealer(healer);
			healInfo.SetEffectAndSoundCode(effectAndSoundCode);
			healInfo.SetShowUI(showUI);
			self.Heal(healInfo);
		}

		
		public void EnvironmentHealTo(WorldCharacter self, HealCalculator calculator, bool showUI, int effectAndSoundCode)
		{
			HealInfo healInfo = calculator.Calculate(self, null);
			healInfo.SetEffectAndSoundCode(effectAndSoundCode);
			healInfo.SetShowUI(showUI);
			self.Heal(healInfo);
		}

		
		public void SelfHealTo(WorldCharacter self, HealCalculator calculator, bool showUI, int effectAndSoundCode)
		{
			HealInfo healInfo = calculator.Calculate(self, self);
			healInfo.SetHealer(self);
			healInfo.SetEffectAndSoundCode(effectAndSoundCode);
			healInfo.SetShowUI(showUI);
			self.Heal(healInfo);
		}
	}
}
