using UnityEngine;

public class HandAestheticMover : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la mano 3D que quieres mover.")]
    [SerializeField] private Transform hand;

    [Header("Rango Horizontal")]
    [Tooltip("Posición X mínima (en espacio local) de la mano.")]
    [SerializeField] private float minX = -0.5f;
    [Tooltip("Posición X máxima (en espacio local) de la mano.")]
    [SerializeField] private float maxX =  0.5f;

    [Header("Altura y Profundidad")]
    [Tooltip("Altura base Y de la mano en espacio local.")]
    [SerializeField] private float fixedY = 0.1f;
    [Tooltip("Profundidad Z de la mano en espacio local.")]
    [SerializeField] private float fixedZ = 0.5f;

    [Header("Rotación")]
    [Tooltip("Grados máximos de tilt (rotación en Z) cuando el cursor está en los extremos.")]
    [SerializeField] private float maxTiltZ = 30f;

    [Header("Bobbing Vertical")]
    [Tooltip("Amplitud del bobbing sobre Y.")]
    [SerializeField] private float bobAmplitude = 0.02f;
    [Tooltip("Frecuencia del bobbing (veces por segundo).")]
    [SerializeField] private float bobFrequency = 2f;

    [Header("Offset según Mouse Y")]
    [Tooltip("Amplitud máxima del desplazamiento vertical extra según la posición del ratón.")]
    [SerializeField] private float mouseYOffsetAmplitude = -0.001f;

    void Update()
    {
        // 1) Normalizamos X de [-1,1]
        float nx = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        nx = Mathf.Clamp(nx, -1f, +1f);
        // 2) Calculamos localX
        float localX = Mathf.Lerp(minX, maxX, (nx + 1f) / 2f);

        // 3) Bobbing en Y
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

        // 4) Offset vertical según mouse Y (normalizado)
        float ny = (Input.mousePosition.y / Screen.height) * 2f - 1f;
        float mouseOffsetY = ny * mouseYOffsetAmplitude;

        // 5) Asignamos posición
        hand.localPosition = new Vector3(
            localX,
            fixedY + bob + (mouseOffsetY * -1),
            fixedZ
        );

        // 6) Rotamos en Z para apuntar
        float tilt = -nx * maxTiltZ;
        hand.localRotation = Quaternion.Euler(-tilt, -90f, -90f);
    }
}
