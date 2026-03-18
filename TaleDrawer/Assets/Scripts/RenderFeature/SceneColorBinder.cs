using UnityEngine;

public class SceneColorBinder : MonoBehaviour
{
    public RenderTexture sceneColor;

    void LateUpdate()
    {
        Shader.SetGlobalTexture("_SceneColorTexture", sceneColor);
    }
}