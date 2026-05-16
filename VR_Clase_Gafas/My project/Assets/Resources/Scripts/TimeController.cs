using UnityEngine;

public class TimeController : MonoBehaviour
{
    // Patrón Singleton para acceder fácilmente desde cualquier script
    public static TimeController Instance { get; private set; }

    [Header("Configuración de SUPERHOT")]
    [Tooltip("El tiempo casi detenido (Ej: 0.05 es 5% de velocidad)")]
    [Range(0.01f, 0.2f)] public float slowTimeScale = 0.05f;
    [Tooltip("El tiempo normal")]
    public float normalTimeScale = 1.0f;
    [Tooltip("Cuánto dura el tiempo normal después de una acción")]
    public float actionDuration = 0.7f;
    [Tooltip("Velocidad con la que el tiempo se suaviza entre lento y normal")]
    public float transitionSpeed = 10f;

    private float actionTimer = 0f;

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // Empezamos en cámara lenta
        Time.timeScale = slowTimeScale;
    }

    void Update()
    {
        // Si hay tiempo en el contador, vamos a velocidad normal
        if (actionTimer > 0)
        {
            // Usamos unscaledDeltaTime para restar tiempo real, sin importar el TimeScale actual
            actionTimer -= Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, normalTimeScale, Time.unscaledDeltaTime * transitionSpeed);
        }
        else // Si el contador llegó a 0, volvemos a cámara lenta
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowTimeScale, Time.unscaledDeltaTime * transitionSpeed);
        }

        // BUENA PRÁCTICA DE FÍSICAS: Ajustar el fixedDeltaTime para que los Rigidbodies no tiemblen en cámara lenta
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    // Método público que otros scripts llamarán para "despertar" el tiempo
    public void TriggerAction()
    {
        actionTimer = actionDuration;
    }
}