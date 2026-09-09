using UnityEngine;

public class TriggerZoneA : MonoBehaviour
{
    public ChangeRoomManager changeRoomManager;
    void OnTriggerEnter(Collider other)
    {
        changeRoomManager.ChangeStateWithEnterZoneATrigger();
    }

    
    void OnTriggerExit(Collider other)
    {
        changeRoomManager.ChangeStateWithExitZoneATrigger();
    }
}