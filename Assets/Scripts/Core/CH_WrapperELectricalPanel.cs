using UnityEngine;

public class CH_WrapperElectricalPanel : MonoBehaviour
{ 

    public void StopAllLights()
    {
        CH_HorrorManager.Instance.TurnOffAllLights();
    }


    public void StartAllLights()
    {
        CH_HorrorManager.Instance.TurnOnAllLights();
    }
}
