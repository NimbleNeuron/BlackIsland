#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

#if UNITY_2018_1_OR_NEWER

namespace CodeStage.AntiCheat.Genuine.CodeHash
{

	/// <summary>
	/// Result produced by CodeHashGenerator. Contains resulting code hash or errors information.
	/// </summary>
	public class HashGeneratorResult
	{
		/// <summary>
		/// Resulting hash calculated for the currently executed build runtime code. May be null in case #Success is not true.
		/// </summary>
		public string CodeHash { get; private set; }

		/// <summary>
		/// Error message you could find useful in case #Success is not true.
		/// </summary>
		public string ErrorMessage { get; private set; }

		/// <summary>
		/// True if generation was successful and resulting hash is stored in #CodeHash, otherwise check #ErrorMessage to find out error cause.
		/// </summary>
		public bool Success
		{
			get { return ErrorMessage == null; }
		}

		internal static HashGeneratorResult FromError(string errorMessage)
		{
			return new HashGeneratorResult()
			{
				ErrorMessage = errorMessage
			};
		}

		internal static HashGeneratorResult FromCodeHash(string codeHash)
		{
			return new HashGeneratorResult()
			{
				CodeHash = codeHash
			};
		}
	}
}

#endif