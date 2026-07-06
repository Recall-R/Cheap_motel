using UnityEngine;

public class SlidingDoor : MonoBehaviour
{


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Door Opened");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Door Closed");
        }
    }


}
