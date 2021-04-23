using UnityEngine;
using UnityEditor;


public class TerrainMeshCreator : EditorWindow
{
    /// <summary>
    /// 0 - all, 1 - warning, 2 - error/critical
    /// </summary>
    private int _logLevel = 0;

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTriangles;

    private int resolution = 128;


    [MenuItem("Terrain/Create Mesh from Terrain")]
    static void Init()
    {
        TerrainMeshCreator window = (TerrainMeshCreator)EditorWindow.GetWindow(typeof(TerrainMeshCreator));
        window.Show();
    }


    void CreateMesh()
    {
        if (Selection.activeGameObject)
        {
            Terrain terrain = Selection.activeGameObject.GetComponent<Terrain>();

            if (terrain)
            {
                int l = (int)resolution;               // длина в точках
                int w = (int)resolution;               // ширина в точках
                                                       // Длинна, ширина террейна
                float width = terrain.terrainData.heightmapResolution * terrain.terrainData.heightmapScale.x;
                float length = terrain.terrainData.heightmapResolution * terrain.terrainData.heightmapScale.z;
                float height = 0f;
                LogMessage("Terrain width = " + width + ", length = " + length, 0);
                Vector3 terrainPos = terrain.transform.position;

                newVertices = new Vector3[l * w];
                newTriangles = new int[l * w * 2 * 3];        // 2 - два треугольника в каждой ячейке, 3 - три точки для каждого треугольника
                int tri = 0;    // счетчик для массива треугольников

                newUV = new Vector2[l * w];
                float uvStepL = 1f / (l - 1);
                float uvStepW = 1f / (w - 1);

                for (int i = 0; i < l; i++)     // цикл в длину
                {
                    for (int j = 0; j < w; j++)     // цикл в ширину
                    {
                        // Получаем высоту в точке
                        height = terrain.SampleHeight(new Vector3(terrainPos.x + width / (w - 1) * j, 0f, terrainPos.z + length / (l - 1) * i));
                        // Создаем вершины меша
                        newVertices[i * w + j] = new Vector3(width / (w - 1) * j, height, length / (l - 1) * i);
                        // Задаем текстурные координаты точек

                        newUV[i * w + j] = new Vector2(uvStepL * i, uvStepW * j);

                        // С каждой следующей точкой задаем 2 треугольника
                        // 1   2
                        // +---+
                        // |  /|
                        // | / |
                        // |/  |
                        // +---+(i,j)
                        // 3   4
                        if (i > 0 && j > 0)     // кроме крайних начальных точек
                        {
                            newTriangles[tri + 0] = i * w + j - 1;    // 3
                            newTriangles[tri + 1] = (i - 1) * w + j;              // 2
                            newTriangles[tri + 2] = (i - 1) * w + j - 1;    // 1

                            tri += 3;
                            newTriangles[tri + 0] = i * w + j;              // 4
                            newTriangles[tri + 1] = (i - 1) * w + j;              // 2
                            newTriangles[tri + 2] = i * w + j - 1;    // 3
                            tri += 3;
                        }
                    }
                }

                // Создаем меш
                Mesh mesh = new Mesh();
                mesh.vertices = newVertices;
                mesh.triangles = newTriangles;
                mesh.uv = newUV;
                mesh.RecalculateNormals();

                //                              // Инвертируем нормали
                //                              //Vector3[] newNormals = new Vector3[mesh.normals.Length];
                //                              int n = 0;
                //                              for (n = 0; n < mesh.normals.Length; n++)
                //                                      //newNormals[n]
                //                                      mesh.normals [n] = new Vector3(mesh.normals[n].x, mesh.normals[n].y * -1, mesh.normals[n].z);
                //                              //mesh.normals = newNormals;
                //                              LogMessage("calc normals: "+n, 0);
                //                              
                //                              //mesh.RecalculateNormals();

                mesh.RecalculateBounds();
                ;

                // Создаем го и присоединяем меш
                GameObject goMesh = new GameObject();
                goMesh.name = "New Terrain Mesh";
                MeshFilter meshFilter = goMesh.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                goMesh.AddComponent<MeshRenderer>();

                LogMessage("New Terrain Mesh is created!", 0);
            }
            else
                LogMessage("This GameObject is not Terrain", 1);
        }
        else
            LogMessage("Terrain is not selected", 1);
    }


    void OnGUI()
    {
        GUILayout.Label("Resolution (2-254):");
        resolution = int.Parse(GUILayout.TextField(resolution.ToString()));
        if (GUILayout.Button("Create Mesh"))
            CreateMesh();
    }


    /// <summary>
    /// Write to log file (0 - all, 1 - warning, 2 - error/critical)
    /// </summary>
    void LogMessage(string msg, int important)
    {
        if (_logLevel == 0)                                             // all messages
            Debug.Log(msg);
        else if (_logLevel == 1 && important >= 1)      // warning messages
            Debug.LogWarning(msg);
        else if (_logLevel >= 2 && important >= 2)      // error/critical messages
            Debug.LogError(msg);
    }
}