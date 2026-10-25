using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform xrOriginRoot; // Lo usamos SOLO para moverlo.

    public PortalTeleporter destinationPortal;
    public Transform playerCamera; // La cámara del jugador, para calcular la posición relativa al portal
    public vrPortalCamera portalCamera; // Referencia al script que controla las cámaras del portal
    
    private bool portalCameraStatus = true; // Estado de las cámaras del portal

    void Update()
    {
        if(portalCameraStatus)
        {
            Vector3 playerPositionRelativeToOtherPortal = destinationPortal.getCameraDistanceToPortal();
            portalCamera.transform.position = transform.position + playerPositionRelativeToOtherPortal;
            portalCamera.transform.rotation = playerCamera.rotation;
        }
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
        return playerCamera.position - transform.position;
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