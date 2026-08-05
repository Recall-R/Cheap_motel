using UnityEngine;

public class CH_RoomUnit : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private int roomIndex = 0;
    [SerializeField] private Transform movementPoint;

    [SerializeField] private GameObject[] negativeRoomIndicators;
    [SerializeField] private GameObject[] neutralRoomIndicators;

    [SerializeField] private bool isOccupied = false;
    [SerializeField] private bool isKillerOnRoom = false;

    [SerializeField] private GameObject npcCharacter;


    [Header("Room Lighting Settings")]
    [SerializeField] private GameObject[] roomLights;
    [SerializeField] private GameObject lightSwitch;
    [SerializeField] private bool areLightsOn = true;

    public int RoomIndex => roomIndex;
    public Transform MovementPoint => movementPoint;
    public bool IsOccupied => isOccupied;
    public bool IsKillerOnRoom => isKillerOnRoom;
    public bool AreLightsOn => areLightsOn;

    public void setOccupied(bool occupied)
    {
        isOccupied = occupied;
    }

    public void setKillerOnRoom(bool killeronroom)
    {
        isKillerOnRoom = killeronroom;
    }

    public void setNpcCharacter(GameObject character)
    {
        npcCharacter = character;
    }
    //this function is used for turn off/on the lights in the room like a panel, blocking switching the lights in the room, and also for the horror manager to turn off all lights in the room
    public void setLightsOn(bool lightsOn)
    {
        areLightsOn = lightsOn;
        foreach (GameObject light in roomLights)
        {
            light.SetActive(lightsOn);
        }
        if (lightSwitch != null)
            lightSwitch.GetComponent<CH_Lights>().setIsSwitchable(lightsOn);
        else
            Debug.LogWarning($"CH_RoomUnit: Light switch is not assigned for room {roomIndex}.");
    }
}
