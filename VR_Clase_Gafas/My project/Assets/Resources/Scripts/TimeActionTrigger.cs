using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class TimeActionTrigger : MonoBehaviour
{
    private XRGrabInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();

        // Nos suscribimos a los eventos de agarrar, soltar y accionar (gatillar)
        interactable.selectEntered.AddListener(OnActionTriggered); // Al agarrar
        interactable.selectExited.AddListener(OnActionTriggered);  // Al soltar
        interactable.activated.AddListener(OnActionTriggered);     // Al disparar
    }

    void OnDestroy()
    {
        // Limpiamos los eventos por optimización
        interactable.selectEntered.RemoveListener(OnActionTriggered);
        interactable.selectExited.RemoveListener(OnActionTriggered);
        interactable.activated.RemoveListener(OnActionTriggered);
    }

    // Como todos los eventos piden un argumento distinto, usamos sobrecarga o ignoramos el argumento
    private void OnActionTriggered(BaseInteractionEventArgs args)
    {
        // Le decimos al Singleton que inicie el tiempo normal
        TimeController.Instance.TriggerAction();
    }
}