using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


namespace CH_AICharacter
{
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
    private bool isFirstSpawn = false;
    private bool isMovingToRoom = false;
    private bool isInQueue = false;
    private bool hasReachedCurrentDestination = false;
    private int currentRoomIndex = -1;
    private readonly Dictionary<int, Transform> movementTargets = new Dictionary<int, Transform>();
    private int currentTargetKey;
    private CH_AICharacterQueueManager queueManager;

    //Animations part
    private Animator animator;
    [SerializeField]
    private string isWalkingParameter = "isWalking";
    [SerializeField]
    private float walkThreshold = 0.1f;
    [SerializeField]
    private float minAnimationSpeed = 0.8f;
    [SerializeField]
    private float maxAnimationSpeed = 1.4f;

    [HideInInspector]
    public int QueueIndex = -1;

    public void Initialize(NavMeshAgent agent, Transform customDestination, Transform[] secondaryTargets, int selectedSecondaryIndex, float arrivalDistance, Animator animator = null)
    {
        this.agent = agent;
        this.customDestination = customDestination;
        this.secondaryTargets = secondaryTargets;
        this.selectedSecondaryIndex = Mathf.Clamp(selectedSecondaryIndex, 0, secondaryTargets != null ? secondaryTargets.Length - 1 : 0);
        this.arrivalDistance = Mathf.Max(0.1f, arrivalDistance);
        this.animator = animator;

        if (this.animator == null && this.agent != null)
        {
            this.animator = this.agent.GetComponent<Animator>() ?? this.agent.GetComponentInChildren<Animator>();
        }

        queueManager = CH_AICharacterQueueManager.Instance;
        if (queueManager == null)
            queueManager = FindObjectOfType<CH_AICharacterQueueManager>();

        if (this.animator != null)
        {
            Debug.Log("Animator field: " + this.animator.name);
            Debug.Log("Animator.controller: " + (this.animator.runtimeAnimatorController != null ? this.animator.runtimeAnimatorController.name : "None"));
        }
        else
        {
            Debug.LogWarning($"{name}: No Animator found on the character root or children.");
        }

        BuildMovementTargets();
        BeginMovement();
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
        if(isFirstSpawn == false)
        {
            isFirstSpawn = true;
            MoveToRoom(0);
        }

        if (agent == null)
            return;

        // Recover Animator if it was missing at Initialize time or if it was added later.
        if (animator == null)
        {
            animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
            if (animator != null)
            {
                Debug.Log($"{name}: Recovered Animator at runtime: {animator.name}");
            }
        }

        // Update animator state and playback speed based on agent velocity.
        float currentSpeed = agent.velocity.magnitude;
        bool isWalking = currentSpeed > walkThreshold;
        float normalizedSpeed = 0f;

        if (agent.speed > 0.0001f)
        {
            normalizedSpeed = Mathf.Clamp01(currentSpeed / agent.speed);
        }

        float targetAnimationSpeed = isWalking ? Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, normalizedSpeed) : 1f;
        Debug.Log($"{name}: Current Speed={currentSpeed:F3}, isWalking={isWalking}, animationSpeed={targetAnimationSpeed:F3}");

        if (animator != null)
        {
            animator.speed = targetAnimationSpeed;

            if (HasAnimatorParameter(isWalkingParameter))
            {
                animator.SetBool(isWalkingParameter, isWalking);
            }
            else
            {
                Debug.LogWarning($"{name}: Animator does not contain parameter '{isWalkingParameter}'.");
            }
        }

        if (agent.pathPending || agent.isStopped)
            return;

        if (agent.remainingDistance <= arrivalDistance)
        {
            if (!hasReachedCurrentDestination)
            {
                hasReachedCurrentDestination = true;
                if (isMovingToRoom)
                {
                    HandleRoomArrival();
                }
                else
                {
                    MoveToNextTarget();
                }
            }
        }
        else
        {
            hasReachedCurrentDestination = false;
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

    public void MoveToRoom(int targetRoomIndex)
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

        if (isInQueue && targetRoomIndex != currentRoomIndex)
        {
            ExitQueue();
        }

        CH_RoomUnit room = FindRoomByIndex(targetRoomIndex);
        if (room == null || room.MovementPoint == null)
        {
            Debug.LogWarning($"No room found for index {targetRoomIndex} or it has no movement point.");
            return;
        }

        currentRoomIndex = targetRoomIndex;
        isMovingToRoom = true;
        hasReachedCurrentDestination = false;

        agent.isStopped = false;
        agent.SetDestination(room.MovementPoint.position);
    }

    private void HandleRoomArrival()
    {
        if (currentRoomIndex == 0)
        {
            EnterQueue();
        }
        else
        {
            ExitQueue();
        }
    }

    private void EnterQueue()
    {
        if (isInQueue)
            return;

        isInQueue = true;
        queueManager?.EnterQueue(gameObject);

        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private void ExitQueue()
    {
        if (!isInQueue)
            return;

        isInQueue = false;
        QueueIndex = -1;
        queueManager?.ExitQueue(gameObject);

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }

    private CH_RoomUnit FindRoomByIndex(int targetRoomIndex)
    {
        CH_RoomUnit[] rooms = FindObjectsOfType<CH_RoomUnit>();
        foreach (CH_RoomUnit room in rooms)
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

        Animator animator = characterInstance.GetComponent<Animator>() ?? characterInstance.GetComponentInChildren<Animator>();
        driver.Initialize(agent, customDestination, secondaryTargets, selectedSecondaryIndex, arrivalDistance, animator);
    }

    private bool HasAnimatorParameter(string parameterName)
    {
        if (animator == null || string.IsNullOrEmpty(parameterName))
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
                return true;
        }

        return false;
    }
}
}