using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;

    [Header("Configuración")]
    public float moveSpeed = 5f;
    public float lookSpeed = 0.5f;

    private float pitch = 0f; // Rotación Arriba/Abajo
    private float yaw = 0f;   // Rotación Izquierda/Derecha

    void Start()
    {
        // Esto bloquea el cursor en el centro y lo oculta (clave para no tener que hacer clic)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Rotación de la cámara (Mouse)
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Evita que la cámara dé vueltas completas

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // 2. Movimiento de la cámara (WASD)
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        
        // Movemos la cámara relativa a hacia dónde está mirando
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }

    // Es obligatorio habilitar y deshabilitar las acciones por código
    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }
}