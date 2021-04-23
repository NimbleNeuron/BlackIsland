using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class IvyPreset : ScriptableObject
	{
		public string presetName;


		public IvyParameters ivyParameters;


		public void CopyFrom(IvyParametersGUI copyFrom)
		{
			ivyParameters.CopyFrom(copyFrom);
		}
	}
}