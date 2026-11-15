using UnityEngine;

namespace BrineUIFix.Monobehaviors;

internal class UICameraCopier : MonoBehaviour
{
    public static UICameraCopier Instance { get; private set; }

    public RenderTexture uiRenderTex;
    private Camera camera;
    
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (uiRenderTex != null) uiRenderTex.Release();

        Graphics.Blit(camera.activeTexture, destination);
        uiRenderTex = destination;
    }
}
