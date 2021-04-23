using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralWorlds.RealIvy
{
	public class RuntimeIvyGrowth
	{
		private RTBranchContainer[] branchesPool;


		private int branchesPoolIndex;


		private int branchPointPoolIndex;


		private RTBranchPoint[] branchPointsPool;


		private GameObject ivyGO;


		private IvyParameters ivyParameters;


		private RTMeshData[] leavesMeshesByChosenLeaf;


		private RTLeafPoint[] leavesPool;


		private int leavesPoolIndex;


		private int maxNumVerticesPerLeaf;


		private int numLeaves;


		private int numPoints;


		public Random.State randomstate;


		private RTIvyContainer rtIvyContainer;


		public void Init(RTIvyContainer ivyContainer, IvyParameters ivyParameters, GameObject ivyGO,
			RTMeshData[] leavesMeshesByChosenLeaf, int numPoints, int numLeaves, int maxNumVerticesPerLeaf)
		{
			rtIvyContainer = ivyContainer;
			this.ivyParameters = ivyParameters;
			this.ivyGO = ivyGO;
			this.leavesMeshesByChosenLeaf = leavesMeshesByChosenLeaf;
			this.numPoints = numPoints;
			this.numLeaves = numLeaves;
			this.maxNumVerticesPerLeaf = maxNumVerticesPerLeaf;
			branchPointsPool = new RTBranchPoint[numPoints];
			branchPointPoolIndex = 0;
			for (int i = 0; i < numPoints; i++)
			{
				RTBranchPoint rtbranchPoint = new RTBranchPoint();
				rtbranchPoint.PreInit(ivyParameters);
				branchPointsPool[i] = rtbranchPoint;
			}

			leavesPool = new RTLeafPoint[numLeaves];
			leavesPoolIndex = 0;
			for (int j = 0; j < numLeaves; j++)
			{
				RTLeafPoint rtleafPoint = new RTLeafPoint();
				rtleafPoint.PreInit(maxNumVerticesPerLeaf);
				leavesPool[j] = rtleafPoint;
			}

			branchesPool = new RTBranchContainer[ivyParameters.maxBranchs];
			for (int k = 0; k < ivyParameters.maxBranchs; k++)
			{
				branchesPool[k] = new RTBranchContainer(numPoints, numLeaves);
			}

			Random.InitState(Environment.TickCount);
			RTBranchContainer nextBranchContainer = GetNextBranchContainer();
			ivyContainer.AddBranch(nextBranchContainer);
			RTBranchPoint nextFreeBranchPoint = GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(ivyGO.transform.position, -ivyGO.transform.up, false, 0);
			nextBranchContainer.AddBranchPoint(nextFreeBranchPoint, ivyParameters.stepSize);
			CalculateVerticesLastPoint(nextBranchContainer);
			ivyContainer.branches[0].growDirection = Quaternion.AngleAxis(Random.value * 360f, ivyGO.transform.up) *
			                                         ivyGO.transform.forward;
			ivyContainer.firstVertexVector = ivyContainer.branches[0].growDirection;
			ivyContainer.branches[0].randomizeHeight = Random.Range(4f, 8f);
			CalculateNewHeight(ivyContainer.branches[0]);
			ivyContainer.branches[0].branchSense = ChooseBranchSense();
			randomstate = Random.state;
		}


		private void CalculateNewHeight(RTBranchContainer branch)
		{
			branch.heightVar = (Mathf.Sin(branch.heightParameter * ivyParameters.DTSFrequency - 45f) + 1f) / 2f;
			branch.newHeight = Mathf.Lerp(ivyParameters.minDistanceToSurface, ivyParameters.maxDistanceToSurface,
				branch.heightVar);
			branch.newHeight +=
				(Mathf.Sin(branch.heightParameter * ivyParameters.DTSFrequency * branch.randomizeHeight) + 1f) / 2f *
				ivyParameters.maxDistanceToSurface / 4f * ivyParameters.DTSRandomness;
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
			for (int i = 0; i < rtIvyContainer.branches.Count; i++)
			{
				rtIvyContainer.branches[i].heightParameter += ivyParameters.stepSize;
				CalculateNewPoint(rtIvyContainer.branches[i]);
			}

			randomstate = Random.state;
		}


		private void CalculateNewPoint(RTBranchContainer branch)
		{
			if (!branch.falling)
			{
				CalculateNewHeight(branch);
				CheckWall(branch);
				return;
			}

			CheckFall(branch);
		}


		private void CheckWall(RTBranchContainer branch)
		{
			RTBranchPoint nextFreeBranchPoint = GetNextFreeBranchPoint();
			nextFreeBranchPoint.point = branch.GetLastBranchPoint().point +
			                            branch.growDirection * ivyParameters.stepSize +
			                            branch.GetLastBranchPoint().grabVector * branch.deltaHeight;
			nextFreeBranchPoint.index = branch.branchPoints.Count;
			Vector3 direction = nextFreeBranchPoint.point - branch.GetLastBranchPoint().point;
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, direction),
				out raycastHit, ivyParameters.stepSize * 1.15f, ivyParameters.layerMask.value))
			{
				CheckFloor(branch, nextFreeBranchPoint, -branch.GetLastBranchPoint().grabVector);
				return;
			}

			NewGrowDirectionAfterWall(branch, -branch.GetLastBranchPoint().grabVector, raycastHit.normal);
			AddPoint(branch, raycastHit.point, raycastHit.normal);
		}


		private void CheckFloor(RTBranchContainer branch, RTBranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point, -oldSurfaceNormal), out raycastHit,
				branch.currentHeight * 2f, ivyParameters.layerMask.value))
			{
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				NewGrowDirection(branch);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}

			if (Random.value < ivyParameters.grabProvabilityOnFall)
			{
				CheckCorner(branch, potentialPoint, oldSurfaceNormal);
				return;
			}

			AddFallingPoint(branch);
			branch.fallIteration += 1f - ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}


		private void CheckCorner(RTBranchContainer branch, RTBranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(
				new Ray(
					potentialPoint.point + branch.branchPoints[branch.branchPoints.Count - 1].grabVector * 2f *
					branch.currentHeight, -branch.growDirection), out raycastHit, ivyParameters.stepSize * 1.15f,
				ivyParameters.layerMask.value))
			{
				AddPoint(branch, potentialPoint.point, oldSurfaceNormal);
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				NewGrowDirectionAfterCorner(branch, oldSurfaceNormal, raycastHit.normal);
				return;
			}

			AddFallingPoint(branch);
			branch.fallIteration += 1f - ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}


		private void CheckFall(RTBranchContainer branch)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, branch.growDirection),
				out raycastHit, ivyParameters.stepSize * 1.15f, ivyParameters.layerMask.value))
			{
				NewGrowDirectionAfterFall(branch, raycastHit.normal);
				AddPoint(branch, raycastHit.point, raycastHit.normal);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}

			if (Random.value < ivyParameters.grabProvabilityOnFall)
			{
				CheckGrabPoint(branch);
				return;
			}

			NewGrowDirectionFalling(branch);
			AddFallingPoint(branch);
			branch.fallIteration += 1f - ivyParameters.stiffness;
			branch.falling = true;
		}


		private void CheckGrabPoint(RTBranchContainer branch)
		{
			for (int i = 0; i < 6; i++)
			{
				float angle = 60f * i;
				RaycastHit raycastHit;
				if (Physics.Raycast(
					new Ray(
						branch.branchPoints[branch.branchPoints.Count - 1].point +
						branch.growDirection * ivyParameters.stepSize,
						Quaternion.AngleAxis(angle, branch.growDirection) * branch.GetLastBranchPoint().grabVector),
					out raycastHit, ivyParameters.stepSize * 2f, ivyParameters.layerMask.value))
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
					branch.fallIteration += 1f - ivyParameters.stiffness;
					branch.falling = true;
				}
			}
		}


		public void AddPoint(RTBranchContainer branch, Vector3 point, Vector3 normal)
		{
			branch.totalLength += ivyParameters.stepSize;
			RTBranchPoint nextFreeBranchPoint = GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(point + normal * branch.currentHeight, -normal);
			branch.AddBranchPoint(nextFreeBranchPoint, ivyParameters.stepSize);
			CalculateVerticesLastPoint(branch);
			if (Random.value < ivyParameters.branchProvability &&
			    rtIvyContainer.branches.Count < ivyParameters.maxBranchs)
			{
				AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point,
					normal);
			}

			if (ivyParameters.generateLeaves)
			{
				AddLeave(branch);
			}
		}


		private float CalculateRadius(float lenght)
		{
			float t = (Mathf.Sin(lenght * ivyParameters.radiusVarFreq + ivyParameters.radiusVarOffset) + 1f) / 2f;
			return Mathf.Lerp(ivyParameters.minRadius, ivyParameters.maxRadius, t);
		}


		private float CalculateLeafScale(BranchContainer branch, LeafPoint leafPoint)
		{
			float num = Random.Range(ivyParameters.minScale, ivyParameters.maxScale);
			if (leafPoint.lpLength - 0.1f >= branch.totalLenght - ivyParameters.tipInfluence)
			{
				num *= Mathf.InverseLerp(branch.totalLenght, branch.totalLenght - ivyParameters.tipInfluence,
					leafPoint.lpLength);
			}

			return num;
		}


		private Quaternion CalculateLeafRotation(LeafPoint leafPoint)
		{
			Vector3 vector;
			Vector3 axis;
			if (!ivyParameters.globalOrientation)
			{
				vector = leafPoint.lpForward;
				axis = leafPoint.left;
			}
			else
			{
				vector = ivyParameters.globalRotation;
				axis = Vector3.Normalize(Vector3.Cross(ivyParameters.globalRotation, leafPoint.lpUpward));
			}

			Quaternion rhs = Quaternion.LookRotation(leafPoint.lpUpward, vector);
			rhs = Quaternion.AngleAxis(ivyParameters.rotation.x, axis) *
			      Quaternion.AngleAxis(ivyParameters.rotation.y, leafPoint.lpUpward) *
			      Quaternion.AngleAxis(ivyParameters.rotation.z, vector) * rhs;
			return Quaternion.AngleAxis(Random.Range(-ivyParameters.randomRotation.x, ivyParameters.randomRotation.x),
				       axis) *
			       Quaternion.AngleAxis(Random.Range(-ivyParameters.randomRotation.y, ivyParameters.randomRotation.y),
				       leafPoint.lpUpward) *
			       Quaternion.AngleAxis(Random.Range(-ivyParameters.randomRotation.z, ivyParameters.randomRotation.z),
				       vector) * rhs;
		}


		private void AddFallingPoint(RTBranchContainer branch)
		{
			Vector3 grabVector = branch.rotationOnFallIteration * branch.GetLastBranchPoint().grabVector;
			RTBranchPoint nextFreeBranchPoint = GetNextFreeBranchPoint();
			nextFreeBranchPoint.point = branch.branchPoints[branch.branchPoints.Count - 1].point +
			                            branch.growDirection * ivyParameters.stepSize;
			nextFreeBranchPoint.grabVector = grabVector;
			branch.AddBranchPoint(nextFreeBranchPoint, ivyParameters.stepSize);
			CalculateVerticesLastPoint(branch);
			if (Random.value < ivyParameters.branchProvability &&
			    rtIvyContainer.branches.Count < ivyParameters.maxBranchs)
			{
				AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point,
					-branch.GetLastBranchPoint().grabVector);
			}

			if (ivyParameters.generateLeaves)
			{
				AddLeave(branch);
			}
		}


		private void CalculateVerticesLastPoint(RTBranchContainer rtBranchContainer)
		{
			if (rtBranchContainer.branchPoints.Count > 1)
			{
				RTBranchPoint rtbranchPoint = rtBranchContainer.branchPoints[rtBranchContainer.branchPoints.Count - 2];
				float radius = CalculateRadius(rtbranchPoint.length);
				Vector3 loopAxis = GetLoopAxis(rtbranchPoint, rtBranchContainer, rtIvyContainer, ivyGO);
				Vector3 firstVector = GetFirstVector(rtbranchPoint, rtBranchContainer, rtIvyContainer, ivyParameters,
					loopAxis);
				rtbranchPoint.CalculateCenterLoop(ivyGO);
				rtbranchPoint.CalculateVerticesLoop(ivyParameters, rtIvyContainer, ivyGO, firstVector, loopAxis,
					radius);
			}
		}


		private void AddLeave(RTBranchContainer branch)
		{
			if (branch.branchPoints.Count %
				(ivyParameters.leaveEvery + Random.Range(0, ivyParameters.randomLeaveEvery)) == 0)
			{
				int chosenLeave = Random.Range(0, ivyParameters.leavesPrefabs.Length);
				RTBranchPoint rtbranchPoint = branch.branchPoints[branch.branchPoints.Count - 2];
				RTBranchPoint rtbranchPoint2 = branch.branchPoints[branch.branchPoints.Count - 1];
				Vector3 point = Vector3.Lerp(rtbranchPoint.point, rtbranchPoint2.point, 0.5f);
				float leafScale = Random.Range(ivyParameters.minScale, ivyParameters.maxScale);
				RTLeafPoint nextLeafPoint = GetNextLeafPoint();
				nextLeafPoint.SetValues(point, branch.totalLength, branch.growDirection,
					-branch.GetLastBranchPoint().grabVector, chosenLeave, rtbranchPoint, rtbranchPoint2, leafScale,
					ivyParameters);
				RTMeshData leafMeshData = leavesMeshesByChosenLeaf[nextLeafPoint.chosenLeave];
				nextLeafPoint.CreateVertices(ivyParameters, leafMeshData, ivyGO);
				branch.AddLeaf(nextLeafPoint);
			}
		}


		public void DeleteLastBranch()
		{
			rtIvyContainer.branches.RemoveAt(rtIvyContainer.branches.Count - 1);
		}


		public void AddBranch(RTBranchContainer branch, RTBranchPoint originBranchPoint, Vector3 point, Vector3 normal)
		{
			RTBranchContainer nextBranchContainer = GetNextBranchContainer();
			RTBranchPoint nextFreeBranchPoint = GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(point, -normal);
			nextBranchContainer.AddBranchPoint(nextFreeBranchPoint, ivyParameters.stepSize);
			nextBranchContainer.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, normal));
			nextBranchContainer.randomizeHeight = Random.Range(4f, 8f);
			nextBranchContainer.currentHeight = branch.currentHeight;
			nextBranchContainer.heightParameter = branch.heightParameter;
			nextBranchContainer.branchSense = ChooseBranchSense();
			rtIvyContainer.AddBranch(nextBranchContainer);
			originBranchPoint.InitBranchInThisPoint(nextBranchContainer.branchNumber);
		}


		private void NewGrowDirection(RTBranchContainer branch)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(
				Quaternion.AngleAxis(
					Mathf.Sin(branch.branchSense * branch.totalLength * ivyParameters.directionFrequency *
					          (1f + Random.Range(-ivyParameters.directionRandomness,
						          ivyParameters.directionRandomness))) * ivyParameters.directionAmplitude *
					ivyParameters.stepSize * 10f * Mathf.Max(ivyParameters.directionRandomness, 1f),
					branch.GetLastBranchPoint().grabVector) * branch.growDirection,
				branch.GetLastBranchPoint().grabVector));
		}


		private void NewGrowDirectionAfterWall(RTBranchContainer branch, Vector3 oldSurfaceNormal,
			Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(oldSurfaceNormal, newSurfaceNormal));
		}


		private void NewGrowDirectionFalling(RTBranchContainer branch)
		{
			Vector3 vector = Vector3.Lerp(branch.growDirection, ivyParameters.gravity, branch.fallIteration / 10f);
			vector = Quaternion.AngleAxis(
				Mathf.Sin(branch.branchSense * branch.totalLength * ivyParameters.directionFrequency * (1f +
					Random.Range(-ivyParameters.directionRandomness / 8f, ivyParameters.directionRandomness / 8f))) *
				ivyParameters.directionAmplitude * ivyParameters.stepSize * 5f *
				Mathf.Max(ivyParameters.directionRandomness / 8f, 1f), branch.GetLastBranchPoint().grabVector) * vector;
			vector = Quaternion.AngleAxis(
				Mathf.Sin(branch.branchSense * branch.totalLength * ivyParameters.directionFrequency / 2f *
				          (1f + Random.Range(-ivyParameters.directionRandomness / 8f,
					          ivyParameters.directionRandomness / 8f))) * ivyParameters.directionAmplitude *
				ivyParameters.stepSize * 5f * Mathf.Max(ivyParameters.directionRandomness / 8f, 1f),
				Vector3.Cross(branch.GetLastBranchPoint().grabVector, branch.growDirection)) * vector;
			branch.rotationOnFallIteration = Quaternion.FromToRotation(branch.growDirection, vector);
			branch.growDirection = vector;
		}


		private void NewGrowDirectionAfterFall(RTBranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection =
				Vector3.Normalize(Vector3.ProjectOnPlane(-branch.GetLastBranchPoint().grabVector, newSurfaceNormal));
		}


		private void NewGrowDirectionAfterGrab(RTBranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, newSurfaceNormal));
		}


		private void NewGrowDirectionAfterCorner(RTBranchContainer branch, Vector3 oldSurfaceNormal,
			Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-oldSurfaceNormal, newSurfaceNormal));
		}


		public Vector3 GetFirstVector(RTBranchPoint rtBranchPoint, RTBranchContainer rtBranchContainer,
			RTIvyContainer rtIvyContainer, IvyParameters ivyParameters, Vector3 axis)
		{
			Vector3 result = Vector3.zero;
			if (rtBranchContainer.branchNumber == 0 && rtBranchPoint.index == 0)
			{
				if (!ivyParameters.halfgeom)
				{
					result = rtIvyContainer.firstVertexVector;
				}
				else
				{
					result = Quaternion.AngleAxis(90f, axis) * rtIvyContainer.firstVertexVector;
				}
			}
			else if (!ivyParameters.halfgeom)
			{
				result = Vector3.Normalize(Vector3.ProjectOnPlane(rtBranchPoint.grabVector, axis));
			}
			else
			{
				result = Quaternion.AngleAxis(90f, axis) *
				         Vector3.Normalize(Vector3.ProjectOnPlane(rtBranchPoint.grabVector, axis));
			}

			return result;
		}


		public Vector3 GetLoopAxis(RTBranchPoint rtBranchPoint, RTBranchContainer rtBranchContainer,
			RTIvyContainer rtIvyContainer, GameObject ivyGo)
		{
			Vector3 result = Vector3.zero;
			if (rtBranchPoint.index == 0 && rtBranchContainer.branchNumber == 0)
			{
				result = ivyGo.transform.up;
			}
			else if (rtBranchPoint.index == 0)
			{
				result = rtBranchPoint.GetNextPoint().point - rtBranchPoint.point;
			}
			else
			{
				result = Vector3.Normalize(Vector3.Lerp(rtBranchPoint.point - rtBranchPoint.GetPreviousPoint().point,
					rtBranchPoint.GetNextPoint().point - rtBranchPoint.point, 0.5f));
			}

			return result;
		}


		private RTBranchPoint GetNextFreeBranchPoint()
		{
			RTBranchPoint result = branchPointsPool[branchPointPoolIndex];
			branchPointPoolIndex++;
			if (branchPointPoolIndex >= branchPointsPool.Length)
			{
				Array.Resize<RTBranchPoint>(ref branchPointsPool, branchPointsPool.Length * 2);
				for (int i = branchPointPoolIndex; i < branchPointsPool.Length; i++)
				{
					RTBranchPoint rtbranchPoint = new RTBranchPoint();
					rtbranchPoint.PreInit(ivyParameters);
					branchPointsPool[i] = rtbranchPoint;
				}
			}

			return result;
		}


		private RTLeafPoint GetNextLeafPoint()
		{
			RTLeafPoint result = leavesPool[leavesPoolIndex];
			leavesPoolIndex++;
			if (leavesPoolIndex >= leavesPool.Length)
			{
				Array.Resize<RTLeafPoint>(ref leavesPool, leavesPool.Length * 2);
				for (int i = leavesPoolIndex; i < leavesPool.Length; i++)
				{
					leavesPool[i] = new RTLeafPoint();
					leavesPool[i].PreInit(maxNumVerticesPerLeaf);
				}
			}

			return result;
		}


		private RTBranchContainer GetNextBranchContainer()
		{
			RTBranchContainer result = branchesPool[branchesPoolIndex];
			branchesPoolIndex++;
			return result;
		}
	}
}