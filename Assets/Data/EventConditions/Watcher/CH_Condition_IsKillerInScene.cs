using UnityEngine;

[CreateAssetMenu(menuName = "DeathEvents/Conditions/Killer In Scene")]

public class CH_Condition_IsKillerInScene : ConditionAbstract
{
    [SerializeField] private string killerType; // The type of killer to check for

    public override bool Evaluate()
    {
        return CH_EventManager.Instance.isWatcherInScene(killerType);
    }
}

