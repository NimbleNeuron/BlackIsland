#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using Common;
	using Storage;
	using ObscuredTypes;

	using System;
	using System.Diagnostics;
	using System.Text;
	using UnityEngine;
	using Debug = UnityEngine.Debug;

	/// <summary>
	/// These super simple and stupid tests allow you to see how slower Obscured types can be compared to the regular types.
	/// Take in account iterations count though.
	/// </summary>
	[AddComponentMenu("")]
	internal class ObscuredPerformanceTests : MonoBehaviour
	{
		public bool boolTest = true;
		public int boolIterations = 2500000;

		public bool byteTest = true;
		public int byteIterations = 2500000;

		public bool shortTest = true;
		public int shortIterations = 2500000;

		public bool ushortTest = true;
		public int ushortIterations = 2500000;

		public bool intTest = true;
		public int intIterations = 2500000;

		public bool uintTest = true;
		public int uintIterations = 2500000;

		public bool longTest = true;
		public int longIterations = 2500000;

		public bool floatTest = true;
		public int floatIterations = 2500000;

		public bool doubleTest = true;
		public int doubleIterations = 2500000;

		public bool stringTest = true;
		public int stringIterations = 250000;

		public bool vector3Test = true;
		public int vector3Iterations = 2500000;

		public bool prefsTest = true;
		public int prefsIterations = 2500;

		private readonly StringBuilder logBuilder = new StringBuilder();

		private void Start()
		{
			Invoke("StartTests", 1f);
		}

		private void StartTests()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine(ACTkConstants.LogPrefix + "<b>[ Performance tests ]</b>");

			if (boolTest) TestBool();
			if (byteTest) TestByte();
			if (shortTest) TestShort();
			if (ushortTest) TestUShort();
			if (intTest) TestInt();
			if (uintTest) TestUInt();
			if (longTest) TestLong();
			if (floatTest) TestFloat();
			if (doubleTest) TestDouble();
			if (stringTest) TestString();
			if (vector3Test) TestVector3();
			if (prefsTest) TestPrefs();

			Debug.Log(logBuilder);
		}

		private void TestBool()
		{
			logBuilder.AppendLine("ObscuredBool vs bool, " + boolIterations + " iterations for read and write");

			ObscuredBool obscured = true;
			bool notObscured = obscured;
			var dummy = false;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < boolIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < boolIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredBool:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < boolIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < boolIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("bool:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy) {}
			if (obscured) {}
			if (notObscured) {}
		}

		private void TestByte()
		{
			logBuilder.AppendLine("ObscuredByte vs byte, " + byteIterations + " iterations for read and write");

			ObscuredByte obscured = 100;
			byte notObscured = obscured;
			byte dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < byteIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < byteIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredByte:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < byteIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < byteIterations; i++)
			{
				notObscured = dummy;
			}

			sw.Stop();
			logBuilder.AppendLine("byte:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestShort()
		{
			logBuilder.AppendLine("ObscuredShort vs short, " + shortIterations + " iterations for read and write");

			ObscuredShort obscured = 100;
			short notObscured = obscured;
			short dummy = 0;

			var sw = Stopwatch.StartNew();


			for (var i = 0; i < shortIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < shortIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
            logBuilder.AppendLine("ObscuredShort:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < shortIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < shortIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("short:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestUShort()
		{
			logBuilder.AppendLine("ObscuredUShort vs ushort, " + ushortIterations + " iterations for read and write");

			ObscuredUShort obscured = 100;
			ushort notObscured = obscured;
			ushort dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < ushortIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < ushortIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredUShort:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < ushortIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < ushortIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ushort:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestDouble()
		{
			logBuilder.AppendLine("ObscuredDouble vs double, " + doubleIterations + " iterations for read and write");

			ObscuredDouble obscured = 100d;
			double notObscured = obscured;
			double dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < doubleIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < doubleIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredDouble:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < doubleIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < doubleIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("double:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (Math.Abs(dummy) > 0.00001d) {}
			if (Math.Abs(obscured) > 0.00001d) {}
			if (Math.Abs(notObscured) > 0.00001d) {}
		}

		private void TestFloat()
		{
			logBuilder.AppendLine("ObscuredFloat vs float, " + floatIterations + " iterations for read and write");

			ObscuredFloat obscured = 100f;
			float notObscured = obscured;
			float dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < floatIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < floatIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredFloat:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < floatIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < floatIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("float:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (Math.Abs(dummy) > 0.00001f) {}
			if (Math.Abs(obscured) > 0.00001f) {}
			if (Math.Abs(notObscured) > 0.00001f) {}
		}

		private void TestInt()
		{
			logBuilder.AppendLine("ObscuredInt vs int, " + intIterations + " iterations for read and write");

			ObscuredInt obscured = 100;
			int notObscured = obscured;
			var dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < intIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < intIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredInt:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < intIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < intIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("int:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestLong()
		{
			logBuilder.AppendLine("ObscuredLong vs long, " + longIterations + " iterations for read and write");

			ObscuredLong obscured = 100L;
			long notObscured = obscured;
			long dummy = 0;

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < longIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < longIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredLong:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < longIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < longIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("long:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestString()
		{
			logBuilder.AppendLine("ObscuredString vs string, " + stringIterations + " iterations for read and write");

			ObscuredString obscured = "abcd";
			string notObscured = obscured;
			var dummy = "";

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < stringIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < stringIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredString:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < stringIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < stringIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("string:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != "") {}
			if (obscured != "") {}
			if (notObscured != "") {}
		}

		private void TestUInt()
		{
			logBuilder.AppendLine("ObscuredUInt vs uint, " + uintIterations + " iterations for read and write");

			ObscuredUInt obscured = 100u;
			uint notObscured = obscured;
			uint dummy = 0;

			var sw = Stopwatch.StartNew();
			for (var i = 0; i < uintIterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < uintIterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredUInt:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < uintIterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < uintIterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("uint:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != 0) {}
			if (obscured != 0) {}
			if (notObscured != 0) {}
		}

		private void TestVector3()
		{
			logBuilder.AppendLine("ObscuredVector3 vs Vector3, " + vector3Iterations + " iterations for read and write");

			ObscuredVector3 obscured = new Vector3(1f, 2f, 3f);
			Vector3 notObscured = obscured;
			var dummy = new Vector3(0, 0, 0);

			var sw = Stopwatch.StartNew();
			for (var i = 0; i < vector3Iterations; i++)
			{
				dummy = obscured;
			}

			for (var i = 0; i < vector3Iterations; i++)
			{
				obscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredVector3:").AppendLine(sw.ElapsedMilliseconds + " ms");

			sw.Reset();
			sw.Start();
			for (var i = 0; i < vector3Iterations; i++)
			{
				dummy = notObscured;
			}

			for (var i = 0; i < vector3Iterations; i++)
			{
				notObscured = dummy;
			}
			sw.Stop();
			logBuilder.AppendLine("Vector3:").AppendLine(sw.ElapsedMilliseconds + " ms");

			if (dummy != Vector3.zero) {}
			if (obscured != Vector3.zero) {}
			if (notObscured != Vector3.zero) {}
		}

		private void TestPrefs()
		{
			logBuilder.AppendLine("ObscuredPrefs vs PlayerPrefs, " + prefsIterations + " iterations for read and write");

			var sw = Stopwatch.StartNew();

			for (var i = 0; i < prefsIterations; i++)
			{
				ObscuredPrefs.SetInt("__a", 1);
				ObscuredPrefs.SetFloat("__b", 2f);
				ObscuredPrefs.SetString("__c", "3");
			}

			for (var i = 0; i < prefsIterations; i++)
			{
				ObscuredPrefs.GetInt("__a", 1);
				ObscuredPrefs.GetFloat("__b", 2f);
				ObscuredPrefs.GetString("__c", "3");
			}
			sw.Stop();
			logBuilder.AppendLine("ObscuredPrefs:").AppendLine(sw.ElapsedMilliseconds + " ms");

			ObscuredPrefs.DeleteKey("__a");
			ObscuredPrefs.DeleteKey("__b");
			ObscuredPrefs.DeleteKey("__c");

			ObscuredPrefs.DeleteCryptoKey();

			sw.Reset();
			sw.Start();
			for (var i = 0; i < prefsIterations; i++)
			{
				PlayerPrefs.SetInt("__a", 1);
				PlayerPrefs.SetFloat("__b", 2f);
				PlayerPrefs.SetString("__c", "3");
			}

			for (var i = 0; i < prefsIterations; i++)
			{
				PlayerPrefs.GetInt("__a", 1);
				PlayerPrefs.GetFloat("__b", 2f);
				PlayerPrefs.GetString("__c", "3");
			}
			sw.Stop();
			logBuilder.AppendLine("PlayerPrefs:").AppendLine(sw.ElapsedMilliseconds + " ms");

			PlayerPrefs.DeleteKey("__a");
			PlayerPrefs.DeleteKey("__b");
			PlayerPrefs.DeleteKey("__c");
		}
	}
}