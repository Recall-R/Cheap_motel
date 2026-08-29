using System;
using CH_AICharacter;
using UnityEngine;

public class CH_RaycastObjects : MonoBehaviour
{
    public static event Action<GameObject> OnObjectClicked;

    [Tooltip("Tag obiectului pe care îl căutăm cu raycast-ul.")]
    public string targetTag = "Interactable";

    [Tooltip("Distanța maximă a raycast-ului.")]
    public float maxDistance = 10f;

    [Tooltip("Layer mask pentru raycast. Lasă implicit pentru toate layerele.")]
    public LayerMask raycastMask = Physics.DefaultRaycastLayers;

    private Camera mainCamera;

    [SerializeField] private GameObject interactableDot;
    
    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("CH_RaycastObjects: Nu s-a găsit nicio Main Camera în scenă.");
        }
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit, maxDistance, raycastMask);

        if (interactableDot != null)
        {
            interactableDot.SetActive(false);
        }

        if (!hitSomething)
            return;

        GameObject hitObject = hit.collider != null ? hit.collider.gameObject : hit.transform?.gameObject;
        if (hitObject == null)
            return;

        CH_IInteractable interactable = hitObject.GetComponent<CH_IInteractable>() ?? hitObject.GetComponentInParent<CH_IInteractable>();
        bool hasSuspicionState = hitObject.GetComponent<CH_NPCSuspicionState>() != null || hitObject.GetComponentInParent<CH_NPCSuspicionState>() != null;
        bool isInteractive = hitObject.CompareTag(targetTag) || interactable != null || hasSuspicionState;

        if (!isInteractive)
            return;

        if (interactableDot != null)
        {
            interactableDot.SetActive(true);
        }

        bool pressedInteractionKey = interactable != null ? interactable.IsInteractionPressed() : false;

        if (!pressedInteractionKey && hasSuspicionState)
        {
            CH_IInteractable interactableForDialog = hitObject.GetComponent<CH_IInteractable>() ?? hitObject.GetComponentInParent<CH_IInteractable>();
            if (interactableForDialog != null)
            {
                pressedInteractionKey = interactableForDialog.IsInteractionPressed();
            }
        }

        if (!pressedInteractionKey)
            return;

        if (interactable != null)
        {
            interactable.Interact();
            OnObjectClicked?.Invoke(hitObject);
            return;
        }

        if (hasSuspicionState)
        {
            CH_IInteractable interactableForDialog = hitObject.GetComponent<CH_IInteractable>() ?? hitObject.GetComponentInParent<CH_IInteractable>();
            if (interactableForDialog != null)
            {
                interactableForDialog.TriggerInteraction(CH_IInteractable.Interaction.CharacterDialog);
            }
            else
            {
                CH_IInteractable interactableFallback = hitObject.AddComponent<CH_IInteractable>();
                interactableFallback.SetInteractionType(CH_IInteractable.Interaction.CharacterDialog);
                interactableFallback.TriggerInteraction(CH_IInteractable.Interaction.CharacterDialog);
            }

            OnObjectClicked?.Invoke(hitObject);
        }
    }
}
