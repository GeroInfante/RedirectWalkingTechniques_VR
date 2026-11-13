using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VRPortal : MonoBehaviour
{
    public Transform xrOriginRoot; // Lo usamos SOLO para moverlo.

    public VRPortal destinationPortal;
    public Camera playerCamera; // La cámara del jugador, para calcular la posición relativa al portal
    public vrPortalCamera portalCamera; // Referencia al script que controla las cámaras del portal
    
    private bool portalCameraStatus = true; // Estado de las cámaras del portal

    //Para mostrar 
    public MeshRenderer portalScreenRenderer;

    private void OnEnable()
    {
        RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
    }

    // private void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    // {
    //     if(portalCameraStatus)
    //     {
    //         Vector3 playerPositionRelativeToOtherPortal = destinationPortal.getCameraDistanceToPortal();
    //         portalCamera.transform.position = transform.position + playerPositionRelativeToOtherPortal;
    //         portalCamera.transform.rotation = playerCamera.rotation;
    //     }
    //     // // 4. Sincronizamos el campo de visión y aplicamos recorte oblicuo
    //     // otherPortalCam.fieldOfView = mainCam.fieldOfView;
    //     // ApplyObliqueNearClipPlane();
    // }
    private void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (!portalCameraStatus || !cameras.Contains(playerCamera)) return;
        Vector3 playerPositionRelativeToOtherPortal = destinationPortal.getCameraDistanceToPortal();
        portalCamera.transform.position = transform.position + playerPositionRelativeToOtherPortal;
        portalCamera.transform.rotation = playerCamera.transform.rotation;

        // Aplicamos las matrices estéreo exactas de cada ojo
        portalCamera.SyncStereoEyes(playerCamera, transform, destinationPortal.transform);
    }

    private void Start()
    {
        Material portalMat = portalScreenRenderer.material;
        portalMat.SetTexture("_leftTex", destinationPortal.portalCamera.leftRenderTexture);
        portalMat.SetTexture("_RightTex", destinationPortal.portalCamera.rightRenderTexture);
    }

    public void HandleTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered portal from front");
        }
    }

    public void HandleTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 playerHeadPosition = other.transform.position;
            Vector3 portalToHead = playerHeadPosition - transform.position;
            bool exitedFromFront = Vector3.Dot(transform.forward, portalToHead) > 0f;
            
            Debug.Log("exitedFromFront: " + exitedFromFront);
            
            if (!exitedFromFront)
            {
                Teleport();
            }
        }
    }

    public void turnOnPortalCameras()
    {
        portalCameraStatus = true;
        portalCamera.turnOnCameras();
    }
    public void turnOffPortalCameras()
    {
        portalCameraStatus = false;
        portalCamera.turnOffCameras();
    }
    public Vector3 getCameraDistanceToPortal()
    {
        return playerCamera.transform.position - transform.position;
    }

    private void Teleport()
    {
        turnOnPortalCameras();
        // Calculamos el offset basándonos en la posición actual de la raíz completa
        Vector3 positionOffset = xrOriginRoot.position - transform.position;
        
        // Movemos la raíz, lo que arrastrará a la cámara consigo manteniendo su posición relativa
        xrOriginRoot.position = destinationPortal.transform.position + positionOffset;
        destinationPortal.turnOffPortalCameras();
    }

}