using UnityEngine;

[CreateAssetMenu(menuName = "DeathEvents/Conditions/Time On Site")]

public class CH_Condition_IsInTime : ConditionAbstract
{

    public override bool Evaluate()
    {
        return CH_NightDirectr.Instance.CurrentHour >= 23 || CH_NightDirectr.Instance.CurrentHour < 5;
    }
}