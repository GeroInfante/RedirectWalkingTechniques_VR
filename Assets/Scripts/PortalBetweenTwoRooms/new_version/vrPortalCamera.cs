using UnityEngine;

public class vrPortalCamera : MonoBehaviour
{
    public Camera rightCamera;
    public Camera leftCamera;
    public int textureResolution = 1024;

    public RenderTexture rightRenderTexture;
    public RenderTexture leftRenderTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rightRenderTexture = new RenderTexture(textureResolution, textureResolution, 24);
        leftRenderTexture = new RenderTexture(textureResolution, textureResolution, 24);
        rightCamera.targetTexture = rightRenderTexture;
        leftCamera.targetTexture = leftRenderTexture;
    }
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
    private void OnDestroy()
    {
        if (leftRenderTexture != null) leftRenderTexture.Release();
        if (rightRenderTexture != null) rightRenderTexture.Release();
    }
}
