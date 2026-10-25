using UnityEngine;

public class vrPortalCamera : MonoBehaviour
{
    public Camera rightCamera;
    public Camera leftCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightCamera.transform.localPosition = new Vector3(0.032f, 0, 0);
        leftCamera.transform.localPosition = new Vector3(-0.032f, 0, 0);
        rightCamera.depth = -10;
        leftCamera.depth = -10;
    }

    public void turnOnCameras()
    {
        this.enabled = true;
    }
    public void turnOffCameras()
    {
        this.enabled = false;
    }
}
