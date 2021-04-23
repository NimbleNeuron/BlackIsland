using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class InfoPool : ScriptableObject
	{
		public IvyContainer ivyContainer;


		public EditorMeshBuilder meshBuilder;


		public IvyParameters ivyParameters;


		public EditorIvyGrowth growth;
	}
}