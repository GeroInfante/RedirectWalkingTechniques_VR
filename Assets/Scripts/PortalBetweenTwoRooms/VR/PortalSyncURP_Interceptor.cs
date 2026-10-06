using UnityEngine;
using UnityEngine.Rendering; // Necesario para URP

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
}