using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // Asigna el "VR Player" (raíz) aquí. Lo usamos SOLO para moverlo.
    public Transform xrOriginRoot; 
    
    // Asigna la "Main Camera" aquí. La usamos SOLO para calcular la posición.
    public Transform headCamera; 

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
            // OJO AQUÍ: Calculamos de qué lado está saliendo usando la CÁMARA, no la raíz
            Vector3 portalToHead = headCamera.position - transform.position;
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