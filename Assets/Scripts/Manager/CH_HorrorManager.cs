using UnityEngine;
using System.Collections;
public class CH_HorrorManager : MonoBehaviour
{
    public static CH_HorrorManager Instance { get; private set; }

    [Header("Light Sources")]
    [Tooltip("All light sources in the scene that can be turned on/off.")]
    [SerializeField] private GameObject[] allLightSources;


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
    
    public void TurnOffAllLights()
    {
        foreach (GameObject lightSource in allLightSources)
        {
            lightSource.GetComponent<Light>().enabled = false;
        }
    }


    public void TurnOnAllLights()
    {
        foreach (GameObject lightSource in allLightSources)
        {
            lightSource.GetComponent<Light>().enabled = true;
        }
    }
    
}
