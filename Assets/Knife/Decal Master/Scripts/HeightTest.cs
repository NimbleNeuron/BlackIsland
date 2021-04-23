using UnityEngine;

[ExecuteAlways]
public class HeightTest : MonoBehaviour
{
    private void Update()
    {
        Terrain terrain = Terrain.activeTerrain;

        float height = terrain.SampleHeight(transform.position);
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;
    }
}
