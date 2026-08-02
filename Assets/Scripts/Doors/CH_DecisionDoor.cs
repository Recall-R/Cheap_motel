using UnityEngine;

public class CH_DecisionDoor : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;



    public void doorSystem()
    {
        if (doorAnimator.GetBool("isOpen"))
        {
            doorAnimator.SetBool("isOpen", false);
        }
        else
        {
            doorAnimator.SetBool("isOpen", true);
        }
    }
}
