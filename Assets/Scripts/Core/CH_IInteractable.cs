using System.Collections.Generic;
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
        RoomLightPanel,

        ElectricalPanelDoor,
        ElectricalButton,
        KnockDoor,
    }

    [System.Serializable]
    public class InteractionBinding
    {
        public Interaction interactionType = Interaction.None;
        public List<KeyCode> keys = new List<KeyCode> { KeyCode.E };
        public bool enabled = true;

        public bool IsPressed()
        {
            if (!enabled || keys == null || keys.Count == 0)
                return false;

            foreach (KeyCode key in keys)
            {
                if (Input.GetKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [SerializeField] private Interaction interactionType = Interaction.None;
    [SerializeField] private List<KeyCode> interactionKeys = new List<KeyCode> { KeyCode.Mouse0 };
    [SerializeField] private List<InteractionBinding> interactionBindings = new List<InteractionBinding>();

    public Interaction InteractionType => interactionType;
    public List<KeyCode> InteractionKeys => interactionKeys;
    public List<InteractionBinding> InteractionBindings => interactionBindings;

    public bool TryGetPressedInteraction(out Interaction pressedInteraction)
    {
        pressedInteraction = Interaction.None;

        if (interactionBindings != null)
        {
            foreach (InteractionBinding binding in interactionBindings)
            {
                if (binding == null || !binding.enabled)
                    continue;

                if (binding.IsPressed())
                {
                    pressedInteraction = binding.interactionType;
                    return true;
                }
            }
        }

        if (interactionType == Interaction.None)
            return false;

        if (interactionKeys == null || interactionKeys.Count == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pressedInteraction = interactionType;
                return true;
            }

            return false;
        }

        foreach (KeyCode key in interactionKeys)
        {
            if (Input.GetKeyDown(key))
            {
                pressedInteraction = interactionType;
                return true;
            }
        }

        return false;
    }

    public bool TryTriggerPressedInteraction()
    {
        if (!TryGetPressedInteraction(out Interaction pressedInteraction))
            return false;

        TriggerInteraction(pressedInteraction);
        return true;
    }

    public bool IsInteractionPressed()
    {
        return TryGetPressedInteraction(out _);
    }

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
        else if (InteractionType == Interaction.RoomLightPanel)
        {
            CH_contorIndex controlIndex = GetComponent<CH_contorIndex>() ?? GetComponentInParent<CH_contorIndex>();
            controlIndex.setLocalRoomPanelLightOn(!controlIndex.IsLocalRoomPanelLightOn);
            CH_RoomManager.Instance.SetRoomLights(controlIndex.Index, controlIndex.IsLocalRoomPanelLightOn);
        }
        else if (interactionType == Interaction.ElectricalPanelDoor)
        {
            Animator animator = GetComponent<Animator>();

            if(animator.GetBool("isOpen"))
            {
                animator.SetBool("isOpen", false);
            } else
            {
                animator.SetBool("isOpen", true);
            }
        }
        else if (interactionType == Interaction.ElectricalButton)
        {
            Animator animator = GetComponent<Animator>();
            if(animator.GetBool("isOn"))
            {
                animator.SetBool("isOn", false);
            } else
            {
                animator.SetBool("isOn", true);
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
        else if (interactionType == Interaction.KnockDoor)
        {
            CH_DecisionDoor door = GetComponent<CH_DecisionDoor>() ?? GetComponentInParent<CH_DecisionDoor>();
            if(door != null)
            {
                door.knockDoor();
            }
        }
    }
}
