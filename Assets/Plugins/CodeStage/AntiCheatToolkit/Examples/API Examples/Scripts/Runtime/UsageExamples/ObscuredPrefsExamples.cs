#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

// add this line in order to use obscured prefs from code:
using CodeStage.AntiCheat.Storage;

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

	internal class ObscuredPrefsExamples : MonoBehaviour
	{
		private const string PrefsString = "name";
		private const string PrefsInt = "money";
		private const string PrefsFloat = "lifeBar";
		private const string PrefsBool = "gameComplete";
		private const string PrefsUint = "demoUint";
		private const string PrefsLong = "demoLong";
		private const string PrefsDouble = "demoDouble";
		private const string PrefsVector2 = "demoVector2";
		private const string PrefsVector3 = "demoVector3";
		private const string PrefsQuaternion = "demoQuaternion";
		private const string PrefsRect = "demoRect";
		private const string PrefsColor = "demoColor";
		private const string PrefsByteArray = "demoByteArray";

		internal string regularPrefs;
		internal string obscuredPrefs;

		internal bool savesAlterationDetected;
		internal bool foreignSavesDetected;
		
		// you can keep original PlayerPrefs values while
		// migrating to the ObscuredPrefs if you wish to
		internal bool PreservePlayerPrefs
		{
			get { return ObscuredPrefs.preservePlayerPrefs; }
			set { ObscuredPrefs.preservePlayerPrefs = value; }
		}

		// in case you can't read locked to the device saves for
		// some reason (e.g. device ID unexpectedly changed), you 
		// can use emergencyMode to bypass lock to device and recover your saves
		// similar to readForeignSaves but does not fire foreign saves detection event
		internal bool EmergencyMode
		{
			get { return ObscuredPrefs.emergencyMode; }
			set { ObscuredPrefs.emergencyMode = value; }
		}

		// same as emergencyMode but still fires OnPossibleForeignSavesDetected event
		internal bool ReadForeignSaves
		{
			get { return ObscuredPrefs.readForeignSaves; }
			set { ObscuredPrefs.readForeignSaves = value; }
		}

		private void Awake()
		{
			// you can detect saves alteration attempts using this event
			ObscuredPrefs.OnAlterationDetected += SavesAlterationDetected;

			// and even may react on foreign saves (from another device)
			ObscuredPrefs.OnPossibleForeignSavesDetected += ForeignSavesDetected;

			MigrateFromV1();
		}

		private void OnDestroy()
		{
			DeleteRegularPrefs();
			DeleteObscuredPrefs();
		}

		private void SavesAlterationDetected()
		{
			savesAlterationDetected = true;
		}

		private void ForeignSavesDetected()
		{
			foreignSavesDetected = true;
		}

		internal void LoadRegularPrefs()
		{
			regularPrefs = "int: " + PlayerPrefs.GetInt(PrefsInt, -1) + "\n";
			regularPrefs += "float: " + PlayerPrefs.GetFloat(PrefsFloat, -1) + "\n";
			regularPrefs += "string: " + PlayerPrefs.GetString(PrefsString, "No saved PlayerPrefs!");
		}

		internal void SaveRegularPrefs()
		{
			PlayerPrefs.SetInt(PrefsInt, 456);
			PlayerPrefs.SetFloat(PrefsFloat, 456.789f);
			PlayerPrefs.SetString(PrefsString, "Hey, there!");
			PlayerPrefs.Save();
		}

		internal void DeleteRegularPrefs()
		{
			PlayerPrefs.DeleteKey(PrefsInt);
			PlayerPrefs.DeleteKey(PrefsFloat);
			PlayerPrefs.DeleteKey(PrefsString);
			PlayerPrefs.Save();
		}

		internal void LockObscuredPrefsToDevice(ObscuredPrefs.DeviceLockLevel level)
		{
			// you can lock saves to the device (so they can't be read on another device)
			// there are few different levels of strictness, please see API docs for
			// lockToDevice property for additional details
			// set to None be default (does not lock to device)
			ObscuredPrefs.lockToDevice = level;
		}

		internal void LoadObscuredPrefs()
		{
			// you can store typical int, float and string at the ObscuredPrefs
			obscuredPrefs = "int: " + ObscuredPrefs.GetInt(PrefsInt, -1) + "\n";
			obscuredPrefs += "float: " + ObscuredPrefs.GetFloat(PrefsFloat, -1) + "\n";
			obscuredPrefs += "string: " + ObscuredPrefs.GetString(PrefsString, "No saved ObscuredPrefs!") + "\n";

			// comparing to the vanilla PlayerPrefs, you have much more freedom on what to save
			obscuredPrefs += "bool: " + ObscuredPrefs.GetBool(PrefsBool, false) + "\n";
			obscuredPrefs += "uint: " + ObscuredPrefs.GetUInt(PrefsUint, 0) + "\n";
			obscuredPrefs += "long: " + ObscuredPrefs.GetLong(PrefsLong, -1) + "\n";
			obscuredPrefs += "double: " + ObscuredPrefs.GetDouble(PrefsDouble, -1) + "\n";
			obscuredPrefs += "Vector2: " + ObscuredPrefs.GetVector2(PrefsVector2, Vector2.zero) + "\n";
			obscuredPrefs += "Vector3: " + ObscuredPrefs.GetVector3(PrefsVector3, Vector3.zero) + "\n";
			obscuredPrefs += "Quaternion: " + ObscuredPrefs.GetQuaternion(PrefsQuaternion, Quaternion.identity) + "\n";
			obscuredPrefs += "Rect: " + ObscuredPrefs.GetRect(PrefsRect, new Rect(0, 0, 0, 0)) + "\n";
			obscuredPrefs += "Color: " + ObscuredPrefs.GetColor(PrefsColor, Color.black) + "\n";

			// you can even store raw byte array with any data inside
			var ba = ObscuredPrefs.GetByteArray(PrefsByteArray, 0, 4);
			obscuredPrefs += "byte[]: {" + ba[0] + "," + ba[1] + "," + ba[2] + "," + ba[3] + "}";
		}

		internal void SaveObscuredPrefs()
		{
			// same types as at the regular PlayerPrefs
			ObscuredPrefs.SetInt(PrefsInt, 123);
			ObscuredPrefs.SetFloat(PrefsFloat, 123.456f);
			ObscuredPrefs.SetString(PrefsString, "Goscurry is not a lie ;)");

			// additional types
			ObscuredPrefs.SetBool(PrefsBool, true);
			ObscuredPrefs.SetUInt(PrefsUint, 1234567891u);
			ObscuredPrefs.SetLong(PrefsLong, 1234567891234567890L);
			ObscuredPrefs.SetDouble(PrefsDouble, 1.234567890123456d);
			ObscuredPrefs.SetVector2(PrefsVector2, Vector2.one);
			ObscuredPrefs.SetVector3(PrefsVector3, Vector3.one);
			ObscuredPrefs.SetQuaternion(PrefsQuaternion, Quaternion.Euler(new Vector3(10, 20, 30)));
			ObscuredPrefs.SetRect(PrefsRect, new Rect(1.5f, 2.6f, 3.7f, 4.8f));
			ObscuredPrefs.SetColor(PrefsColor, Color.red);
			ObscuredPrefs.SetByteArray(PrefsByteArray, new byte[] { 44, 104, 43, 32 });

			ObscuredPrefs.Save();
		}

		internal void DeleteObscuredPrefs()
		{
			ObscuredPrefs.DeleteKey(PrefsInt);
			ObscuredPrefs.DeleteKey(PrefsFloat);
			ObscuredPrefs.DeleteKey(PrefsString);
			ObscuredPrefs.DeleteKey(PrefsBool);
			ObscuredPrefs.DeleteKey(PrefsUint);
			ObscuredPrefs.DeleteKey(PrefsLong);
			ObscuredPrefs.DeleteKey(PrefsDouble);
			ObscuredPrefs.DeleteKey(PrefsVector2);
			ObscuredPrefs.DeleteKey(PrefsVector3);
			ObscuredPrefs.DeleteKey(PrefsQuaternion);
			ObscuredPrefs.DeleteKey(PrefsRect);
			ObscuredPrefs.DeleteKey(PrefsColor);
			ObscuredPrefs.DeleteKey(PrefsByteArray);
			ObscuredPrefs.Save();
		}

		private void MigrateFromV1()
		{
			// here is a migration example for the raw values you got from ACTk v1 ObscuredPrefs.GetRawValue()
			//
			// first you need to encrypt clean prefs key value with the v1 encryption
			// (you can ignore second argument to use default Crypto Key)
			// var v1PrefsKey = ObscuredPrefs.EncryptKeyWithACTkV1Algorithm(prefsKey, cryptoKey);
			//
			// then just set it back as normal (v1RawValue - value you got from v1 GetRawValue())
			// ObscuredPrefs.SetRawValue(v1PrefsKey, v1RawValue);
			// 
			// now you can read it back as usual and it will be automatically migrated to v2 format:
			// var savedData = ObscuredPrefs.GetFloat(prefsKey);
		}
	}
}