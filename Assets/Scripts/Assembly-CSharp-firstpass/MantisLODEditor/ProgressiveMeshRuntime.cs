using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MantisLODEditor
{
	public class ProgressiveMeshRuntime : MonoBehaviour
	{
		public ProgressiveMesh progressiveMesh;


		public Text fpsHint;


		public Text lodHint;


		public Text triangleHint;


		[HideInInspector] public int[] mesh_lod_range;


		[HideInInspector] public bool never_cull = true;


		[HideInInspector] public int lod_strategy = 1;


		[HideInInspector] public float cull_ratio = 0.1f;


		[HideInInspector] public float disappear_distance = 30f;


		[HideInInspector] public float updateInterval = 0.25f;


		private readonly List<Renderer> allBasicRenderers = new List<Renderer>();


		private readonly List<MeshFilter> allFilters = new List<MeshFilter>();


		private readonly List<SkinnedMeshRenderer> allRenderers = new List<SkinnedMeshRenderer>();


		private readonly Dictionary<int, int> imap = new Dictionary<int, int>();


		private readonly Vector3[] sixPoints = new Vector3[6];


		private bool culled;


		private int current_lod = -1;


		private float currentTimeToInterval;


		private Camera mainCamera;


		private string[] mesh_uuids;


		private Mesh[] shared_meshes;


		private bool working;


		private void Awake()
		{
			get_all_meshes();
		}


		private void Start() { }


		private void Update()
		{
			if (progressiveMesh)
			{
				currentTimeToInterval -= Time.deltaTime;
				if (currentTimeToInterval <= 0f)
				{
					bool flag = false;
					if (!culled)
					{
						gameObject.GetComponentsInChildren<Renderer>(allBasicRenderers);
						using (List<Renderer>.Enumerator enumerator = allBasicRenderers.GetEnumerator())
						{
							if (enumerator.MoveNext() && enumerator.Current.isVisible)
							{
								flag = true;
							}
						}
					}

					if (culled || flag)
					{
						float num = 0f;
						Camera camera = GetCamera();
						if (camera != null && camera.gameObject != null && camera.gameObject.activeInHierarchy)
						{
							gameObject.GetComponentsInChildren<Renderer>(allBasicRenderers);
							if (lod_strategy == 0)
							{
								num = ratio_of_screen();
							}

							if (lod_strategy == 1)
							{
								num = ratio_of_distance(disappear_distance);
							}
						}

						if (!never_cull && num < cull_ratio)
						{
							if (!culled)
							{
								gameObject.GetComponentsInChildren<Renderer>(allBasicRenderers);
								foreach (Renderer renderer in allBasicRenderers)
								{
									renderer.enabled = false;
								}

								culled = true;
							}
						}
						else
						{
							if (culled)
							{
								gameObject.GetComponentsInChildren<Renderer>(allBasicRenderers);
								foreach (Renderer renderer2 in allBasicRenderers)
								{
									renderer2.enabled = true;
								}

								culled = false;
							}

							int num2 = progressiveMesh.triangles[0];
							int num3 = (int) ((1f - num) * num2);
							if (num3 > num2 - 1)
							{
								num3 = num2 - 1;
							}

							if (current_lod != num3)
							{
								int num4 = 0;
								int num5 = 0;
								foreach (MeshFilter meshFilter in allFilters)
								{
									string text = mesh_uuids[num5];
									int num6 = Array.IndexOf<string>(progressiveMesh.uuids, text);
									if (num6 != -1)
									{
										int num7 = num3;
										if (num7 < mesh_lod_range[num6 * 2])
										{
											num7 = mesh_lod_range[num6 * 2];
										}

										if (num7 > mesh_lod_range[num6 * 2 + 1])
										{
											num7 = mesh_lod_range[num6 * 2 + 1];
										}

										if (progressiveMesh.lod_meshes != null &&
										    progressiveMesh.lod_meshes.ContainsKey(text))
										{
											Lod_Mesh lod_Mesh = progressiveMesh.lod_meshes[text][num7];
											meshFilter.sharedMesh = lod_Mesh.mesh;
											num4 += lod_Mesh.triangle_count;
										}
									}

									num5++;
								}

								foreach (SkinnedMeshRenderer skinnedMeshRenderer in allRenderers)
								{
									string text2 = mesh_uuids[num5];
									int num8 = Array.IndexOf<string>(progressiveMesh.uuids, text2);
									if (num8 != -1)
									{
										int num9 = num3;
										if (num9 < mesh_lod_range[num8 * 2])
										{
											num9 = mesh_lod_range[num8 * 2];
										}

										if (num9 > mesh_lod_range[num8 * 2 + 1])
										{
											num9 = mesh_lod_range[num8 * 2 + 1];
										}

										if (progressiveMesh.lod_meshes != null &&
										    progressiveMesh.lod_meshes.ContainsKey(text2))
										{
											Lod_Mesh lod_Mesh2 = progressiveMesh.lod_meshes[text2][num9];
											skinnedMeshRenderer.sharedMesh = lod_Mesh2.mesh;
											num4 += lod_Mesh2.triangle_count;
										}
									}

									num5++;
								}

								if (lodHint)
								{
									lodHint.text = "Level Of Detail: " + num3;
								}

								if (triangleHint)
								{
									triangleHint.text = "Triangle Count: " + num4 / 3;
								}

								current_lod = num3;
							}
						}
					}

					if (fpsHint)
					{
						int num10 = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
						fpsHint.text = "FPS: " + num10;
					}

					currentTimeToInterval = updateInterval + (Random.value + 0.5f) * currentTimeToInterval;
				}
			}
		}


		private void OnEnable()
		{
			Awake();
			Start();
		}


		private void OnDisable()
		{
			clean_all();
		}


		private void OnDestroy()
		{
			clean_all();
		}


		private Camera GetCamera()
		{
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}

			return mainCamera;
		}


		private float ratio_of_screen()
		{
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Camera camera = GetCamera();
			foreach (Renderer renderer in allBasicRenderers)
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				float magnitude = bounds.extents.magnitude;
				sixPoints[0] = camera.WorldToScreenPoint(new Vector3(center.x - magnitude, center.y, center.z));
				sixPoints[1] = camera.WorldToScreenPoint(new Vector3(center.x + magnitude, center.y, center.z));
				sixPoints[2] = camera.WorldToScreenPoint(new Vector3(center.x, center.y - magnitude, center.z));
				sixPoints[3] = camera.WorldToScreenPoint(new Vector3(center.x, center.y + magnitude, center.z));
				sixPoints[4] = camera.WorldToScreenPoint(new Vector3(center.x, center.y, center.z - magnitude));
				sixPoints[5] = camera.WorldToScreenPoint(new Vector3(center.x, center.y, center.z + magnitude));
				foreach (Vector3 vector3 in sixPoints)
				{
					if (vector3.x < vector.x)
					{
						vector.x = vector3.x;
					}

					if (vector3.y < vector.y)
					{
						vector.y = vector3.y;
					}

					if (vector3.x > vector2.x)
					{
						vector2.x = vector3.x;
					}

					if (vector3.y > vector2.y)
					{
						vector2.y = vector3.y;
					}
				}
			}

			float num = (vector2.x - vector.x) / camera.pixelWidth;
			float num2 = (vector2.y - vector.y) / camera.pixelHeight;
			float num3 = num > num2 ? num : num2;
			if (num3 > 1f)
			{
				num3 = 1f;
			}

			return num3;
		}


		private float ratio_of_distance(float distance0)
		{
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			foreach (Renderer renderer in allBasicRenderers)
			{
				Bounds bounds = renderer.bounds;
				Vector3 center = bounds.center;
				float magnitude = bounds.extents.magnitude;
				sixPoints[0] = new Vector3(center.x - magnitude, center.y, center.z);
				sixPoints[1] = new Vector3(center.x + magnitude, center.y, center.z);
				sixPoints[2] = new Vector3(center.x, center.y - magnitude, center.z);
				sixPoints[3] = new Vector3(center.x, center.y + magnitude, center.z);
				sixPoints[4] = new Vector3(center.x, center.y, center.z - magnitude);
				sixPoints[5] = new Vector3(center.x, center.y, center.z + magnitude);
				foreach (Vector3 vector3 in sixPoints)
				{
					if (vector3.x < vector.x)
					{
						vector.x = vector3.x;
					}

					if (vector3.y < vector.y)
					{
						vector.y = vector3.y;
					}

					if (vector3.z < vector.z)
					{
						vector.z = vector3.z;
					}

					if (vector3.x > vector2.x)
					{
						vector2.x = vector3.x;
					}

					if (vector3.y > vector2.y)
					{
						vector2.y = vector3.y;
					}

					if (vector3.z > vector2.z)
					{
						vector2.z = vector3.z;
					}
				}
			}

			Vector3 b = (vector + vector2) * 0.5f;
			float num = Vector3.Distance(GetCamera().transform.position, b);
			float num2 = 1f - num / distance0;
			if (num2 < 0f)
			{
				num2 = 0f;
			}

			return num2;
		}


		public int get_triangles_count_from_progressive_mesh(int lod0, int mesh_count0)
		{
			int num = 0;
			int num2 = 0;
			int num3 = progressiveMesh.triangles[num2];
			num2++;
			for (int i = 0; i < num3; i++)
			{
				int num4 = progressiveMesh.triangles[num2];
				num2++;
				for (int j = 0; j < num4; j++)
				{
					int num5 = progressiveMesh.triangles[num2];
					num2++;
					for (int k = 0; k < num5; k++)
					{
						int num6 = progressiveMesh.triangles[num2];
						num2++;
						if (i == lod0 && j == mesh_count0)
						{
							num += num6;
						}

						num2 += num6;
					}
				}
			}

			return num / 3;
		}


		private int[] get_triangles_from_progressive_mesh(int lod0, int mesh_count0, int mat0)
		{
			int num = 0;
			int num2 = progressiveMesh.triangles[num];
			num++;
			for (int i = 0; i < num2; i++)
			{
				int num3 = progressiveMesh.triangles[num];
				num++;
				for (int j = 0; j < num3; j++)
				{
					int num4 = progressiveMesh.triangles[num];
					num++;
					for (int k = 0; k < num4; k++)
					{
						int num5 = progressiveMesh.triangles[num];
						num++;
						if (i == lod0 && j == mesh_count0 && k == mat0)
						{
							int[] array = new int[num5];
							Array.Copy(progressiveMesh.triangles, num, array, 0, num5);
							return array;
						}

						num += num5;
					}
				}
			}

			return null;
		}


		private void set_triangles(Mesh mesh, string uuid, int lod)
		{
			int num = Array.IndexOf<string>(progressiveMesh.uuids, uuid);
			if (num != -1)
			{
				for (int i = 0; i < mesh.subMeshCount; i++)
				{
					int[] triangles = get_triangles_from_progressive_mesh(lod, num, i);
					mesh.SetTriangles(triangles, i);
				}
			}
		}


		private void shrink_mesh(Mesh mesh)
		{
			Vector3[] vertices = mesh.vertices;
			Vector3[] array = null;
			if (vertices != null && vertices.Length != 0)
			{
				array = new Vector3[vertices.Length];
			}

			BoneWeight[] boneWeights = mesh.boneWeights;
			BoneWeight[] array2 = null;
			if (boneWeights != null && boneWeights.Length != 0)
			{
				array2 = new BoneWeight[boneWeights.Length];
			}

			Color[] colors = mesh.colors;
			Color[] array3 = null;
			if (colors != null && colors.Length != 0)
			{
				array3 = new Color[colors.Length];
			}

			Color32[] colors2 = mesh.colors32;
			Color32[] array4 = null;
			if (colors2 != null && colors2.Length != 0)
			{
				array4 = new Color32[colors2.Length];
			}

			Vector4[] tangents = mesh.tangents;
			Vector4[] array5 = null;
			if (tangents != null && tangents.Length != 0)
			{
				array5 = new Vector4[tangents.Length];
			}

			Vector3[] normals = mesh.normals;
			Vector3[] array6 = null;
			if (normals != null && normals.Length != 0)
			{
				array6 = new Vector3[normals.Length];
			}

			Vector2[] uv = mesh.uv;
			Vector2[] array7 = null;
			if (uv != null && uv.Length != 0)
			{
				array7 = new Vector2[uv.Length];
			}

			Vector2[] uv2 = mesh.uv2;
			Vector2[] array8 = null;
			if (uv2 != null && uv2.Length != 0)
			{
				array8 = new Vector2[uv2.Length];
			}

			int[][] array9 = new int[mesh.subMeshCount][];
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				array9[i] = mesh.GetTriangles(i);
			}

			imap.Clear();
			int num = 0;
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				int[] triangles = mesh.GetTriangles(j);
				for (int k = 0; k < triangles.Length; k += 3)
				{
					if (!imap.ContainsKey(triangles[k]))
					{
						if (array != null)
						{
							array[num] = vertices[triangles[k]];
						}

						if (array2 != null)
						{
							array2[num] = boneWeights[triangles[k]];
						}

						if (array3 != null)
						{
							array3[num] = colors[triangles[k]];
						}

						if (array4 != null)
						{
							array4[num] = colors2[triangles[k]];
						}

						if (array5 != null)
						{
							array5[num] = tangents[triangles[k]];
						}

						if (array6 != null)
						{
							array6[num] = normals[triangles[k]];
						}

						if (array7 != null)
						{
							array7[num] = uv[triangles[k]];
						}

						if (array8 != null)
						{
							array8[num] = uv2[triangles[k]];
						}

						imap.Add(triangles[k], num);
						num++;
					}

					if (!imap.ContainsKey(triangles[k + 1]))
					{
						if (array != null)
						{
							array[num] = vertices[triangles[k + 1]];
						}

						if (array2 != null)
						{
							array2[num] = boneWeights[triangles[k + 1]];
						}

						if (array3 != null)
						{
							array3[num] = colors[triangles[k + 1]];
						}

						if (array4 != null)
						{
							array4[num] = colors2[triangles[k + 1]];
						}

						if (array5 != null)
						{
							array5[num] = tangents[triangles[k + 1]];
						}

						if (array6 != null)
						{
							array6[num] = normals[triangles[k + 1]];
						}

						if (array7 != null)
						{
							array7[num] = uv[triangles[k + 1]];
						}

						if (array8 != null)
						{
							array8[num] = uv2[triangles[k + 1]];
						}

						imap.Add(triangles[k + 1], num);
						num++;
					}

					if (!imap.ContainsKey(triangles[k + 2]))
					{
						if (array != null)
						{
							array[num] = vertices[triangles[k + 2]];
						}

						if (array2 != null)
						{
							array2[num] = boneWeights[triangles[k + 2]];
						}

						if (array3 != null)
						{
							array3[num] = colors[triangles[k + 2]];
						}

						if (array4 != null)
						{
							array4[num] = colors2[triangles[k + 2]];
						}

						if (array5 != null)
						{
							array5[num] = tangents[triangles[k + 2]];
						}

						if (array6 != null)
						{
							array6[num] = normals[triangles[k + 2]];
						}

						if (array7 != null)
						{
							array7[num] = uv[triangles[k + 2]];
						}

						if (array8 != null)
						{
							array8[num] = uv2[triangles[k + 2]];
						}

						imap.Add(triangles[k + 2], num);
						num++;
					}
				}
			}

			mesh.Clear(false);
			if (array != null)
			{
				Vector3[] array10 = new Vector3[num];
				Array.Copy(array, array10, num);
				mesh.vertices = array10;
			}

			if (array2 != null)
			{
				BoneWeight[] array11 = new BoneWeight[num];
				Array.Copy(array2, array11, num);
				mesh.boneWeights = array11;
			}

			if (array3 != null)
			{
				Color[] array12 = new Color[num];
				Array.Copy(array3, array12, num);
				mesh.colors = array12;
			}

			if (array4 != null)
			{
				Color32[] array13 = new Color32[num];
				Array.Copy(array4, array13, num);
				mesh.colors32 = array13;
			}

			if (array5 != null)
			{
				Vector4[] array14 = new Vector4[num];
				Array.Copy(array5, array14, num);
				mesh.tangents = array14;
			}

			if (array6 != null)
			{
				Vector3[] array15 = new Vector3[num];
				Array.Copy(array6, array15, num);
				mesh.normals = array15;
			}

			if (array7 != null)
			{
				Vector2[] array16 = new Vector2[num];
				Array.Copy(array7, array16, num);
				mesh.uv = array16;
			}

			if (array8 != null)
			{
				Vector2[] array17 = new Vector2[num];
				Array.Copy(array8, array17, num);
				mesh.uv2 = array17;
			}

			mesh.subMeshCount = array9.Length;
			for (int l = 0; l < mesh.subMeshCount; l++)
			{
				int[] array18 = new int[array9[l].Length];
				for (int m = 0; m < array18.Length; m += 3)
				{
					array18[m] = imap[array9[l][m]];
					array18[m + 1] = imap[array9[l][m + 1]];
					array18[m + 2] = imap[array9[l][m + 2]];
				}

				mesh.SetTriangles(array18, l);
			}
		}


		private string get_uuid_from_mesh(Mesh mesh)
		{
			string text = string.Concat(mesh.name, "_", mesh.vertexCount.ToString(), "_", mesh.subMeshCount.ToString());
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				text = text + "_" + mesh.GetIndexCount(i);
			}

			return text;
		}


		private void create_default_mesh_lod_range()
		{
			int num = progressiveMesh.triangles[0];
			int num2 = progressiveMesh.triangles[1];
			mesh_lod_range = new int[num2 * 2];
			for (int i = 0; i < num2; i++)
			{
				mesh_lod_range[i * 2] = 0;
				mesh_lod_range[i * 2 + 1] = num - 1;
			}
		}


		private void get_all_meshes()
		{
			if (!working)
			{
				int num = progressiveMesh.triangles[0];
				if (mesh_lod_range == null || mesh_lod_range.Length == 0)
				{
					create_default_mesh_lod_range();
				}

				if (allFilters.Count == 0 && allRenderers.Count == 0)
				{
					gameObject.GetComponentsInChildren<MeshFilter>(allFilters);
					gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(allRenderers);
				}

				int num2 = allFilters.Count + allRenderers.Count;
				if (num2 > 0)
				{
					shared_meshes = new Mesh[num2];
					mesh_uuids = new string[num2];
					int num3 = 0;
					foreach (MeshFilter meshFilter in allFilters)
					{
						string text = get_uuid_from_mesh(meshFilter.sharedMesh);
						mesh_uuids[num3] = text;
						shared_meshes[num3] = meshFilter.sharedMesh;
						if (progressiveMesh.lod_meshes == null)
						{
							progressiveMesh.lod_meshes = new Dictionary<string, Lod_Mesh[]>();
						}

						if (!progressiveMesh.lod_meshes.ContainsKey(text) &&
						    Array.IndexOf<string>(progressiveMesh.uuids, text) != -1)
						{
							Lod_Mesh[] array = new Lod_Mesh[num];
							for (int i = 0; i < num; i++)
							{
								array[i] = new Lod_Mesh();
								array[i].mesh = Instantiate<Mesh>(meshFilter.sharedMesh);
								set_triangles(array[i].mesh, text, i);
								if (array[i].mesh.blendShapeCount == 0)
								{
									shrink_mesh(array[i].mesh);
								}

								array[i].mesh.Optimize();
								array[i].triangle_count = array[i].mesh.triangles.Length;
							}

							progressiveMesh.lod_meshes.Add(text, array);
						}

						num3++;
					}

					foreach (SkinnedMeshRenderer skinnedMeshRenderer in allRenderers)
					{
						string text2 = get_uuid_from_mesh(skinnedMeshRenderer.sharedMesh);
						mesh_uuids[num3] = text2;
						shared_meshes[num3] = skinnedMeshRenderer.sharedMesh;
						if (progressiveMesh.lod_meshes == null)
						{
							progressiveMesh.lod_meshes = new Dictionary<string, Lod_Mesh[]>();
						}

						if (!progressiveMesh.lod_meshes.ContainsKey(text2) &&
						    Array.IndexOf<string>(progressiveMesh.uuids, text2) != -1)
						{
							Lod_Mesh[] array2 = new Lod_Mesh[num];
							for (int j = 0; j < num; j++)
							{
								array2[j] = new Lod_Mesh();
								array2[j].mesh = Instantiate<Mesh>(skinnedMeshRenderer.sharedMesh);
								set_triangles(array2[j].mesh, text2, j);
								if (array2[j].mesh.blendShapeCount == 0)
								{
									shrink_mesh(array2[j].mesh);
								}

								array2[j].mesh.Optimize();
								array2[j].triangle_count = array2[j].mesh.triangles.Length;
							}

							progressiveMesh.lod_meshes.Add(text2, array2);
						}

						num3++;
					}
				}

				gameObject.GetComponentsInChildren<Renderer>(allBasicRenderers);
				currentTimeToInterval = Random.value * updateInterval;
				current_lod = -1;
				working = true;
			}
		}


		public void reset_all_parameters()
		{
			mesh_lod_range = null;
			never_cull = true;
			lod_strategy = 1;
			cull_ratio = 0.1f;
			disappear_distance = 250f;
			updateInterval = 0.25f;
		}


		private void clean_all()
		{
			if (working)
			{
				if (allFilters.Count + allRenderers.Count > 0)
				{
					int num = 0;
					foreach (MeshFilter meshFilter in allFilters)
					{
						meshFilter.sharedMesh = shared_meshes[num];
						num++;
					}

					foreach (SkinnedMeshRenderer skinnedMeshRenderer in allRenderers)
					{
						skinnedMeshRenderer.sharedMesh = shared_meshes[num];
						num++;
					}
				}

				shared_meshes = null;
				allBasicRenderers.Clear();
				allFilters.Clear();
				allRenderers.Clear();
				if (progressiveMesh.lod_meshes != null)
				{
					foreach (Lod_Mesh[] array in progressiveMesh.lod_meshes.Values)
					{
						for (int i = 0; i < array.Length; i++)
						{
							DestroyImmediate(array[i].mesh);
						}
					}

					progressiveMesh.lod_meshes = null;
				}

				working = false;
			}
		}
	}
}