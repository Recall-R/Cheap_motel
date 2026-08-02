using UnityEngine;

public class CH_IInteractable : MonoBehaviour
{
    public enum Interaction
    {
        None,
        GetToComputer,
        ExitComputer,
        CharacterDialog,
        LightSwitch,
        DoorSystem,
    }

    [SerializeField] private Interaction interactionType = Interaction.None;

    public Interaction InteractionType => interactionType;

    public void Interact()
    {
        if (interactionType == Interaction.None)
            return;

        SetInteractionType(interactionType);
    }

    public void TriggerInteraction(Interaction type)
    {
        if (type == Interaction.None)
            return;

        SetInteractionType(type);
    }

    public void SetInteractionType(Interaction type)
    {
        interactionType = type;
        Debug.Log($"CH_IInteractable: SetInteractionType {type} pe {gameObject.name}");

        if (interactionType == Interaction.GetToComputer)
        {
            if (CH_RenderingMonitor.Instance == null)
            {
                Debug.LogWarning("CH_IInteractable: CH_RenderingMonitor.Instance este null. Verifică dacă componenta CH_RenderingMonitor este activă în scenă.");
                return;
            }

            CH_RenderingMonitor.Instance.OnEnterMonitor();
        }
        else if (interactionType == Interaction.ExitComputer)
        {
            if (CH_RenderingMonitor.Instance == null)
            {
                Debug.LogWarning("CH_IInteractable: CH_RenderingMonitor.Instance este null. Verifică dacă componenta CH_RenderingMonitor este activă în scenă.");
                return;
            }

            CH_RenderingMonitor.Instance.OnExitMonitor();
        }
        else if (interactionType == Interaction.CharacterDialog)
        {
            CH_DialogClients dialogClient = GetComponent<CH_DialogClients>() ?? GetComponentInParent<CH_DialogClients>() ?? FindAnyObjectByType<CH_DialogClients>();
            if (dialogClient != null)
            {
                dialogClient.OpenDialogueFor(gameObject);
            }
        }
        else if (interactionType == Interaction.DoorSystem)
        {
            CH_DecisionDoor door = GetComponent<CH_DecisionDoor>() ?? GetComponentInParent<CH_DecisionDoor>();
            if (door != null)
            {
                door.doorSystem();
            }
        }
        else if (interactionType == Interaction.LightSwitch)
        {
            CH_Lights lights = GetComponent<CH_Lights>() ?? GetComponentInParent<CH_Lights>();
            if (lights != null)
            {
                if (lights.getISOn())
                {
                    lights.setIsOn(false);
                }
                else
                {
                    lights.setIsOn(true);
                }
            }
        }
    }
}
