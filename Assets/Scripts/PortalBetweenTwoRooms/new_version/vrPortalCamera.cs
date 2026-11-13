using UnityEngine;
using UnityEngine.XR;

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
        if (rightCamera != null)
        {
            rightCamera.targetTexture = rightRenderTexture;
            rightCamera.depth = -1f; // Se renderiza antes que Main Camera (depth 0)
        }

        if (leftCamera != null)
        {
            leftCamera.targetTexture = leftRenderTexture;
            leftCamera.depth = -2f; // Se renderiza antes que Main Camera (depth 0)
        }
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
    // public void SyncStereoEyes(Camera mainCam, Transform thisPortal, Transform destPortal)
    // {
    //     // Puente espacial desde el portal donde está el jugador hacia este portal
    //     Matrix4x4 virtualHeadView = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * transform.worldToLocalMatrix;

    //     SyncSingleEye(leftCamera, mainCam, Camera.StereoscopicEye.Left, virtualHeadView);
    //     SyncSingleEye(rightCamera, mainCam, Camera.StereoscopicEye.Right, virtualHeadView);
    // }

    // private void SyncSingleEye(Camera eyeCam, Camera mainCam, Camera.StereoscopicEye eye, Matrix4x4 virtualHeadView)
    // {
    //     if (eyeCam == null) return;

    //     if (mainCam.stereoEnabled)
    //     {

    //         Matrix4x4 mainHeadToWorld = mainCam.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
    //         Matrix4x4 headToEye = mainCam.GetStereoViewMatrix(eye) * mainHeadToWorld;

    //         eyeCam.worldToCameraMatrix = headToEye * virtualHeadView;
    //         eyeCam.projectionMatrix = mainCam.GetStereoProjectionMatrix(eye);
    //     }
    // }
    public void SyncStereoEyes(Camera mainCam, Matrix4x4 destToThis)
    {
        float halfIpd = mainCam.stereoEnabled ? mainCam.stereoSeparation * 0.5f : 0f;

        SyncSingleEye(leftCamera, mainCam, Camera.StereoscopicEye.Left, -halfIpd, destToThis);
        SyncSingleEye(rightCamera, mainCam, Camera.StereoscopicEye.Right, halfIpd, destToThis);
    }

    private void SyncSingleEye(Camera eyeCam, Camera mainCam, Camera.StereoscopicEye eye, float eyeOffsetX, Matrix4x4 destToThis)
    {
        if (eyeCam == null) return;

        // 1. Pose de cada ojo en el Frame N actual (sin usar GetStereoViewMatrix)
        Vector3 playerEyeWorldPos = mainCam.transform.TransformPoint(new Vector3(eyeOffsetX, 0f, 0f));
        Quaternion playerEyeWorldRot = mainCam.transform.rotation;

        // 2. Trasladamos esa pose al otro portal
        Vector3 portalEyePos = destToThis.MultiplyPoint3x4(playerEyeWorldPos);
        Quaternion portalEyeRot = destToThis.rotation * playerEyeWorldRot;

        // 3. Aplicamos al Transform y sincronizamos vista + Frustum Culling
        eyeCam.transform.SetPositionAndRotation(portalEyePos, portalEyeRot);
        eyeCam.ResetWorldToCameraMatrix();

        // La matriz de proyección (lente asimétrica) es fija por visor y no cambia al girar la cabeza
        eyeCam.projectionMatrix = mainCam.stereoEnabled
            ? mainCam.GetStereoProjectionMatrix(eye)
            : mainCam.projectionMatrix;
    }
}
