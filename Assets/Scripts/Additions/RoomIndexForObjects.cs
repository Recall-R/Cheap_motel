using UnityEngine;

public class RoomIndexForObjects : MonoBehaviour
{
    [SerializeField] private int roomIndex = 0;

    public int RoomIndex => roomIndex;
}
