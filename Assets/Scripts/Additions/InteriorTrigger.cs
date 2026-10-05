using UnityEngine;

public class InteriorTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            CH_EventManager.Instance.IsPlayerInInterior_WATCHER = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            CH_EventManager.Instance.IsPlayerInInterior_WATCHER = false;
        }
    }
}
