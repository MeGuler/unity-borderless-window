using UnityEngine;

public class TransparentMaker : MonoBehaviour
{
    public Material material;

   
    void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, material);
    }
}
