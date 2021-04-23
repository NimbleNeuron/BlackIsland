using UnityEngine;

[ExecuteAlways]
public class ReplacementTest : MonoBehaviour
{
    public Shader Shader;
    public string Tag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Shader != null)
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<Camera>().RenderWithShader(Shader, Tag);
        }
    }
}
