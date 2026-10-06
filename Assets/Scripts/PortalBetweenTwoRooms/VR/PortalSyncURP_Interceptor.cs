using UnityEngine;
using UnityEngine.Rendering; // Necesario para URP
using UnityEngine.XR; // Necesario para detectar las gafas VR

public class PortalSyncURP_Interceptor : MonoBehaviour
{
    [Header("Tu Cámara Principal (La cabeza del jugador)")]
    public Camera mainCameraVR;

    [Header("El Rig de las Cámaras del Portal")]
    public Transform portalCamerasRig; 
    public float altura;

    [Header("Cámaras del Portal")]
    public Camera camaraIzquierda;
    public Camera camaraDerecha;
    [Header("Material")]
    public Material portalMaterial;

    void OnEnable()
    {
        // Esto se ejecuta JUSTO ANTES de que URP dibuje las cámaras
        RenderPipelineManager.beginCameraRendering += SincronizarAntesDeDibujar;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= SincronizarAntesDeDibujar;
    }

    void SincronizarAntesDeDibujar(ScriptableRenderContext context, Camera camera)
    {
        // Solo recalculamos si Unity está a punto de dibujar las texturas del portal
        if (camera == camaraIzquierda || camera == camaraDerecha)
        {
            if (mainCameraVR == null || portalCamerasRig == null) return;

            // 1. Posición y Rotación con cero latencia
            portalCamerasRig.rotation = mainCameraVR.transform.rotation;
            portalCamerasRig.position = new Vector3(
                mainCameraVR.transform.position.x, 
                mainCameraVR.transform.position.y + altura, 
                mainCameraVR.transform.position.z
            );

            // 2. Matrices ópticas actualizadas
            camaraIzquierda.projectionMatrix = mainCameraVR.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            camaraDerecha.projectionMatrix = mainCameraVR.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
        }
    }

    //Portal VR setup
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