using System;
using UnityEngine;
using TMPro;
public class CH_RoomManager : MonoBehaviour {

    public static CH_RoomManager Instance { get; private set; }

    [SerializeField] private CH_RoomUnit[] roomUnits;
    [SerializeField] private TMP_InputField nameInputField;
    private int selectedRoomIndex = -1;

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
    public bool isNameInputFieldEmpty()
    {
        return string.IsNullOrWhiteSpace(nameInputField.text);
    }
    public void setRoomOccupied(int indexRoom, bool occupied)
    {
        roomUnits[indexRoom].setOccupied(occupied);
    }

    public void setIsKillerOnRoom(int indexRoom, bool KillerOnRoom)
    {
        roomUnits[indexRoom].setKillerOnRoom(KillerOnRoom);
    }

    public void uiSetAiToRoom(int roomIndex)
    {
        selectedRoomIndex = roomIndex;
    }


    public void setNameInputFieldReadOnly(bool readOnly)
    {
        if (nameInputField != null)
        {
            nameInputField.readOnly = readOnly;
        }
        else
        {
            Debug.LogWarning("Name input field is not assigned.");
        }
    }

    public void uiSubmitSelectedRoom()
    {
        if (selectedRoomIndex <= 0)
        {
            return;
        }

        if (CH_AICharacterQueueManager.Instance == null || CH_AICharacterQueueManager.Instance.IsQueueEmpty())
        {
            Debug.LogWarning("No AI characters in queue to assign to room.");
            return;

        }

        if (isRoomOccupied(selectedRoomIndex))
        {
            Debug.LogWarning($"Room {selectedRoomIndex} is already occupied.");
            return;

        }
        if (nameInputField == null || isNameInputFieldEmpty())
        {
            Debug.LogWarning("Name input field is empty or not assigned.");
            return;
        }
        
        if (CH_AICharacterQueueManager.Instance.GetNameOfFirstInQueue() != nameInputField.text)
        {
            Debug.LogWarning("Name in input field does not match the first character in the queue.");
            return;
        }


        if (CH_AICharacterQueueManager.Instance != null)
        {
            CH_AICharacterQueueManager.Instance.MoveFirstInQueueToRoom(selectedRoomIndex);
            roomUnits[selectedRoomIndex].setNpcCharacter(CH_AICharacterQueueManager.Instance.gameObject);
            selectedRoomIndex = -1;
        }
        
    }
}