using UnityEngine;
using UnityEngine.Rendering; // Necesario para URP
using UnityEngine.XR; // Necesario para detectar las gafas VR

public class PortalTeleporter : MonoBehaviour
{
    [Header("Movimiento y Teletransporte")]
    public Transform xrOriginRoot; // Lo usamos SOLO para moverlo
    public PortalTeleporter destinationPortal;
    
    [Header("La cabeza del jugador")]
    public Camera playerCamera;

    [Header("El Rig de las Cámaras del Portal")]
    public vrPortalCamera portalCameraRig; 

    [Header("Renderizado Dinámico (Este Portal)")]
    public MeshRenderer portalPlaneRenderer; // El plano físico de ESTE portal
    public Shader portalShader; // Tu shader "Custom/PortalShaderURP_VR_Stereo"

    // Guardamos el material generado para que el OTRO portal pueda inyectarle sus texturas
    [HideInInspector]
    public Material portalMaterial; 

    private bool portalCameraStatus = true;

    void Awake()
    {
        if (portalShader != null && portalPlaneRenderer != null)
        {
            portalMaterial = new Material(portalShader);
            portalPlaneRenderer.material = portalMaterial;
        }
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += SincronizarAntesDeDibujar;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= SincronizarAntesDeDibujar;
    }

    void Start()
    {
        CrearTexturasVR();
    }

    void SincronizarAntesDeDibujar(ScriptableRenderContext context, Camera camera)
    {

        if (camera == portalCameraRig.leftCamera || camera == portalCameraRig.rightCamera)
        {
            if (playerCamera == null || portalCameraRig == null) return;

            // 1. Posición y Rotación con CERO LATENCIA
            Vector3 playerPositionRelativeToOtherPortal = destinationPortal.getCameraDistanceToPortal();
            portalCameraRig.transform.position = transform.position + playerPositionRelativeToOtherPortal;
            portalCameraRig.transform.rotation = playerCamera.transform.rotation;

            // // 2. Recorte Dinámico (Near Clip Plane)
            // Vector3 offsetAlPlano = transform.position - camera.transform.position;
            // float distanceToPortal = Vector3.Dot(offsetAlPlano, camera.transform.forward);
            // camera.nearClipPlane = Mathf.Max(0.01f, distanceToPortal + 0.05f);

            // 3. Matrices ópticas perfectas (estéreo)
            portalCameraRig.leftCamera.projectionMatrix = playerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            portalCameraRig.rightCamera.projectionMatrix = playerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
        }
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
        
        portalCameraRig.leftCamera.targetTexture = texIzq;
        destinationPortal.portalMaterial.SetTexture("_MainTex", texIzq);


        // 2. Preparamos la textura del Ojo Derecho
        RenderTexture texDer = new RenderTexture(width, height, 24, formato, RenderTextureReadWrite.Linear);
        texDer.useMipMap = false;
        texDer.filterMode = FilterMode.Point;
        texDer.antiAliasing = 2;
        
        portalCameraRig.rightCamera.targetTexture = texDer;
        destinationPortal.portalMaterial.SetTexture("_RightTex", texDer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered portal from front");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 playerHeadPosition = other.transform.position;
            Vector3 portalToHead = playerHeadPosition - transform.position;
            bool exitedFromFront = Vector3.Dot(transform.forward, portalToHead) > 0f;
            
            if (!exitedFromFront)
            {
                Teleport();
            }
        }
    }

    public void turnOnPortalCameras()
    {
        portalCameraStatus = true;
        portalCameraRig.leftCamera.enabled = true;
        portalCameraRig.rightCamera.enabled = true;
    }
    
    public void turnOffPortalCameras()
    {
        portalCameraStatus = false;
        portalCameraRig.leftCamera.enabled = false;
        portalCameraRig.rightCamera.enabled = false;
    }

    public Vector3 getCameraDistanceToPortal()
    {
        return playerCamera.transform.position - transform.position;
    }

    private void Teleport()
    {
        turnOnPortalCameras();
        Vector3 positionOffset = xrOriginRoot.position - transform.position;
        xrOriginRoot.position = destinationPortal.transform.position + positionOffset;
        destinationPortal.turnOffPortalCameras();
    }
}