using UnityEngine;

public class ChangeRoomWithCameraVR : ChangeRoomWithCamera
{
    public Transform playerCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
    }

    protected override void changeCamera()
    {
        Transform temp = currentCamera;
        currentCamera.gameObject.SetActive(true);

        player.position = nextRoomCamera.position - playerCamera.localPosition;
        
        currentCamera = nextRoomCamera;
        currentCamera.gameObject.SetActive(false);
        
        nextRoomCamera = temp;
    }
}


