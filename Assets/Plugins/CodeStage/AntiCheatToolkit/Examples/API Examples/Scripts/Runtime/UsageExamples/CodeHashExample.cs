#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Examples
{
	using UnityEngine;

#if UNITY_2018_1_OR_NEWER
	using Genuine.CodeHash;
#else
	using Common;
#endif
	
	internal class CodeHashExample : MonoBehaviour
	{
#if UNITY_2018_1_OR_NEWER
#pragma warning disable 0649
		// fill this field manually after build with value you see in console
		// (don't forget to enable pre-generation in settings)
		public string savedHash;
#pragma warning restore 0649

		public HashGeneratorResult LastResult { get; private set; }

		public bool IsBusy
		{
			get
			{
				return CodeHashGenerator.Instance.IsBusy;
			}
		}

		public bool IsSupported
		{
			get
			{
				return CodeHashGenerator.IsTargetPlatformCompatible(); 
			}
		}

		public bool IsGenuineValueSetInInspector
		{
			get { return !string.IsNullOrEmpty(savedHash); }
		}

		// just to make sure it's added to the scene and Instance will be not empty
		public void Init()
		{
			CodeHashGenerator.AddToSceneOrGetExisting();
		}

		public void StartGeneration()
		{
			// This is a good practice to avoid new requests while generator is busy with previous requests.
			if (CodeHashGenerator.Instance.IsBusy)
			{
				return;
			}

			// Just subscribe to generation event and start generation.
			// Generation runs in separate thread avoiding cpu spikes in main thread.
			// It generates hash only once and cache it for any new requests since compiled code does not change in runtime.
			CodeHashGenerator.HashGenerated += OnHashGenerated;
			CodeHashGenerator.Generate();
		}

		public bool HashMatches()
		{
			if (LastResult == null)
			{
				return false;
			}

			return savedHash == LastResult.CodeHash;
		}

		private void OnHashGenerated(HashGeneratorResult result)
		{
			LastResult = result;
			CodeHashGenerator.HashGenerated -= OnHashGenerated;

			if (result.Success)
			{
				// Here you can upload your hash to the server to make a validation check on the server side and punish cheater with server logic.
				// 
				// This is a preferred use case since cheater will have to figure out proper hash using
				// packets sniffing (https packets harder to sniff) or debugging first to fake it on the client side requiring requires more
				// skills and motivation from cheater.
				// 
				// UploadHash(result.CodeHash);

				// Or you may compare it with hardcoded hash if you did save it somewhere in the build previously.
				//
				// This is less preferred way since cheater can still try to hack your client-side validation check to make it always pass.
				// Anyways, this is better than nothing and will require some additional time from cheater reducing overall motivation to hack your game.
				// In case implementing it fully on the client side,
				// make sure to compile IL2CPP build and use code
				// obfuscation which runs integrated into the Unity build process so hashing will happen AFTER code obfuscation.
				// If obfuscation will happen after hashing it will change code hash and you'll need to re-calculate it
				// using Tools > Code Stage > Anti-Cheat Toolkit > Calculate external build hash feature.
				//
				// if (!CompareHash(result.CodeHash))
				// {
				//		Debug.Log("You patched my code, cheater!");
				// }
			}
			else
			{
				Debug.LogError("Oh, something went wrong while getting the hash!\n" +
				               "Error message: " + result.ErrorMessage);
			}
		}
#else
		public bool IsSupported
		{
			get { return false; }
		}

		public void StartGeneration()
		{
			Debug.Log(ACTkConstants.LogPrefix + "Unity 2018.1 required for CodeHashGenerator to work properly!");
		}
#endif
	}
}