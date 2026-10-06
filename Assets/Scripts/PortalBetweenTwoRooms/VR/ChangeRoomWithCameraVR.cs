using UnityEngine;

public class ChangeRoomWithCameraVR : ChangeRoomWithCamera
{
    public Transform playerCamera;
    public PortalSyncURP_Interceptor portalSync;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = state.roomA;
    }

    protected override void changeCamera()
    {
        player.position = nextRoomCamera.position - playerCamera.localPosition;
        portalSync.altura = -portalSync.altura;
        Debug.Log("Cambiando de habitación con la cámara VR: "+portalSync.altura);
    }
}


