using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PortalCameraSetup : MonoBehaviour
{
    [Header("El Material de tu Portal")]
    public Material portalMaterial; 

    private Camera portalCamera;

    void Start()
    {
        portalCamera = GetComponent<Camera>();
        CrearTexturaDinamica();
    }

    void CrearTexturaDinamica()
{
    if (portalCamera.targetTexture != null)
    {
        portalCamera.targetTexture.Release();
    }

    // 1. Usamos DefaultHDR para asegurar que los colores, brillos y sombras sean idénticos
    RenderTextureFormat formato = RenderTextureFormat.DefaultHDR;
    
    RenderTexture newTexture = new RenderTexture(Screen.width, Screen.height, 24, formato);
    
    // 2. Aplicamos Anti-Aliasing (x8) para eliminar los bordes de serrucho (pixelado)
    newTexture.antiAliasing = 8;
    
    // 3. Mejoramos el filtrado de la textura
    newTexture.filterMode = FilterMode.Bilinear;

    portalCamera.targetTexture = newTexture;

    if (portalMaterial != null)
    {
        portalMaterial.mainTexture = newTexture;
    }
}
}