using UnityEngine;



public class CH_contorIndex: MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private bool isLocalRoomPanelLightOn = true;
    public int Index => index;
    public bool IsLocalRoomPanelLightOn => isLocalRoomPanelLightOn;

    public void setLocalRoomPanelLightOn(bool state)
    {
        isLocalRoomPanelLightOn = state;
    }
}