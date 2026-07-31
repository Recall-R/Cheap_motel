using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

[System.Serializable]
public class ClothesSlotEntry
{
    public string slot;
    public List<string> overlays = new List<string>();
}

public class CH_clothesCharacter : MonoBehaviour
{
    [SerializeField] private DynamicCharacterAvatar avatar;

    // fiecare slot (chest, feet, etc.) poate avea mai multe overlay-uri disponibile
    [SerializeField] private List<ClothesSlotEntry> clothesDictionary = new List<ClothesSlotEntry>();

    void Awake() {
        avatar = this.GetComponent<DynamicCharacterAvatar>();
        if (avatar == null) {
            Debug.LogWarning("DynamicCharacterAvatar component not found on the GameObject.");
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //        ChangeClothes();
    //     }
    // }

    void OnEnable()
    {
        if (avatar != null)
            avatar.CharacterCreated.AddListener(ChangeClothes);
    }

    void OnDisable()
    {
        if (avatar != null)
            avatar.CharacterCreated.RemoveListener(ChangeClothes);
    }

    public void ChangeClothes(UMAData umaData)
    {
        if (avatar == null)
        {
            avatar = GetComponent<DynamicCharacterAvatar>();
        }

        if (avatar == null)
        {
            Debug.LogWarning("DynamicCharacterAvatar component not found on the GameObject.");
            return;
        }

        int slotIndex = 0;
        foreach (var entry in clothesDictionary)
        {
            if (entry == null || string.IsNullOrEmpty(entry.slot) || entry.overlays == null || entry.overlays.Count == 0)
            {
                continue;
            }

            int randomIndex = Random.Range(0, entry.overlays.Count);
            string overlay = entry.overlays[randomIndex];
            avatar.SetSlot(entry.slot, overlay);

            string colorName = GetRandomSharedColorName(slotIndex);
            if (!string.IsNullOrEmpty(colorName))
            {
                OverlayColorData colorData = new OverlayColorData(3);
                colorData.channelMask[0] = Random.ColorHSV(0f, 1f, 0.35f, 0.85f, 0.6f, 1f);
                avatar.SetColor(colorName, colorData, false);
            }

            slotIndex++;
        }

        avatar.UpdateColors();
        avatar.BuildCharacter();
    }

    private string GetRandomSharedColorName(int slotIndex)
    {
        if (avatar == null || avatar.characterColors == null || avatar.characterColors.Colors == null)
        {
            return null;
        }

        List<string> availableColors = new List<string>();
        foreach (var colorValue in avatar.characterColors.Colors)
        {
            if (colorValue == null || string.IsNullOrEmpty(colorValue.name))
            {
                continue;
            }

            string normalizedName = colorValue.name.ToLowerInvariant();
            if (normalizedName == "skin" || normalizedName == "hair" || normalizedName == "eyes")
            {
                continue;
            }

            availableColors.Add(colorValue.name);
        }

        if (availableColors.Count == 0)
        {
            return null;
        }

        return availableColors[slotIndex % availableColors.Count];
    }
}
