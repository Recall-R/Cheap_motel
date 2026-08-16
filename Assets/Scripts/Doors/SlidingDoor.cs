using UnityEngine;

public class SlidingDoor : MonoBehaviour
{

    [SerializeField] private Animator slidingDoorAnimator_R;
    [SerializeField] private Animator slidingDoorAnimator_L;

    void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
        {
            slidingDoorAnimator_R.SetBool("isOpen", true);
            slidingDoorAnimator_L.SetBool("isOpen", true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            slidingDoorAnimator_R.SetBool("isOpen", false);
            slidingDoorAnimator_L.SetBool("isOpen", false);
        }
    }


}
