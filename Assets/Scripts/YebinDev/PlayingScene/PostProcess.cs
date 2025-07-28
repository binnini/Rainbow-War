using UnityEngine;

public class PostProcess : MonoBehaviour
{
    private Material material;
    public Shader shader;

    // Start is called before the first frame update
    void Start()
    {
        material = new Material(shader);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src,dest,material);
    }
}
