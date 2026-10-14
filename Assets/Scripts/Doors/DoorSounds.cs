using UnityEngine;

public class DoorSounds : MonoBehaviour
{
    [SerializeField] private AudioSource openDoorSound;
    [SerializeField] private AudioSource closeDoorSound;


    public void PlayOpenDoorSound()
    {
        if (openDoorSound != null)
        {
            openDoorSound.pitch = Random.Range(0.8f, 1.2f);
            openDoorSound.Play();
        }
    }
    public void PlayCloseDoorSound()
    {
        if (closeDoorSound != null)
        {
            closeDoorSound.pitch = Random.Range(0.8f, 1.2f);
            closeDoorSound.Play();
        }
    }
}
