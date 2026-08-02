using UnityEngine;
using CH_AICharacter;
using System.Collections.Generic;
public class CH_AIManager : MonoBehaviour
{

    public static CH_AIManager Instance { get; private set; }

    [SerializeField]
    private CH_AICharacterClass _aiCharacter = new CH_AICharacterClass();

    [SerializeField]
    private GameObject CH_SpawnPoint;

    private void Start()
    {
        Instance = this;

        if (CH_SpawnPoint == null)
        {
            Debug.LogWarning("Spawn point is not assigned in inspector.");
        }
    }

    private void Update()
    {
        if (CH_SpawnPoint == null)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            _aiCharacter.InvokeCharacter(CH_SpawnPoint.transform.position);
            return;
        }

        CH_AICharacterQueueManager queueManager = CH_AICharacterQueueManager.Instance;
        if (queueManager == null)
            return;

        // if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        // {
        //     queueManager.MoveFirstInQueueToRoom(0); // position 0 always the reception
        //     return;
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        // {
        //     queueManager.MoveFirstInQueueToRoom(1);
            
        //     return;
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        // {
        //     queueManager.MoveFirstInQueueToRoom(2);
        //     return;
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        // {
        //     queueManager.MoveFirstInQueueToRoom(3);
        //     return;
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        // {
        //     queueManager.MoveFirstInQueueToRoom(4);
        //     return;
        // }
    }

}
