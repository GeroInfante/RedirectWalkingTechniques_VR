using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public VRPortal parentPortal;
    
    private void OnTriggerEnter(Collider other)
    {
        if (parentPortal != null)
            parentPortal.HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (parentPortal != null)
            parentPortal.HandleTriggerExit(other);
    }
}