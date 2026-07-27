using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using CH_AICharacter;
using TMPro;
using System.Collections;
public class CH_DialogClients : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private Button originButton;
    [SerializeField] private Button stayButton;
    [SerializeField] private Button appearanceButton;
    [SerializeField] private Button paymentButton;
    private CH_Manager manager;
    private DialogResponseData dialogData;
    private CH_NPCSuspicionProfile currentProfile;

    private void Start()
    {
        dialogData = LoadDialogData();
        HideDialog();
    }

    public void OpenDialogueFor(GameObject interactedObject)
    {
        if (interactedObject == null)
            return;

        CH_NPCSuspicionState suspicionState = interactedObject.GetComponentInParent<CH_NPCSuspicionState>() ?? interactedObject.GetComponent<CH_NPCSuspicionState>();
        if (suspicionState == null || suspicionState.SuspicionProfile == null)
            return;

        ShowDialogue(suspicionState.SuspicionProfile);

        if (CH_Manager.Instance != null)
        {
            CH_Manager.Instance.SetFPSControllerActive(false);
        }
        CH_Manager.Instance?.SetCursorLockState(false);

    }

    private void HideDialog()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        CH_Manager.Instance?.SetFPSControllerActive(true);
        CH_Manager.Instance?.SetCursorLockState(true);
    }

    private void HideDialogueButtons()
    {
        if (originButton != null)
            originButton.gameObject.SetActive(false);

        if (stayButton != null)
            stayButton.gameObject.SetActive(false);

        if (appearanceButton != null)
            appearanceButton.gameObject.SetActive(false);

        if (paymentButton != null)
            paymentButton.gameObject.SetActive(false);
    }

    private void ShowDialogueButtons()
    {
        if (originButton != null)
            originButton.gameObject.SetActive(true);

        if (stayButton != null)
            stayButton.gameObject.SetActive(true);

        if (appearanceButton != null)
            appearanceButton.gameObject.SetActive(true);

        if (paymentButton != null)
            paymentButton.gameObject.SetActive(true);
    }

    private void HideSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.enabled = false;
        }
    }

    private void ShowDialogue(CH_NPCSuspicionProfile profile)
    {
        if (profile == null)
            return;

        currentProfile = profile;
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }

        ShowDialogueButtons();

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.enabled = true;
        }
    }

    public void OnOriginClicked()
    {
        if (currentProfile == null)
            return;

        bool isKiller = currentProfile.IsActuallyDangerous && !string.IsNullOrEmpty(currentProfile.DangerType);
        SetSubtitle(BuildOriginLine(currentProfile, isKiller));
        HideDialogueButtons();
        StartCoroutine(WaitAndHideSubtitleAndDialog(2f));
        
    }

    public void OnStayClicked()
    {
        if (currentProfile == null)
            return;

        bool isKiller = currentProfile.IsActuallyDangerous && !string.IsNullOrEmpty(currentProfile.DangerType);
        SetSubtitle(BuildStayLine(currentProfile, isKiller));
        HideDialogueButtons();
        StartCoroutine(WaitAndHideSubtitleAndDialog(2f));
    }

    public void OnAppearanceClicked()
    {
        if (currentProfile == null)
            return;

        SetSubtitle(BuildAppearanceLine(currentProfile));
        HideDialogueButtons();
        StartCoroutine(WaitAndHideSubtitleAndDialog(2f));
    }

    public void OnPaymentClicked()
    {
        if (currentProfile == null)
            return;

        SetSubtitle(BuildPaymentLine(currentProfile));
        HideDialogueButtons();
        StartCoroutine(WaitAndHideSubtitleAndDialog(2f));
    }

    private void SetSubtitle(string text)
    {
        if (subtitleText != null)
        {
            subtitleText.text = text;
            subtitleText.enabled = true;
        }
    }

    private string BuildOriginLine(CH_NPCSuspicionProfile profile, bool isKiller)
    {
        if (dialogData == null)
            return string.Empty;

        if (!isKiller)
            return dialogData.client.origin;

        if (dialogData.killer != null && dialogData.killer.TryGetValue(profile.DangerType?.ToLowerInvariant() ?? "", out KillerDialogEntry killerEntry))
            return killerEntry.origin;

        return dialogData.client.origin;
    }

    private string BuildStayLine(CH_NPCSuspicionProfile profile, bool isKiller)
    {
        if (dialogData == null)
            return string.Empty;

        if (!isKiller)
            return dialogData.client.stay;

        if (dialogData.killer != null && dialogData.killer.TryGetValue(profile.DangerType?.ToLowerInvariant() ?? "", out KillerDialogEntry killerEntry))
            return killerEntry.stay;

        return dialogData.client.stay;
    }

    private string BuildAppearanceLine(CH_NPCSuspicionProfile profile)
    {
        if (profile.VisibleTraits == null || profile.VisibleTraits.Count == 0)
            return dialogData != null ? dialogData.client.appearance : "What do you look like? Plain, ordinary, hard to remember.";

        List<string> traitSummary = new List<string>();
        foreach (var trait in profile.VisibleTraits)
        {
            if (trait == null || string.IsNullOrEmpty(trait.Id))
                continue;

            if (dialogData != null && dialogData.traits != null && dialogData.traits.TryGetValue(trait.Id, out string description))
            {
                traitSummary.Add(description);
            }
        }

        if (traitSummary.Count == 0)
            return dialogData != null ? dialogData.client.appearance : "What do you look like? Plain, ordinary, hard to remember.";

        return "What do you look like? " + string.Join(", ", traitSummary.ToArray()) + ".";
    }

    private string BuildPaymentLine(CH_NPCSuspicionProfile profile)
    {
        if (dialogData == null)
            return string.Empty;

        if (!profile.IsActuallyDangerous || string.IsNullOrEmpty(profile.DangerType))
            return dialogData.client.payment;

        if (dialogData.killer != null && dialogData.killer.TryGetValue(profile.DangerType?.ToLowerInvariant() ?? "", out KillerDialogEntry killerEntry))
            return killerEntry.payment;

        return dialogData.client.payment;
    }

    private IEnumerator WaitAndHideSubtitleAndDialog(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HideSubtitle();
        HideDialog();
    }

    private DialogResponseData LoadDialogData()
    {
        try
        {
            string path = Path.Combine(Application.dataPath, "Data", "npc_dialog_responses.json");
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<DialogResponseDataWrapper>($"{{\"items\":{json}}}").items;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load dialogue JSON: {ex.Message}");
            return null;
        }
    }

    [Serializable]
    private class DialogResponseDataWrapper
    {
        public DialogResponseData items;
    }

    [Serializable]
    private class DialogResponseData
    {
        public ClientDialogEntry client;
        public Dictionary<string, KillerDialogEntry> killer;
        public Dictionary<string, string> traits;
    }

    [Serializable]
    private class ClientDialogEntry
    {
        public string origin;
        public string stay;
        public string appearance;
        public string payment;
    }

    [Serializable]
    private class KillerDialogEntry
    {
        public string origin;
        public string stay;
        public string appearance;
        public string payment;
    }
}
