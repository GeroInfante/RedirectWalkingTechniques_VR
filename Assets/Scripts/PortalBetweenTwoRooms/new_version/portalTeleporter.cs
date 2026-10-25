using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // Lo usamos SOLO para moverlo.
    public Transform xrOriginRoot; 

    public Transform destinationPortal;
    
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

    private void Teleport()
    {
        // Calculamos el offset basándonos en la posición actual de la raíz completa
        Vector3 positionOffset = xrOriginRoot.position - transform.position;
        
        // Movemos la raíz, lo que arrastrará a la cámara consigo manteniendo su posición relativa
        xrOriginRoot.position = destinationPortal.position + positionOffset;
    }
}