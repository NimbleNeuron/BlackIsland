using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class EditorIvyGrowth : GrowthBuilder
	{
		public Random.State randomstate;


		public void Initialize(Vector3 firstPoint, Vector3 firstGrabVector)
		{
			Random.InitState(infoPool.ivyParameters.randomSeed);
			BranchContainer branchContainer = CreateInstance<BranchContainer>();
			branchContainer.Init();
			branchContainer.currentHeight = infoPool.ivyParameters.minDistanceToSurface;
			infoPool.ivyContainer.AddBranch(branchContainer);
			infoPool.ivyContainer.branches[0]
				.AddBranchPoint(firstPoint, firstGrabVector, true, branchContainer.branchNumber);
			infoPool.ivyContainer.branches[0].growDirection =
				Quaternion.AngleAxis(Random.value * 360f, infoPool.ivyContainer.ivyGO.transform.up) *
				infoPool.ivyContainer.ivyGO.transform.forward;
			infoPool.ivyContainer.firstVertexVector = infoPool.ivyContainer.branches[0].growDirection;
			infoPool.ivyContainer.branches[0].randomizeHeight = Random.Range(4f, 8f);
			CalculateNewHeight(infoPool.ivyContainer.branches[0]);
			infoPool.ivyContainer.branches[0].branchSense = ChooseBranchSense();
			randomstate = Random.state;
		}


		private void CalculateNewHeight(BranchContainer branch)
		{
			branch.heightVar = (Mathf.Sin(branch.heightParameter * infoPool.ivyParameters.DTSFrequency - 45f) + 1f) /
			                   2f;
			branch.newHeight = Mathf.Lerp(infoPool.ivyParameters.minDistanceToSurface,
				infoPool.ivyParameters.maxDistanceToSurface, branch.heightVar);
			branch.newHeight +=
				(Mathf.Sin(branch.heightParameter * infoPool.ivyParameters.DTSFrequency * branch.randomizeHeight) +
				 1f) / 2f * infoPool.ivyParameters.maxDistanceToSurface / 4f * infoPool.ivyParameters.DTSRandomness;
			branch.newHeight = Mathf.Clamp(branch.newHeight, infoPool.ivyParameters.minDistanceToSurface,
				infoPool.ivyParameters.maxDistanceToSurface);
			branch.deltaHeight = branch.currentHeight - branch.newHeight;
			branch.currentHeight = branch.newHeight;
		}


		private int ChooseBranchSense()
		{
			if (Random.value < 0.5f)
			{
				return -1;
			}

			return 1;
		}


		public void Step()
		{
			Random.state = randomstate;
			for (int i = 0; i < infoPool.ivyContainer.branches.Count; i++)
			{
				infoPool.ivyContainer.branches[i].heightParameter += infoPool.ivyParameters.stepSize;
				CalculateNewPoint(infoPool.ivyContainer.branches[i]);
			}

			randomstate = Random.state;
		}


		private void CalculateNewPoint(BranchContainer branch)
		{
			if (!branch.falling)
			{
				CalculateNewHeight(branch);
				CheckWall(branch);
				return;
			}

			CheckFall(branch);
		}


		private void CheckWall(BranchContainer branch)
		{
			BranchPoint branchPoint =
				new BranchPoint(
					branch.GetLastBranchPoint().point + branch.growDirection * infoPool.ivyParameters.stepSize +
					branch.GetLastBranchPoint().grabVector * branch.deltaHeight, branch.branchPoints.Count, 0f, branch);
			Vector3 direction = branchPoint.point - branch.GetLastBranchPoint().point;
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, direction),
				out raycastHit, infoPool.ivyParameters.stepSize * 1.15f, infoPool.ivyParameters.layerMask.value))
			{
				CheckFloor(branch, branchPoint, -branch.GetLastBranchPoint().grabVector);
				return;
			}

			NewGrowDirectionAfterWall(branch, -branch.GetLastBranchPoint().grabVector, raycastHit.normal);
			AddPoint(branch, raycastHit.point, raycastHit.normal);
		}


		private void CheckFloor(BranchContainer branch, BranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point, -oldSurfaceNormal), out raycastHit,
				branch.currentHeight * 2f, infoPool.ivyParameters.layerMask.value))
			{
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				NewGrowDirection(branch);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}

			if (Random.value < infoPool.ivyParameters.grabProvabilityOnFall)
			{
				CheckCorner(branch, potentialPoint, oldSurfaceNormal);
				return;
			}

			AddFallingPoint(branch);
			branch.fallIteration += 1f - infoPool.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}


		private void CheckCorner(BranchContainer branch, BranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(
				new Ray(
					potentialPoint.point + branch.branchPoints[branch.branchPoints.Count - 1].grabVector * 2f *
					branch.currentHeight, -branch.growDirection), out raycastHit,
				infoPool.ivyParameters.stepSize * 1.15f, infoPool.ivyParameters.layerMask.value))
			{
				AddPoint(branch, potentialPoint.point, oldSurfaceNormal);
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				NewGrowDirectionAfterCorner(branch, oldSurfaceNormal, raycastHit.normal);
				return;
			}

			AddFallingPoint(branch);
			branch.fallIteration += 1f - infoPool.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}


		private void CheckFall(BranchContainer branch)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, branch.growDirection),
				out raycastHit, infoPool.ivyParameters.stepSize * 1.15f, infoPool.ivyParameters.layerMask.value))
			{
				NewGrowDirectionAfterFall(branch, raycastHit.normal);
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}

			if (Random.value < infoPool.ivyParameters.grabProvabilityOnFall)
			{
				CheckGrabPoint(branch);
				return;
			}

			NewGrowDirectionFalling(branch);
			AddFallingPoint(branch);
			branch.fallIteration += 1f - infoPool.ivyParameters.stiffness;
			branch.falling = true;
		}


		private void CheckGrabPoint(BranchContainer branch)
		{
			for (int i = 0; i < 6; i++)
			{
				float angle = 60f * i;
				RaycastHit raycastHit;
				if (Physics.Raycast(
					new Ray(
						branch.branchPoints[branch.branchPoints.Count - 1].point +
						branch.growDirection * infoPool.ivyParameters.stepSize,
						Quaternion.AngleAxis(angle, branch.growDirection) * branch.GetLastBranchPoint().grabVector),
					out raycastHit, infoPool.ivyParameters.stepSize * 2f, infoPool.ivyParameters.layerMask.value))
				{
					AddPoint(branch, raycastHit.point, raycastHit.normal);
					NewGrowDirectionAfterGrab(branch, raycastHit.normal);
					branch.fallIteration = 0f;
					branch.falling = false;
					return;
				}

				if (i == 5)
				{
					AddFallingPoint(branch);
					NewGrowDirectionFalling(branch);
					branch.fallIteration += 1f - infoPool.ivyParameters.stiffness;
					branch.falling = true;
				}
			}
		}


		public void AddPoint(BranchContainer branch, Vector3 point, Vector3 normal)
		{
			branch.totalLenght += infoPool.ivyParameters.stepSize;
			branch.heightParameter += infoPool.ivyParameters.stepSize;
			branch.AddBranchPoint(point + normal * branch.currentHeight, -normal);
			if (growing && Random.value < infoPool.ivyParameters.branchProvability &&
			    infoPool.ivyContainer.branches.Count < infoPool.ivyParameters.maxBranchs)
			{
				AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point,
					normal);
			}

			AddLeave(branch);
		}


		private void AddFallingPoint(BranchContainer branch)
		{
			Vector3 grabVector = branch.rotationOnFallIteration * branch.GetLastBranchPoint().grabVector;
			branch.totalLenght += infoPool.ivyParameters.stepSize;
			branch.AddBranchPoint(
				branch.branchPoints[branch.branchPoints.Count - 1].point +
				branch.growDirection * infoPool.ivyParameters.stepSize, grabVector);
			if (Random.value < infoPool.ivyParameters.branchProvability &&
			    infoPool.ivyContainer.branches.Count < infoPool.ivyParameters.maxBranchs)
			{
				AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point,
					-branch.GetLastBranchPoint().grabVector);
			}

			AddLeave(branch);
		}


		private void AddLeave(BranchContainer branch)
		{
			if (branch.branchPoints.Count % (infoPool.ivyParameters.leaveEvery +
			                                 Random.Range(0, infoPool.ivyParameters.randomLeaveEvery)) == 0)
			{
				float[] array = new float[infoPool.ivyParameters.leavesPrefabs.Length];
				int chosenLeave = 0;
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Random.Range(0f, infoPool.ivyParameters.leavesProb[i]);
				}

				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] >= num)
					{
						num = array[j];
						chosenLeave = j;
					}
				}

				BranchPoint branchPoint = branch.branchPoints[branch.branchPoints.Count - 2];
				BranchPoint branchPoint2 = branch.branchPoints[branch.branchPoints.Count - 1];
				Vector3 leafPoint = Vector3.Lerp(branchPoint.point, branchPoint2.point, 0.5f);
				branch.AddLeaf(leafPoint, branch.totalLenght, branch.growDirection,
					-branch.GetLastBranchPoint().grabVector, chosenLeave, branchPoint, branchPoint2);
			}
		}


		public void DeleteLastBranch()
		{
			infoPool.ivyContainer.branches.RemoveAt(infoPool.ivyContainer.branches.Count - 1);
		}


		public void AddBranch(BranchContainer branch, BranchPoint originBranchPoint, Vector3 point, Vector3 normal)
		{
			BranchContainer branchContainer = CreateInstance<BranchContainer>();
			branchContainer.Init();
			branchContainer.AddBranchPoint(point, -normal);
			branchContainer.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, normal));
			branchContainer.randomizeHeight = Random.Range(4f, 8f);
			branchContainer.currentHeight = branch.currentHeight;
			branchContainer.heightParameter = branch.heightParameter;
			branchContainer.branchSense = ChooseBranchSense();
			branchContainer.originPointOfThisBranch = originBranchPoint;
			infoPool.ivyContainer.AddBranch(branchContainer);
			originBranchPoint.InitBranchInThisPoint(branchContainer.branchNumber);
		}


		private void NewGrowDirection(BranchContainer branch)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(
				Quaternion.AngleAxis(
					Mathf.Sin(branch.branchSense * branch.totalLenght * infoPool.ivyParameters.directionFrequency *
					          (1f + Random.Range(-infoPool.ivyParameters.directionRandomness,
						          infoPool.ivyParameters.directionRandomness))) *
					infoPool.ivyParameters.directionAmplitude * infoPool.ivyParameters.stepSize * 10f *
					Mathf.Max(infoPool.ivyParameters.directionRandomness, 1f), branch.GetLastBranchPoint().grabVector) *
				branch.growDirection, branch.GetLastBranchPoint().grabVector));
		}


		private void NewGrowDirectionAfterWall(BranchContainer branch, Vector3 oldSurfaceNormal,
			Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(oldSurfaceNormal, newSurfaceNormal));
		}


		private void NewGrowDirectionFalling(BranchContainer branch)
		{
			Vector3 vector = Vector3.Lerp(branch.growDirection, infoPool.ivyParameters.gravity,
				branch.fallIteration / 10f);
			vector = Quaternion.AngleAxis(
				Mathf.Sin(branch.branchSense * branch.totalLenght * infoPool.ivyParameters.directionFrequency *
				          (1f + Random.Range(-infoPool.ivyParameters.directionRandomness / 8f,
					          infoPool.ivyParameters.directionRandomness / 8f))) *
				infoPool.ivyParameters.directionAmplitude * infoPool.ivyParameters.stepSize * 5f *
				Mathf.Max(infoPool.ivyParameters.directionRandomness / 8f, 1f),
				branch.GetLastBranchPoint().grabVector) * vector;
			vector = Quaternion.AngleAxis(
				Mathf.Sin(branch.branchSense * branch.totalLenght * infoPool.ivyParameters.directionFrequency / 2f *
				          (1f + Random.Range(-infoPool.ivyParameters.directionRandomness / 8f,
					          infoPool.ivyParameters.directionRandomness / 8f))) *
				infoPool.ivyParameters.directionAmplitude * infoPool.ivyParameters.stepSize * 5f *
				Mathf.Max(infoPool.ivyParameters.directionRandomness / 8f, 1f),
				Vector3.Cross(branch.GetLastBranchPoint().grabVector, branch.growDirection)) * vector;
			branch.rotationOnFallIteration = Quaternion.FromToRotation(branch.growDirection, vector);
			branch.growDirection = vector;
		}


		private void NewGrowDirectionAfterFall(BranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection =
				Vector3.Normalize(Vector3.ProjectOnPlane(-branch.GetLastBranchPoint().grabVector, newSurfaceNormal));
		}


		private void NewGrowDirectionAfterGrab(BranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, newSurfaceNormal));
		}


		private void NewGrowDirectionAfterCorner(BranchContainer branch, Vector3 oldSurfaceNormal,
			Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-oldSurfaceNormal, newSurfaceNormal));
		}


		private void Refine(BranchContainer branch)
		{
			if (branch.branchPoints.Count > 3)
			{
				if (Vector3.Distance(branch.branchPoints[branch.branchPoints.Count - 2].point,
					    branch.branchPoints[branch.branchPoints.Count - 3].point) <
				    infoPool.ivyParameters.stepSize * 0.7f ||
				    Vector3.Distance(branch.branchPoints[branch.branchPoints.Count - 2].point,
					    branch.branchPoints[branch.branchPoints.Count - 1].point) <
				    infoPool.ivyParameters.stepSize * 0.7f)
				{
					branch.RemoveBranchPoint(branch.branchPoints.Count - 2);
				}

				if (Vector3.Angle(
					branch.branchPoints[branch.branchPoints.Count - 1].point -
					branch.branchPoints[branch.branchPoints.Count - 2].point,
					branch.branchPoints[branch.branchPoints.Count - 2].point -
					branch.branchPoints[branch.branchPoints.Count - 3].point) > 25f)
				{
					Vector3 a = branch.branchPoints[branch.branchPoints.Count - 1].point -
					            branch.branchPoints[branch.branchPoints.Count - 2].point;
					Vector3 a2 = branch.branchPoints[branch.branchPoints.Count - 3].point -
					             branch.branchPoints[branch.branchPoints.Count - 2].point;
					branch.InsertBranchPoint(branch.branchPoints[branch.branchPoints.Count - 2].point + a2 / 2f,
						branch.branchPoints[branch.branchPoints.Count - 2].grabVector, branch.branchPoints.Count - 2);
					branch.InsertBranchPoint(branch.branchPoints[branch.branchPoints.Count - 2].point + a / 2f,
						branch.branchPoints[branch.branchPoints.Count - 2].grabVector, branch.branchPoints.Count - 1);
					branch.RemoveBranchPoint(branch.branchPoints.Count - 3);
				}
			}
		}


		public void Optimize()
		{
			foreach (BranchContainer branchContainer in infoPool.ivyContainer.branches)
			{
				for (int i = 1; i < branchContainer.branchPoints.Count - 2; i++)
				{
					if (Vector3.Distance(branchContainer.branchPoints[i - 1].point,
						branchContainer.branchPoints[i].point) < infoPool.ivyParameters.stepSize * 0.7f)
					{
						branchContainer.RemoveBranchPoint(i);
					}

					if (Vector3.Angle(branchContainer.branchPoints[i - 1].point - branchContainer.branchPoints[i].point,
						    branchContainer.branchPoints[i].point - branchContainer.branchPoints[i + 1].point) <
					    infoPool.ivyParameters.optAngleBias)
					{
						branchContainer.RemoveBranchPoint(i);
					}
				}
			}
		}
	}
}