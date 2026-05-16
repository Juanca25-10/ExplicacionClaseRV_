using UnityEngine;

/// <summary>
/// SUPERHOT Time Manager - Script Global
/// Controla Time.timeScale basándose en el movimiento del jugador VR.
/// Ponelo en un GameObject vacío en la escena (ej: "GameManager").
/// No necesitás tocar ningún otro script - todo usa Time.deltaTime automáticamente.
/// </summary>
public class SuperHotTimeManager : MonoBehaviour
{
    public static SuperHotTimeManager Instance { get; private set; }

    [Header("Referencias VR")]
    [Tooltip("Arrastrá aquí el transform de la Main Camera (dentro de Camera Offset)")]
    public Transform headsetTransform;

    [Tooltip("Arrastrá aquí el transform de Right Hand")]
    public Transform rightHandTransform;

    [Tooltip("Arrastrá aquí el transform de Left Hand")]
    public Transform leftHandTransform;

    [Header("Configuración de Tiempo")]
    [Tooltip("Velocidad del tiempo cuando el jugador NO se mueve (0.05 = muy lento)")]
    [Range(0.01f, 0.5f)]
    public float minTimeScale = 0.05f;

    [Tooltip("Velocidad del tiempo cuando el jugador SE mueve (1 = normal)")]
    [Range(0.5f, 1f)]
    public float maxTimeScale = 1f;

    [Tooltip("Qué tan rápido transiciona entre lento y rápido")]
    [Range(1f, 10f)]
    public float lerpSpeed = 4f;

    [Header("Detección de Movimiento")]
    [Tooltip("Distancia mínima de movimiento para considerar que el jugador se está moviendo")]
    [Range(0.0001f, 0.01f)]
    public float movementThreshold = 0.001f;

    [Tooltip("Cuántos frames sin movimiento antes de desacelerar")]
    [Range(1, 10)]
    public int stillFramesRequired = 3;

    [Header("Debug (solo info, no editar)")]
    [SerializeField] private float currentTimeScale;
    [SerializeField] private bool playerIsMoving;
    [SerializeField] private float totalMovementDelta;

    // Posiciones anteriores para calcular delta
    private Vector3 lastHeadPos;
    private Vector3 lastRightHandPos;
    private Vector3 lastLeftHandPos;

    private int stillFrameCount = 0;

    void Awake()
    {
        // Singleton: una sola instancia global
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Inicializar posiciones
        if (headsetTransform != null) lastHeadPos = headsetTransform.position;
        if (rightHandTransform != null) lastRightHandPos = rightHandTransform.position;
        if (leftHandTransform != null) lastLeftHandPos = leftHandTransform.position;

        // Empezar lento
        Time.timeScale = minTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void Update()
    {
        DetectMovement();
        UpdateTimeScale();

        currentTimeScale = Time.timeScale; // para debug en Inspector
    }

    void DetectMovement()
    {
        totalMovementDelta = 0f;

        // Sumar el delta de cada parte del cuerpo rastreada
        if (headsetTransform != null)
        {
            totalMovementDelta += Vector3.Distance(headsetTransform.position, lastHeadPos);
            lastHeadPos = headsetTransform.position;
        }

        if (rightHandTransform != null)
        {
            totalMovementDelta += Vector3.Distance(rightHandTransform.position, lastRightHandPos);
            lastRightHandPos = rightHandTransform.position;
        }

        if (leftHandTransform != null)
        {
            totalMovementDelta += Vector3.Distance(leftHandTransform.position, lastLeftHandPos);
            lastLeftHandPos = leftHandTransform.position;
        }

        // Sistema de frames de gracia: evita micro-oscilaciones que activen el tiempo
        if (totalMovementDelta > movementThreshold)
        {
            stillFrameCount = 0;
            playerIsMoving = true;
        }
        else
        {
            stillFrameCount++;
            if (stillFrameCount >= stillFramesRequired)
            {
                playerIsMoving = false;
            }
        }
    }

    void UpdateTimeScale()
    {
        float targetTimeScale = playerIsMoving ? maxTimeScale : minTimeScale;

        // Lerp suave hacia el target usando deltaTime real (no escalado)
        // Usamos unscaledDeltaTime para que la transición no dependa del timeScale actual
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, lerpSpeed * Time.unscaledDeltaTime);

        // IMPORTANTE: fixedDeltaTime debe sincronizarse con timeScale para que Physics sea correcto
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    /// <summary>
    /// Método público para forzar tiempo lento desde otros scripts si lo necesitás
    /// Ej: SuperHotTimeManager.Instance.ForceSlowMotion();
    /// </summary>
    public void ForceSlowMotion()
    {
        Time.timeScale = minTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void OnDestroy()
    {
        // Restaurar tiempo normal al salir
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}