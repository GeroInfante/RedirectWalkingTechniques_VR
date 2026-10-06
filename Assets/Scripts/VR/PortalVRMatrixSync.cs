using UnityEngine;

public class PortalVRMatrixSync : MonoBehaviour
{
    [Header("Tu Cámara Principal (La cabeza del jugador)")]
    public Camera mainCameraVR;

    [Header("Cámaras del Portal")]
    public Camera camaraIzquierda;
    public Camera camaraDerecha;

    void LateUpdate()
    {
        if (mainCameraVR == null || camaraIzquierda == null || camaraDerecha == null) return;

        // 1. Extraemos las matrices asimétricas reales de las Quest 2 en este fotograma
        Matrix4x4 leftMatrix = mainCameraVR.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
        Matrix4x4 rightMatrix = mainCameraVR.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

        // 2. Se las inyectamos a las cámaras del portal para que coincidan milimétricamente
        camaraIzquierda.projectionMatrix = leftMatrix;
        camaraDerecha.projectionMatrix = rightMatrix;

        // Nota: Al hacer esto, el valor de 'Field of View' en el Inspector se ignorará por completo.
    }
}