using UnityEngine;

public class ChangeRoomWithCamera : ChangeRoomManager
{

    public Transform currentCamera;
    public Transform nextRoomCamera;

    public Transform player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
    }


    public override void ChangeStateWithExitZoneATrigger()
    {
        switch(currentState)
        {
            case state.inZoneA:
                currentState = state.roomA;
                break;  
            case state.mid:
                currentState = state.inZoneB;
                changeCamera();
                break;
        }
    }

    public override void ChangeStateWithExitZoneBTrigger()
    {
        switch(currentState)
        {
            case state.inZoneB:
                currentState = state.roomB;
                break;
            case state.mid:
                currentState = state.inZoneA;
                changeCamera();
                break;
        }
    }

    private void changeCamera()
    {
        Transform temp = currentCamera;
        currentCamera.gameObject.SetActive(true);

        player.position = nextRoomCamera.position;
        
        currentCamera = nextRoomCamera;
        currentCamera.gameObject.SetActive(false);
        
        nextRoomCamera = temp;
    }
}


