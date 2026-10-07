using UnityEngine;

public class ChangeRoomWithCamera : ChangeRoomManager
{

    public Transform currentCamera;
    public Transform nextRoomCamera;

    public Transform player;
    private bool isFake = true; // Variable para controlar si la salida de la zona B es falsa

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
        currentRoom = state.roomA;
    }

    
    public override void ChangeStateWithExitZoneATrigger()
    {
        Debug.Log(currentState +" Salida de zona A");
        switch(currentState)
        {
            case state.inZoneA:
                currentState = state.roomA;
                break;  
            case state.mid:
                currentState = state.inZoneB;
                if(currentRoom == state.roomA)
                {
                    currentRoom = state.roomB;
                    changeCamera();
                }
                break;
            default:
                break;
        }
    }

    public override void ChangeStateWithExitZoneBTrigger()
    {
        Debug.Log(currentState +" Salida de zona B");

        switch(currentState)
        {
            case state.inZoneB:
                currentState = state.roomB;
                break;
            case state.mid:
                currentState = state.inZoneA;
                if(currentRoom == state.roomB)
                {
                    currentRoom = state.roomA;
                    changeCamera();                    
                }
                break;
            default:
                break;
        }
    }

    protected virtual void changeCamera()
    {
        Transform temp = currentCamera;
        currentCamera.gameObject.SetActive(true);

        player.position = nextRoomCamera.position;
        
        currentCamera = nextRoomCamera;
        currentCamera.gameObject.SetActive(false);
        
        nextRoomCamera = temp;
    }
}


