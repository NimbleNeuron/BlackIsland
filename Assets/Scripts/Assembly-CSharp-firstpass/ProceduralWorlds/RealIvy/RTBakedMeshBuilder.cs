using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.RealIvy
{
	public class RTBakedMeshBuilder
	{
		public List<RTBranchContainer> activeBranches;


		private float angle;


		private int backtrackingPoints;


		public RTMeshData bakedMeshData;


		private Color blackColor;


		public RTMeshData buildingMeshData;


		private int endIdxLeaves;


		private int[] fromTo;


		private float growthSpeed;


		private int initIdxLeaves;


		public GameObject ivyGO;


		private Quaternion ivyGoInverseRotation;


		private Vector3 ivyGoPosition;


		private Quaternion ivyGoRotation;


		private Mesh ivyMesh;


		public IvyParameters ivyParameters;


		private int lastLeafVertProcessed;


		private int lastPointCopied;


		private int[] lastTriangleIndexPerBranch;


		private int lastVertCount;


		private int lastVertexIndex;


		private int lastVerticesCountProcessed;


		private float leafLengthCorrrectionFactor;


		public bool leavesDataInitialized;


		public List<Material> leavesMaterials;


		private RTMeshData[] leavesMeshesByChosenLeaf;


		private MeshFilter leavesMeshFilter;


		private MeshRenderer leavesMeshRenderer;


		public MeshFilter meshFilter;


		public MeshRenderer meshRenderer;


		private MeshRenderer mrProcessedMesh;


		private bool onOptimizedStretch;


		public List<List<int>> processedBranchesVerticesIndicesPerBranch;


		private Mesh processedMesh;


		public RTMeshData processedMeshData;


		public List<List<int>> processedVerticesIndicesPerBranch;


		public RTIvyContainer rtBakedIvyContainer;


		public RTIvyContainer rtIvyContainer;


		private int[] submeshByChoseLeaf;


		private int submeshCount;


		private int triCount;


		public List<List<int>> typesByMat;


		public Rect[] uv2Rects = new Rect[0];


		private Vector3[] vectors;


		private int vertCount;


		private int[] vertCountLeavesPerBranch;


		private int[] vertCountsPerBranch;


		private Vector2 zeroVector2;


		private Vector3 zeroVector3;


		public RTBakedMeshBuilder() { }


		public RTBakedMeshBuilder(RTIvyContainer ivyContainer, GameObject ivyGo)
		{
			rtIvyContainer = ivyContainer;
			ivyGO = ivyGo;
		}


		public void InitializeMeshBuilder(IvyParameters ivyParameters, RTIvyContainer ivyContainer,
			RTIvyContainer bakedIvyContainer, GameObject ivyGO, Mesh bakedMesh, MeshRenderer meshRenderer,
			MeshFilter meshFilter, int numBranches, Mesh processedMesh, float growSpeed, MeshRenderer mrProcessedMesh,
			int backtrackingPoints, int[] submeshByChoseLeaf, RTMeshData[] leavesMeshesByChosenLeaf,
			Material[] materials)
		{
			this.ivyParameters = ivyParameters;
			rtIvyContainer = ivyContainer;
			rtBakedIvyContainer = bakedIvyContainer;
			this.ivyGO = ivyGO;
			this.meshRenderer = meshRenderer;
			this.meshFilter = meshFilter;
			this.processedMesh = processedMesh;
			this.processedMesh.indexFormat = IndexFormat.UInt16;
			this.mrProcessedMesh = mrProcessedMesh;
			this.submeshByChoseLeaf = submeshByChoseLeaf;
			this.leavesMeshesByChosenLeaf = leavesMeshesByChosenLeaf;
			activeBranches = new List<RTBranchContainer>();
			fromTo = new int[2];
			vectors = new Vector3[2];
			growthSpeed = growSpeed;
			this.backtrackingPoints = backtrackingPoints;
			submeshCount = meshRenderer.sharedMaterials.Length;
			vertCountsPerBranch = new int[numBranches];
			lastTriangleIndexPerBranch = new int[numBranches];
			vertCountLeavesPerBranch = new int[numBranches];
			processedVerticesIndicesPerBranch = new List<List<int>>(numBranches);
			processedBranchesVerticesIndicesPerBranch = new List<List<int>>(numBranches);
			for (int i = 0; i < numBranches; i++)
			{
				processedVerticesIndicesPerBranch.Add(new List<int>());
				processedBranchesVerticesIndicesPerBranch.Add(new List<int>());
			}

			vertCount = 0;
			ivyMesh = new Mesh();
			ivyMesh.subMeshCount = submeshCount;
			ivyMesh.name = "IvyMesh";
			meshFilter.mesh = ivyMesh;
			ivyGO.GetComponent<MeshRenderer>().sharedMaterials = materials;
			mrProcessedMesh.sharedMaterials = materials;
			leavesDataInitialized = true;
			ivyGoPosition = ivyGO.transform.position;
			ivyGoRotation = ivyGO.transform.rotation;
			ivyGoInverseRotation = Quaternion.Inverse(ivyGO.transform.rotation);
			zeroVector3 = Vector3.zero;
			zeroVector2 = Vector2.zero;
			blackColor = Color.black;
		}


		public void InitializeMeshesDataBaked(Mesh bakedMesh, int numBranches)
		{
			CreateBuildingMeshData(bakedMesh, numBranches);
			CreateBakedMeshData(bakedMesh);
			CreateProcessedMeshData(bakedMesh);
			bakedMesh.Clear();
		}


		public void InitializeMeshesDataProcedural(Mesh bakedMesh, int numBranches, float lifetime, float velocity)
		{
			CreateBuildingMeshData(bakedMesh, numBranches);
			CreateBakedMeshData(bakedMesh);
			CreateProcessedMeshDataProcedural(bakedMesh, lifetime, velocity);
			bakedMesh.Clear();
		}


		public void CreateBuildingMeshData(Mesh bakedMesh, int numBranches)
		{
			int num = ivyParameters.sides + 1;
			int num2 = backtrackingPoints * num + backtrackingPoints * 2 * 8;
			num2 *= numBranches;
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			int num3 = (backtrackingPoints - 2) * ivyParameters.sides * 6 + ivyParameters.sides * 3;
			num3 *= numBranches;
			list.Add(num3);
			for (int i = 1; i < subMeshCount; i++)
			{
				int item = backtrackingPoints * 6 * numBranches;
				list.Add(item);
			}

			buildingMeshData = new RTMeshData(num2, subMeshCount, list);
		}


		public void CreateBakedMeshData(Mesh bakedMesh)
		{
			bakedMeshData = new RTMeshData(bakedMesh);
		}


		public void CreateProcessedMeshDataProcedural(Mesh bakedMesh, float lifetime, float velocity)
		{
			int num = Mathf.CeilToInt(lifetime / velocity * 200f);
			int numVertices = num * (ivyParameters.sides + 1);
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			for (int i = 0; i < subMeshCount; i++)
			{
				int item = ivyParameters.sides * num * 9;
				list.Add(item);
			}

			processedMeshData = new RTMeshData(numVertices, subMeshCount, list);
		}


		public void CreateProcessedMeshData(Mesh bakedMesh)
		{
			int vertexCount = bakedMesh.vertexCount;
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			for (int i = 0; i < subMeshCount; i++)
			{
				int item = bakedMesh.GetTriangles(i).Length;
				list.Add(item);
			}

			processedMeshData = new RTMeshData(vertexCount, subMeshCount, list);
		}


		public void SetLeafLengthCorrectionFactor(float leafLengthCorrrectionFactor)
		{
			this.leafLengthCorrrectionFactor = leafLengthCorrrectionFactor;
		}


		public void ClearMesh()
		{
			ivyMesh.Clear();
		}


		private void ClearTipMesh()
		{
			buildingMeshData.Clear();
			for (int i = 0; i < vertCountsPerBranch.Length; i++)
			{
				vertCountsPerBranch[i] = 0;
				lastTriangleIndexPerBranch[i] = 0;
				vertCountLeavesPerBranch[i] = 0;
			}

			vertCount = 0;
			triCount = 0;
		}


		public void CheckCopyMesh(int branchIndex, List<RTBranchContainer> bakedBranches)
		{
			RTBranchContainer rtbranchContainer = rtIvyContainer.branches[branchIndex];
			RTBranchContainer bakedBranchContainer = bakedBranches[branchIndex];
			int initSegmentIdx;
			int endSegmentIdx = (initSegmentIdx =
				Mathf.Clamp(rtbranchContainer.branchPoints.Count - backtrackingPoints - 1, 0, int.MaxValue)) + 1;
			CopyToFixedMesh(branchIndex, initSegmentIdx, endSegmentIdx, rtbranchContainer, bakedBranchContainer);
		}


		public void BuildGeometry02(List<RTBranchContainer> activeBakedBranches,
			List<RTBranchContainer> activeBuildingBranches)
		{
			if (!ivyParameters.halfgeom)
			{
				angle = 360f / ivyParameters.sides;
			}
			else
			{
				angle = 360f / ivyParameters.sides / 2f;
			}

			if (leavesDataInitialized)
			{
				ClearTipMesh();
				for (int i = 0; i < rtIvyContainer.branches.Count; i++)
				{
					int num = vertCount;
					RTBranchContainer rtbranchContainer = activeBuildingBranches[i];
					if (rtbranchContainer.branchPoints.Count > 1)
					{
						lastVertCount = 0;
						int num2 = rtbranchContainer.branchPoints.Count - backtrackingPoints;
						num2 = Mathf.Clamp(num2, 0, int.MaxValue);
						int count = rtbranchContainer.branchPoints.Count;
						for (int j = num2; j < count; j++)
						{
							RTBranchPoint rtbranchPoint = rtbranchContainer.branchPoints[j];
							Vector3 vector = ivyGO.transform.InverseTransformPoint(rtbranchPoint.point);
							Vector3 vertexValue = zeroVector3;
							Vector3 vector2 = zeroVector3;
							Vector2 lastUV = zeroVector2;
							Vector2 vector3 = zeroVector2;
							Color color = blackColor;
							float t = Mathf.InverseLerp(rtbranchContainer.totalLength,
								rtbranchContainer.totalLength - ivyParameters.tipInfluence, rtbranchPoint.length);
							if (j < rtbranchContainer.branchPoints.Count - 1)
							{
								for (int k = 0; k < rtbranchPoint.verticesLoop.Length; k++)
								{
									if (ivyParameters.generateBranches)
									{
										vertexValue = Vector3.LerpUnclamped(rtbranchPoint.centerLoop,
											rtbranchPoint.verticesLoop[k].vertex, t);
										buildingMeshData.AddVertex(vertexValue, rtbranchPoint.verticesLoop[k].normal,
											rtbranchPoint.verticesLoop[k].uv, rtbranchPoint.verticesLoop[k].color);
										vertCountsPerBranch[i]++;
										vertCount++;
										lastVertCount++;
									}
								}
							}
							else
							{
								vertexValue = vector;
								vector2 = Vector3.Normalize(
									rtbranchPoint.point - rtbranchPoint.GetPreviousPoint().point);
								vector2 = ivyGO.transform.InverseTransformVector(vector2);
								lastUV = rtbranchContainer.GetLastUV(ivyParameters);
								buildingMeshData.AddVertex(vertexValue, vector2, lastUV, Color.black);
								vertCountsPerBranch[i]++;
								vertCount++;
								lastVertCount++;
							}
						}

						SetTriangles(rtbranchContainer, vertCount, num2, i);
					}

					fromTo[0] = num;
					fromTo[1] = vertCount - 1;
					if (ivyParameters.generateLeaves)
					{
						BuildLeaves(i, activeBuildingBranches[i], activeBakedBranches[i]);
					}
				}

				RefreshMesh();
			}
		}


		private float CalculateRadius(BranchPoint branchPoint, BranchContainer buildingBranchContainer)
		{
			float num = Mathf.InverseLerp(branchPoint.branchContainer.totalLenght,
				branchPoint.branchContainer.totalLenght - ivyParameters.tipInfluence, branchPoint.length - 0.1f);
			branchPoint.currentRadius = branchPoint.radius * num;
			return branchPoint.currentRadius;
		}


		private void SetTriangles(RTBranchContainer branch, int vertCount, int initIndex, int branchIndex)
		{
			int num = 0;
			int num2 = Mathf.Min(branch.branchPoints.Count - 2, branch.branchPoints.Count - initIndex - 2);
			for (int i = num; i < num2; i++)
			{
				for (int j = 0; j < ivyParameters.sides; j++)
				{
					int num3 = vertCount - lastVertCount;
					int value = j + i * (ivyParameters.sides + 1) + num3;
					int value2 = j + i * (ivyParameters.sides + 1) + 1 + num3;
					int value3 = j + i * (ivyParameters.sides + 1) + ivyParameters.sides + 1 + num3;
					int value4 = j + i * (ivyParameters.sides + 1) + 1 + num3;
					int value5 = j + i * (ivyParameters.sides + 1) + ivyParameters.sides + 2 + num3;
					int value6 = j + i * (ivyParameters.sides + 1) + ivyParameters.sides + 1 + num3;
					buildingMeshData.AddTriangle(0, value);
					buildingMeshData.AddTriangle(0, value2);
					buildingMeshData.AddTriangle(0, value3);
					buildingMeshData.AddTriangle(0, value4);
					buildingMeshData.AddTriangle(0, value5);
					buildingMeshData.AddTriangle(0, value6);
					triCount += 6;
				}
			}

			int k = 0;
			int num4 = 0;
			while (k < ivyParameters.sides * 3)
			{
				buildingMeshData.AddTriangle(0, vertCount - 1);
				buildingMeshData.AddTriangle(0, vertCount - 3 - num4);
				buildingMeshData.AddTriangle(0, vertCount - 2 - num4);
				triCount += 3;
				k += 3;
				num4++;
			}

			lastTriangleIndexPerBranch[branchIndex] = vertCount - 1;
		}


		private void BuildLeaves(int branchIndex, RTBranchContainer buildingBranchContainer,
			RTBranchContainer bakedBranchContainer)
		{
			for (int i = Mathf.Clamp(buildingBranchContainer.branchPoints.Count - backtrackingPoints, 0, int.MaxValue);
				i < buildingBranchContainer.branchPoints.Count;
				i++)
			{
				RTLeafPoint[] array = bakedBranchContainer.leavesOrderedByInitSegment[i];
				RTBranchPoint rtbranchPoint = buildingBranchContainer.branchPoints[i];
				foreach (RTLeafPoint rtleafPoint in array)
				{
					if (rtleafPoint != null)
					{
						float t = Mathf.InverseLerp(buildingBranchContainer.totalLength,
							buildingBranchContainer.totalLength - ivyParameters.tipInfluence, rtbranchPoint.length);
						RTMeshData rtmeshData = leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
						for (int k = 0; k < rtmeshData.triangles[0].Length; k++)
						{
							int value = rtmeshData.triangles[0][k] + vertCount;
							int sumbesh = submeshByChoseLeaf[rtleafPoint.chosenLeave];
							buildingMeshData.AddTriangle(sumbesh, value);
						}

						for (int l = 0; l < rtleafPoint.vertices.Length; l++)
						{
							Vector3 vertexValue = Vector3.LerpUnclamped(rtleafPoint.leafCenter,
								rtleafPoint.vertices[l].vertex, t);
							buildingMeshData.AddVertex(vertexValue, rtleafPoint.vertices[l].normal,
								rtleafPoint.vertices[l].uv, rtleafPoint.vertices[l].color);
							vertCountLeavesPerBranch[branchIndex]++;
							vertCountsPerBranch[branchIndex]++;
							vertCount++;
						}
					}
				}
			}
		}


		public void CopyToFixedMesh(int branchIndex, int initSegmentIdx, int endSegmentIdx,
			RTBranchContainer branchContainer, RTBranchContainer bakedBranchContainer)
		{
			int num = ivyParameters.sides + 1;
			int sides = ivyParameters.sides;
			int num2 = vertCountsPerBranch[branchIndex];
			int num3 = vertCountLeavesPerBranch[branchIndex];
			int num4 = 0;
			for (int i = 1; i <= branchIndex; i++)
			{
				num4 += vertCountsPerBranch[branchIndex];
			}

			int num5;
			if (processedBranchesVerticesIndicesPerBranch[branchIndex].Count <= 0)
			{
				num5 = 2;
			}
			else
			{
				num5 = 1;
				num4 += num;
			}

			for (int j = num5 - 1; j >= 0; j--)
			{
				int index = branchContainer.branchPoints.Count - backtrackingPoints - j;
				RTBranchPoint rtbranchPoint = branchContainer.branchPoints[index];
				for (int k = 0; k < rtbranchPoint.verticesLoop.Length; k++)
				{
					RTVertexData rtvertexData = rtbranchPoint.verticesLoop[k];
					processedMeshData.AddVertex(rtvertexData.vertex, rtvertexData.normal, rtvertexData.uv,
						rtvertexData.color);
					processedBranchesVerticesIndicesPerBranch[branchIndex].Add(processedMeshData.VertexCount() - 1);
				}
			}

			if (branchIndex > 0)
			{
				int num6 = lastTriangleIndexPerBranch[branchIndex];
			}

			if (processedBranchesVerticesIndicesPerBranch[branchIndex].Count >= num * 2)
			{
				int num7 = processedBranchesVerticesIndicesPerBranch[branchIndex].Count - num * 2;
				for (int l = 0; l < ivyParameters.sides; l++)
				{
					int value = processedBranchesVerticesIndicesPerBranch[branchIndex][l + num7];
					int value2 = processedBranchesVerticesIndicesPerBranch[branchIndex][l + 1 + num7];
					int value3 =
						processedBranchesVerticesIndicesPerBranch[branchIndex][l + ivyParameters.sides + 1 + num7];
					int value4 = processedBranchesVerticesIndicesPerBranch[branchIndex][l + 1 + num7];
					int value5 =
						processedBranchesVerticesIndicesPerBranch[branchIndex][l + ivyParameters.sides + 2 + num7];
					int value6 =
						processedBranchesVerticesIndicesPerBranch[branchIndex][l + ivyParameters.sides + 1 + num7];
					processedMeshData.AddTriangle(0, value);
					processedMeshData.AddTriangle(0, value2);
					processedMeshData.AddTriangle(0, value3);
					processedMeshData.AddTriangle(0, value4);
					processedMeshData.AddTriangle(0, value5);
					processedMeshData.AddTriangle(0, value6);
				}
			}

			if (ivyParameters.generateLeaves)
			{
				int num8 = processedMeshData.VertexCount();
				int num9 = 0;
				for (int m = initSegmentIdx; m < endSegmentIdx; m++)
				{
					foreach (RTLeafPoint rtleafPoint in bakedBranchContainer.leavesOrderedByInitSegment[m])
					{
						if (rtleafPoint != null)
						{
							RTMeshData rtmeshData = leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
							int sumbesh = submeshByChoseLeaf[rtleafPoint.chosenLeave];
							for (int num10 = 0; num10 < rtmeshData.triangles[0].Length; num10++)
							{
								int value7 = rtmeshData.triangles[0][num10] + num8;
								processedMeshData.AddTriangle(sumbesh, value7);
							}

							for (int num11 = 0; num11 < rtleafPoint.vertices.Length; num11++)
							{
								RTVertexData rtvertexData2 = rtleafPoint.vertices[num11];
								processedMeshData.AddVertex(rtvertexData2.vertex, rtvertexData2.normal,
									rtvertexData2.uv, rtvertexData2.color);
								processedVerticesIndicesPerBranch[branchIndex].Add(processedMeshData.VertexCount() - 1);
								num8++;
							}

							num9++;
						}
					}
				}
			}
		}


		public void RefreshProcessedMesh()
		{
			processedMesh.MarkDynamic();
			processedMesh.subMeshCount = submeshCount;
			processedMesh.vertices = processedMeshData.vertices;
			processedMesh.normals = processedMeshData.normals;
			processedMesh.colors = processedMeshData.colors;
			processedMesh.uv = processedMeshData.uv;
			processedMesh.SetTriangles(processedMeshData.triangles[0], 0);
			if (ivyParameters.generateLeaves)
			{
				for (int i = 1; i < submeshCount; i++)
				{
					processedMesh.SetTriangles(processedMeshData.triangles[i], i);
				}
			}

			processedMesh.RecalculateBounds();
		}


		private void RefreshMesh()
		{
			ivyMesh.Clear();
			ivyMesh.subMeshCount = submeshCount;
			ivyMesh.MarkDynamic();
			ivyMesh.vertices = buildingMeshData.vertices;
			ivyMesh.normals = buildingMeshData.normals;
			ivyMesh.colors = buildingMeshData.colors;
			ivyMesh.uv = buildingMeshData.uv;
			ivyMesh.SetTriangles(buildingMeshData.triangles[0], 0);
			if (ivyParameters.generateLeaves)
			{
				for (int i = 1; i < submeshCount; i++)
				{
					ivyMesh.SetTriangles(buildingMeshData.triangles[i], i);
				}
			}

			ivyMesh.RecalculateBounds();
		}
	}
}