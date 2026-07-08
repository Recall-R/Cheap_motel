using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CH_AICharacterMovementDriver : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform customDestination;
    [SerializeField]
    private Transform[] secondaryTargets;
    [SerializeField]
    private int selectedSecondaryIndex;
    [SerializeField]
    private float arrivalDistance;

    private readonly Dictionary<int, Transform> movementTargets = new Dictionary<int, Transform>();
    private int currentTargetKey;

    public void Initialize(NavMeshAgent agent, Transform customDestination, Transform[] secondaryTargets, int selectedSecondaryIndex, float arrivalDistance)
    {
        this.agent = agent;
        this.customDestination = customDestination;
        this.secondaryTargets = secondaryTargets;
        this.selectedSecondaryIndex = Mathf.Clamp(selectedSecondaryIndex, 0, secondaryTargets != null ? secondaryTargets.Length - 1 : 0);
        this.arrivalDistance = Mathf.Max(0.1f, arrivalDistance);

        BuildMovementTargets();
        BeginMovement();

        MoveToRoom(0);
    }

    private void BeginMovement()
    {
        if (agent == null)
        {
            Debug.LogWarning("CH_AICharacterMovementDriver.BeginMovement called without a NavMeshAgent.");
            enabled = false;
            return;
        }

        // if (movementTargets.Count == 0)
        // {
        //     Debug.LogWarning($"{name}: No movement targets assigned for CH_AICharacterMovementDriver.");
        //     enabled = false;
        //     return;
        // }

        SetCurrentDestination();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            MoveToRoom(0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            MoveToRoom(1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            MoveToRoom(2);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            MoveToRoom(3);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            MoveToRoom(4);
            return;
        }

        if (agent == null || agent.pathPending)
            return;

        if (agent.remainingDistance <= arrivalDistance)
        {
            MoveToNextTarget();
        }
    }

    private void BuildMovementTargets()
    {
        movementTargets.Clear();
        int key = 0;

        if (customDestination != null)
        {
            movementTargets[key++] = customDestination;
        }

        if (secondaryTargets != null)
        {
            for (int i = 0; i < secondaryTargets.Length; i++)
            {
                Transform target = secondaryTargets[i];
                if (target == null)
                    continue;

                if (customDestination != null && target == customDestination)
                    continue;

                movementTargets[key++] = target;
            }
        }

        // if (movementTargets.Count == 0)
        // {
        //     enabled = false;
        //     return;
        // }

        currentTargetKey = Mathf.Clamp(selectedSecondaryIndex, 0, movementTargets.Count - 1);
    }

    private void SetCurrentDestination()
    {
        if (!movementTargets.TryGetValue(currentTargetKey, out Transform target) || target == null)
        {
            Debug.LogWarning($"{name}: Current movement target is missing.");
           // enabled = false;
            return;
        }

        agent.SetDestination(target.position);
    }

    private void MoveToRoom(int targetRoomIndex)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent == null)
        {
            Debug.LogWarning("No NavMeshAgent found for room navigation.");
            return;
        }

        CH_RoomManager room = FindRoomByIndex(targetRoomIndex);
        if (room == null || room.MovementPoint == null)
        {
            Debug.LogWarning($"No room found for index {targetRoomIndex} or it has no movement point.");
            return;
        }

        agent.SetDestination(room.MovementPoint.position);
    }

    private CH_RoomManager FindRoomByIndex(int targetRoomIndex)
    {
        CH_RoomManager[] rooms = FindObjectsOfType<CH_RoomManager>();
        foreach (CH_RoomManager room in rooms)
        {
            if (room != null && room.RoomIndex == targetRoomIndex)
            {
                return room;
            }
        }

        return null;
    }

    private void MoveToNextTarget()
    {
        if (movementTargets.Count == 0)
            return;

        currentTargetKey = (currentTargetKey + 1) % movementTargets.Count;
        SetCurrentDestination();
    }

    public void StartMovement(GameObject characterInstance)
    {
        if (characterInstance == null)
        {
            Debug.LogWarning("StartMovement called with null character instance.");
            return;
        }

        agent = characterInstance.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning($"{characterInstance.name} does not have a NavMeshAgent component.");
            return;
        }

        CH_AICharacterMovementDriver driver = characterInstance.GetComponent<CH_AICharacterMovementDriver>();
        if (driver == null)
        {
            driver = characterInstance.AddComponent<CH_AICharacterMovementDriver>();
        }

        driver.Initialize(agent, customDestination, secondaryTargets, selectedSecondaryIndex, arrivalDistance);
    }
}