using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class RuntimeBakedIvy : RTIvy
	{
		public override bool IsGrowingFinished()
		{
			bool flag = true;
			if (rtIvyContainer.branches.Count > rtBuildingIvyContainer.branches.Count)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < activeBakedBranches.Count; i++)
				{
					flag = flag && rtBuildingIvyContainer.branches[i].branchPoints.Count >=
						activeBakedBranches[i].branchPoints.Count;
				}
			}

			return flag;
		}


		protected override void Init(IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			base.Init(ivyContainer, ivyParameters);
			CalculateLifetime();
		}


		private void CalculateLifetime()
		{
			float num = 0f;
			for (int i = 0; i < rtIvyContainer.branches.Count; i++)
			{
				num += rtIvyContainer.branches[i].totalLength;
			}

			currentLifetime = num / growthParameters.growthSpeed;
			currentLifetime *= 2f;
		}


		protected override float GetNormalizedLifeTime()
		{
			return Mathf.Clamp(rtBuildingIvyContainer.branches[0].totalLength / rtIvyContainer.branches[0].totalLength,
				0.1f, 1f);
		}


		protected override void InitializeMeshesData(Mesh bakedMesh, int numBranches)
		{
			meshBuilder.InitializeMeshesDataBaked(bakedMesh, numBranches);
		}


		protected override int GetMaxNumPoints()
		{
			return 0;
		}


		protected override int GetMaxNumLeaves()
		{
			return 0;
		}


		public override void InitIvy(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer,
			IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			Init(ivyContainer, ivyParameters);
			InitMeshBuilder();
			AddFirstBranch();
		}


		public void InitIvyEditor(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer,
			IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			Init(ivyContainer, ivyParameters);
		}
	}
}