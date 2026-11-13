using UnityEngine;
using UnityEngine.XR;

public class vrPortalCamera : MonoBehaviour
{
    public Camera rightCamera;
    public Camera leftCamera;
    public int textureResolution = 1024;
    public float clipPlaneOffset = -0.01f;

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
    public void SyncStereoEyes(Camera mainCam, Matrix4x4 destToThis, Transform portalTransform)
    {
        float halfIpd = mainCam.stereoEnabled ? mainCam.stereoSeparation * 0.5f : 0f;

        SyncSingleEye(leftCamera, mainCam, Camera.StereoscopicEye.Left, -halfIpd, destToThis, portalTransform);
        SyncSingleEye(rightCamera, mainCam, Camera.StereoscopicEye.Right, halfIpd, destToThis, portalTransform);
    }

    private void SyncSingleEye(Camera eyeCam, Camera mainCam, Camera.StereoscopicEye eye, float eyeOffsetX, Matrix4x4 destToThis, Transform portalTransform)
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
        // 5. Alineamos el plano Near exactamente con la superficie del portal
        Vector3 portalNormal = portalTransform.forward;
        float signedDistance = Vector3.Dot(portalNormal, portalEyePos - portalTransform.position);

        // Solo aplicamos el corte si el ojo está detrás del plano del portal (evita errores al cruzarlo)
        if (signedDistance < -0.05f)
        {
            Vector4 clipPlane = CameraSpacePlane(eyeCam, portalTransform.position, portalNormal, 1.0f, clipPlaneOffset);
            eyeCam.projectionMatrix = eyeCam.CalculateObliqueMatrix(clipPlane);
        }
    }
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float offset)
    {
        Vector3 offsetPos = pos + normal * offset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
}
