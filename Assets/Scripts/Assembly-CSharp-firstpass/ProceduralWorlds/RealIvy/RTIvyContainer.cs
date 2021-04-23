using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class RTIvyContainer
	{
		public int lastBranchNumberAssigned;


		public Vector3 firstVertexVector;


		public List<RTBranchContainer> branches;


		public void Initialize(Vector3 firstVertexVector)
		{
			lastBranchNumberAssigned = 0;
			this.firstVertexVector = firstVertexVector;
			branches = new List<RTBranchContainer>();
		}


		public void Initialize(IvyContainer ivyContainer, IvyParameters ivyParameters, GameObject ivyGO,
			RTMeshData[] leavesMeshesByChosenLeaf, Vector3 firstVertexVector)
		{
			lastBranchNumberAssigned = 0;
			branches = new List<RTBranchContainer>(ivyContainer.branches.Count);
			for (int i = 0; i < ivyContainer.branches.Count; i++)
			{
				RTBranchContainer item = new RTBranchContainer(ivyContainer.branches[i], ivyParameters, this, ivyGO,
					leavesMeshesByChosenLeaf);
				branches.Add(item);
			}

			this.firstVertexVector = firstVertexVector;
		}


		public void Initialize()
		{
			branches = new List<RTBranchContainer>();
		}


		public void AddBranch(RTBranchContainer rtBranch)
		{
			rtBranch.branchNumber = lastBranchNumberAssigned;
			branches.Add(rtBranch);
			lastBranchNumberAssigned++;
		}


		public RTBranchContainer GetBranchContainerByBranchNumber(int newBranchNumber)
		{
			RTBranchContainer result = null;
			for (int i = 0; i < branches.Count; i++)
			{
				if (branches[i].branchNumber == newBranchNumber)
				{
					result = branches[i];
					break;
				}
			}

			return result;
		}
	}
}