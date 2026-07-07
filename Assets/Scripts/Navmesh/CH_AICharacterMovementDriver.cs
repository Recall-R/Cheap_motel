using UnityEngine;
using CH_AICharacter;
using UnityEngine.AI;
public class CH_AICharacterMovementDriver : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]

    private Transform firstTarget;
    [SerializeField]
    private Transform customDestination;
    [SerializeField]
    private Transform[] secondaryTargets;
    [SerializeField]
    private int selectedSecondaryIndex;
    [SerializeField]
    private float arrivalDistance;
    [SerializeField]
    private bool firstTargetReached;

    public void Initialize(NavMeshAgent agent, Transform firstTarget, Transform customDestination, Transform[] secondaryTargets, int selectedSecondaryIndex, float arrivalDistance)
    {
        this.agent = agent;
        this.firstTarget = firstTarget;
        this.customDestination = customDestination;
        this.secondaryTargets = secondaryTargets;
        this.selectedSecondaryIndex = Mathf.Clamp(selectedSecondaryIndex, 0, secondaryTargets != null ? secondaryTargets.Length - 1 : 0);
        this.arrivalDistance = Mathf.Max(0.1f, arrivalDistance);
        this.firstTargetReached = false;
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

        if (firstTarget != null)
        {
            agent.SetDestination(firstTarget.position);
        }
        else
        {
            SetSecondaryDestination();
        }
    }

    private void Update()
    {
        if (agent == null || agent.pathPending)
            return;

        if (agent.remainingDistance <= arrivalDistance)
        {
            if (!firstTargetReached && firstTarget != null)
            {
                firstTargetReached = true;
                SetSecondaryDestination();
            }
        }
    }

    private void SetSecondaryDestination()
    {
        Transform target = customDestination ?? GetSelectedSecondaryTarget();
        if (target == null)
        {
            Debug.LogWarning($"{name}: No secondary destination assigned for CH_AICharacterMovementDriver.");
            enabled = false;
            return;
        }

        agent.SetDestination(target.position);
    }

    private Transform GetSelectedSecondaryTarget()
    {
        if (secondaryTargets == null || secondaryTargets.Length == 0)
            return null;

        int index = Mathf.Clamp(selectedSecondaryIndex, 0, secondaryTargets.Length - 1);
        return secondaryTargets[index];
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

        driver.Initialize(agent, firstTarget, customDestination, secondaryTargets, selectedSecondaryIndex, arrivalDistance);
    }

}