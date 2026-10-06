using UnityEngine;
using UnityEngine.XR; // Necesario para detectar las gafas VR

public class PortalVRSetup : MonoBehaviour
{
    [Header("Cámaras del Portal")]
    public Camera camaraIzquierda;
    public Camera camaraDerecha;
    
    [Header("Material")]
    public Material portalMaterial;

    void Start()
    {
        CrearTexturasVR();
    }

    void CrearTexturasVR()
    {
        // Obtenemos la resolución nativa de las lentes de la Quest 2
        int width = XRSettings.eyeTextureWidth;
        int height = XRSettings.eyeTextureHeight;

        // Fallback por si ejecutas el juego sin las gafas puestas
        if (width == 0) { width = 1920; height = 1832; }

        RenderTextureFormat formato = RenderTextureFormat.DefaultHDR;

        // 1. Preparamos la textura del Ojo Izquierdo
        RenderTexture texIzq = new RenderTexture(width, height, 24, formato, RenderTextureReadWrite.Linear);
        texIzq.useMipMap = false;
        texIzq.filterMode = FilterMode.Point;
        texIzq.antiAliasing = 2; // Puedes bajarlo a 2 si el juego va lento
        
        camaraIzquierda.targetTexture = texIzq;
        portalMaterial.SetTexture("_MainTex", texIzq);

        // 2. Preparamos la textura del Ojo Derecho
        RenderTexture texDer = new RenderTexture(width, height, 24, formato, RenderTextureReadWrite.Linear);
        texDer.useMipMap = false;
        texDer.filterMode = FilterMode.Point;
        texDer.antiAliasing = 2;
        
        camaraDerecha.targetTexture = texDer;
        portalMaterial.SetTexture("_RightTex", texDer);
    }
}