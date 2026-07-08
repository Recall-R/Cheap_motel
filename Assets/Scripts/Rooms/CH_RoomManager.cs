using UnityEngine;

public class CH_RoomManager : MonoBehaviour
{
    [SerializeField] private int roomIndex = 0;
    [SerializeField] private Transform movementPoint;

    public int RoomIndex => roomIndex;
    public Transform MovementPoint => movementPoint;
}
