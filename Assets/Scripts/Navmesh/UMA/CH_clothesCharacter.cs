using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class CH_clothesCharacter : MonoBehaviour
{

    [SerializeField] private DynamicCharacterAvatar avatar;

    



    void Awake() {
        avatar = this.GetComponent<DynamicCharacterAvatar>();
        if (avatar == null) {
            Debug.LogWarning("DynamicCharacterAvatar component not found on the GameObject.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            avatar.SetSlot("Legs", "MaleJeans");
            avatar.BuildCharacter();
        }
    }

}
