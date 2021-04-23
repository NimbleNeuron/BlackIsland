using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	
	public class EffectAndSoundDB
	{
		
		public EffectAndSoundDB()
		{
			this.CreateOpenBoxSoundData();
		}

		
		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(EffectAndSoundData))
			{
				this.effectAndSoundDataList = data.Cast<EffectAndSoundData>().ToList<EffectAndSoundData>();
				return;
			}
			if (typeFromHandle == typeof(NoiseData))
			{
				this.noiseDataList = data.Cast<NoiseData>().ToList<NoiseData>();
				return;
			}
			if (typeFromHandle == typeof(SecurityConsoleEventData))
			{
				this.securityConsoleEventDataList = data.Cast<SecurityConsoleEventData>().ToList<SecurityConsoleEventData>();
			}
		}

		
		public EffectAndSoundData GetEffectSoundData(int code)
		{
			List<EffectAndSoundData> list = this.effectAndSoundDataList;
			if (list == null)
			{
				return null;
			}
			return list.Find((EffectAndSoundData x) => x.code == code);
		}

		
		public NoiseData GetNoiseData(NoiseType noiseType)
		{
			return this.noiseDataList.Find((NoiseData x) => x.noiseType == noiseType);
		}

		
		public SecurityConsoleEventData GetSecurityConsoleEventData(SecurityConsoleEvent eventType)
		{
			return this.securityConsoleEventDataList.Find((SecurityConsoleEventData x) => x.securityConsoleEvent == eventType);
		}

		
		public string GetOpenBoxSoundName(string boxName)
		{
			if (!this.openBoxSounds.ContainsKey(boxName))
			{
				return "";
			}
			List<string> list = this.openBoxSounds[boxName];
			int index = 0;
			if (1 < list.Count)
			{
				index = UnityEngine.Random.Range(0, list.Count);
			}
			return list[index];
		}

		
		private void CreateOpenBoxSoundData()
		{
			this.openBoxSounds = new Dictionary<string, List<string>>();
			this.openBoxSounds.Add("ItemBox_AltarTable", new List<string>
			{
				"OpenSound_AltarTable_01"
			});
			this.openBoxSounds.Add("ItemBox_ATM_01", new List<string>
			{
				"OpenSound_ATM_01"
			});
			this.openBoxSounds.Add("ItemBox_Bag_01", new List<string>
			{
				"OpenSound_Bag_01"
			});
			this.openBoxSounds.Add("ItemBox_Bag_02", new List<string>
			{
				"OpenSound_Bag_01"
			});
			this.openBoxSounds.Add("ItemBox_BarbequeGrill_01", new List<string>
			{
				"OpenSound_BarbequeGrill_01"
			});
			this.openBoxSounds.Add("ItemBox_Biotoilet_01", new List<string>
			{
				"OpenSound_Biotoilet_01"
			});
			this.openBoxSounds.Add("ItemBox_Boat_01", new List<string>
			{
				"OpenSound_Boat_01"
			});
			this.openBoxSounds.Add("ItemBox_Box_01", new List<string>
			{
				"OpenSound_Box_01"
			});
			this.openBoxSounds.Add("ItemBox_Box_02", new List<string>
			{
				"OpenSound_Box_02"
			});
			this.openBoxSounds.Add("ItemBox_CartonBox_01", new List<string>
			{
				"OpenSound_CartonBox_01"
			});
			this.openBoxSounds.Add("ItemBox_CoffeeMachine_01", new List<string>
			{
				"OpenSound_CoffeeMachine_01"
			});
			this.openBoxSounds.Add("ItemBox_Coffin_Large", new List<string>
			{
				"OpenSound_Coffin_01"
			});
			this.openBoxSounds.Add("ItemBox_Coffin_Middle", new List<string>
			{
				"OpenSound_Coffin_01"
			});
			this.openBoxSounds.Add("ItemBox_Coffin_Small", new List<string>
			{
				"OpenSound_Coffin_01"
			});
			this.openBoxSounds.Add("ItemBox_ConcreteBag_Set_01", new List<string>
			{
				"OpenSound_ConcreteBag_Set_01"
			});
			this.openBoxSounds.Add("ItemBox_Confessional_01", new List<string>
			{
				"OpenSound_Confessional_01"
			});
			this.openBoxSounds.Add("ItemBox_Drum_R", new List<string>
			{
				"OpenSound_Drum_01",
				"OpenSound_Drum_02",
				"OpenSound_Drum_03"
			});
			this.openBoxSounds.Add("ItemBox_Drum_W", new List<string>
			{
				"OpenSound_Drum_01",
				"OpenSound_Drum_02",
				"OpenSound_Drum_03"
			});
			this.openBoxSounds.Add("ItemBox_Dumpster_01", new List<string>
			{
				"OpenSound_Dumpster_01"
			});
			this.openBoxSounds.Add("ItemBox_EraserCleaner_01", new List<string>
			{
				"OpenSound_EraserCleaner_01"
			});
			this.openBoxSounds.Add("ItemBox_FilingCabinets_01", new List<string>
			{
				"OpenSound_FilingCabinets_01"
			});
			this.openBoxSounds.Add("ItemBox_FilingCabinets_02", new List<string>
			{
				"OpenSound_FilingCabinets_01"
			});
			this.openBoxSounds.Add("ItemBox_GarbageBag_Set", new List<string>
			{
				"OpenSound_GarbageBag_Set_01"
			});
			this.openBoxSounds.Add("ItemBox_Hospital_Cabinet_01", new List<string>
			{
				"OpenSound_Hospital_Cabinet_01"
			});
			this.openBoxSounds.Add("ItemBox_Hospital_Cart_01", new List<string>
			{
				"OpenSound_Hospital_Cart_01"
			});
			this.openBoxSounds.Add("ItemBox_IceBox_B", new List<string>
			{
				"OpenSound_IceBox_01"
			});
			this.openBoxSounds.Add("ItemBox_IceBox_R", new List<string>
			{
				"OpenSound_IceBox_01"
			});
			this.openBoxSounds.Add("ItemBox_Jar_01", new List<string>
			{
				"OpenSound_Jar_01"
			});
			this.openBoxSounds.Add("ItemBox_Jar_Big_01", new List<string>
			{
				"OpenSound_Jar_Big_01"
			});
			this.openBoxSounds.Add("ItemBox_Locker_01", new List<string>
			{
				"OpenSound_Locker_01"
			});
			this.openBoxSounds.Add("ItemBox_Organ_01", new List<string>
			{
				"OpenSound_Organ_01"
			});
			this.openBoxSounds.Add("ItemBox_Pallet_Loaded_01", new List<string>
			{
				"OpenSound_Pallet_Loaded_01",
				"OpenSound_Pallet_Loaded_02",
				"OpenSound_Pallet_Loaded_03"
			});
			this.openBoxSounds.Add("ItemBox_Sedan_Brown_01", new List<string>
			{
				"OpenSound_Sedan_Brown_01"
			});
			this.openBoxSounds.Add("ItemBox_Sedan_Police_01", new List<string>
			{
				"OpenSound_Sedan_Police_01"
			});
			this.openBoxSounds.Add("ItemBox_Sedan_Taxi_01", new List<string>
			{
				"OpenSound_Sedan_Taxi_01"
			});
			this.openBoxSounds.Add("ItemBox_SteelBox_02", new List<string>
			{
				"OpenSound_SteelBox_01",
				"OpenSound_SteelBox_02",
				"OpenSound_SteelBox_05"
			});
			this.openBoxSounds.Add("ItemBox_SteelBox_03", new List<string>
			{
				"OpenSound_SteelBox_03",
				"OpenSound_SteelBox_04"
			});
			this.openBoxSounds.Add("ItemBox_Suitcase_01", new List<string>
			{
				"OpenSound_Suitcase_01"
			});
			this.openBoxSounds.Add("ItemBox_Suitcase_02", new List<string>
			{
				"OpenSound_Suitcase_01"
			});
			this.openBoxSounds.Add("ItemBox_Suitcase_03", new List<string>
			{
				"OpenSound_Suitcase_01"
			});
			this.openBoxSounds.Add("ItemBox_Suitcase_04", new List<string>
			{
				"OpenSound_Suitcase_01"
			});
			this.openBoxSounds.Add("ItemBox_Switchboard_01", new List<string>
			{
				"OpenSound_Switchboard_01"
			});
			this.openBoxSounds.Add("ItemBox_Temple_Box_01", new List<string>
			{
				"OpenSound_Temple_Box_01"
			});
			this.openBoxSounds.Add("ItemBox_Tomb", new List<string>
			{
				"OpenSound_Tomb_01"
			});
			this.openBoxSounds.Add("ItemBox_TrashCan_01", new List<string>
			{
				"OpenSound_TrashCan_01"
			});
			this.openBoxSounds.Add("ItemBox_TrashCan_02", new List<string>
			{
				"OpenSound_TrashCan_01"
			});
			this.openBoxSounds.Add("ItemBox_TrashCan_03", new List<string>
			{
				"OpenSound_TrashCan2_01"
			});
			this.openBoxSounds.Add("ItemBox_TreeofLife", new List<string>
			{
				"OpenSound_TreeofLife_01"
			});
			this.openBoxSounds.Add("ItemBox_TreeStump_01", new List<string>
			{
				"OpenSound_TreeStump_01",
				"OpenSound_TreeStump_03",
				"OpenSound_TreeStump_04"
			});
			this.openBoxSounds.Add("ItemBox_TreeStump_02", new List<string>
			{
				"OpenSound_TreeStump_02",
				"OpenSound_TreeStump_05"
			});
			this.openBoxSounds.Add("ItemBox_Uptown_BookCase_01", new List<string>
			{
				"OpenSound_Uptown_BookCase_01"
			});
			this.openBoxSounds.Add("ItemBox_Uptown_Drawer_01", new List<string>
			{
				"OpenSound_Uptown_Drawer_01"
			});
			this.openBoxSounds.Add("ItemBox_Uptown_FirePlace_01", new List<string>
			{
				"OpenSound_Uptown_FirePlace_01"
			});
			this.openBoxSounds.Add("ItemBox_VendingMachine_01", new List<string>
			{
				"OpenSound_VendingMachine_01"
			});
			this.openBoxSounds.Add("ItemBox_VendingMachine_02", new List<string>
			{
				"OpenSound_VendingMachine_01"
			});
			this.openBoxSounds.Add("ItemBox_WaterTank_01", new List<string>
			{
				"OpenSound_WaterTank_01"
			});
			this.openBoxSounds.Add("ItemBox_Wheelbarrow", new List<string>
			{
				"OpenSound_Wheelbarrow_01"
			});
			this.openBoxSounds.Add("ItemBox_WreckCar_Blue_01", new List<string>
			{
				"OpenSound_WreckCar_01",
				"OpenSound_WreckCar_02"
			});
			this.openBoxSounds.Add("ItemBox_WreckCar_White_01", new List<string>
			{
				"OpenSound_WreckCar_01",
				"OpenSound_WreckCar_02"
			});
			this.openBoxSounds.Add("ItemBox_WreckCar_Yellow_01", new List<string>
			{
				"OpenSound_WreckCar_01",
				"OpenSound_WreckCar_02"
			});
			this.openBoxSounds.Add("ItemBox_Quiver_01", new List<string>
			{
				"OpenSound_FilingCabinets_01"
			});
		}

		
		private List<EffectAndSoundData> effectAndSoundDataList;

		
		private List<NoiseData> noiseDataList;

		
		private List<SecurityConsoleEventData> securityConsoleEventDataList;

		
		private Dictionary<string, List<string>> openBoxSounds;
	}
}
