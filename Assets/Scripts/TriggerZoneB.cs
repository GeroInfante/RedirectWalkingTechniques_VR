using UnityEngine;

public class TriggerZoneB : MonoBehaviour
{
    public ChangeRoomManager changeRoomManager;
    void OnTriggerEnter(Collider other)
    {
        changeRoomManager.ChangeStateWithEnterZoneBTrigger();
    }

    
    void OnTriggerExit(Collider other)
    {
        changeRoomManager.ChangeStateWithExitZoneBTrigger();
    }
}