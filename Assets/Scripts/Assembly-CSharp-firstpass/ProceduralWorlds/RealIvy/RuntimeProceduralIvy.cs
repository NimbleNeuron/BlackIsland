using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class RuntimeProceduralIvy : RTIvy
	{
		private RuntimeIvyGrowth rtIvyGrowth;


		protected override void Init(IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			base.Init(ivyContainer, ivyParameters);
			rtIvyGrowth = new RuntimeIvyGrowth();
			rtIvyGrowth.Init(rtIvyContainer, ivyParameters, gameObject, leavesMeshesByChosenLeaf, GetMaxNumPoints(),
				GetMaxNumLeaves(), GetMaxNumVerticesPerLeaf());
			for (int i = 0; i < 10; i++)
			{
				rtIvyGrowth.Step();
			}

			currentLifetime = growthParameters.lifetime;
		}


		protected override void NextPoints(int branchIndex)
		{
			base.NextPoints(branchIndex);
			rtIvyGrowth.Step();
		}


		public override bool IsGrowingFinished()
		{
			return currentTimer > currentLifetime;
		}


		protected override float GetNormalizedLifeTime()
		{
			return Mathf.Clamp(currentTimer / growthParameters.lifetime, 0.1f, 1f);
		}


		public void SetIvyParameters(IvyPreset ivyPreset)
		{
			ivyParameters.CopyFrom(ivyPreset);
		}


		protected override void InitializeMeshesData(Mesh bakedMesh, int numBranches)
		{
			meshBuilder.InitializeMeshesDataProcedural(bakedMesh, numBranches, growthParameters.lifetime,
				growthParameters.growthSpeed);
		}


		protected override int GetMaxNumPoints()
		{
			float num = ivyParameters.stepSize / growthParameters.growthSpeed;
			Mathf.CeilToInt(growthParameters.lifetime / num);
			int maxBranchs = ivyParameters.maxBranchs;
			return 20;
		}


		protected override int GetMaxNumLeaves()
		{
			return GetMaxNumPoints();
		}


		public override void InitIvy(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer,
			IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			Init(null, ivyParameters);
			InitMeshBuilder();
			AddFirstBranch();
		}


		private int GetMaxNumVerticesPerLeaf()
		{
			int num = 0;
			for (int i = 0; i < ivyParameters.leavesPrefabs.Length; i++)
			{
				if (num <= leavesMeshesByChosenLeaf[i].vertices.Length)
				{
					num = leavesMeshesByChosenLeaf[i].vertices.Length;
				}
			}

			return num;
		}
	}
}