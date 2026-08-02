using UnityEngine;

public class CH_RoomUnit : MonoBehaviour
{
    [SerializeField] private int roomIndex = 0;
    [SerializeField] private Transform movementPoint;

    [SerializeField] private GameObject[] negativeRoomIndicators;
    [SerializeField] private GameObject[] neutralRoomIndicators;

    [SerializeField] private bool isOccupied = false;
    [SerializeField] private bool isKillerOnRoom = false;

    [SerializeField] private GameObject npcCharacter;

    public int RoomIndex => roomIndex;
    public Transform MovementPoint => movementPoint;
    public bool IsOccupied => isOccupied;
    public bool IsKillerOnRoom => isKillerOnRoom;


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

}
