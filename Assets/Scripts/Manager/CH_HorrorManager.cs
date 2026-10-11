using UnityEngine;
using System.Collections;
public class CH_HorrorManager : MonoBehaviour
{
    public static CH_HorrorManager Instance { get; private set; }

    [Header("Light Sources")]
    [Tooltip("All light sources in the scene that can be turned on/off.")]

    [SerializeField] private bool isAllLightsOn = true;    
    [SerializeField] private GameObject[] allLightSources;

    [Header("Electrical Panel")]
    [SerializeField] private Animator electricalPanelAnimator;

    public bool IsAllLightsOn
    {
        get { return isAllLightsOn; }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    //ligthing horror system !! maybe should move to animation and here just to call the animation
    public void TurnOffAllLights()
    {
        foreach (GameObject lightSource in allLightSources)
        {
            lightSource.GetComponent<Light>().enabled = false;
            CH_SoundManager.instance.PlaySound("PowerOutage");
            isAllLightsOn = false; // Update the flag to indicate that all lights are off
        }
    }

    public void TurnOffAllLightsAnim()
    {
        if (electricalPanelAnimator != null)
        {
            if(electricalPanelAnimator.GetBool("isOn"))
            {
                electricalPanelAnimator.SetBool("isOn", false);
            }
        }
    }


    public void TurnOnAllLights()
    {
        foreach (GameObject lightSource in allLightSources)
        {
            lightSource.GetComponent<Light>().enabled = true;
            isAllLightsOn = true; // Update the flag to indicate that all lights are on
            CH_EventManager.Instance.IsBusy = false;
        }
    }

    public void TurnOffLightsInRoom(int roomIndex)
    {
        CH_RoomManager.Instance.SetRoomLights(roomIndex, false);
    }
    
}
