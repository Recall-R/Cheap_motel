using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using CH_AICharacter;

public class CH_UIControllerAI : MonoBehaviour
{

    // void MoveToPoint()
    // {
    //     // Find the CH_RoomUnit component in the scene
    //     CH_RoomUnit roomManager = FindObjectOfType<CH_RoomUnit>();

    //     if (roomManager == null)
    //     {
    //         Debug.LogWarning("No CH_RoomUnit found in the scene.");
    //         return;
    //     }

    //     // Get the movement point from the room manager
    //     Transform movementPoint = roomManager.MovementPoint;

    //     if (movementPoint == null)
    //     {
    //         Debug.LogWarning("Movement point is not assigned in the CH_RoomUnit.");
    //         return;
    //     }

    //     // Find the CH_AICharacterMovementDriver component in the scene
    //     CH_AICharacterMovementDriver movementDriver = FindObjectOfType<CH_AICharacterMovementDriver>();

    //     if (movementDriver == null)
    //     {
    //         Debug.LogWarning("No CH_AICharacterMovementDriver found in the scene.");
    //         return;
    //     }

    //     // Move to the specified room index (0 in this case)
    //     movementDriver.MoveToRoom(roomManager.RoomIndex);
    // }
}