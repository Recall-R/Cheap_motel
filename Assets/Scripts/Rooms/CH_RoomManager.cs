using System;
using UnityEngine;

public class CH_RoomManager : MonoBehaviour {

    public static CH_RoomManager Instance { get; private set; }

    [SerializeField] private CH_RoomUnit[] roomUnits;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Debug.LogWarning("Multiple CH_RoomManager instances found in scene.");
        }
    }


    public bool isRoomOccupied(int indexRoom)
    {
        return roomUnits[indexRoom].IsOccupied;
    }

    public bool isInRoomKiller(int indexRoom)
    {
        return roomUnits[indexRoom].IsKillerOnRoom;
    }

    public void setRoomOccupied(int indexRoom, bool occupied)
    {
        roomUnits[indexRoom].setOccupied(occupied);
    }

    public void setIsKillerOnRoom(int indexRoom, bool KillerOnRoom)
    {
        roomUnits[indexRoom].setKillerOnRoom(KillerOnRoom);
    }
}