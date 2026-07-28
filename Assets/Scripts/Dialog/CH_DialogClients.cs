using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        CH_Manager.Instance?.PrintDebugMessage($"IsKiller: {isKiller}");
        SetSubtitle(BuildOriginLine(currentProfile, isKiller));
        HideDialogueButtons();
        StartCoroutine(WaitAndHideSubtitleAndDialog(2f));
        
    }

    public void OnStayClicked()
    {
        if (currentProfile == null)
            return;

        bool isKiller = currentProfile.IsActuallyDangerous && !string.IsNullOrEmpty(currentProfile.DangerType);
        CH_Manager.Instance?.PrintDebugMessage($"IsKiller: {isKiller}");
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
            DialogResponseData data = JsonUtility.FromJson<DialogResponseDataWrapper>($"{{\"items\":{json}}}").items;
            if (data != null)
            {
                data.killer = ParseKillerDialogDictionary(json);
                data.traits = ParseStringDictionary(json, "traits");
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load dialogue JSON: {ex.Message}");
            return null;
        }
    }

    // Parses the killer dialog entries from the JSON string and returns a dictionary mapping danger types to KillerDialogEntry objects.
    private Dictionary<string, KillerDialogEntry> ParseKillerDialogDictionary(string json)
    {
        Dictionary<string, KillerDialogEntry> result = new Dictionary<string, KillerDialogEntry>(StringComparer.OrdinalIgnoreCase);
        string killerJson = ExtractJsonObject(json, "\"killer\"");
        if (string.IsNullOrEmpty(killerJson))
            return result;

        int index = 1;
        while (index < killerJson.Length)
        {
            SkipWhitespace(killerJson, ref index);
            if (index >= killerJson.Length || killerJson[index] == '}')
                break;

            string key = ParseJsonString(killerJson, ref index);
            if (key == null)
                break;

            SkipWhitespace(killerJson, ref index);
            if (index >= killerJson.Length || killerJson[index] != ':')
                break;
            index++;
            SkipWhitespace(killerJson, ref index);

            if (index >= killerJson.Length || killerJson[index] != '{')
                break;

            int valueStart = index;
            int depth = 0;
            for (; index < killerJson.Length; index++)
            {
                char c = killerJson[index];
                if (c == '"')
                {
                    index = SkipJsonString(killerJson, index);
                    if (index < 0)
                        return result;
                    continue;
                }
                else if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        index++;
                        break;
                    }
                }
            }

            if (valueStart >= killerJson.Length || index > killerJson.Length)
                break;

            string valueJson = killerJson.Substring(valueStart, index - valueStart);
            KillerDialogEntry entry = JsonUtility.FromJson<KillerDialogEntry>(valueJson);
            if (entry != null)
                result[key.ToLowerInvariant()] = entry;

            SkipWhitespace(killerJson, ref index);
            if (index < killerJson.Length && killerJson[index] == ',')
            {
                index++;
                continue;
            }
            break;
        }

        return result;
    }

    private Dictionary<string, string> ParseStringDictionary(string json, string propertyName)
    {
        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string dictJson = ExtractJsonObject(json, $"\"{propertyName}\"");
        if (string.IsNullOrEmpty(dictJson))
            return result;

        int index = 1;
        while (index < dictJson.Length)
        {
            SkipWhitespace(dictJson, ref index);
            if (index >= dictJson.Length || dictJson[index] == '}')
                break;

            string key = ParseJsonString(dictJson, ref index);
            if (key == null)
                break;

            SkipWhitespace(dictJson, ref index);
            if (index >= dictJson.Length || dictJson[index] != ':')
                break;
            index++;
            SkipWhitespace(dictJson, ref index);

            string value = ParseJsonString(dictJson, ref index);
            if (value == null)
                break;

            result[key] = value;

            SkipWhitespace(dictJson, ref index);
            if (index < dictJson.Length && dictJson[index] == ',')
            {
                index++;
                continue;
            }
            break;
        }

        return result;
    }

    private string ExtractJsonObject(string json, string propertyName)
    {
        if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(propertyName))
            return null;

        int index = json.IndexOf(propertyName, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return null;

        index = json.IndexOf('{', index);
        if (index < 0)
            return null;

        int start = index;
        int depth = 0;
        for (; index < json.Length; index++)
        {
            char c = json[index];
            if (c == '"')
            {
                index = SkipJsonString(json, index);
                if (index < 0)
                    return null;
                continue;
            }
            else if (c == '{')
            {
                depth++;
            }
            else if (c == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return json.Substring(start, index - start + 1);
                }
            }
        }

        return null;
    }

    private int SkipJsonString(string json, int index)
    {
        index++;
        while (index < json.Length)
        {
            char c = json[index];
            if (c == '\\')
            {
                index += 2;
                continue;
            }

            if (c == '"')
                return index;
            index++;
        }

        return -1;
    }

    private void SkipWhitespace(string json, ref int index)
    {
        while (index < json.Length && char.IsWhiteSpace(json[index]))
            index++;
    }

    private string ParseJsonString(string json, ref int index)
    {
        if (index >= json.Length || json[index] != '"')
            return null;

        int start = index + 1;
        StringBuilder builder = new StringBuilder();
        index = start;
        while (index < json.Length)
        {
            char c = json[index];
            if (c == '\\')
            {
                index++;
                if (index >= json.Length)
                    break;

                char escape = json[index];
                switch (escape)
                {
                    case '"': builder.Append('"'); break;
                    case '\\': builder.Append('\\'); break;
                    case '/': builder.Append('/'); break;
                    case 'b': builder.Append('\b'); break;
                    case 'f': builder.Append('\f'); break;
                    case 'n': builder.Append('\n'); break;
                    case 'r': builder.Append('\r'); break;
                    case 't': builder.Append('\t'); break;
                    case 'u':
                        if (index + 4 < json.Length)
                        {
                            string hex = json.Substring(index + 1, 4);
                            if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int codePoint))
                                builder.Append((char)codePoint);
                            index += 4;
                        }
                        break;
                }
            }
            else if (c == '"')
            {
                index++;
                return builder.ToString();
            }
            else
            {
                builder.Append(c);
            }
            index++;
        }

        return null;
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
