using UnityEngine;
using System.Collections;

/// <summary>
/// BangEffect - Va en el Prefab "BangEffect"
/// Animación con squash & stretch + wobble para que se vea orgánico y cartoon.
/// Usa unscaledTime para que siempre sea snappy incluso en slow-mo.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BangEffect : MonoBehaviour
{
    [Header("Animación de Entrada")]
    public float punchScale = 1.3f;
    public float finalScale = 1.0f;
    public float punchDuration = 0.10f;

    [Header("Fade Out")]
    public float holdDuration = 0.3f;
    public float fadeDuration = 0.18f;

    [Header("Rotación Aleatoria")]
    public float randomRotationRange = 20f;

    [Header("Offset desde el GunShooter")]
    [Tooltip("Estos valores los usa GunShooter para posicionarlo. Aquí son solo referencia.")]
    public float rightOffset = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Transform cameraTransform;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraTransform = Camera.main?.transform;
    }

    void Start()
    {
        float randomZ = Random.Range(-randomRotationRange, randomRotationRange);
        transform.Rotate(0f, 0f, randomZ, Space.Self);
        StartCoroutine(AnimateBang());
    }

    void LateUpdate()
    {
        // Billboard: siempre mira a la cámara
        if (cameraTransform != null)
        {
            transform.LookAt(
                transform.position + cameraTransform.rotation * Vector3.forward,
                cameraTransform.rotation * Vector3.up
            );
        }
    }

    private IEnumerator AnimateBang()
    {
        transform.localScale = Vector3.zero;

        // --- FASE 1: Entrada con squash & stretch ---
        // Arranca ancho y chato (squash) y se estira hasta el punch
        float elapsed = 0f;
        while (elapsed < punchDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / punchDuration);

            // Ease out exponencial: entra MUY rápido al principio
            float easedT = 1f - Mathf.Pow(1f - t, 4f);

            // Squash & stretch: X crece más rápido que Y al inicio, luego se igualan
            float scaleX = Mathf.Lerp(0f, punchScale * 1.2f, easedT);  // más ancho
            float scaleY = Mathf.Lerp(0f, punchScale * 0.85f, easedT); // más chato
            float scaleZ = 1f;

            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            yield return null;
        }

        // --- FASE 2: Settle — vuelve a proporción correcta con pequeño wobble ---
        elapsed = 0f;
        float settleDuration = 0.14f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);

            // Spring damping: oscila un poco antes de asentarse
            float spring = 1f + Mathf.Sin(t * Mathf.PI * 2.5f) * (1f - t) * 0.12f;
            float scale = Mathf.Lerp(punchScale, finalScale, t) * spring;

            transform.localScale = Vector3.one * scale;
            yield return null;
        }

        transform.localScale = Vector3.one * finalScale;

        // --- FASE 3: Hold ---
        yield return new WaitForSecondsRealtime(holdDuration);

        // --- FASE 4: Fade Out con ligero scale down ---
        elapsed = 0f;
        Color originalColor = spriteRenderer.color;
        float startScale = finalScale;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Ease in: fade empieza lento y acelera
            float easedT = t * t;

            float alpha = Mathf.Lerp(1f, 0f, easedT);
            float scale = Mathf.Lerp(startScale, startScale * 1.1f, easedT); // crece levemente al desvanecerse

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        Destroy(gameObject);
    }
}