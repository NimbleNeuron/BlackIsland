using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.RealIvy
{
	public class EditorMeshBuilder : ScriptableObject
	{
		public InfoPool infoPool;


		public Mesh ivyMesh;


		public Vector3[] verts;


		public List<Material> leavesMaterials;


		public Rect[] uv2Rects = new Rect[0];


		public bool leavesDataInitialized;


		private readonly Dictionary<int, int[]> branchesLeavesIndices = new Dictionary<int, int[]>();


		private float angle;


		private Vector3[] normals;


		private int[] trisBranches;


		private List<List<int>> trisLeaves;


		public List<List<int>> typesByMat;


		private Vector2[] uvs;


		private Color[] vColor;


		public void InitLeavesData()
		{
			if (infoPool.ivyContainer.ivyGO)
			{
				infoPool.meshBuilder.typesByMat = new List<List<int>>();
				infoPool.meshBuilder.leavesMaterials = new List<Material>();
				if (infoPool.ivyParameters.generateLeaves)
				{
					for (int i = 0; i < infoPool.ivyParameters.leavesPrefabs.Length; i++)
					{
						bool flag = false;
						for (int j = 0; j < infoPool.meshBuilder.leavesMaterials.Count; j++)
						{
							if (infoPool.meshBuilder.leavesMaterials[j] == infoPool.ivyParameters.leavesPrefabs[i]
								.GetComponent<MeshRenderer>().sharedMaterial)
							{
								infoPool.meshBuilder.typesByMat[j].Add(i);
								flag = true;
							}
						}

						if (!flag)
						{
							infoPool.meshBuilder.leavesMaterials.Add(infoPool.ivyParameters.leavesPrefabs[i]
								.GetComponent<MeshRenderer>().sharedMaterial);
							infoPool.meshBuilder.typesByMat.Add(new List<int>());
							infoPool.meshBuilder.typesByMat[infoPool.meshBuilder.typesByMat.Count - 1].Add(i);
						}
					}

					Material[] array = new Material[leavesMaterials.Count + 1];
					for (int k = 0; k < array.Length; k++)
					{
						if (k == 0)
						{
							array[k] = infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterial;
						}
						else
						{
							array[k] = infoPool.meshBuilder.leavesMaterials[k - 1];
						}
					}

					infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterials = array;
				}
				else
				{
					infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterials = new[]
					{
						infoPool.ivyParameters.branchesMaterial
					};
				}

				leavesDataInitialized = true;
			}
		}


		public void Initialize()
		{
			infoPool.meshBuilder.trisLeaves = new List<List<int>>();
			for (int i = 0; i < infoPool.meshBuilder.leavesMaterials.Count; i++)
			{
				infoPool.meshBuilder.trisLeaves.Add(new List<int>());
			}

			ivyMesh.Clear();
			if (infoPool.ivyParameters.buffer32Bits)
			{
				ivyMesh.indexFormat = IndexFormat.UInt32;
			}

			ivyMesh.name = "Ivy Mesh";
			ivyMesh.subMeshCount = leavesMaterials.Count + 1;
			branchesLeavesIndices.Clear();
			int num = 0;
			int num2 = 0;
			if (infoPool.ivyParameters.generateBranches)
			{
				for (int j = 0; j < infoPool.ivyContainer.branches.Count; j++)
				{
					if (infoPool.ivyContainer.branches[j].branchPoints.Count > 1)
					{
						num += (infoPool.ivyContainer.branches[j].branchPoints.Count - 1) *
							(infoPool.ivyParameters.sides + 1) + 1;
						num2 += (infoPool.ivyContainer.branches[j].branchPoints.Count - 2) *
							infoPool.ivyParameters.sides * 2 * 3 + infoPool.ivyParameters.sides * 3;
					}
				}
			}

			if (infoPool.ivyParameters.generateLeaves && infoPool.ivyParameters.leavesPrefabs.Length != 0)
			{
				for (int k = 0; k < infoPool.ivyContainer.branches.Count; k++)
				{
					if (infoPool.ivyContainer.branches[k].branchPoints.Count > 1)
					{
						for (int l = 0; l < infoPool.ivyContainer.branches[k].leaves.Count; l++)
						{
							BranchContainer branchContainer = infoPool.ivyContainer.branches[k];
							MeshFilter component = infoPool.ivyParameters
								.leavesPrefabs[branchContainer.leaves[l].chosenLeave].GetComponent<MeshFilter>();
							num += component.sharedMesh.vertexCount;
						}
					}
				}
			}

			verts = new Vector3[num];
			normals = new Vector3[num];
			uvs = new Vector2[num];
			vColor = new Color[num];
			trisBranches = new int[Mathf.Max(num2, 0)];
			if (!infoPool.ivyParameters.halfgeom)
			{
				angle = 360f / infoPool.ivyParameters.sides;
				return;
			}

			angle = 360f / infoPool.ivyParameters.sides / 2f;
		}


		private void BuildLeaves(int b, ref int vertCount)
		{
			for (int i = 0; i < leavesMaterials.Count; i++)
			{
				Random.InitState(b + infoPool.ivyParameters.randomSeed + i);
				for (int j = 0; j < infoPool.ivyContainer.branches[b].leaves.Count; j++)
				{
					LeafPoint leafPoint = infoPool.ivyContainer.branches[b].leaves[j];
					if (typesByMat[i].Contains(leafPoint.chosenLeave))
					{
						leafPoint.verticesLeaves = new List<RTVertexData>();
						Mesh sharedMesh = infoPool.ivyParameters.leavesPrefabs[leafPoint.chosenLeave]
							.GetComponent<MeshFilter>().sharedMesh;
						int num = vertCount;
						Vector3 vector;
						Vector3 vector2;
						if (!infoPool.ivyParameters.globalOrientation)
						{
							vector = leafPoint.lpForward;
							vector2 = leafPoint.left;
						}
						else
						{
							vector = infoPool.ivyParameters.globalRotation;
							vector2 = Vector3.Normalize(Vector3.Cross(infoPool.ivyParameters.globalRotation,
								leafPoint.lpUpward));
						}

						Quaternion quaternion = Quaternion.LookRotation(leafPoint.lpUpward, vector);
						quaternion = Quaternion.AngleAxis(infoPool.ivyParameters.rotation.x, vector2) *
						             Quaternion.AngleAxis(infoPool.ivyParameters.rotation.y, leafPoint.lpUpward) *
						             Quaternion.AngleAxis(infoPool.ivyParameters.rotation.z, vector) * quaternion;
						quaternion =
							Quaternion.AngleAxis(
								Random.Range(-infoPool.ivyParameters.randomRotation.x,
									infoPool.ivyParameters.randomRotation.x), vector2) *
							Quaternion.AngleAxis(
								Random.Range(-infoPool.ivyParameters.randomRotation.y,
									infoPool.ivyParameters.randomRotation.y), leafPoint.lpUpward) *
							Quaternion.AngleAxis(
								Random.Range(-infoPool.ivyParameters.randomRotation.z,
									infoPool.ivyParameters.randomRotation.z), vector) * quaternion;
						quaternion = leafPoint.forwarRot * quaternion;
						float num2 = Random.Range(infoPool.ivyParameters.minScale, infoPool.ivyParameters.maxScale);
						leafPoint.leafScale = num2;
						num2 *= Mathf.InverseLerp(infoPool.ivyContainer.branches[b].totalLenght,
							infoPool.ivyContainer.branches[b].totalLenght - infoPool.ivyParameters.tipInfluence,
							leafPoint.lpLength);
						leafPoint.leafRotation = quaternion;
						leafPoint.dstScale = num2;
						for (int k = 0; k < sharedMesh.triangles.Length; k++)
						{
							int item = sharedMesh.triangles[k] + vertCount;
							trisLeaves[i].Add(item);
						}

						for (int l = 0; l < sharedMesh.vertexCount; l++)
						{
							Vector3 b2 = vector2 * infoPool.ivyParameters.offset.x +
							             leafPoint.lpUpward * infoPool.ivyParameters.offset.y +
							             leafPoint.lpForward * infoPool.ivyParameters.offset.z;
							verts[vertCount] = quaternion * sharedMesh.vertices[l] * num2 + leafPoint.point + b2;
							normals[vertCount] = quaternion * sharedMesh.normals[l];
							uvs[vertCount] = sharedMesh.uv[l];
							vColor[vertCount] = sharedMesh.colors[l];
							normals[vertCount] = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
							                     normals[vertCount];
							verts[vertCount] -= infoPool.ivyContainer.ivyGO.transform.position;
							verts[vertCount] = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
							                   verts[vertCount];
							RTVertexData item2 = new RTVertexData(verts[vertCount], normals[vertCount], uvs[vertCount],
								Vector2.zero, vColor[vertCount]);
							leafPoint.verticesLeaves.Add(item2);
							leafPoint.leafCenter = leafPoint.point - infoPool.ivyContainer.ivyGO.transform.position;
							leafPoint.leafCenter = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
							                       leafPoint.leafCenter;
							vertCount++;
						}

						int[] value =
						{
							num,
							vertCount - 1
						};
						branchesLeavesIndices.Add(branchesLeavesIndices.Count, value);
					}
				}
			}
		}


		public void BuildGeometry()
		{
			if (leavesDataInitialized)
			{
				Initialize();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < infoPool.ivyContainer.branches.Count; i++)
				{
					int num3 = num;
					Random.InitState(i + infoPool.ivyParameters.randomSeed);
					if (infoPool.ivyContainer.branches[i].branchPoints.Count > 1)
					{
						int num4 = 0;
						for (int j = 0; j < infoPool.ivyContainer.branches[i].branchPoints.Count; j++)
						{
							BranchPoint branchPoint = infoPool.ivyContainer.branches[i].branchPoints[j];
							branchPoint.verticesLoop = new List<RTVertexData>();
							Vector3 vector = branchPoint.point - infoPool.ivyContainer.ivyGO.transform.position;
							vector = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) * vector;
							float num5 = CalculateRadius(branchPoint.length,
								infoPool.ivyContainer.branches[i].totalLenght);
							branchPoint.radius = num5;
							if (j != infoPool.ivyContainer.branches[i].branchPoints.Count - 1)
							{
								Vector3[] array =
									CalculateVectors(infoPool.ivyContainer.branches[i].branchPoints[j].point, j, i);
								branchPoint.firstVector = array[0];
								branchPoint.axis = array[1];
								for (int k = 0; k < infoPool.ivyParameters.sides + 1; k++)
								{
									if (infoPool.ivyParameters.generateBranches)
									{
										float tipInfluence = GetTipInfluence(branchPoint.length,
											infoPool.ivyContainer.branches[i].totalLenght);
										infoPool.ivyContainer.branches[i].branchPoints[j].radius = num5;
										Vector3 vector2 = Quaternion.AngleAxis(angle * k, array[1]) * array[0];
										if (infoPool.ivyParameters.halfgeom && infoPool.ivyParameters.sides == 1)
										{
											normals[num] = -infoPool.ivyContainer.branches[i].branchPoints[j]
												.grabVector;
										}
										else
										{
											normals[num] = vector2;
										}

										Vector3 vertex = vector2 * num5 + vector;
										verts[num] = vector2 * num5 * tipInfluence +
										             infoPool.ivyContainer.branches[i].branchPoints[j].point;
										verts[num] -= infoPool.ivyContainer.ivyGO.transform.position;
										verts[num] =
											Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
											verts[num];
										uvs[num] = new Vector2(
											branchPoint.length * infoPool.ivyParameters.uvScale.y +
											infoPool.ivyParameters.uvOffset.y - infoPool.ivyParameters.stepSize,
											1f / infoPool.ivyParameters.sides * k * infoPool.ivyParameters.uvScale.x +
											infoPool.ivyParameters.uvOffset.x);
										normals[num] =
											Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
											normals[num];
										RTVertexData item = new RTVertexData(vertex, normals[num], uvs[num],
											Vector2.zero, vColor[num]);
										branchPoint.verticesLoop.Add(item);
										num++;
										num4++;
									}
								}
							}
							else if (infoPool.ivyParameters.generateBranches)
							{
								verts[num] = infoPool.ivyContainer.branches[i].branchPoints[j].point;
								verts[num] -= infoPool.ivyContainer.ivyGO.transform.position;
								verts[num] = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
								             verts[num];
								if (infoPool.ivyParameters.halfgeom && infoPool.ivyParameters.sides == 1)
								{
									normals[num] = -infoPool.ivyContainer.branches[i].branchPoints[j].grabVector;
								}
								else
								{
									normals[num] = Vector3.Normalize(
										infoPool.ivyContainer.branches[i].branchPoints[j].point -
										infoPool.ivyContainer.branches[i].branchPoints[j - 1].point);
								}

								uvs[num] = new Vector2(
									infoPool.ivyContainer.branches[i].totalLenght * infoPool.ivyParameters.uvScale.y +
									infoPool.ivyParameters.uvOffset.y,
									0.5f * infoPool.ivyParameters.uvScale.x + infoPool.ivyParameters.uvOffset.x);
								normals[num] = Quaternion.Inverse(infoPool.ivyContainer.ivyGO.transform.rotation) *
								               normals[num];
								Vector3 vertex2 = vector;
								RTVertexData item2 = new RTVertexData(vertex2, normals[num], uvs[num], Vector2.zero,
									vColor[num]);
								branchPoint.verticesLoop.Add(item2);
								num++;
								num4++;
								TriangulateBranch(i, ref num2, num, num4);
							}
						}
					}

					int[] value =
					{
						num3,
						num - 1
					};
					branchesLeavesIndices.Add(branchesLeavesIndices.Count, value);
					if (infoPool.ivyParameters.generateLeaves)
					{
						BuildLeaves(i, ref num);
					}
				}

				ivyMesh.vertices = verts;
				ivyMesh.normals = normals;
				ivyMesh.uv = uvs;
				ivyMesh.colors = vColor;
				ivyMesh.SetTriangles(trisBranches, 0);
				for (int l = 0; l < leavesMaterials.Count; l++)
				{
					ivyMesh.SetTriangles(trisLeaves[l], l + 1);
				}

				ivyMesh.RecalculateTangents();
				ivyMesh.RecalculateBounds();
			}
		}


		private Vector3[] CalculateVectors(Vector3 branchPoint, int p, int b)
		{
			Vector3 vector;
			Vector3 vector2;
			if (b == 0 && p == 0)
			{
				vector = infoPool.ivyContainer.ivyGO.transform.up;
				if (!infoPool.ivyParameters.halfgeom)
				{
					vector2 = infoPool.ivyContainer.firstVertexVector;
				}
				else
				{
					vector2 = Quaternion.AngleAxis(90f, vector) * infoPool.ivyContainer.firstVertexVector;
				}
			}
			else
			{
				if (p == 0)
				{
					vector = infoPool.ivyContainer.branches[b].branchPoints[p + 1].point -
					         infoPool.ivyContainer.branches[b].branchPoints[p].point;
				}
				else
				{
					vector = Vector3.Normalize(Vector3.Lerp(
						infoPool.ivyContainer.branches[b].branchPoints[p].point -
						infoPool.ivyContainer.branches[b].branchPoints[p - 1].point,
						infoPool.ivyContainer.branches[b].branchPoints[p + 1].point -
						infoPool.ivyContainer.branches[b].branchPoints[p].point, 0.5f));
				}

				if (!infoPool.ivyParameters.halfgeom)
				{
					vector2 = Vector3.Normalize(
						Vector3.ProjectOnPlane(infoPool.ivyContainer.branches[b].branchPoints[p].grabVector, vector));
				}
				else
				{
					vector2 = Quaternion.AngleAxis(90f, vector) * Vector3.Normalize(
						Vector3.ProjectOnPlane(infoPool.ivyContainer.branches[b].branchPoints[p].grabVector, vector));
				}
			}

			return new[]
			{
				vector2,
				vector
			};
		}


		private float CalculateRadius(float lenght, float totalLenght)
		{
			float t =
				(Mathf.Sin(lenght * infoPool.ivyParameters.radiusVarFreq + infoPool.ivyParameters.radiusVarOffset) +
				 1f) / 2f;
			return Mathf.Lerp(infoPool.ivyParameters.minRadius, infoPool.ivyParameters.maxRadius, t);
		}


		private float GetTipInfluence(float lenght, float totalLenght)
		{
			float result = 1f;
			if (lenght - 0.1f >= totalLenght - infoPool.ivyParameters.tipInfluence)
			{
				result = Mathf.InverseLerp(totalLenght, totalLenght - infoPool.ivyParameters.tipInfluence,
					lenght - 0.1f);
			}

			return result;
		}


		private void TriangulateBranch(int b, ref int triCount, int vertCount, int lastVertCount)
		{
			for (int i = 0; i < infoPool.ivyContainer.branches[b].branchPoints.Count - 2; i++)
			{
				for (int j = 0; j < infoPool.ivyParameters.sides; j++)
				{
					trisBranches[triCount] = j + i * (infoPool.ivyParameters.sides + 1) + vertCount - lastVertCount;
					trisBranches[triCount + 1] =
						j + i * (infoPool.ivyParameters.sides + 1) + 1 + vertCount - lastVertCount;
					trisBranches[triCount + 2] = j + i * (infoPool.ivyParameters.sides + 1) +
						infoPool.ivyParameters.sides + 1 + vertCount - lastVertCount;
					trisBranches[triCount + 3] =
						j + i * (infoPool.ivyParameters.sides + 1) + 1 + vertCount - lastVertCount;
					trisBranches[triCount + 4] = j + i * (infoPool.ivyParameters.sides + 1) +
						infoPool.ivyParameters.sides + 2 + vertCount - lastVertCount;
					trisBranches[triCount + 5] = j + i * (infoPool.ivyParameters.sides + 1) +
						infoPool.ivyParameters.sides + 1 + vertCount - lastVertCount;
					triCount += 6;
				}
			}

			int k = 0;
			int num = 0;
			while (k < infoPool.ivyParameters.sides * 3)
			{
				trisBranches[triCount] = vertCount - 1;
				trisBranches[triCount + 1] = vertCount - 3 - num;
				trisBranches[triCount + 2] = vertCount - 2 - num;
				triCount += 3;
				k += 3;
				num++;
			}
		}
	}
}