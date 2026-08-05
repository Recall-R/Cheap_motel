using UnityEngine;


public class CH_Lights : MonoBehaviour
{

    //this is switch lighting system for a room

    [SerializeField] private GameObject[] lightsObject;
    [SerializeField] private GameObject[] lightsBulb;
    [SerializeField] private bool isOn = true;


    [SerializeField] private bool isSwitchable = true;

    public bool getIsSwitchable()
    {
        return isSwitchable;
    }
    public void setIsSwitchable(bool state)
    {
        isSwitchable = state;
    }

    public bool getISOn()
    {
        return isOn;
    }
    public void setIsOn(bool state)
    {
        isOn = state;
        SetLightsState(state);
        if (state)
        {
            CH_SoundManager.instance.PlaySound("LightOn");
        }
        else
        {
            CH_SoundManager.instance.PlaySound("LightOff");
        }
    }

    //need to implement a switch for emission of a light bulb 
    public void SetLightsState(bool state)
    {
        if(isSwitchable) {
        foreach (GameObject lightObj in lightsObject)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(state);
            }
        }
        }
    }
}