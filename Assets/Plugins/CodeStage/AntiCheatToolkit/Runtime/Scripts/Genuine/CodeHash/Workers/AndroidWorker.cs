#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_2018_1_OR_NEWER

#if UNITY_ANDROID && !UNITY_EDITOR
#define ACTK_ANDROID_DEVICE
#endif

#if ACTK_ANDROID_DEVICE

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	using Common;
	using System;
	using UnityEngine;

	internal class AndroidWorker : BaseWorker
	{
		private class CodeHashGeneratorCallback : AndroidJavaProxy
		{
			private readonly AndroidWorker parent;

			public CodeHashGeneratorCallback(AndroidWorker parent) : base("net.codestage.actk.androidnative.CodeHashCallback")
			{
				this.parent = parent;
			}

			// called from native Android plugin, from separate thread
			public void OnSuccess(string hash)
			{
				parent.Complete(HashGeneratorResult.FromCodeHash(hash));
			}

			// called from native Android plugin, from separate thread
			public void OnError(string errorMessage)
			{
				parent.Complete(HashGeneratorResult.FromError(errorMessage));
			}
		}

		public override void Execute()
		{
			base.Execute();

		    const string classPath = "net.codestage.actk.androidnative.CodeHashGenerator";

		    try
		    {
			    using (var nativeClass = new AndroidJavaClass(classPath))
			    {
#if ENABLE_IL2CPP
					var il2cpp = true;
#else
				    var il2cpp = false;
#endif

				    var filters = CodeHashGenerator.GetFileFiltersAndroid(il2cpp);

				    /*
				    var jAryPtr = AndroidJNIHelper.ConvertToJNIArray(GenerateStringArrayFromFilters(filters));

					Debug.LogWarning("jAryPtr: " + jAryPtr);

					var blah = new jvalue[1];
					blah[0].l = jAryPtr;	
					var methodId = AndroidJNIHelper.GetMethodID(nativeClass.GetRawClass(), "GetCodeHash");

					Debug.LogWarning("methodId: " + methodId);

					AndroidJNI.CallStaticVoidMethod(nativeClass.GetRawClass(), methodId, blah);*/

				   nativeClass.CallStatic("GetCodeHash", GenerateStringArrayFromFilters(filters), new CodeHashGeneratorCallback(this));
			    }
		    }
		    catch (Exception e)
		    {
			    Debug.LogError(ACTkConstants.LogPrefix + "Can't initialize NativeRoutines!\n" + e);
				return;
		    }
		}

		private string[] GenerateStringArrayFromFilters(FileFilter[] filters)
		{
			var itemsCount = filters.Length;
			var result = new string[itemsCount];
			for (var i = 0; i < itemsCount; i++)
			{
				result[i] = filters[i].ToString();
			}

			return result;
		}
	}
}

#endif
#endif