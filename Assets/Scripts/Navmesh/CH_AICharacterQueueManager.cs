using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using CH_AICharacter;

public class CH_AICharacterQueueManager : MonoBehaviour
{
    public static CH_AICharacterQueueManager Instance { get; private set; }

    [SerializeField]
    private Transform queueStartPoint;

    [SerializeField]
    private float queueSpacing = 1.5f;

    [SerializeField]
    private readonly List<GameObject> queueObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple CH_AICharacterQueueManager instances found in scene.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool IsInQueue(GameObject character)
    {
        return character != null && queueObjects.Contains(character);
    }

    public bool IsFirstInQueue(GameObject character)
    {
        return character != null && queueObjects.Count > 0 && queueObjects[0] == character;
    }

    public GameObject GetFirstInQueue()
    {
        return queueObjects.Count > 0 ? queueObjects[0] : null;
    }

    public void MoveFirstInQueueToRoom(int targetRoomIndex)
    {
        GameObject firstCharacter = GetFirstInQueue();
        if (firstCharacter == null)
            return;

        CH_AICharacterMovementDriver driver = firstCharacter.GetComponent<CH_AICharacterMovementDriver>();
        if (driver == null)
            return;

        driver.MoveToRoom(targetRoomIndex);
    }

    public void EnterQueue(GameObject character)
    {
        if (character == null || queueObjects.Contains(character))
            return;

        queueObjects.Add(character);
        UpdateQueuePositions();

        Debug.Log("Character entered queue: " + character.name + " on position: " + character.GetComponent<CH_AICharacterMovementDriver>()?.QueueIndex);
    }

    public void ExitQueue(GameObject character)
    {
        if (character == null)
            return;

        CH_AICharacterMovementDriver driver = character.GetComponent<CH_AICharacterMovementDriver>();
        if (driver != null)
        {
            driver.QueueIndex = -1;
        }

        if (queueObjects.Remove(character))
        {
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        if (queueStartPoint == null)
            return;

        for (int i = 0; i < queueObjects.Count; i++)
        {
            GameObject character = queueObjects[i];
            if (character == null)
                continue;

            Vector3 queuePosition = queueStartPoint.position + queueStartPoint.forward * queueSpacing * i;
            character.transform.rotation = queueStartPoint.rotation;

            NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(queuePosition);
            }
            else
            {
                character.transform.position = queuePosition;
            }

            CH_AICharacterMovementDriver driver = character.GetComponent<CH_AICharacterMovementDriver>();
            if (driver != null)
            {
                driver.QueueIndex = i;
            }
        }
    }
}
