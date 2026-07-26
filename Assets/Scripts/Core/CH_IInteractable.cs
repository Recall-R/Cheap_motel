using UnityEngine;

public class CH_IInteractable : MonoBehaviour
{
    public enum Interaction
    {
        None,
        GetToComputer,
        ExitComputer
    }

    [SerializeField] private Interaction interactionType = Interaction.None;

    public void SetInteractionType(Interaction type)
    {
        interactionType = type;
        Debug.Log($"CH_IInteractable: SetInteractionType {type} pe {gameObject.name}");

        if (CH_RenderingMonitor.Instance == null)
        {
            Debug.LogWarning("CH_IInteractable: CH_RenderingMonitor.Instance este null. Verifică dacă componenta CH_RenderingMonitor este activă în scenă.");
            return;
        }

        if (interactionType == Interaction.GetToComputer)
        {
            CH_RenderingMonitor.Instance.OnEnterMonitor();
        }
        else if (interactionType == Interaction.ExitComputer)
        {
            CH_RenderingMonitor.Instance.OnExitMonitor();
        }
    }
}
