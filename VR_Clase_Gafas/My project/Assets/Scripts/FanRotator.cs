using UnityEngine;

/// <summary>
/// Fan Rotator - Script para ventiladores (o cualquier objeto que rote)
/// Se ve afectado automáticamente por Time.timeScale del SuperHotTimeManager.
/// Ponelo en el GameObject del ventilador.
/// </summary>
public class FanRotator : MonoBehaviour
{
    [Header("Configuración del Ventilador")]
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = 360f;

    [Tooltip("Eje de rotación del ventilador")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("¿Sentido horario?")]
    public bool clockwise = true;

    void Update()
    {
        // Time.deltaTime ya está afectado por Time.timeScale
        // Entonces esto automáticamente va lento/rápido con el SuperHotTimeManager
        float direction = clockwise ? -1f : 1f;
        float angle = rotationSpeed * direction * Time.deltaTime;

        transform.Rotate(rotationAxis, angle, Space.Self);
    }
}
