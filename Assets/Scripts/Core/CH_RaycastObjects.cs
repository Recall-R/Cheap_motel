using UnityEngine;

public class CH_RaycastObjects : MonoBehaviour
{
    [Tooltip("Tag obiectului pe care îl căutăm cu raycast-ul.")]
    public string targetTag = "Interactable";

    [Tooltip("Distanța maximă a raycast-ului.")]
    public float maxDistance = 10f;

    [Tooltip("Layer mask pentru raycast. Lasă implicit pentru toate layerele.")]
    public LayerMask raycastMask = Physics.DefaultRaycastLayers;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("CH_RaycastObjects: Nu s-a găsit nicio Main Camera în scenă.");
        }
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, raycastMask))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                if (Input.GetMouseButtonDown(0)) // Verifică dacă s-a apăsat butonul stâng al mouse-ului
                {
                    hit.collider.GetComponent<CH_IInteractable>()?.SetInteractionType(CH_IInteractable.Interaction.GetToComputer);
                }
            }
        }
    }

    private void OnTaggedObjectHit(GameObject taggedObject)
    {
        Debug.Log($"CH_RaycastObjects: Am găsit obiectul cu tag '{targetTag}' => {taggedObject.name}");

        Renderer renderer = taggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }
}
