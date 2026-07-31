using UnityEngine;


public class CH_Lights : MonoBehaviour
{

    [SerializeField] private GameObject[] lightsObject;
    [SerializeField] private GameObject[] lightsBulb;
    [SerializeField] private bool isOn = true;

    public bool getISOn()
    {
        return isOn;
    }
    public void setIsOn(bool state)
    {
        isOn = state;
        SetLightsState(state);
    }

    //need to implement a switch for emission of a light bulb 
    public void SetLightsState(bool state)
    {
        foreach (GameObject lightObj in lightsObject)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(state);
            }
        }
    }
}