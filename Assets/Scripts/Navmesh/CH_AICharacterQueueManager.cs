using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using CH_AICharacter;

public class CH_AICharacterQueueManager : MonoBehaviour
{
    public static CH_AICharacterQueueManager Instance { get; private set; }

    [SerializeField]
    private Transform queueStartPoint;

    [SerializeField]
    private float queueSpacing = 1.5f;

    private const float secondsToWaitBeforeGoingToExit = 30f;

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

    public int GetQueueCount()
    {
        return queueObjects.Count;
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

    public bool IsQueueEmpty()
    {
        return queueObjects.Count == 0;
    }

    public string GetNameOfFirstInQueue()
    {
        GameObject firstCharacter = GetFirstInQueue();
        if (firstCharacter != null)
        {
            CH_NPCSuspicionState suspicionState = firstCharacter.GetComponent<CH_NPCSuspicionState>();
            if (suspicionState != null)
            {
                return suspicionState.SuspicionProfile.Id;
            }
        }
        return string.Empty;
    }

    public void MoveFirstInQueueToRoom(int targetRoomIndex)
    {
        GameObject firstCharacter = GetFirstInQueue();
        if (firstCharacter == null)
            return;

        CH_AICharacterMovementDriver driver = firstCharacter.GetComponent<CH_AICharacterMovementDriver>();
        CH_NPCSuspicionState suspicionState = firstCharacter.GetComponent<CH_NPCSuspicionState>();
        if (driver == null)
            return;
        if (suspicionState.SuspicionProfile.IsActuallyDangerous)
        {
            CH_RoomManager.Instance.setIsKillerOnRoom(targetRoomIndex, true);
            CH_EventManager.Instance.isAKillerInScene();
        }
        
        CH_RoomManager.Instance.setRoomOccupied(targetRoomIndex, true);
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

    public IEnumerator GoToExitAfterSeconds(GameObject character)
    {
        if (!IsInQueue(character))
            yield break;

        yield return new WaitForSeconds(secondsToWaitBeforeGoingToExit);

        if (!IsInQueue(character))
            yield break;

        GameObject exitPoint = CH_Manager.Instance != null ? CH_Manager.Instance.GetExitPointForNpcs() : null;
        if (exitPoint == null)
            yield break;

        CH_AICharacterMovementDriver movementDriver = character.GetComponent<CH_AICharacterMovementDriver>();
        if (movementDriver == null)
        {
            Debug.LogWarning($"{character.name}: Cannot leave the queue because CH_AICharacterMovementDriver is missing.");
            yield break;
        }

        movementDriver.LeaveQueueForExit(exitPoint.transform);
    }
}
