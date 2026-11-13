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
    private static readonly Matrix4x4 PortalFlip = Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f));
    //Para mostrar 
    public MeshRenderer portalScreenRenderer;

    private void OnEnable()
    {
        RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
        Application.onBeforeRender += OnBeforeRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
        Application.onBeforeRender -= OnBeforeRender;
    }

    private void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (!IsReady() || !cameras.Contains(playerCamera)) return;
        SyncPortalView();
    }

    private void OnBeforeRender()
    {
        if (IsReady())
            SyncPortalView();
    }

    private bool IsReady()
    {
        return portalCameraStatus &&
               portalCamera != null &&
               playerCamera != null &&
               destinationPortal != null;
    }

    private void SyncPortalView()
    {
        Matrix4x4 destToThis = transform.localToWorldMatrix * PortalFlip * destinationPortal.transform.worldToLocalMatrix;

        portalCamera.transform.SetPositionAndRotation(
            destToThis.MultiplyPoint3x4(playerCamera.transform.position),
            destToThis.rotation * playerCamera.transform.rotation
        );

        portalCamera.SyncStereoEyes(playerCamera, destToThis, this.transform);
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