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
    public void SyncStereoEyes(Camera mainCam, Transform thisPortal, Transform destPortal)
    {
        // Puente espacial desde el portal donde está el jugador hacia este portal
        Matrix4x4 virtualHeadView = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * transform.worldToLocalMatrix;

        SyncSingleEye(leftCamera, mainCam, Camera.StereoscopicEye.Left, virtualHeadView);
        SyncSingleEye(rightCamera, mainCam, Camera.StereoscopicEye.Right, virtualHeadView);
    }

    private void SyncSingleEye(Camera eyeCam, Camera mainCam, Camera.StereoscopicEye eye, Matrix4x4 virtualHeadView)
    {
        if (eyeCam == null) return;

        if (mainCam.stereoEnabled)
        {

            Matrix4x4 mainHeadToWorld = mainCam.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
            Matrix4x4 headToEye = mainCam.GetStereoViewMatrix(eye) * mainHeadToWorld;

            eyeCam.worldToCameraMatrix = headToEye * virtualHeadView;
            eyeCam.projectionMatrix = mainCam.GetStereoProjectionMatrix(eye);
        }
    }
}
