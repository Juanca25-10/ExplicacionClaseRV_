using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FirearmController : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    public Transform firePoint; // De donde sale la bala
    public GameObject bulletPrefab; // El proyectil

    [Header("Atributos del Arma")]
    public float fireRate = 0.5f;
    public int bulletsPerPull = 1; // 1 para pistola, varios para escopeta
    public float precisionSpread = 0f; // 0 es perfecto, mayor a 0 las balas salen desviadas

    private XRGrabInteractable interactable;
    private float nextFireTime = 0f;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        // Suscribimos el método "Fire" al evento de apretar el gatillo (Activate)
        interactable.activated.AddListener(Fire);
    }

    void OnDestroy()
    {
        // Buena práctica de optimización: desuscribir eventos
        interactable.activated.RemoveListener(Fire);
    }

    private void Fire(ActivateEventArgs args)
    {
        // Validar cadencia de disparo
        if (Time.time < nextFireTime) return;

        // Bucle por si es una escopeta (bulletsPerPull > 1)
        for (int i = 0; i < bulletsPerPull; i++)
        {
            // Lógica para instanciar la bala. 
            // Aquí en el futuro podrías agregar un check de "if (currentAmmo > 0)"
            ShootBullet();
        }

        nextFireTime = Time.time + fireRate;
    }

    private void ShootBullet()
    {
        // Calcula la dispersión para la precisión
        Vector3 spread = new Vector3(
            Random.Range(-precisionSpread, precisionSpread),
            Random.Range(-precisionSpread, precisionSpread),
            0f
        );

        Quaternion fireRotation = Quaternion.Euler(firePoint.eulerAngles + spread);

        // Instancia la bala
        Instantiate(bulletPrefab, firePoint.position, fireRotation);

        // Aquí podrías reproducir sonido, partículas, etc.
    }
}